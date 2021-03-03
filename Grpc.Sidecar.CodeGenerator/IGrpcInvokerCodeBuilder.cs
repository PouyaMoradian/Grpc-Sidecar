﻿using Google.Protobuf.Reflection;
using static Grpc.Sidecar.CodeGenerator.Abstraction.GrpcUnaryInvokerCodeBuilder;

namespace Grpc.Sidecar.CodeGenerator.Abstraction
{
    public interface IGrpcInvokerCodeBuilder
    {
        string GenerateCode(ServiceDescription serviceDescription, string namespaceName);
    }

    public interface IGrpcInvokerCodeGenerator 
    {
        string GenerateCode(ServiceDescriptorProto serviceDescription, string namespaceName);
    }
}