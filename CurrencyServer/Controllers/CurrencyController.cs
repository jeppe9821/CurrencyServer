using CurrencyServer.Data;
using CurrencyServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CurrencyServer.Controllers
{
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyDeltaService;

        public CurrencyController(ICurrencyService currencyDeltaService)
        {
            _currencyDeltaService = currencyDeltaService ?? throw new ArgumentNullException(nameof(currencyDeltaService));
        }

        [HttpPost]
        [Route("currencydelta")]
        public async Task<ActionResult<IEnumerable<CurrencyDeltaResponse>>> Post([FromBody] CurrencyDeltaRequest request)
        {
            var dateComparison = DateTime.Compare(request.fromDate, request.toDate);

            if (dateComparison >= 0)
            {
                return ControllerHelper.CreateResponse(HttpStatusCode.BadRequest, "dateError", "fromDate cannot be later or at the same point as toDate");
            }

            var currenciesContainDuplicate = request.Currencies.Count() != request.Currencies.Distinct().Count();

            if (currenciesContainDuplicate)
            {
                return ControllerHelper.CreateResponse(HttpStatusCode.BadRequest, "duplicateCurrencyError", "The currencies are not unique and contains duplicates");
            }

            var result = await _currencyDeltaService.CalculateRoundedCurrencyDeltaList(request);

            return Ok(result);
        }
    }
}
