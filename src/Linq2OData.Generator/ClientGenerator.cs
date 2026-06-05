using Linq2OData.Core;
using Linq2OData.Core.Metadata;
using Linq2OData.Generator.Models;
using Linq2OData.Generator.Templates.Client;
using Linq2OData.Generator.Templates.Input;
using Linq2OData.Generator.Templates.Types;


namespace Linq2OData.Generator;

public class ClientGenerator(ClientRequest request)
{
    private List<FileEntry> files = [];
    //private List<ODataMetadata> metadataCollection = [];
    private ODataVersion? version = null;

    public List<FileEntry> GenerateClient()
    {

        files.Clear();

        if (request.Metadata == null || request.Metadata.Count == 0)
        {
            throw new Exception("At least one metadata document must be provided.");
        }


        //Check/Set version Metadata
        foreach (var requestMetadata in request.Metadata)
        {

            if (version != null && requestMetadata.Metadata.ODataVersion != version)
            {
                throw new Exception($"All metadata documents must have the same OData version. Current is {version.ToString()}, trying to add {requestMetadata.Metadata.ODataVersion}");
            }
            else
            {
                version = requestMetadata.Metadata.ODataVersion;
            }

        }

        GenerateTypesCode();
        GenerateClientCode();
        GenerateInputTypesCode();

        return files;
    }



    public List<FileEntry> GenerateClient(string outputFolder)
    {
        var files = GenerateClient();

        foreach (var file in files)
        {

            var directoryPath = Path.Combine(outputFolder, file.FolderPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var filePath = Path.Combine(directoryPath, file.FileName);
            File.WriteAllText(filePath, file.Content);

        }

        return files;
    }

   
    private void GenerateClientCode()
    {
        var templateText = new ClientTemp(request, (ODataVersion)version!).TransformText();
        AddFile("Client", request.Name + ".cs", templateText);

        if (request.IncludeServiceMetadata)
        {
            var templateHelperText = new ClientHelperTemplate(request).TransformText();
            AddFile("Client", request.Name + "Helpers.cs", templateHelperText);
        }

        GenerateActionsCode();
    }

    private void GenerateActionsCode()
    {
        foreach (var clientMetadata in request.Metadata)
        {
            if (!clientMetadata.Metadata.Schemas.Any(s => s.Functions.Any()))
                continue;

            var actionsText = new ActionsTemplate(request, clientMetadata, (ODataVersion)version!).TransformText();
            AddFile("Client", clientMetadata.ActionsClassName + ".cs", actionsText);
        }
    }

    private void GenerateTypesCode()
    {
        foreach (var clientMetadata in request.Metadata)
        {
            var metadata = clientMetadata.Metadata;

            // Map from simple type name → full C# namespace for cross-schema using directives
            var typeToNsMap = metadata.Schemas
                .SelectMany(s => s.EntityTypes.Select(et => (Schema: s, EntityType: et)))
                .ToDictionary(
                    x => x.EntityType.Name,
                    x => request.Namespace + "." + (x.EntityType.SchemaNamespace ?? x.Schema.Namespace)
                );

            foreach (var schema in metadata.Schemas)
            {
                // Generate enums
                foreach (var enumType in schema.EnumTypes)
                {
                    var enumNs = request.Namespace + "." + (enumType.SchemaNamespace ?? schema.Namespace);
                    var enumText = new Templates.Types.EnumTemplate(enumType, enumNs).TransformText();
                    AddFile("Enums", enumType.Name + ".cs", enumText);
                }

                // Generate entity and complex types
                foreach (var entityType in schema.EntityTypes)
                {
                    var typeNs = request.Namespace + "." + (entityType.SchemaNamespace ?? schema.Namespace);
                    var schemaNamespace = entityType.SchemaNamespace ?? schema.Namespace;
                    var classText = new TypeTemplate(entityType, typeNs, clientMetadata.ServicePath, request.InterfaceName, schema.GetAllDerivedTypes(entityType.Name), schemaNamespace, (ODataVersion)version!, typeToNsMap).TransformText();
                    AddFile("Types", entityType.ClassName + ".cs", classText);
                }
            }
        }
    }

    private void GenerateInputTypesCode()
    {
        foreach (var clientMetadata in request.Metadata)
        {
            var metadata = clientMetadata.Metadata;

            var typeToNsMap = metadata.Schemas
                .SelectMany(s => s.EntityTypes.Select(et => (Schema: s, EntityType: et)))
                .ToDictionary(
                    x => x.EntityType.Name,
                    x => request.Namespace + "." + (x.EntityType.SchemaNamespace ?? x.Schema.Namespace)
                );

            foreach (var schema in metadata.Schemas)
            {
                foreach (var entityType in schema.EntityTypes)
                {
                    var typeNs = request.Namespace + "." + (entityType.SchemaNamespace ?? schema.Namespace);
                    var classText = new InputTemplate(entityType, typeNs, typeToNsMap).TransformText();
                    AddFile("Inputs", entityType.InputName + ".cs", classText);
                }
            }
        }
    }


    private void AddFile(string directoryName, string fileName, string content)
    {
        var infoText = $@"// <auto-generated>
//     This code was generated by Linq2OData, https://github.com/joadan/Linq2OData/ 
//     Changes to this sharedFile may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>

#nullable enable
";

        content = infoText + content;

        files.Add(new FileEntry
        {
            FolderPath = directoryName,
            FileName = GetUniqueFileName(directoryName, fileName),
            Content = content
        });
    }

    private string GetUniqueFileName(string directoryName, string fileName)
    {
        
        var existingFile = files.FirstOrDefault(f => f.FolderPath.Equals(directoryName, StringComparison.CurrentCultureIgnoreCase) && f.FileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));
        if (existingFile == null)
        {
            return fileName;
        }
       
        return GetUniqueFileName(directoryName, "_" + fileName);

    }

}
