using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyServer.Http;

namespace CurrencyServer.UnitTests.Http
{
    [TestClass]
    public class UrlHelperTests
    {
        [TestMethod]
        public void FixBaseAddress_UrlEndsWithSlash_ReturnsSameUrl()
        {
            // Arrange
            const string url = "http://foo.com/";

            // Act
            var result = UrlHelper.FixBaseAddress(url);

            // Assert   
            Assert.AreEqual(url, result);
        }

        [TestMethod]
        public void FixBaseAddress_UrlDoesNotEndWithSlash_ReturnsUrlWithSlash()
        {
            // Arrange
            const string url = "http://foo.com";
            const string expectedUrl = "http://foo.com/";

            // Act
            var result = UrlHelper.FixBaseAddress(url);

            // Assert
            Assert.AreEqual(expectedUrl, result);
        }

        [TestMethod]
        public void FixBaseAddress_NullUrl_ThrowsArgumentException()
        {
            // Arrange
            string? url = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => UrlHelper.FixBaseAddress(url));
        }

        [TestMethod]
        public void FixBaseAddress_EmptyUrl_ThrowsArgumentException()
        {
            // Arrange
            var url = string.Empty;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => UrlHelper.FixBaseAddress(url));
        }
    }
}