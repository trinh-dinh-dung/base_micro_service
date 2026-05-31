using Application.Abstractions.Services;
using Refit;

namespace Evo.Mes.Template.Api.HttpClients;

/// <summary>
/// Adapter: implements IBaseDataService (Application) bằng cách dùng IBaseServiceClient (Refit).
/// Application layer hoàn toàn không biết class này tồn tại.
/// Đăng ký: services.AddScoped&lt;IBaseDataService, BaseDataService&gt;()
/// </summary>
public class BaseDataService : IBaseDataService
{
    private readonly IBaseServiceClient _client;
    private readonly ILogger<BaseDataService> _logger;

    public BaseDataService(IBaseServiceClient client, ILogger<BaseDataService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<UserInfo?> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetUserByIdAsync(userId, ct);
            if (!response.IsSuccessStatusCode || response.Content == null)
                return null;

            var u = response.Content;
            return new UserInfo(u.Id, u.UserName, u.FullName, u.Email);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Cannot get user {UserId} from Service_Base", userId);
            return null;
        }
    }

    public async Task<IReadOnlyList<DepartmentInfo>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetDepartmentsAsync(ct);
            if (!response.IsSuccessStatusCode || response.Content == null)
                return [];

            return response.Content
                .Select(d => new DepartmentInfo(d.Id, d.DepartmentCode, d.DepartmentName, d.ParentId))
                .ToList();
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Cannot get departments from Service_Base");
            return [];
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PingAsync(ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
