using Elastic.Apm.NetCoreAll;
using Gateway.Middleware;
using Gateway.ServiceDiscovery;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;

        var authority = builder.Configuration["Auth:Authority"];
        if (!string.IsNullOrWhiteSpace(authority))
        {
            options.Authority = authority.TrimEnd('/');
            options.TokenValidationParameters = new()
            {
                ValidateAudience = false
            };
            return;
        }

        var jwtSecret = builder.Configuration["Auth:JwtSecret"]
            ?? builder.Configuration["Auth:IssuerSigningKey"]
            ?? string.Empty;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = builder.Configuration.GetValue("Auth:ValidateIssuerSigningKey", true),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = builder.Configuration.GetValue("Auth:ValidateIssuer", false),
            ValidIssuer = builder.Configuration["Auth:ValidIssuer"],
            ValidateAudience = builder.Configuration.GetValue("Auth:ValidateAudience", false),
            ValidAudience = builder.Configuration["Auth:ValidAudience"],
            RequireExpirationTime = builder.Configuration.GetValue("Auth:RequireExpirationTime", true),
            ValidateLifetime = builder.Configuration.GetValue("Auth:ValidateLifetime", true),
            ClockSkew = TimeSpan.FromMinutes(builder.Configuration.GetValue("Auth:ClockSkewMinutes", 5))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("Anonymous", policy => policy.RequireAssertion(_ => true));
});
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();

var useConsul = builder.Configuration.GetValue<bool>("ServiceDiscovery:UseConsul");

if (useConsul)
{
    builder.Services.AddSingleton<IConsulServiceResolver, ConsulServiceResolver>();
    builder.Services.AddHostedService<ConsulClusterUpdater>();
}

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ── Elastic APM ──
builder.Services.AddAllElasticApm();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GatewayTokenValidationMiddleware>();
app.UseMiddleware<TraceparentLoggingMiddleware>();

// Health check endpoint — dùng cho Docker healthcheck và load balancer
app.MapHealthChecks("/health");

// Gateway info endpoint
app.MapGet("/", () => Results.Ok(new
{
    service = "API Gateway",
    status = "running",
    timestamp = DateTime.UtcNow
}));

app.MapGet("/api/auth/sso/signin-url", (IConfiguration configuration) =>
{
    var tokenUrl = configuration["Auth:SsoTokenUrl"]?.TrimEnd('/');
    var callbackUrl = configuration["Auth:SsoCallbackUrl"];
    var appId = configuration["Auth:SsoAppId"];
    var privateKey = configuration["Auth:SsoPrivateKey"];

    if (string.IsNullOrWhiteSpace(tokenUrl) ||
        string.IsNullOrWhiteSpace(callbackUrl) ||
        string.IsNullOrWhiteSpace(appId) ||
        string.IsNullOrWhiteSpace(privateKey))
    {
        return Results.Problem(
            detail: "SSO signing config is missing. Please set Auth:SsoTokenUrl, Auth:SsoCallbackUrl, Auth:SsoAppId, and Auth:SsoPrivateKey.",
            statusCode: StatusCodes.Status500InternalServerError);
    }

    try
    {
        using var rsa = RSA.Create();
        var normalizedKey = NormalizePrivateKey(privateKey);
        var keyBytes = Convert.FromBase64String(normalizedKey);

        try
        {
            rsa.ImportPkcs8PrivateKey(keyBytes, out _);
        }
        catch (CryptographicException)
        {
            rsa.ImportRSAPrivateKey(keyBytes, out _);
        }

        var signatureBytes = rsa.SignData(
            Encoding.UTF8.GetBytes(appId),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        var sign = Convert.ToBase64String(signatureBytes);
        var signInUrl =
            $"{tokenUrl}/api/Authentication/SignIn" +
            $"?app_id={Uri.EscapeDataString(appId)}" +
            $"&callback_url={Uri.EscapeDataString(callbackUrl)}" +
            $"&sign={Uri.EscapeDataString(sign)}";

        return Results.Ok(new { url = signInUrl });
    }
    catch
    {
        return Results.Problem(
            detail: "Cannot create SSO signature from configured private key.",
            statusCode: StatusCodes.Status500InternalServerError);
    }
}).AllowAnonymous();

app.MapReverseProxy();

app.Run();

static string NormalizePrivateKey(string privateKey)
{
    return privateKey
        .Replace("-----BEGIN PRIVATE KEY-----", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("-----END PRIVATE KEY-----", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("-----BEGIN RSA PRIVATE KEY-----", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("-----END RSA PRIVATE KEY-----", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("\r", string.Empty, StringComparison.Ordinal)
        .Replace("\n", string.Empty, StringComparison.Ordinal)
        .Trim();
}
