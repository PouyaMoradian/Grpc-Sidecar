using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Sidecar.CodeGenerator
{

    public class GrpcUnaryInvokerCodeBuilder : IGrpcInvokerCodeBuilder
    {
        public string GenerateCode(ServiceDescription serviceDescription, string namespaceName)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(GrpcInvokerCodeGeneratorHelper
                .CreateImportStatements(serviceDescription.ServiceNamespaceName));

            stringBuilder.AppendLine(CreateNamespaceCode(namespaceName,
                () => GenerateInvokerClassesCode(serviceDescription.MethodDescriptions)));

            return stringBuilder.ToString();
        }

        protected string CreateNamespaceCode(string namespaceName, Func<string> classesCodeBuilder)
        {
            var namespaceIdentifier = string.IsNullOrWhiteSpace(namespaceName) ? "Grpc.MethodInvoker" : namespaceName;

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"namespace {namespaceIdentifier}");
            stringBuilder.AppendLine("{");

            //Generating class for each method of service contract
            stringBuilder.AppendLine(classesCodeBuilder());

            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }

        protected string GenerateInvokerClassesCode(IList<MethodDescription> methodDescriptions)
        {
            var stringBuilder = new StringBuilder();

            foreach (var methodDescription in methodDescriptions)
            {
                switch (methodDescription.CommmunicationType)
                {
                    case MethodDescription.MethodType.Unary:
                        stringBuilder.AppendLine(GrpcInvokerCodeGeneratorHelper.CreateUnaryMethod(methodDescription.Name,
                                                                            methodDescription.ServiceTypeName,
                                                                            methodDescription.RequestTypeName,
                                                                            methodDescription.ResponseTypeName));
                        break;
                    case MethodDescription.MethodType.ClientStreaming:
                        break;
                    case MethodDescription.MethodType.ServerStreaming:
                        break;
                    case MethodDescription.MethodType.Bidirectional:
                        break;
                    default:
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        public class ServiceDescription
        {
            public string ServiceNamespaceName { get; set; }
            public IList<MethodDescription> MethodDescriptions { get; set; } = new List<MethodDescription>();
        }

        public class MethodDescription
        {
            public string Name { get; set; }
            public string RequestTypeName { get; set; }
            public string ResponseTypeName { get; set; }
            public string ServiceTypeName { get; set; }
            public MethodType CommmunicationType { get; set; }

            public enum MethodType
            {
                Unary,
                ClientStreaming,
                ServerStreaming,
                Bidirectional
            }
        }
    }

    public static class GrpcInvokerCodeGeneratorHelper
    {
        public static string CreateImportStatements(string serviceNamespace)
        {
            return
                $@" 
                    using {serviceNamespace};
                    using Grpc.Core;
                    using ProtoBuf.Grpc.Configuration;
                    using System.Threading.Tasks;
                    using Grpc.Sidecar.CodeGenerator;
                ";
        }

        public static string CreateUnaryMethod(string methodName,
                                              string serviceTypeName,
                                              string requestTypeName,
                                              string responseTypeName)
        {

            //TODO use nameof(GrpcUnaryInvoker)
            return $@"
                    public class {methodName}Invoker : GrpcUnaryInvoker<{serviceTypeName}, 
                                                                   {requestTypeName}, 
                                                                   {responseTypeName}>
                    {{
                        public async override ValueTask<{responseTypeName}> Invoke({requestTypeName} requst,
                                                                                 ChannelBase channel,
                                                                                 ClientFactory clientFactory)
                        {{
                            return await CreateService(channel, clientFactory).{methodName}(requst);
                        }}
                    }}";
        }
    }
}
