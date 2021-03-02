using Google.Protobuf;
using Grpc.Core;
using Hades.Core.Abbstraction.Grpc;
using System;
using System.Threading.Tasks;

namespace Hades.Core.Grpc
{
    public class UnaryGrpcMethod<TRequest, TResponse> : GrpcMethodBase<TRequest, TResponse>, IUnaryGrpcMethod<TRequest, TResponse>
        where TRequest : class, IMessage<TRequest>, new()
        where TResponse : class, IMessage<TResponse>, new()
    {
        public override MethodType MethodType => MethodType.Unary;
        private readonly Func<TRequest, Task<TResponse>> _func;

        public UnaryGrpcMethod(string name, Func<TRequest, Task<TResponse>> func)
            : base(name)
        {
            _func = func
                ?? throw new ArgumentNullException(nameof(func));
        }

        public async virtual Task<TResponse> Execute(TRequest request, ServerCallContext context)
        {
            return await _func(request);
        }

        public override void Bind(IGrpcService grpcService, ServerServiceDefinition.Builder builder)
        {
            grpcService.Visit(this, builder);
        }
    }
}
