//using Application.GetMap;
//using Application.IServices;
//using Application.Request;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System.Threading.Tasks;
//using System;

//namespace Api.Controllers
//{
//    /// <summary>
//    /// service quan ly chuc vu
//    /// </summary>
//    [Route("api/department-service/position")]
//    [ApiController]
//    public class PositionController : ControllerBase
//    {
//        private readonly IService _service;
//        private readonly ILogger _logger;
//        public PositionController(ILogger<PositionController> logger, IService service)
//        {
//            _logger = logger;
//            _service = service;
//        }

//        /// <summary>
//        /// tao moi chuc vu
//        /// </summary>
//        /// <param name="Request"></param>
//        /// <returns></returns>
//        [HttpPost("create")]
//        public async Task<IActionResult> Create(PositionRequest Request)
//        {
//            var response = await _service.CreatePosition(Request);
//            return Ok(new ResponseApi(response, true));
//        }

//        /// <summary>
//        /// cap nhat chuc vu
//        /// </summary>
//        /// <param name="Request"></param>
//        /// <returns></returns>
//        [HttpPost("update")]
//        public async Task<IActionResult> Update(PositionRequest Request)
//        {
//            var response = await _service.UpdatePosition(Request);
//            return Ok(new ResponseApi(response, true));
//        }

//        /// <summary>
//        /// xoa chuc vu
//        /// </summary>
//        /// <param name="UserId">id user</param>
//        /// <returns></returns>
//        [HttpGet("delete")]
//        public async Task<IActionResult> Delete(Guid UserId)
//        {
//            var response = await _service.DeletePosition(UserId);
//            return Ok(new ResponseApi(response, true));
//        }


//        /// <summary>
//        /// lay danh sach chuc vu trong he thong
//        /// </summary>
//        /// <param name="UserId">id user</param>
//        /// <returns></returns>
//        [HttpGet("get-list-all-posititon")]
//        public async Task<IActionResult> GetListAllPosition()
//        {
//            var response = await _service.GetListAllPosition();
//            return Ok(new ResponseApi(response, true));
//        }

//    }
//}
