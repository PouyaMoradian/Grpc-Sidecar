using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;

namespace Grpc.Sidecar.Client.Internal.ContractResolver
{
    interface IMessageContractProvider
    {
        public Type GetMessageType(string typeName);
    }
}
