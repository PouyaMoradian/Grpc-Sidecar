using Grpc.Core;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Configuration;
using System.Threading.Tasks;

namespace Grpc.Sidecar.CodeGenerator
{
    public abstract class GrpcUnaryInvoker<TService, TRequest, TResponse> : IGrpcInvoker
        where TService : class
    {
        protected TService CreateService(ChannelBase channel, ClientFactory clientFactory)
        {
            return GrpcClientFactory.CreateGrpcService<TService>(channel, clientFactory);
        }

        public abstract ValueTask<TResponse> Invoke(TRequest requst, ChannelBase channel, ClientFactory clientFactory);

        async Task<object> IGrpcInvoker.Invoke(object request, ChannelBase channel, ClientFactory clientFactory)
        {
            return await Invoke((TRequest)request, channel, clientFactory);
        }
    }
}
