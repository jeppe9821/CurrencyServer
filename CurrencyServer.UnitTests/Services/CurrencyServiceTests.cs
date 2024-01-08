using CurrencyServer.Data;
using CurrencyServer.Http;
using CurrencyServer.Options;
using CurrencyServer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CurrencyServer.UnitTests
{
    [TestClass]
    public class CurrencyServiceTests
    {
        private CurrencyService _sut;
        private Mock<IExchangeRateHttpClient> _mockHttpClient;

        [TestInitialize]
        public void Setup()
        {
            _mockHttpClient = new Mock<IExchangeRateHttpClient>();
             
            var options = new CurrencyServiceOptions { MaxWorkers = 4 };
            _sut = new CurrencyService(options, _mockHttpClient.Object);
        }

        [TestMethod]
        public async Task GetCurrencyDeltaList_ValidRequest_ReturnsExpectedResult()
        {
            // Arrange
            var request = new CurrencyDeltaRequest
            {
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now,
                Baseline = "GBP",
                Currencies = new List<string> { "SEK" }
            };

            var pastRate = new ExchangeRates { Rates = new Dictionary<string, decimal> { { "SEK", 0.85m } } };
            var futureRate = new ExchangeRates { Rates = new Dictionary<string, decimal> { { "SEK", 0.86m } } };

            _mockHttpClient.Setup(x => x.GetExchangeRate(request.FromDate, request.Baseline, request.Currencies))
                .ReturnsAsync(pastRate);

            _mockHttpClient.Setup(x => x.GetExchangeRate(request.ToDate, request.Baseline, request.Currencies))
                .ReturnsAsync(futureRate);

            // Act
            var result = await _sut.CalculateRoundedCurrencyDeltaList(request);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("SEK", result.First().Currency);
            Assert.AreEqual(0.01m, result.First().Delta);
        }

        [TestMethod]
        public void GetCurrencyDeltaList_EmptyCurrencies_ThrowsArgumentException()
        {
            // Arrange
            var request = new CurrencyDeltaRequest
            {
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now,
                Baseline = "GBP",
                Currencies = new List<string>()
            };

            // Act & Assert
            Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _sut.CalculateRoundedCurrencyDeltaList(request));
        }

        [TestMethod]
        public void GetCurrencyDeltaList_NullRequest_ThrowsArgumentNullException() 
        {
            // Arrange
            CurrencyDeltaRequest? request = null;

            // Act & Assert
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _sut.CalculateRoundedCurrencyDeltaList(request));
        }
    }
}