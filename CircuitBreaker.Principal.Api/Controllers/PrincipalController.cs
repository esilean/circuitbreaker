using CircuitBreaker.Principal.Api.HttpFactories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CircuitBreaker.Principal.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrincipalController : ControllerBase
    {
        private readonly ILogger<PrincipalController> _logger;
        private readonly IPaymentHttpFactory _paymentHttpFactory;

        public PrincipalController(ILogger<PrincipalController> logger, IPaymentHttpFactory paymentHttpFactory)
        {
            _logger = logger;
            _paymentHttpFactory = paymentHttpFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetPrincipal()
        {
            //await Task.Delay(1000);
            _logger.LogInformation($"Log {nameof(GetPrincipal)}");

            var statusCode = await _paymentHttpFactory.MakePayment();

            return StatusCode((int)statusCode, new { message = "hello", status = statusCode.ToString() });
        }
    }
}
