using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hades.Core.Grpc;
using Hades.Test.Common;
using System;
using System.Threading.Tasks;

namespace Hades.Test.Server
{
    class Program
    {
        const int Port = 30051;

        async static Task Main(string[] args)
        {
            Console.WriteLine("Grpc Server!");
            await AddgRpcServerByServer();
        }



        private static async Task AddgRpcServerByServer()
        {
            var method = new UnaryGrpcMethod<HelloRequest, HelloReply>("SayHello", request =>
            {
                var val = new HelloReply { Discription = "Hello " + request.Name };
                val.Faild = new Faild() { Error = "Unexpected error occured." };
                val.EchoValue = Any.Pack(request.RequestPayload);
                return Task.FromResult(val);
            });
            var grpcService = new GrpcService("helloworld.Greeter", new[] { method });

            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { grpcService.Build() },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            await server.ShutdownAsync();
        }
    }
}
