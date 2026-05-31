using Application.Abstractions.Services;
using Application.IServices;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Upload;

/// <summary>
/// Nghiệp vụ upload kèm enrichment dữ liệu từ Service_Base.
/// Inject IBaseDataService — không biết gì về Refit hay HTTP.
/// Gọi Service_Base 2 lần trong cùng một nghiệp vụ (user + department).
/// </summary>
public class FileMetadataService : IFileMetadataService
{
    private readonly IBaseDataService _baseData;

    public FileMetadataService(IBaseDataService baseData)
    {
        _baseData = baseData;
    }

    public async Task<FileWithMetadata> UploadWithMetadataAsync(
        string fileName,
        Guid uploaderId,
        Guid departmentId,
        CancellationToken ct = default)
    {
        // ── Gọi Service_Base lần 1: thông tin user ──────────────
        var userTask = _baseData.GetUserAsync(uploaderId, ct);

        // ── Gọi Service_Base lần 2: danh sách phòng ban ─────────
        var departmentsTask = _baseData.GetDepartmentsAsync(ct);

        // Song song hóa 2 lần gọi
        await Task.WhenAll(userTask, departmentsTask);

        var user = await userTask;
        var departments = await departmentsTask;

        var department = departments.FirstOrDefault(d => d.Id == departmentId);

        return new FileWithMetadata(
            FileName: fileName,
            UploaderName: user?.FullName ?? $"User {uploaderId}",
            DepartmentName: department?.Name ?? $"Dept {departmentId}",
            IsBaseServiceAvailable: user != null || departments.Count > 0);
    }
}
