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
                return;
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

                model.Baseline = model.Baseline.ToUpperInvariant();
                model.Currencies = model.Currencies.Select(x => x.ToUpperInvariant());

                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }

}
