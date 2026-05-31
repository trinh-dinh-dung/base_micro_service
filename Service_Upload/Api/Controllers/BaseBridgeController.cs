using Api.Base;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evo.Mes.Template.Api.Controllers;

/// <summary>
/// Controller chỉ gọi IFileMetadataService (Application layer).
/// Không biết Refit, không biết Service_Base ở đâu.
/// </summary>
[Route("api/[controller]")]
public class BaseBridgeController : BaseController
{
    private readonly IFileMetadataService _metadataService;

    public BaseBridgeController(IFileMetadataService metadataService)
    {
        _metadataService = metadataService;
    }

    /// <summary>
    /// Upload file kèm metadata từ Service_Base (user + department).
    /// Nghiệp vụ gọi Service_Base 2 lần song song (Task.WhenAll).
    /// POST /api/BaseBridge/upload-with-meta
    /// </summary>
    [HttpPost("upload-with-meta")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadWithMetadata(
        [FromForm] IFormFile file,
        [FromQuery] Guid uploaderId,
        [FromQuery] Guid departmentId,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "File is required." });

        var result = await _metadataService.UploadWithMetadataAsync(
            file.FileName, uploaderId, departmentId, ct);

        return Ok(new
        {
            success = true,
            data = new
            {
                fileName = result.FileName,
                uploader = result.UploaderName,
                department = result.DepartmentName,
                baseServiceOnline = result.IsBaseServiceAvailable
            }
        });
    }
}
