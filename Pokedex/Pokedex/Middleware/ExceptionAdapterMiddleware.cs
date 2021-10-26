using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Pokedex.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Pokedex.Middleware
{
    public class ExceptionAdapterMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionAdapterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            string result = JsonConvert.SerializeObject(new { error = exception.Message });
            int code = (int)HttpStatusCode.InternalServerError;
            // add exception detail

            if (exception is IApplicationException serviceException)
            {
                code = serviceException.GetCode();
                result = JsonConvert.SerializeObject(new { error = serviceException.GetMessage() });
            }

            context.Response.StatusCode = code;
            await context.Response.WriteAsync(result);

        }

    }
}
