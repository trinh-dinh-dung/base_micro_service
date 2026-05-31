using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Services;

/// <summary>
/// Interface trừu tượng — Application layer gọi khi cần thao tác file.
/// Implementation (Refit/HTTP) nằm ở Api layer.
/// Application layer KHÔNG biết gì về HTTP, Refit hay địa chỉ service.
/// </summary>
public interface IFileStorageService
{
    /// <summary>Upload file lên File Storage Service.</summary>
    Task<UploadFileResult> UploadAsync(
        string fileName,
        string contentType,
        Stream content,
        CancellationToken ct = default);

    /// <summary>Lấy danh sách file.</summary>
    Task<IReadOnlyList<StoredFileInfo>> GetFilesAsync(CancellationToken ct = default);

    /// <summary>Xóa file theo tên.</summary>
    Task DeleteAsync(string fileName, CancellationToken ct = default);
}

public record UploadFileResult(bool Success, string? Url, string? SavedAs, string? ErrorMessage);

public record StoredFileInfo(string Name, string Url, long SizeBytes, System.DateTime LastModified);
