using Linq2OData.Generator;
using Linq2OData.Generator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace Linq2OData.Tests;

public class ClientGeneratorTests
{

    private string odataDemoMetadataV2;
    private string odataDemoMetadataV4;
    private string sapSalesQuotationMetadataV2;
    private string largeMetadaV4;
    private string trippinMetadataV4;

    public ClientGeneratorTests()
    {

        odataDemoMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "ODataDemo.xml"));
        odataDemoMetadataV4 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "ODataDemo.xml"));
        sapSalesQuotationMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "SapSalesQuotation.xml"));
        largeMetadaV4 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "LargeMetadata.xml"));
        trippinMetadataV4 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "Trippin.xml"));
    }

    [Fact]
    public void GenerateClientAsyncV2_WithValidMetadata_ShouldGenerateFiles()
    {
   
        var request = new ClientRequest
        {
            Name = "ODataDemoClient",
            Namespace = "MyApp.OData",
        };
        request.AddMetadata(odataDemoMetadataV2);

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        var supplier = files.First(f => f.FileName == "Supplier.cs" && f.FolderPath == "Types"); 

        Console.WriteLine($"{supplier.Content}");

        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files);
    }

    [Fact]
    public void GenerateClientAsyncV4_WithValidMetadata_ShouldGenerateFiles()
    {

        var request = new ClientRequest
        {
            Name = "ODataDemoClient",
            Namespace = "MyApp.OData",
        };
        request.AddMetadata(odataDemoMetadataV4);

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        var supplier = files.First(f => f.FileName == "Supplier.cs" && f.FolderPath == "Types");

        Console.WriteLine($"{supplier.Content}");

        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files);
    }

    [Fact]
    public void GenerateClientAsyncV4_WithValidLargeMetadata_ShouldGenerateFiles()
    {

        var request = new ClientRequest
        {
            Name = "ODataLargeClient",
            Namespace = "MyNamespace",
        };
        request.AddMetadata(odataDemoMetadataV4);

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();


        // Act - Compile the generated code using Roslyn
        var compilation = CompileGeneratedCode(files);

        // Assert
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (diagnostics.Any())
        {
            var errors = string.Join("\n", diagnostics.Select(d =>
                $"{d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
            Assert.Fail($"Compilation failed with {diagnostics.Count} error(s):\n{errors}");
        }

        Assert.Empty(diagnostics);

    }

    /// <summary>
    /// Tests that the generated OData client code (V4) compiles successfully using Roslyn.
    /// This validates that the code generator produces syntactically correct C# code
    /// with all necessary references and using directives.
    /// </summary>
    [Fact]
    public void GeneratedClient_ShouldCompileSuccessfully()
    {
        // Arrange
        var request = new ClientRequest
        {
            Name = "ODataDemoClient",
            Namespace = "MyApp.OData",
        };
        request.AddMetadata(odataDemoMetadataV4);

        var generator = new ClientGenerator(request);
        var files = generator.GenerateClient();

        // Act - Compile the generated code using Roslyn
        var compilation = CompileGeneratedCode(files);

        // Assert
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (diagnostics.Any())
        {
            var errors = string.Join("\n", diagnostics.Select(d => 
                $"{d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
            Assert.Fail($"Compilation failed with {diagnostics.Count} error(s):\n{errors}");
        }

        Assert.Empty(diagnostics);
    }

    /// <summary>
    /// Tests that the generated OData client code (V2) compiles successfully using Roslyn.
    /// This validates that the code generator produces syntactically correct C# code
    /// for OData V2 metadata with all necessary references and using directives.
    /// </summary>
    [Fact]
    public void GeneratedClientV2_ShouldCompileSuccessfully()
    {
        // Arrange
        var request = new ClientRequest
        {
            Name = "ODataDemoClient",
            Namespace = "MyApp.OData",
        };

        request.AddMetadata(odataDemoMetadataV2);

        var generator = new ClientGenerator(request);
        var files = generator.GenerateClient();

        // Act - Compile the generated code using Roslyn
        var compilation = CompileGeneratedCode(files);

        // Assert
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (diagnostics.Any())
        {
            var errors = string.Join("\n", diagnostics.Select(d =>
                $"{d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
            Assert.Fail($"Compilation failed with {diagnostics.Count} error(s):\n{errors}");
        }

        Assert.Empty(diagnostics);
    }

    /// <summary>
    /// Compiles the generated client code using Roslyn to verify syntactic correctness.
    /// This method:
    /// 1. Creates a global usings file to provide implicit .NET types (Task, List, etc.)
    /// 2. Parses all generated files into syntax trees
    /// 3. Loads all necessary assembly references including Linq2OData.Core
    /// 4. Creates a CSharpCompilation and returns it for diagnostic analysis
    /// </summary>
    /// <param name="files">The list of generated files to compile</param>
    /// <returns>A CSharpCompilation object containing the compilation result</returns>
    private CSharpCompilation CompileGeneratedCode(List<FileEntry> files)
    {
        // Add global usings that are implicit in .NET projects
        var globalUsings = @"
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
";

        // Parse all generated files into syntax trees
        var syntaxTrees = new List<SyntaxTree>
        {
            CSharpSyntaxTree.ParseText(globalUsings, path: "GlobalUsings.cs")
        };

        syntaxTrees.AddRange(files.Select(file =>
            CSharpSyntaxTree.ParseText(file.Content, path: $"{file.FolderPath}/{file.FileName}")
        ));

        // Get the location of basic runtime assemblies
        var runtimePath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        // Build comprehensive list of references using all loaded assemblies
        var references = new List<MetadataReference>();

        // Force load Linq2OData.Client if not already loaded
        var linq2ODataClientAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Linq2OData.Core");

        if (linq2ODataClientAssembly == null)
        {
            // Load it from the output directory
            var clientPath = Path.Combine(AppContext.BaseDirectory, "Linq2OData.Core.dll");
            if (File.Exists(clientPath))
            {
                linq2ODataClientAssembly = Assembly.LoadFrom(clientPath);
            }
        }

        // Add all currently loaded assemblies (after ensuring Linq2OData.Core is loaded)
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToList();

        foreach (var assembly in loadedAssemblies)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
            catch
            {
                // Skip any problematic assemblies
            }
        }

        // Ensure we have critical runtime DLLs
        var criticalAssemblies = new[]
        {
            "System.Runtime.dll",
            "System.Collections.dll",
            "System.Linq.dll",
            "System.Net.Primitives.dll",
            "System.Net.Http.dll",
            "System.Text.Json.dll",
            "System.ComponentModel.Annotations.dll",
            "System.ComponentModel.Primitives.dll",
            "System.Threading.Tasks.dll",
            "System.Private.CoreLib.dll",
            "netstandard.dll"
        };

        foreach (var dllName in criticalAssemblies)
        {
            var dllPath = Path.Combine(runtimePath, dllName);
            if (File.Exists(dllPath))
            {
                try
                {
                    var reference = MetadataReference.CreateFromFile(dllPath);
                    if (!references.Any(r => r.Display == reference.Display))
                    {
                        references.Add(reference);
                    }
                }
                catch
                {
                    // Skip if can't load
                }
            }
        }

        // Create compilation
        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratedClient",
            syntaxTrees: syntaxTrees,
            references: references.Distinct(),
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable
            )
        );

        return compilation;
    }

    [Fact]
    public void GenerateClient_WithEnums_ShouldGenerateEnumFiles()
    {
        // Arrange
        var request = new ClientRequest
        {
            Name = "TrippinClient",
            Namespace = "MyApp.OData",
        };
        request.AddMetadata(trippinMetadataV4);

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        // Assert - Check that enum files are generated
        var personGenderEnum = files.FirstOrDefault(f => f.FileName == "PersonGender.cs" && f.FolderPath == "Enums");
        Assert.NotNull(personGenderEnum);
        Assert.Contains("public enum PersonGender", personGenderEnum.Content);
        Assert.Contains("Male = 0", personGenderEnum.Content);
        Assert.Contains("Female = 1", personGenderEnum.Content);
        Assert.Contains("Unknown = 2", personGenderEnum.Content);
        Assert.Contains("[JsonConverter(typeof(JsonStringEnumConverter))]", personGenderEnum.Content);

        var featureEnum = files.FirstOrDefault(f => f.FileName == "Feature.cs" && f.FolderPath == "Enums");
        Assert.NotNull(featureEnum);
        Assert.Contains("public enum Feature", featureEnum.Content);
        Assert.Contains("Feature1 = 0", featureEnum.Content);
        Assert.Contains("Feature4 = 3", featureEnum.Content);
        Assert.Contains("[JsonConverter(typeof(JsonStringEnumConverter))]", featureEnum.Content);

        // Assert - Check that Person entity has properties with enum types
        var personEntity = files.FirstOrDefault(f => f.FileName == "Person.cs" && f.FolderPath == "Types");
        Assert.NotNull(personEntity);
        Assert.Contains("public PersonGender Gender { get; set; }", personEntity.Content);
        Assert.Contains("public Feature FavoriteFeature { get; set; }", personEntity.Content);
    }

    [Fact]
    public void GeneratedClientWithEnums_ShouldCompileSuccessfully()
    {
        // Arrange
        var request = new ClientRequest
        {
            Name = "TrippinClient",
            Namespace = "MyApp.OData"
        };
        request.AddMetadata(trippinMetadataV4);

        var generator = new ClientGenerator(request);
        var files = generator.GenerateClient();

        // Act - Compile the generated code using Roslyn
        var compilation = CompileGeneratedCode(files);

        // Assert
        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (diagnostics.Any())
        {
            var errors = string.Join("\n", diagnostics.Select(d =>
                $"{d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan()}"));
            Assert.Fail($"Compilation failed with {diagnostics.Count} error(s):\n{errors}");
        }

        Assert.Empty(diagnostics);
    }

}
