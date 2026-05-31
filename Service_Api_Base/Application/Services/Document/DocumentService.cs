using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Services;
using Application.Exceptions;
using Application.IServices;

namespace Application.Services.Document;

/// <summary>
/// Nghiệp vụ xử lý tài liệu đính kèm.
/// Inject IFileStorageService — không biết gì về Refit hay HTTP.
/// Có thể gọi IFileStorageService nhiều lần trong một method nghiệp vụ.
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly IFileStorageService _fileStorage;

    private static readonly string[] _allowedTypes = ["image/jpeg", "image/png", "application/pdf", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"];
    private const long MaxSizeBytes = 20 * 1024 * 1024; // 20 MB

    public DocumentService(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<AttachFileResult> AttachFileToEntityAsync(
        string entityType,
        string entityId,
        string fileName,
        string contentType,
        Stream fileStream,
        CancellationToken ct = default)
    {
        // ── Nghiệp vụ: kiểm tra loại file ──────────────────────
        if (!IsAllowedType(contentType))
            return new AttachFileResult(false, null, $"Loại file '{contentType}' không được phép.");

        // ── Nghiệp vụ: giới hạn kích thước ─────────────────────
        if (fileStream.Length > MaxSizeBytes)
            return new AttachFileResult(false, null, "File vượt quá 20 MB.");

        // ── Gọi FileStorage 1 lần: upload ───────────────────────
        var uploadResult = await _fileStorage.UploadAsync(fileName, contentType, fileStream, ct);
        if (!uploadResult.Success)
            return new AttachFileResult(false, null, uploadResult.ErrorMessage);

        // TODO: lưu metadata (entityType, entityId, url) vào DB qua repository

        return new AttachFileResult(true, uploadResult.Url, null);
    }

    public async Task<IReadOnlyList<StoredFileInfo>> GetFilesOfEntityAsync(
        string entityType,
        string entityId,
        CancellationToken ct = default)
    {
        // ── Gọi FileStorage 1 lần: lấy danh sách ───────────────
        var allFiles = await _fileStorage.GetFilesAsync(ct);

        // Nghiệp vụ: lọc theo prefix entityType/entityId
        // (thực tế có thể filter từ DB metadata thay vì filter tất cả)
        var prefix = $"{entityType}_{entityId}_";
        return allFiles
            .Where(f => f.Name.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task RemoveFileAsync(string fileName, CancellationToken ct = default)
    {
        // Nghiệp vụ: kiểm tra file có tồn tại trước khi xóa
        var files = await _fileStorage.GetFilesAsync(ct);          // gọi lần 1
        var exists = files.Any(f => f.Name == fileName);

        if (!exists)
            throw new AppException($"File '{fileName}' không tồn tại.");

        await _fileStorage.DeleteAsync(fileName, ct);               // gọi lần 2
        // TODO: xóa metadata trong DB
    }

    private static bool IsAllowedType(string contentType) =>
        _allowedTypes.Contains(contentType.ToLowerInvariant());
}
