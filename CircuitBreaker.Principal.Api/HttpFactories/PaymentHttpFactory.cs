using CircuitBreaker.Principal.Api.HttpFactories.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CircuitBreaker.Principal.Api.HttpFactories
{
    public class PaymentHttpFactory : IPaymentHttpFactory
    {
        private readonly ILogger<PaymentHttpFactory> _logger;
        private readonly HttpClient _client;
        public int Retries { get; set; } = 0;

        private static readonly AsyncRetryPolicy<HttpResponseMessage> TransientErrorRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(message => ((int)message.StatusCode == 400 || (int)message.StatusCode >= 500))
                    .WaitAndRetryAsync(1, retryAttempt =>
                    {
                        Console.WriteLine($"Retrying...........{retryAttempt}");
                        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    });

        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy =
                Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 400)
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> AdvancedCircuitBreaker =
                Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 400)
                    .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(30), 100, TimeSpan.FromSeconds(30));

        private readonly AsyncPolicyWrap<HttpResponseMessage> _asyncPolicyWrap =
                CircuitBreakerPolicy.WrapAsync(TransientErrorRetryPolicy);


        public PaymentHttpFactory(IHttpClientFactory factory, ILogger<PaymentHttpFactory> logger)
        {
            _logger = logger;

            _client = factory.CreateClient("PaymentApi");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpStatusCode> MakePayment()
        {
            try
            {
                if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
                {
                    _logger.LogWarning($"Circuit is Opened");
                    return HttpStatusCode.ServiceUnavailable;
                }


                _logger.LogError("Trying again...................................");

                var response = await CircuitBreakerPolicy
                                            .ExecuteAsync(() =>
                                                TransientErrorRetryPolicy
                                                    .ExecuteAsync(() => _client.GetAsync("/payment")));

                //var responseWrap = await _asyncPolicyWrap
                //                                    .ExecuteAsync(() => _client.GetAsync("/payment"));

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
