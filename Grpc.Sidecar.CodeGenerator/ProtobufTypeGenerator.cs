using Google.Protobuf.Reflection;
using Grpc.Sidecar.CodeGenerator.Abstraction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ProtoBuf.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Grpc.Sidecar.CodeGenerator
{
    public class ProtobufTypeGenerator
    {
        public Assembly GeneratedAssembly { get; set; }
        private readonly FileDescriptorSet _fileDescriptorSet;
        private readonly IGrpcInvokerCodeGenerator _grpcInvokerCodeGenerator;

        public ProtobufTypeGenerator(IGrpcInvokerCodeGenerator grpcInvokerCodeGenerator)
        {
            _fileDescriptorSet = new FileDescriptorSet();
            _grpcInvokerCodeGenerator = grpcInvokerCodeGenerator ?? throw new ArgumentNullException(nameof(grpcInvokerCodeGenerator));
        }

        public void AddProtoFile(string protoName, byte[] protoByte)
        {
            using var memoryStream = new MemoryStream(protoByte);
            using var textReader = new StreamReader(memoryStream);

            //TODO includeInOutput=true ?
            _fileDescriptorSet.Add(protoName, true, textReader);
        }


        public void GenerateAssembly()
        {
            var generatedCodes = new List<string>();

            var generatedProtoCodes = GenerateProtoCSharpCode();

            generatedCodes.AddRange(generatedProtoCodes);

            var invokerCodes = GenerateInvokersCode();

            generatedCodes.AddRange(invokerCodes);


            var metadataReferences = new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.GetAssembly(typeof(IGrpcInvoker)).Location),
                    MetadataReference.CreateFromFile(typeof(Core.ChannelBase).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.Grpc.CallContext).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.ProtoContractAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.Grpc.Configuration.ClientFactory).Assembly.Location),
                    MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(t=>t.GetName().Name == "System.Runtime").Location)
                };

            GeneratedAssembly = GenerateAssembly(generatedCodes, metadataReferences);
        }


        private IList<string> GenerateProtoCSharpCode()
        {
            _fileDescriptorSet.Process();

            //var codeFiles = CSharpCodeGenerator.Default.Generate(set, options: new Dictionary<string, string> { { "services", "grpc" } });

            //This is because of the fact that the current version of protobuf-net nuget does not support grpc service generation
            var data = File.ReadAllText("D:\\Projects\\gRPC-Sidecar\\Grpc.Sidecar\\Grpc.Sidecar.CodeGenerator\\ProtoProxy.txt");
            var codeFiles = new List<CodeFile> { new CodeFile("greeting.proto", data) };
            return codeFiles.Select(t => t.Text).ToList();
        }

        private IList<string> GenerateInvokersCode()
        {
            var result = new List<string>();

            foreach (var file in _fileDescriptorSet.Files)
            {
                foreach (var service in file.Services)
                {
                    
                    var generatedInvoker = _grpcInvokerCodeGenerator.GenerateCode(service, "Greet"/*file.Package*/);
                    result.Add(generatedInvoker);
                }
            }

            return result;
        }

        //This can be moved into another helper class
        private Assembly GenerateAssembly(IList<string> sourceCodes, MetadataReference[] metadataReferences)
        {
            try
            {
                var comp = CSharpCompilation.Create(
                   assemblyName: Path.GetRandomFileName(),
                   syntaxTrees: sourceCodes.Select(t => CSharpSyntaxTree.ParseText(t)),
                   references: metadataReferences,
                   options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
               );

                using var ms = new MemoryStream(8000);
                var result = comp.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.WriteLine($"{diagnostic.Id} - {diagnostic.GetMessage()}");
                    }
                }

                var assemblyBytes = ms.ToArray();
                var assebly = AppDomain.CurrentDomain.Load(assemblyBytes); //Assembly.Load(assemblyBytes);

                var data = assebly.GetTypes();

                return assebly;
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
