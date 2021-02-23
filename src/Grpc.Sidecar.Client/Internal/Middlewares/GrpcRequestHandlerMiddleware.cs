using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Sidecar.Client.Internal.ContractResolver;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

                ////Deserializeing message
                //if (resolvedType is not null)
                //{
                Stream stream = new MemoryStream(data.AsSpan(5).ToArray());
                var greetingRequest = Serializer.Deserialize(resolvedType, stream);
                //}

                var invoker = _messageDescriptionProvider.GetInvoker(methodInfo.Name);


                var channel = GrpcChannel.ForAddress("https://localhost:5001");

                var invokerResult = await invoker.Invoke(greetingRequest,channel,null);


                var createGrpcServiceMethodInfo = typeof(GrpcClientFactory).GetMethod(nameof(GrpcClientFactory.CreateGrpcService), new Type[] { typeof(ChannelBase), typeof(ClientFactory) })
                    .MakeGenericMethod(methodInfo.DeclaringType);

                var mehtodArg = Expression.Parameter(typeof(GrpcChannel));
                var mehtodArg2 = Expression.Parameter(typeof(ClientFactory));
                var body = Expression.Call(createGrpcServiceMethodInfo, mehtodArg, mehtodArg2);

                var expr = Expression.Lambda<Func<GrpcChannel, ClientFactory, object>>(body, mehtodArg, mehtodArg2);

                var function = expr.Compile();

                var service = function(channel, null);

                var method = service.GetType().GetRuntimeMethods().FirstOrDefault(t => t.Name.Contains(methodInfo.Name));



                dynamic result = method.Invoke(service, new object[] { greetingRequest, null });
                await result;
                var response = result.GetAwaiter().GetResult();

                var converted = Convert.ChangeType(response, methodInfo.ReturnType.GetGenericArguments()[0]);

                using var responseMemoryStream = new MemoryStream();
                using var responseDataMemoryStream = new MemoryStream();

                Serializer.Serialize(responseDataMemoryStream, response);

                await responseMemoryStream.WriteAsync(new byte[] { 0, 0, 0, 0, (byte)responseDataMemoryStream.Length });

                context.Response.Headers.Add("content-type", new Microsoft.Extensions.Primitives.StringValues("application/grpc"));

                await responseMemoryStream.WriteAsync(responseDataMemoryStream.ToArray());

                await responseMemoryStream.CopyToAsync(context.Response.Body);


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
