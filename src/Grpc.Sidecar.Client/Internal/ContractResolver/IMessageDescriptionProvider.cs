using Google.Protobuf.Reflection;
using System.Collections.Generic;

namespace Grpc.Sidecar.Client.Internal.ContractResolver
{
    interface IMessageContractProvider
    {
        public IList<MessageDescriptor> GetMessageDescriptors();
    }
}
