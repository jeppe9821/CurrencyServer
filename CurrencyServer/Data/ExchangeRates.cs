namespace CurrencyServer.Data
{
    /// <summary>
    /// Keeps track of a set number of currencies and their respective exchange rates
    /// </summary>
    public class ExchangeRates
    {
        public IDictionary<string, decimal> Rates { get; set; }
    }
}
