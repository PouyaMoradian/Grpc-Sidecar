using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GrpcClientMock.Controllers
{
    [Route("api/[controller]")]
    public class GrpcRequestsController : ControllerBase
    {
        private readonly ILogger<GrpcRequestsController> _logger;
        private readonly IConfiguration _configuration;

        public GrpcRequestsController(ILogger<GrpcRequestsController> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
        }

        [HttpPost("Unary")]
        public async Task<IActionResult> SendUnary()
        {
            try
            {
                var sidecarAddress = _configuration["CommunicationSettings:Address"];

                using var channel = GrpcChannel.ForAddress(sidecarAddress);
                var client = new Greet.GreetingService.GreetingServiceClient(channel);

                var reply = await client.GreetSimpleAsync(new Greet.Greeting()
                {
                    FirstName = "Pouya",
                    LastName = "Moradian"
                });

                return Ok(reply);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
