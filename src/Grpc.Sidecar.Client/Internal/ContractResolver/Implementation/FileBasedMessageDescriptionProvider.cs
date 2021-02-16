using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Sidecar.Client.Internal.ContractResolver.Implementation
{
    public class FileBasedMessageDescriptionProvider : IMessageDescriptionProvider
    {
        private readonly IConfiguration _configuration;

        public List<FileDescriptor> FileDescriptors { get; set; } = new List<FileDescriptor>();


        public FileBasedMessageDescriptionProvider(IConfiguration configuration)
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

                FileDescriptors.AddRange(descriptors);
            }
        }

        public IList<ProtoMessageDescription> GetMessageDescriptions()
        {

            var result = new List<ProtoMessageDescription>();


            foreach (var fileDescriptor in FileDescriptors)
            {

                foreach (var messageType in fileDescriptor.MessageTypes)
                {
                    var messageDescritprion = new ProtoMessageDescription();

                    messageDescritprion.ClassName = messageType.Name;

                    foreach (var fieldDescription in messageType.Fields.InDeclarationOrder())
                    {
                        //messageDescritprion.ad



                    }


                }
            }

            return null;
        }
    }
}
