using Grpc.Sidecar.Client.Internal.ContractResolver;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Grpc.Sidecar.Client.Internal.Middlewares
{
    internal class GrpcRequestHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMessageDescriptionProvider _messageDescriptionProvider;
        private readonly ILogger<GrpcRequestHandlerMiddleware> _logger;

        public GrpcRequestHandlerMiddleware(RequestDelegate next, 
            IMessageDescriptionProvider messageDescriptionProvider,
            ILogger<GrpcRequestHandlerMiddleware> logger)
        {
            _next = next;
            _messageDescriptionProvider = messageDescriptionProvider ?? throw new ArgumentNullException(nameof(messageDescriptionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {

            await ResolveMessageDescription(context);

            //Resolving message description
            //Deserializeing message

            //Resolving service description
            //Descovering the service target
            //Creating client dynamically
            //forwarding message

            //Retrieve response from stream
            //forwarding response to client
        }


        private async Task ResolveMessageDescription(HttpContext context)
        {
            try
            {
                var requestStream = context.Request.Body;
                using var memoryStream = new MemoryStream();
                await requestStream.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();

                var isCompressed = data.AsSpan().Slice(0, 1)[0] == 1;
                var messageLength = data.AsSpan().Slice(4, 1)[0];

                var messageDescriptions = _messageDescriptionProvider.GetMessageDescriptions();

                //var greetingRequest = Serializer.Deserialize<Greeting>(data.AsSpan(5));

                //var descriptor = fileDescriptionProvider.FileDescriptors[0];

                //var messageType = descriptor.MessageTypes[0];

                //var fieldDecleration = messageType.Fields.InDeclarationOrder()[0];


                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }
    }
}
