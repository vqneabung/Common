using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Common.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                await HandleCommonStatusCodesAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var (statusCode, message) = ex switch
                {
                    TaskCanceledException or TimeoutException =>
                        (StatusCodes.Status408RequestTimeout, "The request has timed out. Please try again later."),

                    _ => (StatusCodes.Status500InternalServerError, "Sorry, an internal server error occurred. Kindly try again.")
                };

                _logger.LogWarning("Request resulted in status code {StatusCode}: {Message}", statusCode, message);
                await WriteJsonResponseAsync(context, statusCode, BaseResponse<string>.Fail(message));
            }
        }

        private async Task HandleCommonStatusCodesAsync(HttpContext context)
        {
            if (context.Response.HasStarted) return;

            var statusCode = context.Response.StatusCode;
            string? message = statusCode switch
            {
                StatusCodes.Status401Unauthorized => "You are not authorized to access this resource.",
                StatusCodes.Status403Forbidden => "You do not have permission to access this resource.",
                StatusCodes.Status429TooManyRequests => "Too many requests. Please try again later.",
                _ => null
            };

            if (message != null)
            {
                await WriteJsonResponseAsync(context, statusCode, BaseResponse<string>.Fail(message));
            }
        }

        private static async Task WriteJsonResponseAsync(HttpContext context, int statusCode, BaseResponse<string> response)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
