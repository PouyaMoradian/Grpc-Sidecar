using Google.Protobuf;
using Grpc.Core;
using System.Collections.Generic;

namespace Hades.Core.Abbstraction.Grpc
{
    public interface IDuplexStreamingGrpcMethod<TRequest, TResponse> : IGrpcMethod
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        IAsyncEnumerable<TResponse> Execute(IAsyncEnumerable<TRequest> request, ServerCallContext context);
    }
}
