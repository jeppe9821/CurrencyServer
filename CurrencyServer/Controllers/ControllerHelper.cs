using CurrencyServer.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CurrencyServer.Controllers
{
    public class ControllerHelper
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
