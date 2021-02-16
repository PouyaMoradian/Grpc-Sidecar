using System.Collections.Generic;

namespace Grpc.Sidecar.Client.Internal.ContractResolver
{
    interface IMessageDescriptionProvider
    {
        public IList<ProtoMessageDescription> GetMessageDescriptions();
    }
}
