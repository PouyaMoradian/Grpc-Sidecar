using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Sidecar.Client.Internal.ContractResolver.Implementation
{
    public class FileBasedMessageContractProvider : IMessageContractProvider
    {
        private readonly IConfiguration _configuration;
        protected List<FileDescriptor> _fileDescriptors = new List<FileDescriptor>();


        public FileBasedMessageContractProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadDescriptorFiles(_configuration["DescriptionDiscoveryOption:DescriptionFilePath"]).Wait();
        }

        public async Task LoadDescriptorFiles(string descriptorsfilePath)
        {
            string[] descriptorFiles = Directory.GetFiles(descriptorsfilePath, "*.desc");

            foreach (var descriptorFile in descriptorFiles)
            {
                var file = await File.ReadAllBytesAsync(descriptorFile);
                var byteString = ByteString.CopyFrom(file);

                var descriptorSet = FileDescriptorSet.Parser.ParseFrom(file);
                var byteStrings = descriptorSet.File.Select(f => f.ToByteString()).ToList();
                IReadOnlyList<FileDescriptor> descriptors = FileDescriptor.BuildFromByteStrings(byteStrings);

                _fileDescriptors.AddRange(descriptors);
            }







        }

        public IList<ServiceDescriptor> GetServiceDescriptors()
        {
            var result = new List<ServiceDescriptor>();

            foreach (var fileDescriptor in _fileDescriptors)
            {
                result.AddRange(fileDescriptor.Services);
            }

            return result;
        }

        public IList<MessageDescriptor> GetMessageDescriptors()
        {
            var result = new List<MessageDescriptor>();

            foreach (var fileDescriptor in _fileDescriptors)
            {
                result.AddRange(fileDescriptor.MessageTypes);
            }

            return result;

            //var result = new List<MessageDescription>();

            //foreach (var fileDescriptor in FileDescriptors)
            //{

            //    foreach (var messageType in fileDescriptor.MessageTypes)
            //    {
            //        var messageDescritprion = new MessageDescription
            //        {
            //            ClassName = messageType.Name
            //        };

            //        foreach (var fieldDescription in messageType.Fields.InDeclarationOrder())
            //        {
            //            messageDescritprion.Fields.Add(fieldDescription.Name, fieldDescription.FieldType);
            //        }

            //        result.Add(messageDescritprion);
            //    }
            //}

            //return result;
        }
    }
}
