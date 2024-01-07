namespace CurrencyServer.Options
{
    /// <summary>
    /// Configuration for the exchange rate api
    /// </summary>
    public class ExchangeRateApiOptions
    {
        public string BaseUrl { get; set; }
        public string Version { get; set; }
        public string SecretKey { get; set; }
    }
}
