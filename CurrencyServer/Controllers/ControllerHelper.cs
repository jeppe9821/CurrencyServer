using CurrencyServer.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CurrencyServer.Controllers
{
    /// <summary>
    /// General purpose class for helper methods for Controller classes
    /// </summary>
    public static class ControllerHelper
    {
        public static ActionResult CreateResponse(HttpStatusCode statusCode, string errorCode, string errorDetails)
        {
            var errorResponse = new ApiRequestErrorResponse
            {
                ErrorCode = errorCode,
                ErrorDetails = errorDetails
            };

            return statusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestObjectResult(errorResponse),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(errorResponse),
                HttpStatusCode.NotFound => new NotFoundObjectResult(errorResponse),
                _ => new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                }
            };
        }
    }
}
