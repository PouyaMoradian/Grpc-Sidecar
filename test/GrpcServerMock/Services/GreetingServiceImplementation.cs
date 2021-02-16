using Greet;
using Grpc.Core;
using System.Threading.Tasks;

namespace GrpcServerMock.Services
{
    public class GreetingServiceImplementation : Greet.GreetingService.GreetingServiceBase
    {
        public async override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new GreetingResponse
            {
                Result = $"Hi {request.Greeting.FirstName} {request.Greeting.LastName}"
            });
        }
    }
}