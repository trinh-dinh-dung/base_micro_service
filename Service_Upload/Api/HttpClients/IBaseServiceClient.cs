using Application.HttpClients.Models;
using Refit;

namespace Evo.Mes.Template.Api.HttpClients;

/// <summary>
/// Refit client — Service_Upload gọi sang Service_Base.
/// Base URL được inject qua IHttpClientFactory (xem RefitClientExtensions.cs).
/// </summary>
public interface IBaseServiceClient
{
    /// <summary>Lấy thông tin user theo ID.</summary>
    [Get("/api/User/{id}")]
    Task<ApiResponse<UserDto>> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>Lấy danh sách phòng ban.</summary>
    [Get("/api/Department")]
    Task<ApiResponse<List<DepartmentDto>>> GetDepartmentsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Kiểm tra health của Service_Base.</summary>
    [Get("/api/Home")]
    Task<ApiResponse<object>> PingAsync(
        CancellationToken cancellationToken = default);
}
