using Google.Protobuf.Reflection;
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
    public class ProtoTypeGenerator
    {
        public static IList<Type> GetProtoClrTypes(string protoName, byte[] descriptorbyte)
        {
            var result = new List<Type>();

            try
            {
                var set = new FileDescriptorSet();

                using var memoryStream = new MemoryStream(descriptorbyte);
                using var textReader = new StreamReader(memoryStream);

                //TODO includeInOutput=true ?
                set.Add("greeting.proto", true, textReader);
                set.Process();

                var codeFiles = CSharpCodeGenerator.Default.Generate(set, options: new Dictionary<string, string> { { "services", "grpc" } });

                foreach (var codeFile in codeFiles)
                {
                    var generatedType = GenerateClrTypeCore(codeFile.Text);

                    result.AddRange(generatedType);
                }

                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private static IList<Type> GenerateClrTypeCore(string sourceCode)
        {
            try
            {
                MetadataReference[] references = new MetadataReference[]
                {
                    MetadataReference.CreateFromFile("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\5.0.0\\ref\\net5.0\\System.Runtime.dll"),
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

                var assembly = Assembly.Load(ms.ToArray());
                return assembly.GetTypes();
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
