using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyServer.Http;
using Moq;
using Newtonsoft.Json;
using CurrencyServer.Options;
using CurrencyServer.Data;
using Moq.Protected;
using System.Net;

namespace CurrencyServer.UnitTests.Http
{
    [TestClass]
    public class ExchangeRateHttpClientTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private ExchangeRateApiOptions _apiOptions;
        private ExchangeRateHttpClient _exchangeRateHttpClient;

        [TestInitialize]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _httpClient.BaseAddress = new Uri("http://example.com/");
            _apiOptions = new ExchangeRateApiOptions
            {
                BaseUrl = _httpClient.BaseAddress.ToString(),
                Version = "v1",
                SecretKey = "secretKey"
            };
            _exchangeRateHttpClient = new ExchangeRateHttpClient(_apiOptions, _httpClient);
        }

        [TestMethod]
        public async Task GetExchangeRate_ValidResponse_ReturnsExchangeRates()
        {
            // Arrange
            var exchangeRates = new ExchangeRates
            {
                Rates = new Dictionary<string, decimal>
                {
                    { "GBP", 1.0m },
                    { "SEK", 0.85m }
                }
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(exchangeRates))
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _exchangeRateHttpClient.GetExchangeRate(DateTime.Now, "GBP", new List<string> { "SEK" });

            // Assert
            Assert.AreEqual(1, result.Rates.Count); // We need to account for the removed baseline in the response
            Assert.AreEqual(exchangeRates.Rates["SEK"], result.Rates["SEK"]);
        }

        [TestMethod]
        public async Task GetExchangeRate_InvalidResponse_ThrowsHttpRequestException()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _exchangeRateHttpClient.GetExchangeRate(DateTime.Now, "GBP", new List<string> { "SEK" }));
        }

        [TestMethod]
        public async Task GetExchangeRate_InvalidJsonResponse_ThrowsJsonSerializationException()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"invalid\": \"json\" }")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<JsonSerializationException>(() => _exchangeRateHttpClient.GetExchangeRate(DateTime.Now, "GBP", new List<string> { "SEK" }));
        }
    }
}