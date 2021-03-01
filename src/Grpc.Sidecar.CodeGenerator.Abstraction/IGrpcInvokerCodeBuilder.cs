using static Grpc.Sidecar.CodeGenerator.Abstraction.GrpcUnaryInvokerCodeBuilder;

namespace Grpc.Sidecar.CodeGenerator.Abstraction
{
    public interface IGrpcInvokerCodeBuilder
    {
        string GenerateCode(ServiceDescription serviceDescription, string namespaceName);
        string GenerateCode(ServiceDescriptorProto serviceDescription, string namespaceName);
    }
}
