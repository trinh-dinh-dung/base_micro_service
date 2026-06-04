using Application.GetMap;
using Application.IServices;
using Application.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// service quan ly phong ban
    /// </summary>
    [Route("api/department-service/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger _logger;

        public DepartmentController(ILogger<DepartmentController> logger, IDepartmentService departmentService)
        {
            _logger = logger;
            _departmentService = departmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDepartment(DepartmentRequest departmentRequest, CancellationToken cancellationToken)
        {
            var response = await _departmentService.CreateDepartment(departmentRequest, cancellationToken);
            return Ok(new ResponseApi(response, true));
        }

        [HttpPost("create-test-1")]
        public async Task<IActionResult> CreateDepartmentTest(DepartmentRequest departmentRequest, CancellationToken cancellationToken)
        {
            var response = await _departmentService.CreateDepartment(departmentRequest, cancellationToken);
            return Ok(new ResponseApi(response, true));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateDepartment(DepartmentRequest departmentRequest, CancellationToken cancellationToken)
        {
            var response = await _departmentService.UpdateDepartment(departmentRequest, cancellationToken);
            return Ok(new ResponseApi(response, true));
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DeleteDepartment([Required] Guid departmentId, CancellationToken cancellationToken)
        {
            var response = await _departmentService.DeleteDepartment(departmentId, cancellationToken);
            return Ok(new ResponseApi(response, true));
        }

        [HttpGet("get-list-department-by-parent-id")]
        public async Task<IActionResult> GetListDepartmentByParentId(Guid? parentId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetListDepartmentByParentId called. parentId={ParentId}", parentId);
            var response = await _departmentService.GetListDepartmentByParentId(parentId, cancellationToken);
            _logger.LogInformation("GetListDepartmentByParentId result: {@Response}", response);
            return Ok(new ResponseApi(response, true));
        }

        [HttpGet("get-audit-logs")]
        public async Task<IActionResult> GetDepartmentAuditLogs([Required] Guid departmentId, int limit = 20, string lang = "vi", CancellationToken cancellationToken = default)
        {
            var response = await _departmentService.GetDepartmentAuditLogs(departmentId, limit, lang, cancellationToken);
            return Ok(new ResponseApi(response, true));
        }

        [HttpPost("sync-audit-metadata")]
        public async Task<IActionResult> SyncAuditMetadata([FromBody] AuditMetadataSyncRequest request, CancellationToken cancellationToken)
        {
            var response = await _departmentService.SyncAuditMetadata(request, cancellationToken);
            return Ok(new ResponseApi(response, true));
        }
    }
}
