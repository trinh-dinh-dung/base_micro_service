using Api.Controllers;
using Application.Common.Appsetting;
using Application.Exceptions;
using Domain.Exceptions;
using Application.Abstractions.Messaging;
using Application.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Application.GetMap;

namespace Api.Base
{
    public class ErrorHandlerMiddleware
    {
        public readonly IRabbitMQClient _rabbitMQClient;
        private readonly RequestDelegate _next;
        private readonly IOptions<Appsettings> _appsettings;
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, IRabbitMQClient rabbitMQClient, IOptions<Appsettings> appsettings, IWebHostEnvironment env, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _rabbitMQClient = rabbitMQClient;
            _appsettings = appsettings;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //var queuesRequest = new QueuesRequest()
                //{
                //    QueueId="",
                //    QueueName="",
                //    QueueContent="",
                //    Rabbitmq_Queue_Name=_appsettings.Value.Rabbitmq_Queue_Log_Service,
                //    QueueStatus=1,
                //};
                //_rabbitMQClient.SendRabbitMQClientQueues(queuesRequest);

                //SetCulture(context);
                await _next(context);
            }
            catch (Exception exception)
            {

                _logger.LogError(exception, $"Logs exception {Assembly.GetExecutingAssembly().GetName().Name.ToLower()} - {_env.EnvironmentName}");

                // start save log
                // code here
                // end save log

                var response = context.Response;
                response.ContentType = "application/json";
                var message = exception?.Message;
                switch (exception)
                {
                    case AppException:
                    case DomainException:
                        response.StatusCode = 999;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        if (_env.EnvironmentName == "Production")
                        {
                            message = "Lỗi phát sinh";
                        }
                        break;
                }

                var result = JsonSerializer.Serialize(new ResponseApi()
                {
                    message = message,
                    data = null,
                    isSuccess = false
                });
                await response.WriteAsync(result);
                //var queuesRequest = new QueuesRequest()
                //{
                //    QueueId="",
                //    QueueName="",
                //    QueueContent="",
                //    Rabbitmq_Queue_Name=_appsettings.Value.Rabbitmq_Queue_Log_Service,
                //    QueueStatus=1,
                //};
                //_rabbitMQClient.SendRabbitMQClientQueues(queuesRequest);
            }
        }

        //private void SetCulture(HttpContext context)
        //{
        //    string getHeaderLangue = context?.Request?.Headers?["language"];
        //    if (string.IsNullOrEmpty(getHeaderLangue))
        //    {
        //        getHeaderLangue = "vi";
        //    }
        //    var currentLang = CurrentLang(context);
        //    if (getHeaderLangue != currentLang)
        //    {
        //        string culture = getHeaderLangue;
        //        context.Response.Cookies.Append(
        //            CookieRequestCultureProvider.DefaultCookieName,
        //            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        //            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        //        );
        //    }
        //}

        //private string CurrentLang(HttpContext context)
        //{
        //    string lang = "vi";
        //    var requestCulture = context?.Features?.Get<IRequestCultureFeature>();

        //    if (requestCulture != null)
        //        lang = requestCulture.RequestCulture.UICulture.Name;
        //    return lang;
        //}
    }
}
