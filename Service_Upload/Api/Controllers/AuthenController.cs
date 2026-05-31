using Api.Base;
using Api.Base.JWT;
using Application.Base;
using Application.Exceptions;
using Application.GetMap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/e-invoice-holding/Authen")]
    [ApiController]
    public class AuthenController : BaseController
    {
        private readonly JwtSettings _jwtSettings;
        public AuthenController(JwtSettings jwtSettings)
        {
            this._jwtSettings = jwtSettings;
        }

        [HttpPost("get-token")]
        public async Task<IActionResult> Login(UserLogin userLogins)
        {
            //var finUser = await _loginService.Login(userLogins);
            if (userLogins.UserName == "e_invoice@gmail.com" && userLogins.Password == "pvi123456!@#$%^2025#@!")
            {
                var Token = JwtHelpers.GenTokenkey(new UserTokens()
                {
                    GuidId = Guid.NewGuid(),
                    EmailId = "",
                    ten_user = userLogins.UserName,
                    ma_user = "000000",
                    //MaBv = finUser.ma_benhvien,
                    //ma_benhvien = finUser.ma_benhvien,
                    //ma_donvi = finUser.ma_donvi,
                    //full_name = finUser.full_name,
                    //qtht_BLVP = finUser.qtht_BLVP,
                    //TypeLogin = finUser.TypeLogin,
                    //duyet = finUser.duyet,
                    //chuyen = finUser.chuyen,
                    //huy = finUser.huy,
                    //tien_duyet = finUser.tien_duyet,
                    ExpC = _jwtSettings.ExpaiTime,
                }, _jwtSettings);
                return Ok(new ResponseApi(Token.Token, true));
            }
            else
            {
                throw new AppException("Không tìm thấy tài khoản");
            }
        }

        public class UserLogin
        {
            [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
            public string UserName { get; set; }
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            public string Password { get; set; }
            public int TypeLogin { get; set; }
            //public string? CaptchaResponse {  get; set; }
        }


    }
}
