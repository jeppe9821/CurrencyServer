using CurrencyServer.Controllers;
using CurrencyServer.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace CurrencyServer.UnitTests.Controllers
{
    [TestClass]
    public class ControllerHelperTests
    {
        [TestMethod]
        public void CreateResponse_StatusCodeBadRequest_ReturnsBadRequestObjectResult()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            const string errorCode = "SomeErrorCode";
            const string errorDetails = "Some error details";
            const int expectedStatusCode = (int)HttpStatusCode.BadRequest;

            // Act
            var result = ControllerHelper.CreateResponse(statusCode, errorCode, errorDetails) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(ApiRequestErrorResponse));
        }

        [TestMethod]
        public void CreateResponse_StatusCodeUnathorized_ReturnsUnathorizedObjectResult()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.Unauthorized;
            const string errorCode = "SomeErrorCode";
            const string errorDetails = "Some error details";
            const int expectedStatusCode = (int)HttpStatusCode.Unauthorized;

            // Act
            var result = ControllerHelper.CreateResponse(statusCode, errorCode, errorDetails) as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(ApiRequestErrorResponse));
        }

        [TestMethod]
        public void CreateResponse_StatusCodeNotFound_ReturnsNotFoundObjectResult()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.NotFound;
            const string errorCode = "SomeErrorCode";
            const string errorDetails = "Some error details";
            const int expectedStatusCode = (int)HttpStatusCode.NotFound;

            // Act
            var result = ControllerHelper.CreateResponse(statusCode, errorCode, errorDetails) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(ApiRequestErrorResponse));
        }

        [TestMethod]
        public void CreateResponse_InternalServerError_ReturnsInternalServerErrorObjectResult()
        {
            // Arrange
            const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            const string errorCode = "SomeErrorCode";
            const string errorDetails = "Some error details";
            const int expectedStatusCode = (int)HttpStatusCode.InternalServerError;

            // Act
            var result = ControllerHelper.CreateResponse(statusCode, errorCode, errorDetails) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
            Assert.IsInstanceOfType(result.Value, typeof(ApiRequestErrorResponse));
        }
    }
}
