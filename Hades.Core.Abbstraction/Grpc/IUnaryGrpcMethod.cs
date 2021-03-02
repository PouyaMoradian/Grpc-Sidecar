using Google.Protobuf;
using Grpc.Core;
using System.Threading.Tasks;

namespace Hades.Core.Abbstraction.Grpc
{
    public interface IUnaryGrpcMethod<TRequest> : IGrpcMethod
        where TRequest : IMessage<TRequest>
    {
        Task Execute(TRequest request, ServerCallContext context);
    }
}
