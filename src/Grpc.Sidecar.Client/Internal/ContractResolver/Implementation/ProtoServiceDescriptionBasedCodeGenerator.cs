using Grpc.Sidecar.CodeGenerator;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Grpc.Sidecar.Client.Internal.ContractResolver.Implementation
{
    public class ProtoServiceDescriptionBasedCodeGenerator : IMessageContractProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ProtobufTypeGenerator _typeGenerator;

        public ProtoServiceDescriptionBasedCodeGenerator(IConfiguration configuration,
            ProtobufTypeGenerator typeGenerator)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _typeGenerator = typeGenerator ?? throw new ArgumentNullException(nameof(typeGenerator));
            GenerateType();
        }

        public void GenerateType()
        {
            //AddExistingProtoes
            string[] protoFiles = Directory.GetFiles(_configuration["DescriptionDiscoveryOption:DescriptionFilePath"], "*.proto");

            foreach (var protoFile in protoFiles)
            {
                _typeGenerator.AddProtoFile("greeting.proto", File.ReadAllBytes(protoFile));
            }

            _typeGenerator.GenerateAssembly();
        }

        public Type GetMessageType(string typeName)
        {
            return _typeGenerator.GeneratedAssembly.GetTypes()
                .FirstOrDefault(x => x.Name == typeName);
        }

        public MethodInfo GetMethodInfo(string methodName)
        {
            return _typeGenerator.GeneratedAssembly.GetTypes()
                .SelectMany(r => r.GetMethods()).FirstOrDefault(t => t.Name == methodName + "Async" || t.Name == methodName);
        }

        public IGrpcInvoker GetInvoker(string methodName)
        {
            //Here it can search in chached expression
            var generatedGrpcInvoker = _typeGenerator.GeneratedAssembly.GetTypes().FirstOrDefault(r => r.Name == $"{methodName}Invoker");
            
            return (IGrpcInvoker)Activator.CreateInstance(generatedGrpcInvoker);
        }





    }
}
