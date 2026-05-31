
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Threading.Tasks;
namespace Api.Base
{

    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string cultureQuery = context?.Request?.Headers?["language"];
            //if (!string.IsNullOrWhiteSpace(cultureQuery) && CurrentLang(context) != cultureQuery)
            //{
            if (string.IsNullOrEmpty(cultureQuery))
            {
                cultureQuery = "vi";
            }
            var culture = new CultureInfo(cultureQuery);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            //}
            await _next(context);
        }
        private string CurrentLang(HttpContext context)
        {
            string lang = "vi";
            var requestCulture = context?.Features?.Get<IRequestCultureFeature>();

            if (requestCulture != null)
                lang = requestCulture.RequestCulture.UICulture.Name;
            return lang;
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCultureMiddleware>();
        }
    }
}
