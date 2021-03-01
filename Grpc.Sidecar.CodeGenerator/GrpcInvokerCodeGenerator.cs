using Google.Protobuf.Reflection;
using Grpc.Sidecar.CodeGenerator.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Sidecar.CodeGenerator
{
    public class GrpcInvokerCodeGenerator : IGrpcInvokerCodeGenerator
    {
        public string GenerateCode(ServiceDescriptorProto serviceDescription, string namespaceName)
        {
            var stringBuilder = new StringBuilder();
            //TODO
            stringBuilder.AppendLine(CreateImportStatements(namespaceName));

            stringBuilder.AppendLine(CreateNamespaceCode(namespaceName,
                () => GenerateInvokerClassesCode(serviceDescription)));

            return stringBuilder.ToString();
        }

        protected static string GenerateInvokerClassesCode(ServiceDescriptorProto serviceDescription)
        {
            var stringBuilder = new StringBuilder();

            foreach (var methodDescription in serviceDescription.Methods)
            {
                

                if (!methodDescription.ClientStreaming && !methodDescription.ServerStreaming)
                    stringBuilder.AppendLine(GrpcInvokerCodeGeneratorHelper.CreateUnaryMethod(methodDescription.Name,
                                                                        serviceDescription.Name,
                                                                        methodDescription.InputType.Split('.')[^1],
                                                                        methodDescription.OutputType.Split('.')[^1]));
                //implement the rest of method types
            }

            return stringBuilder.ToString();
        }

        protected static string CreateImportStatements(string serviceNamespace)
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

        protected static string CreateNamespaceCode(string namespaceName, Func<string> classesCodeBuilder)
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

        protected static string CreateUnaryMethod(string methodName,
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
