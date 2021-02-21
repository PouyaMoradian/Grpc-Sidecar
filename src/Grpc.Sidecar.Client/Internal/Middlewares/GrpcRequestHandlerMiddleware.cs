using Grpc.Sidecar.Client.Internal.ContractResolver;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Grpc.Sidecar.Client.Internal.Middlewares
{
    internal class GrpcRequestHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMessageContractProvider _messageDescriptionProvider;
        private readonly ILogger<GrpcRequestHandlerMiddleware> _logger;

        public GrpcRequestHandlerMiddleware(RequestDelegate next,
            IMessageContractProvider messageDescriptionProvider,
            ILogger<GrpcRequestHandlerMiddleware> logger)
        {
            _next = next;
            _messageDescriptionProvider = messageDescriptionProvider ?? throw new ArgumentNullException(nameof(messageDescriptionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                var requestStream = context.Request.Body;
                using var memoryStream = new MemoryStream();
                await requestStream.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();

                var isCompressed = data.AsSpan().Slice(0, 1)[0] == 1;
                var messageLength = data.AsSpan().Slice(4, 1)[0];

                //Resolving message description

                var methodInfo = GetMethodInfo(context);
                var parameter = methodInfo.GetParameters();
                var resolvedType = parameter[0].ParameterType;

                //Deserializeing message
                if (resolvedType is not null)
                {
                    Stream stream = new MemoryStream(data.AsSpan(5).ToArray());
                    var greetingRequest = Serializer.Deserialize(resolvedType, stream);
                }

                //Resolving service description

                //Descovering the service target

                //Creating client dynamically
                //forwarding message

                //Retrieve response from stream
                //forwarding response to client
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }

        }

        private Type ResolveMessageType(HttpContext context)
        {
            return _messageDescriptionProvider.GetMessageType("Greeting");
        }

        private MethodInfo GetMethodInfo(HttpContext context)
        {
            var requestPath = context.Request.Path.Value;
            var methodName = requestPath.Split('/')[2];

            return _messageDescriptionProvider.GetMethodInfo(methodName);
        }
    }
}
