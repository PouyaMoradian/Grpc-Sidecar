using Google.Protobuf;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;

namespace Hades.Core.Abbstraction.Grpc
{
    public interface IClientStreamingGrpcMethod<TRequest, TResponse> : IGrpcMethod
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        Task<TResponse> Execute(IAsyncEnumerable<TRequest> request, ServerCallContext context);
    }
}
