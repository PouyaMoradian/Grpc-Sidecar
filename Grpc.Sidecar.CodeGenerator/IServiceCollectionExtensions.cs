using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Sidecar.CodeGenerator
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddProtoTypeInfoProvider(this IServiceCollection services) 
        {
            //TODO create an interface for this
            services.AddSingleton<IGrpcInvokerCodeBuilder, GrpcUnaryInvokerCodeBuilder>();
            services.AddSingleton<ProtoTypeInfoProvider>();
          
            return services;
        }
    }
}
