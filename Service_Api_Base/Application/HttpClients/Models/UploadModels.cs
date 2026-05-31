using System;
using System.Collections.Generic;

namespace Application.HttpClients.Models;

public record UploadResult(bool Success, List<UploadedFileInfo> Files);

public record UploadedFileInfo(
    string File,
    bool Success,
    string? SavedAs,
    string? Url,
    long SizeBytes,
    string? Message);

public record FileListResult(bool Success, List<FileInfo2> Files);

public record FileInfo2(
    string Name,
    string Url,
    long SizeBytes,
    DateTime LastModified);

public record SimpleResult(bool Success, string Message);
