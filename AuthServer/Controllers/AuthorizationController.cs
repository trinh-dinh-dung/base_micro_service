using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthServer.Controllers;

public class AuthorizationController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public AuthorizationController(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Challenge(
                authenticationSchemes: ["Cookies"],
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType
                            ? Request.Form.ToList()
                            : Request.Query.ToList())
                });
        }

        var identity = new ClaimsIdentity(
            authenticationType: "Bearer",
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity!.Name)
                .SetClaim(Claims.Email, User.FindFirstValue(ClaimTypes.Email))
                .SetClaim(Claims.Name, User.Identity!.Name)
                .SetClaims(Claims.Role, [.. (User.FindAll(ClaimTypes.Role).Select(c => c.Value))]);

        identity.SetScopes(request.GetScopes());
        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var identity = new ClaimsIdentity(result.Principal!.Claims,
                authenticationType: "Bearer",
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.SetDestinations(GetDestinations);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    [HttpGet("~/connect/logout")]
    [HttpPost("~/connect/logout")]
    public async Task<IActionResult> EndSession()
    {
        await HttpContext.SignOutAsync("Cookies");
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties { RedirectUri = "/" });
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        return claim.Type switch
        {
            Claims.Name or Claims.Email =>
                claim.Subject?.HasScope(Scopes.Profile) == true
                    ? [Destinations.AccessToken, Destinations.IdentityToken]
                    : [Destinations.AccessToken],
            Claims.Role =>
                claim.Subject?.HasScope(Scopes.Roles) == true
                    ? [Destinations.AccessToken, Destinations.IdentityToken]
                    : [Destinations.AccessToken],
            _ => [Destinations.AccessToken]
        };
    }
}
