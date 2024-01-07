using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyServer.Http;

namespace CurrencyServer.UnitTests.Http
{
    [TestClass]
    public class RelativeUrlBuilderTests
    {
        private RelativeUrlBuilder _relativeUriBuilder;

        [TestInitialize]
        public void Setup()
        {
            _relativeUriBuilder = new RelativeUrlBuilder();
        }

        [TestMethod]
        public void AddPath_ValidPath_AppendsToRelativePath()
        {
            // Arrange
            const string path = "api/data";

            // Act
            _relativeUriBuilder.AddPath(path);

            // Assert
            Assert.IsTrue(_relativeUriBuilder.Build().Equals(path));
        }

        [TestMethod]
        public void AddQuery_ValidQuery_AppendsToQueryParams()
        {
            // Arrange
            const string name = "key";
            const string value = "value";

            // Act
            _relativeUriBuilder.AddQuery(name, value);

            // Assert
            Assert.IsTrue(_relativeUriBuilder.Build().Equals($"?{name}={value}"));
        }

        [TestMethod]
        public void Build_ValidPathAndQuery_ReturnsExpectedUrl()
        {
            // Arrange
            const string path = "api/data";
            const string name = "key";
            const string value = "value";
            const string expectedUrl = $"{path}?{name}={value}";

            // Act
            _relativeUriBuilder.AddPath(path);
            _relativeUriBuilder.AddQuery(name, value);

            var result = _relativeUriBuilder.Build();

            // Assert
            Assert.AreEqual(expectedUrl, result);
        }
    }
}