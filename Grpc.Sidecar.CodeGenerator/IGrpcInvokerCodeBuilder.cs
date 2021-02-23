using static Grpc.Sidecar.CodeGenerator.GrpcUnaryInvokerCodeBuilder;

namespace Grpc.Sidecar.CodeGenerator
{
    public interface IGrpcInvokerCodeBuilder
    {
        string GenerateCode(ServiceDescription serviceDescription, string namespaceName);
    }
}
