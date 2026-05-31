using Application.Abstractions.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.IServices;

public interface IFileMetadataService
{
    /// <summary>
    /// Upload file và enrich metadata từ Service_Base (tên phòng ban, tên user...).
    /// Nghiệp vụ: cần gọi Service_Base 2 lần (lấy user + lấy department).
    /// </summary>
    Task<FileWithMetadata> UploadWithMetadataAsync(
        string fileName,
        Guid uploaderId,
        Guid departmentId,
        CancellationToken ct = default);
}

public record FileWithMetadata(
    string FileName,
    string UploaderName,
    string DepartmentName,
    bool IsBaseServiceAvailable);
