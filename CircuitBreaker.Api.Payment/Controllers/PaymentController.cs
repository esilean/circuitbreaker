using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CircuitBreaker.Api.Payment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                GenerateRandomError();
                return Ok(new { message = "Ok" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        private void GenerateRandomError()
        {
            var randomNumber = new Random().Next(1, 10);
            if (randomNumber % 2 == 1)
                throw new Exception("Something went wrong");
        }
    }


}
