﻿using CurrencyServer.Data;
using CurrencyServer.Options;
using Newtonsoft.Json;

namespace CurrencyServer.Http
{

    public class ExchangeRateHttpClient : IExchangeRateHttpClient
    {
        private readonly string _exchangeRateApiBaseUrl;
        private readonly string _exchangeApiVersion;
        private readonly string _apiKey;

        private readonly HttpClient _httpClient;

        public ExchangeRateHttpClient(ExchangeRateApiOptions apiOptions, HttpClient httpClient)
        {
            _exchangeRateApiBaseUrl = apiOptions.BaseUrl ?? throw new ArgumentException(nameof(apiOptions));
            _exchangeApiVersion = apiOptions.Version ?? throw new ArgumentException(nameof(apiOptions));
            _apiKey = apiOptions.SecretKey ?? throw new ArgumentException(nameof(apiOptions));

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc/>
        public async Task<ExchangeRates> GetExchangeRate(DateTime date, string baseline, IEnumerable<string> currencies)
        {
            var mergedCurrencies = string.Join(",", currencies);

            //The baseline has to be added for the computation, but it should be only be added if the user already haven't added it themselves as they intend it to be viewable
            var addBaselineToSymbols = !currencies.Contains(baseline);

            var symbolsQuery = addBaselineToSymbols
                ? $"{mergedCurrencies},{baseline}"
                : mergedCurrencies;

            var relativeUri = new RelativeUrlBuilder()
                .AddPath(_exchangeApiVersion)
                .AddPath(date.ToString("yyyy-MM-dd"))
                .AddQuery("access_key", _apiKey)
                .AddQuery("symbols", symbolsQuery)
                .Build();

            HttpResponseMessage response = await _httpClient.GetAsync(relativeUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var exchangeRates = JsonConvert.DeserializeObject<ExchangeRates>(content);

            if (exchangeRates == null || exchangeRates.Rates == null)
            {
                throw new JsonSerializationException($"Failed to deserialize API response into {nameof(ExchangeRates)} object using content {content}. Please validate API versions");
            }

            if(!exchangeRates.Rates.Keys.Contains(baseline))
            {
                throw new ArgumentException($"The baseline is not a valid currency. Current baseline: {baseline}");
            }

            var currenciesOffset = addBaselineToSymbols ? 1 : 0;

            if (!exchangeRates.Rates.Keys.Count.Equals(currencies.Count() + currenciesOffset)) //+currenciesOffset to account for the baseline
            {
                throw new ArgumentException($"One of the currencies supplied is invalid. Current currencies supplied: {string.Join(",", currencies)}");
            }

            /*
             * Free tier of API does not support using baseline rates therefor we need to convert it manually
             */

            //Cache baseline rate for computation
            var cacheBaselineRate = exchangeRates.Rates[baseline];

            //Remove the baseline after caching to remove unescessary computation
            if(addBaselineToSymbols) //if the baseline was added for the hack we don't want to remove it as the user intends to view it (even though the delta will always be 0.0)
            {
                exchangeRates.Rates.Remove(baseline);
            }

            //Remap currency rates to the real baseline
            exchangeRates.Rates = exchangeRates.Rates.ToDictionary(kv => kv.Key, kv => kv.Value / cacheBaselineRate);

            return exchangeRates;
        }
    }
}
