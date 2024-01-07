using CurrencyServer.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Mime;

namespace CurrencyServer.Tests.Middleware
{
    public class GenericExceptionHandlingMiddlewareTests
    {
        private Mock<RequestDelegate> _nextRequestDelegateMock;
        private Mock<HttpContext> _httpContextMock;
        private Mock<HttpResponse> _httpResponseMock;

        [TestInitialize]
        public void Setup()
        {
            _nextRequestDelegateMock = new Mock<RequestDelegate>();
            _httpContextMock = new Mock<HttpContext>();
            _httpResponseMock = new Mock<HttpResponse>();

            _httpContextMock.SetupGet(c => c.Response).Returns(_httpResponseMock.Object);
        }

        [TestMethod]
        public async Task InvokeAsync_ValidRequest_ReturnsTask()
        {
            // Arrange
            var middleware = new GenericExceptionHandlingMiddleware(_nextRequestDelegateMock.Object);

            // Act
            await middleware.InvokeAsync(_httpContextMock.Object);

            // Assert
            _nextRequestDelegateMock.Verify(next => next(_httpContextMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_ApplicationException_WritesJson()
        {
            // Arrange
            var middleware = new GenericExceptionHandlingMiddleware(_nextRequestDelegateMock.Object);
            var exception = new ApplicationException();

            _nextRequestDelegateMock.Setup(next => next(_httpContextMock.Object)).Throws(exception);

            // Act
            await middleware.InvokeAsync(_httpContextMock.Object);

            // Assert
            _httpResponseMock.VerifySet(response => response.ContentType = MediaTypeNames.Application.Json, Times.Once);
            _httpResponseMock.Verify(response => response.WriteAsJsonAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_KeyNotFoundException_WritesJson()
        {
            // Arrange
            var middleware = new GenericExceptionHandlingMiddleware(_nextRequestDelegateMock.Object);
            var exception = new KeyNotFoundException();

            _nextRequestDelegateMock.Setup(next => next(_httpContextMock.Object)).Throws(exception);

            // Act
            await middleware.InvokeAsync(_httpContextMock.Object);

            // Assert
            _httpResponseMock.VerifySet(response => response.ContentType = MediaTypeNames.Application.Json, Times.Once);
            _httpResponseMock.Verify(response => response.WriteAsJsonAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_UnathorizedAccessException_WritesJson()
        {
            // Arrange
            var middleware = new GenericExceptionHandlingMiddleware(_nextRequestDelegateMock.Object);
            var exception = new UnauthorizedAccessException();

            _nextRequestDelegateMock.Setup(next => next(_httpContextMock.Object)).Throws(exception);

            // Act
            await middleware.InvokeAsync(_httpContextMock.Object);

            // Assert
            _httpResponseMock.VerifySet(response => response.ContentType = MediaTypeNames.Application.Json, Times.Once);
            _httpResponseMock.Verify(response => response.WriteAsJsonAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_Exception_WritesJson()
        {
            // Arrange
            var middleware = new GenericExceptionHandlingMiddleware(_nextRequestDelegateMock.Object);
            var exception = new Exception();

            _nextRequestDelegateMock.Setup(next => next(_httpContextMock.Object)).Throws(exception);

            // Act
            await middleware.InvokeAsync(_httpContextMock.Object);

            // Assert
            _httpResponseMock.VerifySet(response => response.ContentType = MediaTypeNames.Application.Json, Times.Once);
            _httpResponseMock.Verify(response => response.WriteAsJsonAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}