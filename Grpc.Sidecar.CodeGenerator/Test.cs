//using Greet;
//using Grpc.Core;
//using ProtoBuf.Grpc.Configuration;
//using System.Threading.Tasks;

//namespace Grpc.Sidecar.CodeGenerator
//{
//    public class GreetSimpleInvoker : GrpcInvoker<IGreetingService, Greeting, GreetingResponse>
//    {
//        public async override ValueTask<GreetingResponse> Invoke(Greeting requst,
//                                                                 ChannelBase channel,
//                                                                 ClientFactory clientFactory)
//        {
//            return await CreateService(channel, clientFactory)
//                .GreetSimpleAsync(requst);
//        }
//    }
//}