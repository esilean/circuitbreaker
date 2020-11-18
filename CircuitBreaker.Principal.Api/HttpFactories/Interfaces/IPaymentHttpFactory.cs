using System.Net;
using System.Threading.Tasks;

namespace CircuitBreaker.Principal.Api.HttpFactories.Interfaces
{
    public interface IPaymentHttpFactory
    {
        Task<HttpStatusCode> MakePayment();
    }
}
