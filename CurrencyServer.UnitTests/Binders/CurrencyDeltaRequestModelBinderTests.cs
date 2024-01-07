using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using Microsoft.AspNetCore.Http;
using CurrencyServer.Binders;
using CurrencyServer.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CurrencyServer.UnitTests.Binders
{
    [TestClass]
    public class CurrencyDeltaRequestModelBinderTests
    {
        private CurrencyDeltaRequestModelBinder _modelBinder;

        [TestInitialize]
        public void Setup()
        {
            _modelBinder = new CurrencyDeltaRequestModelBinder();
        }

        [TestMethod]
        public async void BindModelAsync_ValidRequest_ReturnsTask()
        {
            // Arrange
            var request = new CurrencyDeltaRequest
            {
                Baseline = "usd",
                Currencies = new List<string> { "eur", "gbp" }
            };

            var requestBody = System.Text.Json.JsonSerializer.Serialize(request);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            var actionContext = new ActionContext()
            {
                HttpContext = httpContext,
            };

            var provider = new EmptyModelMetadataProvider();
            var metadata = provider.GetMetadataForType(typeof(CurrencyDeltaRequest));

            var bindingContext = new DefaultModelBindingContext
            {
                ModelMetadata = metadata,
                ModelState = new ModelStateDictionary(),
                ActionContext = actionContext,
                ModelName = nameof(CurrencyDeltaRequest),
                BindingSource = BindingSource.Body
            };

            // Act
            await _modelBinder.BindModelAsync(bindingContext);

            // Assert
            var model = bindingContext.Result.Model as CurrencyDeltaRequest;
            Assert.IsNotNull(model);
            Assert.AreEqual(request.Baseline.ToUpperInvariant(), model.Baseline);
            Assert.IsTrue(request.Currencies.Select(x => x.ToUpperInvariant()).SequenceEqual(model.Currencies));
        }

        [TestMethod]
        public void BindModelAsync_InvalidJson_JsonException()
        {
            // Arrange
            var requestBody = "invalid json";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            var actionContext = new ActionContext()
            {
                HttpContext = httpContext,
            };

            var provider = new EmptyModelMetadataProvider();
            var metadata = provider.GetMetadataForType(typeof(CurrencyDeltaRequest));

            var bindingContext = new DefaultModelBindingContext
            {
                ModelMetadata = metadata,
                ModelState = new ModelStateDictionary(),
                ActionContext = actionContext,
                ModelName = nameof(CurrencyDeltaRequest),
                BindingSource = BindingSource.Body
            };

            // Act
            Assert.ThrowsExceptionAsync<JsonException>(async () => await _modelBinder.BindModelAsync(bindingContext));

            // Assert
            Assert.IsFalse(bindingContext.Result.IsModelSet);
        }
    }
}