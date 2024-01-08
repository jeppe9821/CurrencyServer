using CurrencyServer.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CurrencyServer.Middleware
{
    /// <summary>
    /// Global exception handler middleware to return a json upon exception instead of the actual exception
    /// </summary>
    public class GenericExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _nextRequestDelegate;
        
        public GenericExceptionHandlingMiddleware(RequestDelegate nextRequestDelegate)
        {
            _nextRequestDelegate = nextRequestDelegate;
        }

        /// <summary>
        /// Invokes next request and handles any exception
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _nextRequestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = exception switch
            {
                ApplicationException _ => new BadRequestObjectResult("Application exception occurred"),
                KeyNotFoundException _ => new NotFoundObjectResult("The request key not found"),
                UnauthorizedAccessException _ => new UnauthorizedObjectResult("Unauthorized"),
                _ => new ObjectResult("Internal server error") { StatusCode = 500 }
            };

            var exceptionResponse = new GenericExceptionResponse()
            {
                StatusCode = response.StatusCode == null ? 500 : response.StatusCode.Value,
                ExceptionMessage = $"{response.Value} - {exception.Message}",
                Stacktrace = exception.StackTrace == null ? string.Empty : exception.StackTrace
            };

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsJsonAsync(exceptionResponse);
        }
    }
}
