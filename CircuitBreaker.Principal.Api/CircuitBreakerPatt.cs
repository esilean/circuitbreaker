using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;

namespace CircuitBreaker.Principal.Api
{
    public class CircuitBreakerPatt
    {


        //Retry Pattern
        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest)
                .WaitAndRetryAsync(2, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        //CircuitBreaker Pattern
        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(15));
        }


        // Wrap All Policies
        public IAsyncPolicy<HttpResponseMessage> GetAllResiliencePolicies()
        {
            return Policy.WrapAsync(GetRetryPolicy(), GetCircuitBreakerPolicy());
        }
    }
}
