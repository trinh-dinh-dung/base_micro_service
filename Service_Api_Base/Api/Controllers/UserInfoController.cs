using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Evo.Mes.Template.Api.Controllers;

/// <summary>
/// Trả về thông tin user cơ bản (demo) — có thể đọc từ JWT khi đã đăng nhập SSO.
/// GET /api/UserInfo/me
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserInfoController : ControllerBase
{
    private static readonly Dictionary<string, UserInfoResponse> DemoUsers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"] = new("Nguyễn Văn Admin", 35, "0901234567", "123 Lê Lợi, Quận 1, TP.HCM"),
        ["user"] = new("Trần Thị User", 28, "0912345678", "456 Nguyễn Huệ, Quận 3, TP.HCM"),
    };

    private static readonly UserInfoResponse GuestProfile =
        new("Khách (Guest)", 0, "—", "—");

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {
        var userName = User.FindFirstValue(ClaimTypes.Name)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.Identity?.Name
            ?? "guest";

        var profile = DemoUsers.TryGetValue(userName, out var demo)
            ? demo
            : GuestProfile with { FullName = $"User: {userName}" };

        return Ok(new
        {
            success = true,
            data = new
            {
                userName,
                fullName = profile.FullName,
                age = profile.Age,
                phone = profile.Phone,
                address = profile.Address,
            },
        });
    }

    private sealed record UserInfoResponse(string FullName, int Age, string Phone, string Address);
}
