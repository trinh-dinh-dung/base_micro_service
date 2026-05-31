using Application.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Api.Base.JWT
{
    public static class AddJWTTokenServicesExtensions
    {
        public static void AddJWTTokenServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            var bindJwtSettings = new JwtSettings();
            Configuration.Bind("JsonWebTokenKeys", bindJwtSettings);
            Services.AddSingleton(bindJwtSettings);

            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                // OpenIddict (RS256) — KHÔNG gán new TokenValidationParameters (mất JWKS từ metadata)
                if (!string.IsNullOrWhiteSpace(bindJwtSettings.Authority))
                {
                    var authority = bindJwtSettings.Authority.TrimEnd('/');
                    options.Authority = authority;
                    options.MetadataAddress = $"{authority}/.well-known/openid-configuration";

                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.ValidateIssuer = bindJwtSettings.ValidateIssuer;
                    options.TokenValidationParameters.ValidateAudience = bindJwtSettings.ValidateAudience;
                    options.TokenValidationParameters.ValidateLifetime = bindJwtSettings.ValidateLifetime;
                    options.TokenValidationParameters.ClockSkew =
                        TimeSpan.FromHours(bindJwtSettings.ExpaiTime > 0 ? bindJwtSettings.ExpaiTime : 1);

                    if (bindJwtSettings.ValidIssuers is { Length: > 0 })
                    {
                        options.TokenValidationParameters.ValidIssuers = bindJwtSettings.ValidIssuers;
                    }
                    else if (!string.IsNullOrEmpty(bindJwtSettings.ValidIssuer))
                    {
                        options.TokenValidationParameters.ValidIssuer = bindJwtSettings.ValidIssuer;
                    }

                    if (!string.IsNullOrEmpty(bindJwtSettings.ValidAudience))
                    {
                        options.TokenValidationParameters.ValidAudience = bindJwtSettings.ValidAudience;
                    }

                    options.Events = new JwtBearerEvents
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
                        },
                    };
                    return;
                }

                // Legacy symmetric key (HMAC)
                options.TokenValidationParameters.ValidateIssuerSigningKey = bindJwtSettings.ValidateIssuerSigningKey;
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(bindJwtSettings.IssuerSigningKey ?? string.Empty));
                options.TokenValidationParameters.ValidateIssuer = bindJwtSettings.ValidateIssuer;
                options.TokenValidationParameters.ValidIssuer = bindJwtSettings.ValidIssuer;
                options.TokenValidationParameters.ValidateAudience = bindJwtSettings.ValidateAudience;
                options.TokenValidationParameters.ValidAudience = bindJwtSettings.ValidAudience;
                options.TokenValidationParameters.RequireExpirationTime = bindJwtSettings.RequireExpirationTime;
                options.TokenValidationParameters.ValidateLifetime = bindJwtSettings.ValidateLifetime;
                options.TokenValidationParameters.ClockSkew =
                    TimeSpan.FromHours(bindJwtSettings.ExpaiTime > 0 ? bindJwtSettings.ExpaiTime : 1);
            });
        }
    }
}
