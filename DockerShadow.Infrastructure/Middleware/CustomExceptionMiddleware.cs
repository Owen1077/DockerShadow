using DockerShadow.Core.Exceptions;
using DockerShadow.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;


namespace DockerShadow.Infrastructure.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;
        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exceptionObj)
            {
                switch (exceptionObj)
                {
                    case ApiException ex:
                        await HandleApiExceptionAsync(context, ex);
                        break;
                    case Exception ex:
                        await HandleExceptionAsync(context, ex, _logger);
                        break;
                    default:
                        break;
                }
            }
        }

        private static async Task HandleApiExceptionAsync(HttpContext context, ApiException ex)
        {
            var result = JsonConvert.SerializeObject(new Response<string>(ex.Message, ex.ErrorCode));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)ex.HttpStatusCode;
            await context.Response.WriteAsync(result);
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<CustomExceptionMiddleware> logger)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            //logger.LogError("The following error occured:" + ex.Message, ex.InnerException);
            //logger.LogError("The following stack trace:" + ex.StackTrace);
            logger.LogError(ex, "An unhandled exception has occurred while executing the request.");


            var result = JsonConvert.SerializeObject(new Response<string>(ex.Message, (int)code));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(result);
        }
    }

}
