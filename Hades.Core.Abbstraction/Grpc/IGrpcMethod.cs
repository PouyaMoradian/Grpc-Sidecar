using Grpc.Core;

namespace Hades.Core.Abbstraction.Grpc
{

    public interface IGrpcMethod
    {
        string Name { get; }
        MethodType MethodType { get; }
        void Bind(IGrpcService grpcService, ServerServiceDefinition.Builder builder);
    }
}
