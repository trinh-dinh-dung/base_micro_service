using Application.GetMap;
using Application.IServices;
using Application.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/department-service/department-dapper")]
    [ApiController]
    public class DepartmentDapperController : ControllerBase
    {
        private readonly IDepartmentDapperService _departmentDapperService;
        private readonly ILogger _logger;

        public DepartmentDapperController(
            ILogger<DepartmentDapperController> logger,
            IDepartmentDapperService departmentDapperService)
        {
            _logger = logger;
            _departmentDapperService = departmentDapperService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDepartment(DepartmentRequest request)
        {
            var response = await _departmentDapperService.CreateDepartmentAsync(request);
            return Ok(new ResponseApi(response, true));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateDepartment(DepartmentRequest request)
        {
            var response = await _departmentDapperService.UpdateDepartmentAsync(request);
            return Ok(new ResponseApi(response, true));
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DeleteDepartment(Guid departmentId)
        {
            var response = await _departmentDapperService.DeleteDepartmentAsync(departmentId);
            return Ok(new ResponseApi(response, true));
        }

        [HttpGet("get-list-department-by-parent-id")]
        public async Task<IActionResult> GetListDepartmentByParentId(Guid? parentId)
        {
            var data = await _departmentDapperService.GetListDepartmentByParentIdAsync(parentId);
            return Ok(new ResponseApi(data, true));
        }
    }
}
