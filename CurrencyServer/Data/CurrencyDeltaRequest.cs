using CurrencyServer.Binders;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyServer.Data
{
    /// <summary>
    /// The body of CurrencyController's POST request 
    /// </summary>
    [ModelBinder(BinderType = typeof(CurrencyDeltaRequestModelBinder))]
    public class CurrencyDeltaRequest
    {
        public string? Baseline { get; set; }
        public IEnumerable<string>? Currencies { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
