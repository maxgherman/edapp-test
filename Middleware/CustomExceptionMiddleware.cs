using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using EdAppTest.Exceptions;
using Microsoft.AspNetCore.Http;

namespace EdAppTest.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate next;
        
        public CustomExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is HttpRequestException)
            {
                var httpException = exception as HttpRequestException;
                context.Response.StatusCode = (int)httpException.HttpStatusCode;
                await context.Response.WriteAsync(exception.Message);
            }
            else
            {
                var errorResponse = await GenerateServerResponse(context, exception);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(errorResponse);
            }
        }

        public async Task<string> GenerateServerResponse(HttpContext context, Exception exception)
        {
            var body = string.Empty;
            using (var stream = new StreamReader(context.Request?.Body))
            {
                body = await stream.ReadToEndAsync();
            }

            return JsonSerializer.Serialize(
                new
                {
                    path = context.Request.Path,
                    body,
                    method = context.Request.Method,
                    message = exception?.Message,
                    innerMessage = exception?.InnerException?.Message,
                    exceptionType = exception.GetType().Name
                },
                new JsonSerializerOptions { WriteIndented = true }
            );
        }
    }
}