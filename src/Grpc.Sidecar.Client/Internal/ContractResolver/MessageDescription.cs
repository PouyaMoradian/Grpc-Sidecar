using Google.Protobuf.Reflection;
using System.Collections.Generic;

namespace Grpc.Sidecar.Client.Internal.ContractResolver
{
    public class MessageDescription
    {
        public string ClassName { get; set; }
        public Dictionary<string, FieldType> Fields { get; set; } = new Dictionary<string, FieldType>();
    }
}
