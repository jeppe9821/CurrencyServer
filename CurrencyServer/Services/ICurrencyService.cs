using CurrencyServer.Data;

namespace CurrencyServer.Services
{
    /// <summary>
    /// Manages the business logic of the CurrencyController
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Calculates the delta of a set of currencies. Rounding the value to nearest even number with specified amount of decimals
        /// </summary>
        /// <param name="request"></param>
        /// <param name="numberOfDecimals"></param>
        /// <returns></returns>
        Task<IEnumerable<CurrencyDeltaResponse>> CalculateRoundedCurrencyDeltaList(CurrencyDeltaRequest request, int numberOfDecimals = 3);
    }
}