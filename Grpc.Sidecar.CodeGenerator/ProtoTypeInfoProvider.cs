using Google.Protobuf.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ProtoBuf.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;

namespace Grpc.Sidecar.CodeGenerator
{
    public class ProtoTypeInfoProvider
    {
        public IList<Assembly> GeneratedAssemblies { get; set; } = new List<Assembly>();

        public void LoadProtoFile(string protoName, byte[] protoByte)
        {
            var sourceCodes = GenerateCSharpCode(protoName, protoByte);

            foreach (var sourceCode in sourceCodes)
            {
                var generatedAssembly = GenerateAndLoadAssembly(sourceCode);
                GeneratedAssemblies.Add(generatedAssembly);
            }
        }

        private IList<string> GenerateCSharpCode(string protoName, byte[] protoByte)
        {
            //Error handling
            try
            {
                var set = new FileDescriptorSet();

                using var memoryStream = new MemoryStream(protoByte);
                using var textReader = new StreamReader(memoryStream);

                //TODO includeInOutput=true ?
                set.Add(protoName, true, textReader);
                set.Process();

                //This is because of the fact that the current version of protobuf-net nuget does not support grpc service generation
                var data = File.ReadAllText("D:\\Projects\\gRPC-Sidecar\\Grpc.Sidecar\\Grpc.Sidecar.CodeGenerator\\ProtoProxy.txt");
                var codeFiles = new List<CodeFile> { new CodeFile("greeting.proto", data) };

                return codeFiles.Select(t => t.Text).ToList();
                //var codeFiles = CSharpCodeGenerator.Default.Generate(set, options: new Dictionary<string, string> { { "services", "grpc" } });

            }
            catch (Exception e)
            {

                throw;
            }
        }

        private Assembly GenerateAndLoadAssembly(string sourceCode)
        {
            try
            {
                MetadataReference[] references = new MetadataReference[]
                {
                    MetadataReference.CreateFromFile("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\5.0.0\\ref\\net5.0\\System.Runtime.dll"),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.Grpc.CallContext).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.ProtoContractAttribute).Assembly.Location),
                };

                var comp = CSharpCompilation.Create(
                   assemblyName: Path.GetRandomFileName(),
                   syntaxTrees: new[] { CSharpSyntaxTree.ParseText(sourceCode) },
                   references: references,
                   options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
               );

                using var ms = new MemoryStream();
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

                return Assembly.Load(ms.ToArray());
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
