using Google.Protobuf;
using Grpc.Core;
using System.Collections.Generic;

namespace Hades.Core.Abbstraction.Grpc
{
    public interface IServerStreamingGrpcMethod<TRequest, TResponse> : IGrpcMethod
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        IAsyncEnumerable<TResponse> Execute(TRequest request, ServerCallContext context);
    }
}
