using AuthServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthServer.Controllers;

public class AccountController : Controller
{
    private readonly IReadOnlyDictionary<string, string> _users;

    public AccountController(IConfiguration configuration)
    {
        var configuredUsers = configuration.GetSection("Auth:Users").Get<Dictionary<string, string>>();
        _users = (configuredUsers ?? new Dictionary<string, string>())
            .Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
            .ToDictionary(x => x.Key.Trim(), x => x.Value, StringComparer.OrdinalIgnoreCase);
    }

    [HttpGet("~/account/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("~/account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (_users.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Authentication users are not configured.");
            return View(model);
        }

        var username = model.Username.Trim();

        if (!_users.TryGetValue(username, out var password) || password != model.Password)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, username),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Email, $"{username}@example.com"),
            new(ClaimTypes.Role, username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User")
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

        var safeReturnUrl = !string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
            ? model.ReturnUrl
            : "/";

        return LocalRedirect(safeReturnUrl);
    }

    [HttpGet("~/account/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Redirect("/");
    }
}
