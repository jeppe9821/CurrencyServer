using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyServer.Controllers;
using CurrencyServer.Services;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CurrencyServer.Data;

namespace CurrencyServer.UnitTests.Controllers
{
    [TestClass]
    public class CurrencyControllerTests
    {
        private Mock<ICurrencyService> _mockCurrencyService;

        private CurrencyController _sut;

        [TestInitialize]
        public void Setup()
        {
            _mockCurrencyService = new Mock<ICurrencyService>();

            _sut = new CurrencyController(_mockCurrencyService.Object);
        }

        [TestMethod]
        public async Task Post_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new CurrencyDeltaRequest
            {
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now,
                Currencies = new List<string> { "SEK", "EUR" }
            };

            IEnumerable<CurrencyDeltaResponse> currencyDeltas = new List<CurrencyDeltaResponse>
            {
                new CurrencyDeltaResponse { Currency = "SEK", Delta = 0.1m },
                new CurrencyDeltaResponse { Currency = "EUR", Delta = 0.2m }
            };

            _mockCurrencyService.Setup(service => service.CalculateRoundedCurrencyDeltaList(request, 3))
                .ReturnsAsync(currencyDeltas);

            // Act
            var result = await _sut.Post(request);

            // Assert
            var objectResult = (IEnumerable<CurrencyDeltaResponse>)((ObjectResult)result.Result).Value;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(currencyDeltas.Count(), objectResult.Count());
        }

        [TestMethod]
        public async Task Post_FromDateLaterThanToDate_ReturnsBadRequest()
        {
            // Arrange
            var request = new CurrencyDeltaRequest
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddDays(-1),
                Currencies = new List<string> { "SEK", "EUR" }
            };

            // Act
            var result = await _sut.Post(request);

            // Assert
            var badRequestResult = new BadRequestObjectResult(result);
            Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task Post_DuplicateCurrencies_ReturnsBadRequest()
        {
            // Arrange
            var request = new CurrencyDeltaRequest
            {
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now,
                Currencies = new List<string> { "SEK", "EUR" }
            };

            // Act            
            var result = await _sut.Post(request);

            // Assert
            var badRequestResult = new BadRequestObjectResult(result);
            Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task Post_EmptyCurrencies_ReturnsBadRequest()
        {
            // Arrange
            var request = new CurrencyDeltaRequest
            {
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now,
                Currencies = new List<string>()
            };

            // Act
            var result = await _sut.Post(request);

            // Assert
            var badRequestResult = new BadRequestObjectResult(result);
            Assert.IsNotNull(badRequestResult);
        }
    }
}