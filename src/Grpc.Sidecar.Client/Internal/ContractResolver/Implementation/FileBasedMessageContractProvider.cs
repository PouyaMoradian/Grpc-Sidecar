using Google.Protobuf.Reflection;
using Grpc.Sidecar.CodeGenerator;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Sidecar.Client.Internal.ContractResolver.Implementation
{
    public class FileBasedMessageContractProvider : IMessageContractProvider
    {
        private readonly IConfiguration _configuration;
        protected List<Type> _clrTypes = new List<Type>();

        public FileBasedMessageContractProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            GenerateClrType(_configuration["DescriptionDiscoveryOption:DescriptionFilePath"]);
        }

        protected void GenerateClrType(string descriptorsfilePath)
        {
            string[] protoFiles = Directory.GetFiles(descriptorsfilePath, "*.proto");
            foreach (var protoFile in protoFiles)
                _clrTypes.AddRange(ProtoTypeGenerator.
                    GetProtoClrTypes("greeting.proto", File.ReadAllBytes(protoFile)));
        }

        public Type GetMessageType(string typeName) 
        {
            return _clrTypes.Where(t => t.Name == typeName).FirstOrDefault();
        }
    }
}
