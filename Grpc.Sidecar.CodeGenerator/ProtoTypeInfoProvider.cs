using Google.Protobuf.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ProtoBuf.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static Grpc.Sidecar.CodeGenerator.GrpcUnaryInvokerCodeBuilder;

namespace Grpc.Sidecar.CodeGenerator
{
    public class ProtoTypeInfoProvider
    {
        private readonly IGrpcInvokerCodeBuilder _invokerCodeBuilder;
        public ProtoTypeInfoProvider(IGrpcInvokerCodeBuilder invokerCodeBuilder)
        {
            _invokerCodeBuilder = invokerCodeBuilder ?? throw new ArgumentNullException(nameof(invokerCodeBuilder));
        }

        public IList<Assembly> GeneratedAssemblies { get; set; } = new List<Assembly>();

        public void LoadProtoFile(string protoName, byte[] protoByte)
        {
            var sourceCodes = GenerateCSharpCode(protoName, protoByte);

            var generatedProtoAssembly = GenerateAndLoadProtoAssembly(sourceCodes, new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.Grpc.CallContext).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.ProtoContractAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(t=>t.GetName().Name == "System.Runtime").Location)
                });

            GeneratedAssemblies.Add(generatedProtoAssembly.assembly);

            var GrpcServiceInvokers = GenerateAndLoadGrpcInvokers(generatedProtoAssembly.assembly, generatedProtoAssembly.assemblyBytes);
            GeneratedAssemblies.Add(GrpcServiceInvokers);
        }

        private Assembly GenerateAndLoadGrpcInvokers(Assembly generatedProtoAssembly, byte[] generatedProtoAssemblyByte)
        {
            var serviceDescriptions = CreateServiceDescriptions(generatedProtoAssembly);

            //TODO change the null value
            var codes = serviceDescriptions.Select(t => _invokerCodeBuilder.GenerateCode(t, null));

            var (result, _) = GenerateAndLoadProtoAssembly(codes.ToList(), new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    //This one might be changed due to refactoring
                    MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location),
                    MetadataReference.CreateFromFile(typeof(Core.ChannelBase).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ProtoBuf.Grpc.Configuration.ClientFactory).Assembly.Location),
                    MetadataReference.CreateFromFile(AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(t=>t.GetName().Name == "System.Runtime").Location)
                });

            return result;
        }

        private IList<ServiceDescription> CreateServiceDescriptions(Assembly protoAssemblies)
        {
            var serviceTypes = protoAssemblies.GetTypes()
                .Where(t => t.IsInterface && t.CustomAttributes.Any(a => a.AttributeType ==
                typeof(ProtoBuf.Grpc.Configuration.ServiceAttribute)));

            return serviceTypes.Select(t => GetTypeDescriptions(t)).ToList();
        }

        private static ServiceDescription GetTypeDescriptions(Type serviceType)
        {
            return new ServiceDescription()
            {
                ServiceNamespaceName = serviceType.Namespace,
                MethodDescriptions = serviceType.GetMethods().Where(t => t.Name == "GreetSimpleAsync").Select(m =>
                   new MethodDescription
                   {
                       //TODO
                       CommmunicationType = MethodDescription.MethodType.Unary,
                       Name = m.Name,
                       RequestTypeName = m.GetParameters()[0].ParameterType.Name,
                       ResponseTypeName = m.ReturnType.GenericTypeArguments[0].Name,
                       ServiceTypeName = serviceType.Name
                   }).ToList()
            };
        }

        private IList<string> GenerateCSharpCode(string protoName, byte[] protoByte)
        {
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

        private (Assembly assembly, byte[] assemblyBytes) GenerateAndLoadProtoAssembly(IList<string> sourceCodes, MetadataReference[] metadataReferences)
        {
            try
            {
                var comp = CSharpCompilation.Create(
                   assemblyName: Path.GetRandomFileName(),
                   syntaxTrees: sourceCodes.Select(t => CSharpSyntaxTree.ParseText(t)),
                   references: metadataReferences,
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

                var assemblyBytes = ms.ToArray();
                return (Assembly.Load(assemblyBytes), assemblyBytes);
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }




}
