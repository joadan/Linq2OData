using Linq2OData.Core.Metadata;
using Linq2OData.Generator.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Linq2OData.Generator;


internal static class MetadataExtensions
{
    // Shared Edm type to C# type mapping
    private static readonly Dictionary<string, string> EdmTypeToCSharpMapping = new()
    {
        // Common for v2 & v4
        { "Edm.String", "string" },
        { "Edm.Boolean", "bool" },
        { "Edm.Byte", "byte" },
        { "Edm.SByte", "sbyte" },
        { "Edm.Int16", "short" },
        { "Edm.Int32", "int" },
        { "Edm.Int64", "long" },
        { "Edm.Decimal", "decimal" },
        { "Edm.Single", "float" },
        { "Edm.Double", "double" },
        { "Edm.Guid", "Guid" },
        { "Edm.Binary", "byte[]" },
        // OData v2 only
        { "Edm.DateTime", "DateTime" },
        { "Edm.Time", "TimeSpan" },
        // OData v4 only
        { "Edm.Date", "DateOnly" },
        { "Edm.DateTimeOffset", "DateTimeOffset" },
        { "Edm.TimeOfDay", "TimeSpan" },
        { "Edm.GeographyPoint", "object" },
        { "Edm.GeometryPoint", "object" },
        { "Edm.Stream", "object" }
    };

    // Helper method to strip namespace from type names
    private static string StripNamespace(string typeName)
        => typeName.Contains('.') ? typeName.Split('.').Last() : typeName;

    // Helper method to map Edm types to C# types
    private static string MapEdmTypeToCSharp(string edmType)
        => EdmTypeToCSharpMapping.TryGetValue(edmType, out var csharpType) ? csharpType : "object";

    extension(ClientRequest clientRequest)
    {
        internal string NamespaceName => $"{clientRequest.Namespace.Replace(".", "_")}";
        internal string InterfaceName => $"I{clientRequest.NamespaceName}EntitySet";
        internal string ServicesName => $"{clientRequest.NamespaceName}Services";
    }

    extension(ClientMetadata navigation)
    {
        internal string JsonName => $"{navigation.Metadata.Namespace}_Json";
        internal string HelperName => $"{navigation.Metadata.Namespace}_Helper";
        internal string ServiceName => $"{navigation.Metadata.Namespace}_Service";

        internal string MetadataAsJson => System.Text.Json.JsonSerializer.Serialize(navigation.Metadata);

    }


        extension(ODataNavigation navigation)
    {
        internal string CSharpProperty
        {
            get
            {
                if (navigation.NavigationType == ODataNavigationType.Many)
                {
                    return $"List<{navigation.ToEntity}>?";
                }
                return navigation.ToEntity + "?";
            }
        }

        internal string CSharpPropertyInput
        {
            get
            {
                if (navigation.NavigationType == ODataNavigationType.Many)
                {
                    return $"List<{navigation.ToEntity}Input>?";
                }
                return navigation.ToEntity + "Input" + "?";
            }
        }

    }


    extension(ODataEntityType entityType)
    {

        internal string InputName => $"{entityType.Name}Input";

        internal string KeyResultString
        {
            get
            {
                if (!entityType.KeyProperties.Any()) { return string.Empty; }

                var keysResult = entityType.KeyProperties.Select(p =>
                {
                    return $"{p.KeyResult}";
                });

                return string.Join(",", keysResult);
            }
        }
    }

    extension(ODataProperty property)
    {

        internal string ODataAttributeString
        {
            get
            {
                if (!property.IsPrimitiveType)
                {
                    return $"[ODataMember(\"{property.Name}\", true)]";
                }

                // For primitive types, we can omit the "isComplex" parameter since it defaults to false
                return $"[ODataMember(\"{property.Name}\")]";

            }
        }

        internal string KeyResult
        {
            get
            {
                if (property.DataType.Equals("edm.string", StringComparison.InvariantCultureIgnoreCase))
                {
                    return $"{property.Name}='{{{property.Name}}}'";
                }
                else
                {
                    return $"{property.Name}={{{property.Name}}}";
                }

            }
        }

        internal string CSharpNameInput
        {
            get
            {
                // Handle Collection types
                if (property.IsCollection)
                {
                    // DataType now contains the inner type directly
                    var innerType = property.DataType;

                    // Check if it's an Edm type
                    if (innerType.StartsWith("Edm."))
                    {
                        // For Edm collections, use CSharpTypeRaw which already handles this
                        return property.CSharpTypeRaw + "?";
                    }
                    else
                    {
                        // Custom type (complex type or enum)
                        var typeName = innerType.Contains('.') ? innerType.Split('.').Last() : innerType;

                        // If the inner type is an enum, don't add "Input"
                        if (property.IsEnumType)
                        {
                            return $"List<{typeName}>?";
                        }

                        // For complex types, add "Input"
                        return $"List<{typeName}Input>?";
                    }
                }

                if (!property.DataType.StartsWith("Edm."))
                {
                    // For enum types, use the enum type directly without appending "Input"
                    if (property.IsEnumType)
                    {
                        return property.CSharpTypeRaw + "?";
                    }

                    // For complex types, append "Input"
                    return property.DataType + "Input?";
                }

                return property.CSharpTypeRaw + "?";

            }
        }

        internal bool IsPrimitiveType => property.DataType.StartsWith("Edm.") ? true : false;

        internal string CSharpType
        {
            get
            {
                var csharpType = property.CSharpTypeRaw;

                if (property.Nullable || csharpType == "string" || (!property.IsPrimitiveType && !property.IsEnumType))
                {
                    return csharpType + "?";
                }

                return csharpType;

            }
        }

        internal string CSharpTypeRaw
        {
            get
            {
                // Handle Collection types
                if (property.IsCollection)
                {
                    // DataType now contains the inner type directly
                    var innerType = property.DataType;

                    // Check if it's an Edm type
                    if (innerType.StartsWith("Edm."))
                    {
                        var elementType = MapEdmTypeToCSharp(innerType);
                        return $"List<{elementType}>";
                    }
                    else
                    {
                        // Custom type (complex type or enum) - strip namespace
                        var typeName = StripNamespace(innerType);
                        return $"List<{typeName}>";
                    }
                }

                // For custom types (complex types or enums), strip namespace prefix
                if (!property.DataType.StartsWith("Edm."))
                {
                    return StripNamespace(property.DataType);
                }

                // For Edm primitive types, map to C# types
                return MapEdmTypeToCSharp(property.DataType);
            }

        }


    }

}
