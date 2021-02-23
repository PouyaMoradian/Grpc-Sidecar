using Grpc.Sidecar.CodeGenerator;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Grpc.Sidecar.Client.Internal.ContractResolver.Implementation
{
    public class FileBasedMessageContractProvider : IMessageContractProvider
    {
        private readonly IConfiguration _configuration;
        protected IList<Type> _clrTypes = new List<Type>();
        protected IList<MethodInfo> _methodInfo = new List<MethodInfo>();
        private readonly ProtoTypeInfoProvider _protoTypeInfoProvider;

        public FileBasedMessageContractProvider(IConfiguration configuration, /* Interface => */ ProtoTypeInfoProvider protoTypeInfoProvider)
        {
            _configuration = configuration;
            _protoTypeInfoProvider = protoTypeInfoProvider;
            GenerateClrType(_configuration["DescriptionDiscoveryOption:DescriptionFilePath"]);
        }

        protected void GenerateClrType(string descriptorsfilePath)
        {
            string[] protoFiles = Directory.GetFiles(descriptorsfilePath, "*.proto");

            foreach (var protoFile in protoFiles)
            {
                _protoTypeInfoProvider.LoadProtoFile("greeting.proto", File.ReadAllBytes(protoFile));
            }
        }

        public Type GetMessageType(string typeName)
        {
            return _protoTypeInfoProvider?.GeneratedAssemblies?.SelectMany(t => t.GetTypes())
                .FirstOrDefault(x => x.Name == typeName);
        }

        public MethodInfo GetMethodInfo(string methodName)
        {
            return _protoTypeInfoProvider?.GeneratedAssemblies?.SelectMany(t => t.GetTypes())
                .SelectMany(r => r.GetMethods()).FirstOrDefault(t => t.Name == methodName + "Async" || t.Name == methodName);
        }

        public IGrpcInvoker GetInvoker(string methodName)
        {
            //Here it can search in chached expression
            var generatedGrpcInvoker = 
                _protoTypeInfoProvider?.GeneratedAssemblies?.SelectMany(t => t.GetTypes()).FirstOrDefault(t=>t.Name == $"{methodName}Invoker");

            return (IGrpcInvoker)Activator.CreateInstance(generatedGrpcInvoker);
        }

    }
}
