using Dapper;
using Application.Common.Appsetting;
using Application.Exceptions;
using Application.GetMap;
using Application.IServices.DataConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StackExchange.Redis;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Base
{
    public class BaseController : ControllerBase
    {
        private readonly IOptions<Appsettings> _appsettings;
        //private readonly IDatabase _db;
        //private readonly ConnectionMultiplexer _redis;

        public BaseController(IOptions<Appsettings> appsettings = null)
        {
            _appsettings = appsettings;
            //SetCultureRequest();
        }

        protected string Sid
        {
            get
            {
                string userId = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var sellerClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals(ClaimTypes.NameIdentifier));
                    if (sellerClaim != null)
                    {
                        userId = sellerClaim.Value != null ? sellerClaim.Value : "";
                    }

                }
                return userId;
            }
        }
        /// <summary>
        /// administrator
        /// </summary>
        protected string IsAdminSystem
        {
            get
            {
                string isAministrator = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var FindisAministrator = userClaims.FirstOrDefault(uc => uc.Type.Equals("resource_access"));
                    if (FindisAministrator != null)
                    {
                        isAministrator = FindisAministrator.Value != null ? FindisAministrator.Value : "";
                    }

                }
                return isAministrator;
            }
        }

        protected bool IsSuperAdmin
        {
            get
            {
                try
                {
                    bool IsSuperadmin = false;
                    if (User.Identity.IsAuthenticated)
                    {
                        var userClaims = ((ClaimsPrincipal)User).Claims;
                        var FindisAministrator = userClaims.FirstOrDefault(uc => uc.Type.Equals("resource_access"));
                        if (FindisAministrator != null && !string.IsNullOrEmpty(FindisAministrator.Value))
                            IsSuperadmin = JObject.Parse(FindisAministrator.Value)?.SelectToken("evomes")?.SelectToken("$.roles[?(@ == 'superadmin')]")?.ToString() != null ? true : false;
                        //if (IsSuperadmin == false)
                        //    IsSuperadmin = JObject.Parse(FindisAministrator.Value)?.SelectToken("evomes")?.SelectToken("$.roles[?(@ == 'admin')]")?.ToString() != null ? true : false;
                    }
                    return IsSuperadmin;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// UserID = userNLD
        /// </summary>
        protected string UserID
        {
            get
            {
                string userId = "";
                if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(UserName))
                    return GetUserId(UserName).Result;
                return userId;
            }
        }

        protected Guid GetUserId()
        {
            string userId = "";
            if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(UserName))
                userId = GetUserId(UserName).Result;
            return !string.IsNullOrWhiteSpace(userId) ? Guid.Parse(userId) : new Guid();
        }

        /// <summary>
        /// TenantName
        /// </summary>
        protected string TenantName
        {
            get
            {
                string TenantName = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var sellerClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals("TenantName"));
                    if (sellerClaim != null)
                    {
                        TenantName = sellerClaim.Value != null ? sellerClaim.Value : "";
                    }
                    else
                    {
                        TenantName = "";
                    }
                }
                return TenantName;
            }
        }
        protected string AccessToken
        {
            get
            {
                string authorizationToken = "";
                if (User.Identity.IsAuthenticated)
                {
                    authorizationToken = HttpContext?.Request?.Headers?["Authorization"];
                }
                return authorizationToken;
            }
        }
        protected string CurrentLang
        {
            get
            {
                string lang = "vi";
                var requestCulture = HttpContext?.Features?.Get<IRequestCultureFeature>();

                if (requestCulture != null)
                    lang = requestCulture.RequestCulture.UICulture.Name;
                return lang;
            }
        }
        protected string RequesetId
        {
            get
            {
                var RequesetId = HttpContext?.Request?.Headers?["requesetid"];
                return RequesetId;
            }
        }
        protected string FullName
        {
            get
            {
                string given_name = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var sellerClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals("name"));
                    if (sellerClaim != null)
                    {
                        given_name = sellerClaim.Value != null ? sellerClaim.Value : "";
                    }
                    else
                    {
                        given_name = "";
                    }
                }
                return given_name;
            }
        }
        protected string Given_Name
        {
            get
            {
                string given_name = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var sellerClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals("given_name"));
                    if (sellerClaim != null)
                    {
                        given_name = sellerClaim.Value != null ? sellerClaim.Value : "";
                    }
                    else
                    {
                        given_name = "";
                    }
                }
                return given_name;
            }
        }
        protected string Family_Name
        {
            get
            {
                string family_name = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var sellerClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals("family_name"));
                    if (sellerClaim != null)
                    {
                        family_name = sellerClaim.Value != null ? sellerClaim.Value : "";
                    }
                    else
                    {
                        family_name = "";
                    }
                }
                return family_name;
            }
        }
        protected string EmailUser
        {
            get
            {
                string email = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var sellerClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals("email"));
                    if (sellerClaim != null)
                    {
                        email = sellerClaim.Value != null ? sellerClaim.Value : "";
                    }
                    else
                    {
                        email = "";
                    }
                }
                return email;
            }
        }
        protected string UserName
        {
            get
            {
                string UserName = "";
                if (User.Identity.IsAuthenticated)
                {
                    var userClaims = ((ClaimsPrincipal)User).Claims;
                    var sellerClaim = userClaims.FirstOrDefault(uc => uc.Type.Equals("preferred_username"));
                    if (sellerClaim != null)
                    {
                        UserName = sellerClaim.Value != null ? sellerClaim.Value : "";
                    }
                    else
                    {
                        UserName = "";
                    }
                }
                return UserName;
            }
        }

        protected void SetCulture()
        {
            var getHeaderLangue = HttpContext?.Request?.Headers?["language"];
            var currentLang = CurrentLang;
            if (!string.IsNullOrEmpty(getHeaderLangue) && getHeaderLangue.ToString() != currentLang)
            {
                string culture = getHeaderLangue;
                Response.Cookies.Append(
                   CookieRequestCultureProvider.DefaultCookieName,
                   CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                   new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
               );
            }
        }

        protected ResponseApi callApiService(string api, Method method, object dataObject = null)
        {
            try
            {
                var client = new RestClient
                {
                    BaseUrl = new Uri(_appsettings.Value.Api_Gateway)
                };
                var request = new RestRequest(api, method) { RequestFormat = DataFormat.Json };
                if (User != null)
                {
                    //request.AddHeader("Authorization", string.Format("Bearer {0}", AccessToken));
                    request.AddHeader("Authorization", string.Format(AccessToken));
                    request.AddHeader("Accept", "application/json");
                }
                if (dataObject != null)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    request.AddBody(dataObject);
#pragma warning restore CS0618 // Type or member is obsolete
                    //request.AddObject(dataObject);
                }
                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var result1 = JsonConvert.DeserializeObject<ResponseApi>(response.Content);
                    return new ResponseApi()
                    {
                        isSuccess = false,
                        status = (int)response.StatusCode,
                        message = !string.IsNullOrEmpty(result1?.message) ? result1?.message : response.StatusDescription,
                    };
                }

                var result = JsonConvert.DeserializeObject<ResponseApi>(response.Content);
                return result;
                //return new ResultApp()
                //{
                //    Success = true,
                //    Data = result,
                //    StatusCode = (int)response.StatusCode,
                //    Message = response.StatusDescription
                //};
            }
            catch (Exception e)
            {
                return new ResponseApi()
                {
                    isSuccess = false,
                    status = (int)HttpStatusCode.InternalServerError,
                    message = e.Message,
                    data = e.Data
                };
            }
        }

        protected async Task<string> GetUserId(string UserName)
        {
            var Service = HttpContext.RequestServices.GetRequiredService<IDataConfig>();
            var UserId = await Service.GetUserIDByUserName(UserName);
            return UserId;
        }

        private void SetCultureRequest()
        {
            //HttpContext neu set langue o cho nay =? truyen HttpContext
            string getHeaderLangue = HttpContext?.Request?.Headers?["language"];
            if (string.IsNullOrEmpty(getHeaderLangue))
            {
                getHeaderLangue = "vi";
            }
            var currentLang = CurrentLang;
            if (getHeaderLangue != currentLang)
            {
                string culture = getHeaderLangue;
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }
        }

        public class ResultApp
        {
            public object Data { get; set; }
            public int StatusCode { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public string MesageStatus { get; set; }
        }

    }
}
