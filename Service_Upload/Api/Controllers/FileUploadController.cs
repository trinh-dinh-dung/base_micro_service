using System.IO;
using Api.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Evo.Mes.Template.Api.Controllers;

/// <summary>
/// File upload/download endpoints for Service_Upload microservice.
/// </summary>
[Route("api/[controller]")]
public class FileUploadController : BaseController
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileUploadController> _logger;

    private static readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".xlsx", ".docx", ".zip"];
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

    public FileUploadController(IWebHostEnvironment env, ILogger<FileUploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>Upload one or multiple files.</summary>
    [HttpPost("upload")]
    [AllowAnonymous]
    [RequestSizeLimit(52_428_800)]
    public async Task<IActionResult> Upload([FromForm] IFormFileCollection files)
    {
        if (files == null || files.Count == 0)
            return BadRequest(new { success = false, message = "No files provided." });

        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadPath);

        var results = new List<object>();

        foreach (var file in files)
        {
            if (file.Length > MaxFileSizeBytes)
            {
                results.Add(new { file = file.FileName, success = false, message = "File exceeds 50 MB limit." });
                continue;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
            {
                results.Add(new { file = file.FileName, success = false, message = $"Extension '{ext}' not allowed." });
                continue;
            }

            var safeFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadPath, safeFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            _logger.LogInformation("File uploaded: {Original} -> {Saved}", file.FileName, safeFileName);

            results.Add(new
            {
                file = file.FileName,
                success = true,
                savedAs = safeFileName,
                url = $"/uploads/{safeFileName}",
                sizeBytes = file.Length
            });
        }

        return Ok(new { success = true, files = results });
    }

    /// <summary>List all uploaded files.</summary>
    [HttpGet("list")]
    [AllowAnonymous]
    public IActionResult List()
    {
        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadPath))
            return Ok(new { success = true, files = Array.Empty<object>() });

        var files = Directory.GetFiles(uploadPath)
            .Select(f => new FileInfo(f))
            .Select(fi => new
            {
                name = fi.Name,
                url = $"/uploads/{fi.Name}",
                sizeBytes = fi.Length,
                lastModified = fi.LastWriteTimeUtc
            })
            .OrderByDescending(f => f.lastModified)
            .ToList();

        return Ok(new { success = true, files });
    }

    /// <summary>Delete a file by name.</summary>
    [HttpDelete("{fileName}")]
    [Authorize]
    public IActionResult Delete(string fileName)
    {
        if (fileName.Contains('/') || fileName.Contains('\\'))
            return BadRequest(new { success = false, message = "Invalid file name." });

        var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound(new { success = false, message = "File not found." });

        System.IO.File.Delete(filePath);
        _logger.LogInformation("File deleted: {FileName}", fileName);

        return Ok(new { success = true, message = "File deleted." });
    }
}
