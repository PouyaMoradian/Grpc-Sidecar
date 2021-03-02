using Google.Protobuf;
using Grpc.Core;
using System.Collections.Generic;

namespace Hades.Core.Abbstraction.Grpc
{
    public interface IGrpcService
    {
        string Name { get; }
        IEnumerable<IGrpcMethod> Methods { get; }
        ServerServiceDefinition Build();

        void Visit<TRequest, TResponse>(IUnaryGrpcMethod<TRequest, TResponse> method, ServerServiceDefinition.Builder builder)
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new();


        void Visit<TRequest, TResponse>(IClientStreamingGrpcMethod<TRequest, TResponse> method, ServerServiceDefinition.Builder builder)
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new();

        void Visit<TRequest, TResponse>(IServerStreamingGrpcMethod<TRequest, TResponse> method, ServerServiceDefinition.Builder builder)
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new();

        void Visit<TRequest, TResponse>(IDuplexStreamingGrpcMethod<TRequest, TResponse> method, ServerServiceDefinition.Builder builder)
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new();
    }
}
