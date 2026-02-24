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
        { "Edm.Duration", "TimeSpan" },
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
        internal string NamespaceName => $"{navigation.Metadata.Namespace.Replace(".", "_")}";
        internal string JsonName => $"{navigation.NamespaceName}_Json";
        internal string HelperName => $"{navigation.NamespaceName}_Helper";
        internal string ServiceName => $"{navigation.NamespaceName}_Service";

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

        internal string ClassName => entityType.Name.ToValidCSharpClassName();

        internal string InputName => $"{entityType.ClassName}Input";

        internal string BaseTypeCSharp
        {
            get
            {
                if (string.IsNullOrWhiteSpace(entityType.BaseType))
                {
                    return string.Empty;
                }
                return $"{StripNamespace(entityType.BaseType).ToValidCSharpClassName()}";
            }

        }

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

                if (property.IsPrimitiveType || property.IsEnumType)
                {
                    return property.GetCSharpTypeRaw(false) + "?";
                }


                return property.GetCSharpTypeRaw(true) + "?";

            }
        }

        internal bool IsPrimitiveType => property.DataType.StartsWith("Edm.") ? true : false;

        internal string CSharpType
        {
            get
            {
                var csharpType = property.GetCSharpTypeRaw(false);

                if (property.Nullable || csharpType == "string" || (!property.IsPrimitiveType && !property.IsEnumType))
                {
                    return csharpType + "?";
                }

                return csharpType;

            }
        }

        internal string GetCSharpTypeRaw(bool isInput)
        {
            var dataType = property.DataType;

            if (isInput)
            {
                dataType = dataType + "Input";
            }

            // Handle Collection types
            if (property.IsCollection)
            {

                // Check if it's an Edm type
                if (property.IsPrimitiveType)
                {
                    var elementType = MapEdmTypeToCSharp(property.DataType);
                    return $"List<{elementType}>";
                }
                else
                {
                    // Custom type (complex type or enum) - strip namespace
                    var typeName = StripNamespace(dataType).ToValidCSharpClassName();
                    return $"List<{typeName}>";
                }
            }

            // For custom types (complex types or enums), strip namespace prefix
            if (!property.IsPrimitiveType)
            {
                return StripNamespace(dataType).ToValidCSharpClassName();
            }

            // For Edm primitive types, map to C# types
            return MapEdmTypeToCSharp(property.DataType);
        }

    }

    extension(ODataFunction function)
    {
        /// <summary>
        /// Returns the C# return type for this function/action.
        /// Returns "void" (empty) when there is no return type.
        /// </summary>
        internal string CSharpReturnType
        {
            get
            {
                if (string.IsNullOrEmpty(function.ReturnType))
                    return "void";

                var rt = function.ReturnType;

                bool isCollection = rt.StartsWith("Collection(") && rt.EndsWith(")");
                if (isCollection)
                    rt = rt.Substring("Collection(".Length, rt.Length - "Collection(".Length - 1);

                string csharpType = rt.StartsWith("Edm.")
                    ? MapEdmTypeToCSharp(rt)
                    : StripNamespace(rt).ToValidCSharpClassName();

                return isCollection ? $"List<{csharpType}>" : csharpType;
            }
        }

        /// <summary>
        /// Returns the C# method parameter list string (e.g. "double lat, double lon").
        /// </summary>
        internal string CSharpParameters
        {
            get
            {
                var parts = function.Parameters.Select(p => $"{p.CSharpType} {p.CSharpParameterName}");
                return string.Join(", ", parts);
            }
        }

        /// <summary>
        /// Builds the OData URL segment for a function call (GET).
        /// E.g. for GetNearestAirport(lat, lon) → "GetNearestAirport(lat={lat},lon={lon})"
        /// </summary>
        internal string FunctionUrlTemplate
        {
            get
            {
                if (!function.Parameters.Any())
                    return function.Name;

                var paramParts = function.Parameters.Select(p =>
                {
                    var isString = p.DataType.Equals("Edm.String", StringComparison.OrdinalIgnoreCase);
                    var varName = p.CSharpParameterName;
                    return isString ? $"{p.Name}='{{Uri.EscapeDataString({varName})}}'" : $"{p.Name}={{{varName}}}";
                });
                return $"{function.Name}({string.Join(",", paramParts)})";
            }
        }

        /// <summary>
        /// Returns the C# method name (Pascal-case).
        /// </summary>
        internal string CSharpMethodName => function.Name.ToValidCSharpPascalCase();
    }

    extension(ODataFunctionParameter param)
    {
        /// <summary>
        /// Returns the C# type for this parameter.
        /// </summary>
        internal string CSharpType
        {
            get
            {
                if (param.DataType.StartsWith("Edm."))
                    return MapEdmTypeToCSharp(param.DataType);
                return StripNamespace(param.DataType).ToValidCSharpClassName();
            }
        }

        /// <summary>
        /// Returns a safe camelCase C# parameter name.
        /// </summary>
        internal string CSharpParameterName => param.Name.ToValidCSharpCamelCaseParameterName();
    }

}
