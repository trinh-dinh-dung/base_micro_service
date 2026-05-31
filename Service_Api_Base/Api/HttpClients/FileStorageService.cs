using System.IO;
using Application.Abstractions.Services;
using Refit;

namespace Evo.Mes.Template.Api.HttpClients;

/// <summary>
/// Adapter: implements IFileStorageService (Application) bằng cách dùng IUploadServiceClient (Refit).
/// Application layer hoàn toàn không biết class này tồn tại.
/// Đăng ký: services.AddScoped&lt;IFileStorageService, FileStorageService&gt;()
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly IUploadServiceClient _client;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IUploadServiceClient client, ILogger<FileStorageService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<UploadFileResult> UploadAsync(
        string fileName,
        string contentType,
        Stream content,
        CancellationToken ct = default)
    {
        try
        {
            var part = new StreamPart(content, fileName, contentType);
            var response = await _client.UploadFilesAsync([part], ct);

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                _logger.LogWarning("Upload failed: {Status}", response.StatusCode);
                return new UploadFileResult(false, null, null, $"HTTP {(int)response.StatusCode}");
            }

            var uploaded = response.Content.Files.FirstOrDefault(f => f.Success);
            return uploaded != null
                ? new UploadFileResult(true, uploaded.Url, uploaded.SavedAs, null)
                : new UploadFileResult(false, null, null, response.Content.Files.FirstOrDefault()?.Message);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Refit ApiException uploading {FileName}", fileName);
            return new UploadFileResult(false, null, null, ex.Message);
        }
    }

    public async Task<IReadOnlyList<StoredFileInfo>> GetFilesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetFilesAsync(ct);
            if (!response.IsSuccessStatusCode || response.Content?.Files == null)
                return [];

            return response.Content.Files
                .Select(f => new StoredFileInfo(f.Name, f.Url, f.SizeBytes, f.LastModified))
                .ToList();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Refit ApiException getting file list");
            return [];
        }
    }

    public async Task DeleteAsync(string fileName, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.DeleteFileAsync(fileName, ct);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Delete failed: HTTP {(int)response.StatusCode}");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Refit ApiException deleting {FileName}", fileName);
            throw new InvalidOperationException($"Delete failed: {ex.Message}", ex);
        }
    }
}
