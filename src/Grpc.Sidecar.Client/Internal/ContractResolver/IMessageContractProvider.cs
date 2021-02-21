using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grpc.Sidecar.Client.Internal.ContractResolver
{
    interface IMessageContractProvider
    {
        public Type GetMessageType(string typeName);
        public MethodInfo GetMethodInfo(string methodName);
    }
}
