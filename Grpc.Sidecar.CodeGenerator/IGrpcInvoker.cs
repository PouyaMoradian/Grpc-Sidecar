using Grpc.Core;
using ProtoBuf.Grpc.Configuration;
using System.Threading.Tasks;

namespace Grpc.Sidecar.CodeGenerator
{
    // : GrpcInvoker => override Invoke and call the target method
    public interface IGrpcInvoker
    {
        //This should be changed for server streaming scenario
        Task<object> Invoke(object request, ChannelBase channel, ClientFactory clientFactory);
    }
}
