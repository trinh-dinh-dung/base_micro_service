using Application.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Base.JWT
{
    public static class AddJWTTokenServicesExtensions
    {
        public static void AddJWTTokenServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind("JsonWebTokenKeys", jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = BuildTokenValidationParameters(jwtSettings);
                // .NET 8+ defaults to JsonWebTokenHandler which ignores SignatureValidator.
                // When external validation bypasses signature checking, force the legacy
                // JwtSecurityTokenHandler so our custom SignatureValidator is honoured.
                if (jwtSettings.UseExternalValidation && !jwtSettings.ValidateIssuerSigningKey)
                {
                    options.UseSecurityTokenValidators = true;
                }
                ConfigureAuthority(options, jwtSettings);
                options.Events = BuildJwtEvents(jwtSettings);
            });
        }

        private static TokenValidationParameters BuildTokenValidationParameters(JwtSettings settings)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = settings.ValidateIssuerSigningKey,
                ValidateIssuer = settings.ValidateIssuer,
                ValidateAudience = settings.ValidateAudience,
                RequireExpirationTime = settings.RequireExpirationTime,
                ValidateLifetime = settings.ValidateLifetime,
                ClockSkew = TimeSpan.FromHours(settings.ExpaiTime > 0 ? settings.ExpaiTime : 1)
            };

            if (settings.ValidIssuers is { Length: > 0 })
            {
                tokenValidationParameters.ValidIssuers = settings.ValidIssuers;
            }
            else if (!string.IsNullOrWhiteSpace(settings.ValidIssuer))
            {
                tokenValidationParameters.ValidIssuer = settings.ValidIssuer;
            }

            if (!string.IsNullOrWhiteSpace(settings.ValidAudience))
            {
                tokenValidationParameters.ValidAudience = settings.ValidAudience;
            }

            if (!string.IsNullOrWhiteSpace(settings.Authority))
            {
                return tokenValidationParameters;
            }

            // External validation mode: skip signature check entirely — IAM API validates the token.
            if (!settings.ValidateIssuerSigningKey)
            {
                tokenValidationParameters.SignatureValidator =
                    (token, _) => new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token);
                return tokenValidationParameters;
            }

            if (string.IsNullOrWhiteSpace(settings.IssuerSigningKey))
            {
                throw new InvalidOperationException(
                    "JsonWebTokenKeys:IssuerSigningKey must be configured when JsonWebTokenKeys:Authority is empty.");
            }

            tokenValidationParameters.IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.IssuerSigningKey));

            return tokenValidationParameters;
        }

        private static void ConfigureAuthority(JwtBearerOptions options, JwtSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Authority))
            {
                return;
            }

            var authority = settings.Authority.TrimEnd('/');
            options.Authority = authority;
            options.MetadataAddress = $"{authority}/.well-known/openid-configuration";
        }

        private static JwtBearerEvents BuildJwtEvents(JwtSettings settings)
        {
            var events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetService<ILoggerFactory>()
                        ?.CreateLogger("JwtBearer");
                    logger?.LogWarning(
                        context.Exception,
                        "JWT validation failed: {Message}",
                        context.Exception.Message);
                    return Task.CompletedTask;
                }
            };

            if (settings.UseExternalValidation)
            {
                events.OnTokenValidated = context => ValidateWithExternalIamAsync(context, settings);
            }

            return events;
        }

        private static async Task ValidateWithExternalIamAsync(TokenValidatedContext context, JwtSettings settings)
        {
            var validationUrl = settings.ExternalValidationUrl;
            if (string.IsNullOrWhiteSpace(validationUrl))
            {
                context.Fail("External validation URL is not configured.");
                return;
            }

            var authHeader = context.Request.Headers.Authorization.ToString();
            var accessToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader["Bearer ".Length..]
                : authHeader;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                context.Fail("Access token is missing.");
                return;
            }

            var httpClientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();

            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsJsonAsync(
                    validationUrl,
                    new { AccessToken = accessToken },
                    context.HttpContext.RequestAborted);
            }
            catch
            {
                context.Fail("Cannot reach external IAM validation endpoint.");
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                context.Fail($"External validation failed with status {(int)response.StatusCode}.");
                return;
            }

            try
            {
                await using var stream = await response.Content.ReadAsStreamAsync(context.HttpContext.RequestAborted);
                using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: context.HttpContext.RequestAborted);
                if (!IsTokenValid(payload.RootElement))
                {
                    context.Fail("External IAM validation returned invalid token.");
                }
            }
            catch
            {
                context.Fail("Invalid external IAM validation response.");
            }
        }

        private static bool IsTokenValid(JsonElement root)
        {
            foreach (var prop in root.EnumerateObject())
            {
                var key = prop.Name.ToLowerInvariant();
                if (key is "success" or "issuccess" or "isvalid" or "valid")
                {
                    return prop.Value.ValueKind == JsonValueKind.True;
                }
            }

            return false;
        }
    }
}
