using Google.Protobuf;
using Grpc.Core;
using System.Threading.Tasks;

namespace Hades.Core.Abbstraction.Grpc
{
    public interface IUnaryGrpcMethod<TRequest, TResponse> : IGrpcMethod
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        Task<TResponse> Execute(TRequest request, ServerCallContext context);
    }
}
