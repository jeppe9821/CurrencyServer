using CurrencyServer.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace CurrencyServer.Binders
{
    /// <summary>
    /// A model binder for the CurrencyDeltaRequest which modifies the request before being sent to the Controller
    /// </summary>
    public class CurrencyDeltaRequestModelBinder : IModelBinder
    {
        /// <inheritdoc/>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                var requestBody = await reader.ReadToEndAsync();

                var model = JsonSerializer.Deserialize<CurrencyDeltaRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if(model == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                if(string.IsNullOrEmpty(model.Baseline))
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid request. Baseline cannot be null or empty");
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                if (model.Currencies == null || !model.Currencies.Any())
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid request. Currencies cannot be null or empty");
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                if (model.FromDate == DateTime.MinValue)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid request. From date cannot be empty");
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                if (model.ToDate == DateTime.MinValue)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid request. To date cannot be empty");
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                model.Baseline = model.Baseline.ToUpperInvariant();
                model.Currencies = model.Currencies.Select(x => x.ToUpperInvariant());

                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }

}
