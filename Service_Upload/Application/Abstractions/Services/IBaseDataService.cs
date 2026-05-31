using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Services;

/// <summary>
/// Interface trừu tượng — Service_Upload gọi khi cần dữ liệu từ Service_Base.
/// Implementation (Refit/HTTP) nằm ở Api layer.
/// </summary>
public interface IBaseDataService
{
    /// <summary>Lấy thông tin user theo ID.</summary>
    Task<UserInfo?> GetUserAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Lấy danh sách phòng ban.</summary>
    Task<IReadOnlyList<DepartmentInfo>> GetDepartmentsAsync(CancellationToken ct = default);

    /// <summary>Kiểm tra service có sẵn không.</summary>
    Task<bool> IsAvailableAsync(CancellationToken ct = default);
}

public record UserInfo(Guid Id, string UserName, string FullName, string? Email);

public record DepartmentInfo(Guid Id, string Code, string Name, Guid? ParentId);
