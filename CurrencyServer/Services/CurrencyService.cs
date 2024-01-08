using CurrencyServer.Data;
using CurrencyServer.Http;
using CurrencyServer.Options;

namespace CurrencyServer.Services
{
    public class CurrencyService : ICurrencyService
    {
        private CurrencyServiceOptions _serviceOptions;
        private IExchangeRateHttpClient _exchangeRateHttpClient;

        public CurrencyService(CurrencyServiceOptions serviceOptions, IExchangeRateHttpClient exchangePairHttpClient)
        {
            _serviceOptions = serviceOptions ?? throw new ArgumentNullException(nameof(serviceOptions));
            _exchangeRateHttpClient = exchangePairHttpClient ?? throw new ArgumentNullException(nameof(exchangePairHttpClient));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CurrencyDeltaResponse>> CalculateRoundedCurrencyDeltaList(CurrencyDeltaRequest request, int numberOfDecimals = 3)
        {
            var currencyDeltas = new List<CurrencyDeltaResponse>();

            var exchangePastRate = await _exchangeRateHttpClient.GetExchangeRate(request.FromDate, request.Baseline, request.Currencies);
            var exchangeFutureRate = await _exchangeRateHttpClient.GetExchangeRate(request.ToDate, request.Baseline, request.Currencies);

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = _serviceOptions.MaxWorkers
            };

            Parallel.ForEach(request.Currencies, options, currency =>
            {
                var futureRate = exchangeFutureRate.Rates[currency];
                var pastRate = exchangePastRate.Rates[currency];

                var delta = futureRate - pastRate;

                currencyDeltas.Add(new CurrencyDeltaResponse()
                {
                    Currency = currency,
                    Delta = Math.Round(delta, numberOfDecimals)
                });
            });

            return currencyDeltas;
        }
    }
}
