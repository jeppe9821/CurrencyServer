using CurrencyServer.Data;

namespace CurrencyServer.Http
{
    /// <summary>
    /// Manages any communication with the ExchangeRate API 
    /// </summary>
    public interface IExchangeRateHttpClient
    {
        /// <summary>
        /// Gets the exchange rate for a set number of currencies based on a baseline currency
        /// </summary>
        /// <param name="date"></param>
        /// <param name="baseline"></param>
        /// <param name="currencies"></param>
        /// <returns>The exchange rates</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="JsonSerializationException"></exception>
        Task<ExchangeRates> GetExchangeRate(DateTime date, string baseline, IEnumerable<string> currencies);
    }
}