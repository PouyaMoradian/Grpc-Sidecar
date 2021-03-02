using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hades.Core.Abbstraction.Grpc;
using Hades.Core.Abbstraction.ServiceDiscovery;
using Hades.Core.Grpc;
using Hades.Core;
using Hades.Test.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hades.Test.Server
{
    class Program
    {
        const int Port = 30051;

        async static Task Main(string[] args)
        {
            Console.WriteLine("Grpc Server!");
            //await AddgRpcServerByServer();
        }



        private static async Task AddgRpcServerByServer(IServiceDefinition[] services)
        {
            var grpcServices = new List<GrpcService>();
            foreach (var service in services)
            {
                var methods = service.Methods.Select(t =>
                {
                    if (!t.HasClientStreaming && !t.HasServerStreaming)
                    {
                        var method = new UnaryGrpcMethod<HelloRequest, HelloReply>(t.Name, request =>
                        {
                            //push to queue
                            return Task.FromResult(new HelloReply());
                        });
                        return method;
                    }
                    //else if (t.HasClientStreaming && !t.HasServerStreaming)
                    //{
                    //    var method = new ClientStreamingGrpcMethod<HelloRequest, HelloReply>(t.Name, request =>
                    //    {
                    //        return Task.FromResult(new HelloReply());
                    //    });
                    //    return method;
                    //}
                    throw new NotImplementedException();
                });
                var grpcService = new GrpcService(service.Name, methods);
                grpcServices.Add(grpcService);

            }

            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            grpcServices.Select(t => t.Build()).Foreach(server.Services.Add);


            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            await server.ShutdownAsync();
        }
    }
}
