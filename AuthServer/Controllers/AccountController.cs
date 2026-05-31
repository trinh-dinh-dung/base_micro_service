using AuthServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthServer.Controllers;

public class AccountController : Controller
{
    private static readonly Dictionary<string, string> _users = new()
    {
        ["admin"] = "admin123",
        ["user"] = "user123"
    };

    [HttpGet("~/account/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("~/account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (!_users.TryGetValue(model.Username, out var password) || password != model.Password)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, model.Username),
            new(ClaimTypes.Name, model.Username),
            new(ClaimTypes.Email, $"{model.Username}@example.com"),
            new(ClaimTypes.Role, model.Username == "admin" ? "Admin" : "User")
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

        return Redirect(model.ReturnUrl ?? "/");
    }

    [HttpGet("~/account/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Redirect("/");
    }
}
