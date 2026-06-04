using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace Gateway.Middleware;

public sealed class GatewayTokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public GatewayTokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, IHttpClientFactory httpClientFactory)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var isTokenIssueEndpoint = path.Equals("/api/base/api/e-invoice-holding/Authen/get-token", StringComparison.OrdinalIgnoreCase);

        if (isTokenIssueEndpoint)
        {
            await _next(context);
            return;
        }

        var requiresAuth = path.StartsWith("/api/base", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/api/upload", StringComparison.OrdinalIgnoreCase);

        if (!requiresAuth)
        {
            await _next(context);
            return;
        }

        var useExternalValidation = _configuration.GetValue("Auth:UseExternalValidation", false);
        if (!useExternalValidation)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { success = false, message = "Unauthorized request." });
                return;
            }

            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Missing Bearer token." });
            return;
        }

        var validationUrl = _configuration["Auth:ExternalValidationUrl"];
        if (string.IsNullOrWhiteSpace(validationUrl))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Gateway external validation URL is not configured." });
            return;
        }

        var token = authHeader["Bearer ".Length..];
        var httpClient = httpClientFactory.CreateClient();

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync(validationUrl, new { AccessToken = token }, context.RequestAborted);
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Cannot reach IAM validation endpoint." });
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Token rejected by IAM validation endpoint." });
            return;
        }

        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(context.RequestAborted);
            using var payload = await JsonDocument.ParseAsync(stream, cancellationToken: context.RequestAborted);

            var root = payload.RootElement;
            var isValid = IsTokenValid(root);

            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { success = false, message = "Invalid token." });
                return;
            }
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Invalid IAM validation response." });
            return;
        }

        if (context.User?.Identity?.IsAuthenticated != true)
        {
            var identity = new ClaimsIdentity("ExternalIamValidation");
            identity.AddClaim(new Claim(ClaimTypes.Name, "external-user"));
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
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
