using Grpc.Sidecar.CodeGenerator;
using System;
using System.Reflection;

namespace Grpc.Sidecar.Client.Internal.ContractResolver
{
    interface IMessageContractProvider
    {
        IGrpcInvoker GetInvoker(string methodName);
        public Type GetMessageType(string typeName);
        public MethodInfo GetMethodInfo(string methodName);
    }
}
