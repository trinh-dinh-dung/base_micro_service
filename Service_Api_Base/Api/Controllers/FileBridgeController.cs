using System.IO;
using Api.Base;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evo.Mes.Template.Api.Controllers;

/// <summary>
/// Controller chỉ gọi IDocumentService (Application layer).
/// Không biết Refit, không biết Service_Upload ở đâu.
/// </summary>
[Route("api/[controller]")]
public class FileBridgeController : BaseController
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<FileBridgeController> _logger;

    public FileBridgeController(
        IDocumentService documentService,
        ILogger<FileBridgeController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    /// <summary>
    /// Đính kèm file vào entity — nghiệp vụ đầy đủ (validate + upload + lưu metadata).
    /// POST /api/FileBridge/attach?entityType=Department&amp;entityId=abc123
    /// </summary>
    [HttpPost("attach")]
    [AllowAnonymous]
    [RequestSizeLimit(20_971_520)]
    public async Task<IActionResult> AttachFile(
        [FromQuery] string entityType,
        [FromQuery] string entityId,
        [FromForm] IFormFile file,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "File is required." });

        await using var stream = file.OpenReadStream();
        var result = await _documentService.AttachFileToEntityAsync(
            entityType, entityId,
            file.FileName, file.ContentType,
            stream, ct);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.ErrorMessage });

        return Ok(new { success = true, url = result.Url });
    }

    /// <summary>
    /// Lấy file của entity.
    /// GET /api/FileBridge/files?entityType=Department&amp;entityId=abc123
    /// </summary>
    [HttpGet("files")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFiles(
        [FromQuery] string entityType,
        [FromQuery] string entityId,
        CancellationToken ct)
    {
        var files = await _documentService.GetFilesOfEntityAsync(entityType, entityId, ct);
        return Ok(new { success = true, data = files });
    }

    /// <summary>
    /// Xóa file (nghiệp vụ: kiểm tra tồn tại rồi mới xóa — gọi Service_Upload 2 lần).
    /// DELETE /api/FileBridge/files/{fileName}
    /// </summary>
    [HttpDelete("files/{fileName}")]
    [Authorize]
    public async Task<IActionResult> RemoveFile(string fileName, CancellationToken ct)
    {
        try
        {
            await _documentService.RemoveFileAsync(fileName, ct);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "RemoveFile failed: {FileName}", fileName);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
