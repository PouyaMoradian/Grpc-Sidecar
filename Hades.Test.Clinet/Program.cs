using grpc=Grpc.Core;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Hades.Test.Common;
using Google.Protobuf.WellKnownTypes;

namespace Hades.Test.Clinet
{
    class Program
    {
        async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }


        //private static async Task RungRpcClientByChannel()
        //{
        //    grpc::Channel channel = new grpc::Channel("127.0.0.1:30051", grpc::ChannelCredentials.Insecure);
        //    //var channel = GrpcChannel.ForAddress("Http://127.0.0.1:30051", new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure, });

        //    string user = "you";

        //    var req = new HelloRequest
        //    {
        //        Name = user,
        //    };
        //    req.RequestPayload = Any.Pack(new RequestPayload() { Name = "name", Id = 1 });
        //    using var call = _greeterClient.SayHelloAsync(req);

        //    await channel.ShutdownAsync();
        //}
    }
}
