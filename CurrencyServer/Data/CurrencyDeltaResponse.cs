namespace CurrencyServer.Data
{
    /// <summary>
    /// The response to CurrencyController's POST request
    /// </summary>
    public class CurrencyDeltaResponse
    {
        public string Currency { get; set; }
        public decimal Delta { get; set; }
    }
}
