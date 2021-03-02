using Google.Protobuf;
using Grpc.Core;
using Hades.Core.Abbstraction.Grpc;
using System;
using System.Reflection;

namespace Hades.Core.Grpc
{
    public abstract class GrpcMethodBase<TRequest, TResponse> : IGrpcMethod
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new()
    {
        public string Name { get; }

        public abstract MethodType MethodType { get; }

        public GrpcMethodBase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
        public abstract void Bind(IGrpcService grpcService, ServerServiceDefinition.Builder builder);
    }
}
