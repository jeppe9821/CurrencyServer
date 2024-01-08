using CurrencyServer.ErrorHandling;
using CurrencyServer.Http;
using CurrencyServer.Options;
using CurrencyServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace CurrencyServer
{
    public static class RegistrationExtensions
    {
        public static void AddCurrencyService(this IServiceCollection @this, IConfigurationSection currencyOptionsConfigurationSection)
        {
            var options = currencyOptionsConfigurationSection.Get<CurrencyServiceOptions>();
            currencyOptionsConfigurationSection.Bind(nameof(CurrencyServiceOptions), options);
            @this.AddSingleton(options);

            @this.AddSingleton<ICurrencyService, CurrencyService>();
        }

        public static void AddExchangeRateHttpClient(this IServiceCollection @this, IConfiguration configuration)
        {
            var apiOptions = configuration.Get<ExchangeRateApiOptions>();
            apiOptions.SecretKey = configuration.GetValue<string>(EnvironmentVariableConstants.SecretKeyName);
            configuration.Bind(nameof(ExchangeRateApiOptions), apiOptions);
            @this.AddSingleton(apiOptions);

            @this.AddHttpClient<IExchangeRateHttpClient, ExchangeRateHttpClient>((provider, client) =>
            {
                var exchangeRateConfig = provider.GetRequiredService<ExchangeRateApiOptions>();

                client.BaseAddress = new Uri(UrlHelper.FixBaseAddress(exchangeRateConfig.BaseUrl));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            });
        }

        public static void ConfigureApiBehaviourOptions(this IServiceCollection @this)
        {
            @this.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var error = actionContext.ModelState
                        .FirstOrDefault(e => e.Value?.Errors.Count > 0);

                    const string errorCode = "invalidModel";
                    var errorDetails = error.Value?.Errors.FirstOrDefault()?.ErrorMessage;

                    var formattedErrorResponse = new ApiRequestErrorResponse()
                    {
                        ErrorCode = errorCode,
                        ErrorDetails = errorDetails == null ? "An internal error occured - could not fetch error message" : errorDetails
                    };

                    return new BadRequestObjectResult(formattedErrorResponse);
                };
            });
        }
    }
}
