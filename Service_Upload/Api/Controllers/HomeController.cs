//using Api.Base;
//using Application.GetMap;
//using Application.IServices;
//using Application.Request;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Localization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Api.Controllers
//{

//    [Route("api/department-service/home")]
//    [Authorize]
//    [ApiController]
//    public class HomeController : BaseController
//    {
//        private int executionCount = 0;
//        private readonly IService _service;
//        private readonly ILogger _logger;
//        public HomeController(ILogger<HomeController> logger, IService service)
//        {
//            _logger = logger;
//            _service = service;
//        }
//        [HttpGet("demo-dynamic")]
//        public async Task<IActionResult> DemoDynamic()
//        {

//            var admin = IsSuperAdmin;
//            var response = await _service.DemoDynamic();
//            return Ok(new ResponseApi(response, true));
//        }

//        //[HttpGet("test-lang")]
//        //public IActionResult TestLang()
//        //{
//        //    var lang = CurrentLang;
//        //    return Ok(new ResponseApi(lang, true));
//        //}

//        //[HttpGet("set-lang")]
//        //public IActionResult SetLanguage(string culture)
//        //{
//        //    Response.Cookies.Append(
//        //        CookieRequestCultureProvider.DefaultCookieName,
//        //        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
//        //        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
//        //    );
//        //    return Ok(new ResponseApi(culture, true));
//        //}

//        //[HttpGet("tes-call-api-controller")]
//        //public async Task<IActionResult> testCallApiController(CancellationToken stoppingToken)
//        //{
//        //    while (!stoppingToken.IsCancellationRequested)
//        //    {

//        //        executionCount++;
//        //        _logger.LogInformation(
//        //            "Scoped Processing Controller is working. Count: {Count}", executionCount);
//        //        await Task.Delay(200, stoppingToken);
//        //    }
//        //    return Ok(new ResponseApi(true, true));
//        //}
//    }
//}
