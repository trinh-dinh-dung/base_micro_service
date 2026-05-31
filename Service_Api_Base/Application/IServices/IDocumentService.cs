using Application.Abstractions.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.IServices;

public interface IDocumentService
{
    /// <summary>
    /// Gắn file vào một entity (vd: Department, User...).
    /// Nghiệp vụ: validate → upload → lưu metadata vào DB.
    /// </summary>
    Task<AttachFileResult> AttachFileToEntityAsync(
        string entityType,
        string entityId,
        string fileName,
        string contentType,
        Stream fileStream,
        CancellationToken ct = default);

    /// <summary>Lấy tất cả file đính kèm của entity.</summary>
    Task<IReadOnlyList<StoredFileInfo>> GetFilesOfEntityAsync(
        string entityType,
        string entityId,
        CancellationToken ct = default);

    /// <summary>Xóa file khỏi entity và storage.</summary>
    Task RemoveFileAsync(string fileName, CancellationToken ct = default);
}

public record AttachFileResult(bool Success, string? Url, string? ErrorMessage);
