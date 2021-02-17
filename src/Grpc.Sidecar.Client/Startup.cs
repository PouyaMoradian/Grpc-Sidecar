using Grpc.Sidecar.Client.Internal.ContractResolver;
using Grpc.Sidecar.Client.Internal.ContractResolver.Implementation;
using Grpc.Sidecar.Client.Internal.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Grpc.Sidecar.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(option =>
            {
                option.IgnoreUnknownServices = true;
            });

            services.AddSingleton<IMessageContractProvider, FileBasedMessageContractProvider>();

            //This is for development purpose only
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client");
                });
            });

            app.UseMiddleware<GrpcRequestHandlerMiddleware>();
        }
    }
}
