//using Application.GetMap;
//using Application.IServices;
//using Application.Request;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading.Tasks;

//namespace Api.Controllers
//{
//    /// <summary>
//    /// service quan ly user
//    /// </summary>
//    [Route("api/department-service/user")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly IService _service;
//        private readonly ILogger _logger;
//        public UserController(ILogger<UserController> logger, IService service)
//        {
//            _logger = logger;
//            _service = service;
//        }

//        /// <summary>
//        /// tao moi user
//        /// </summary>
//        /// <param name="Request"></param>
//        /// <returns></returns>
//        [HttpPost("create")]
//        public async Task<IActionResult> Create(UserRequest Request)
//        {
//            var response = await _service.CreateUser(Request);
//            return Ok(new ResponseApi(response, true));
//        }

//        /// <summary>
//        /// cap nhat user
//        /// </summary>
//        /// <param name="Request"></param>
//        /// <returns></returns>
//        [HttpPost("update")]
//        public async Task<IActionResult> Update(UserRequest Request)
//        {
//            var response = await _service.UpdateUser(Request);
//            return Ok(new ResponseApi(response, true));
//        }

//        /// <summary>
//        /// xoa user
//        /// </summary>
//        /// <param name="UserId">id user</param>
//        /// <returns></returns>
//        [HttpGet("delete")]
//        public async Task<IActionResult> Delete(Guid UserId)
//        {
//            var response = await _service.DeleteUser(UserId);
//            return Ok(new ResponseApi(response, true));
//        }
//        /// <summary>
//        /// get thong tin user
//        /// </summary>
//        /// <param name="UserId">id user</param>
//        /// <returns></returns>
//        [HttpGet("get-user-by-id")]
//        public async Task<IActionResult> GetUserById(Guid UserId)
//        {
//            var response = await _service.GetUserById(UserId);
//            return Ok(new ResponseApi(response, true));
//        }


//    }
//}
