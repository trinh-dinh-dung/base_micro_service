using Application.Base;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Base.JWT
{
    public static class JwtHelpers
    {
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, Guid Id)
        {
            IEnumerable<Claim> claims = new Claim[]
                    {
                        new Claim("ma_user",userAccounts.ma_user??""),
                        new Claim("ten_user",userAccounts.ten_user??""),
                        new Claim("ma_donvi",userAccounts.ma_donvi??""),
                        new Claim("full_name",userAccounts.full_name??""),
                        new Claim("EmailId",userAccounts.EmailId??""),
                        new Claim("ExpC",userAccounts?.ExpC.ToString()??""),
                        new Claim("TypeLogin",userAccounts?.TypeLogin.ToString()??""),
                        new Claim(ClaimTypes.Expiration,DateTime.Now.AddHours(userAccounts.ExpC).ToString("MMM ddd dd yyyy HH:mm:ss tt"))
                    };
            return claims;
        }
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts)
        {
            return GetClaims(userAccounts, userAccounts.Id);
        }
        public static UserTokens GenTokenkey(UserTokens model, JwtSettings jwtSettings)
        {
            try
            {
                var UserToken = new UserTokens();
                if (model == null) throw new ArgumentException(nameof(model));

                // Get secret key
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);
                Guid Id = model.Id;
                DateTime expireTime = DateTime.Now.AddHours(jwtSettings.ExpaiTime);
                UserToken.Validaty = expireTime.TimeOfDay;
                var JWToken = new JwtSecurityToken(
                    issuer: jwtSettings.ValidIssuer,
                    audience: jwtSettings.ValidAudience,
                    claims: model.GetClaims(),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(expireTime).DateTime,
                    signingCredentials: new SigningCredentials
                    (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

                UserToken.Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                var idRefreshToken = Guid.NewGuid();
                UserToken.UserName = model.UserName;
                UserToken.Id = model.Id;
                UserToken.GuidId = Id;
                return UserToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
