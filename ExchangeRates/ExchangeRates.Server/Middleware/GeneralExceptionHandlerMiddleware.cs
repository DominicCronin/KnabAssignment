using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeRates.Server.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GeneralExceptionHandlerMiddleware(ILogger<GeneralExceptionHandlerMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // TODO - say sorry nicely in the client
                logger.LogError(ex, "An error occurred while processing the request.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                ProblemDetails problemDetails = new()
                {
                    Detail = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred while processing the request."
                };

                var errorMessage = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(errorMessage);
            }
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGeneralExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GeneralExceptionHandlerMiddleware>();
        }
    }
}
