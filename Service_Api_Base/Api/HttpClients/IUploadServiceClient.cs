using Application.HttpClients.Models;
using Refit;

namespace Evo.Mes.Template.Api.HttpClients;

/// <summary>
/// Refit client — Service_Base gọi sang Service_Upload.
/// Base URL được inject qua IHttpClientFactory (xem RefitClientExtensions.cs).
/// </summary>
public interface IUploadServiceClient
{
    /// <summary>Upload một hoặc nhiều file sang Service_Upload.</summary>
    [Multipart]
    [Post("/api/FileUpload/upload")]
    Task<ApiResponse<UploadResult>> UploadFilesAsync(
        [AliasAs("files")] IEnumerable<StreamPart> files,
        CancellationToken cancellationToken = default);

    /// <summary>Lấy danh sách file đã upload.</summary>
    [Get("/api/FileUpload/list")]
    Task<ApiResponse<FileListResult>> GetFilesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Xóa file theo tên (cần Bearer token).</summary>
    [Delete("/api/FileUpload/{fileName}")]
    Task<ApiResponse<SimpleResult>> DeleteFileAsync(
        string fileName,
        CancellationToken cancellationToken = default);
}
