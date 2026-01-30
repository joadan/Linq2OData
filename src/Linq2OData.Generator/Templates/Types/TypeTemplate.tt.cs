using Linq2OData.Generator.Models;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Linq2OData.Generator.Templates.Types
{
    public partial class TypeTemplate(ODataEntityType entityType, string fullNamspace, IEnumerable<ODataEntityType> derivedTypes, string metadataNamespace)
    {


        //public string BaseTypeDerived => string.IsNullOrWhiteSpace(entityType.BaseType) ? "" : $": {entityType.BaseType}";

        public string BaseTypeAndInterface
        {
            get
            {
                var result = string.IsNullOrWhiteSpace(entityType.BaseType) ? "" : $": {entityType.BaseType}";

                if (entityType.IsEntitySet)
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = " : IODataEntitySet";
                    }
                    else
                    {
                        result += ", IODataEntitySet";
                    }
                }

                return result;
            }
        }


        private string GetEntitySetAttribute()
        {
            if (entityType.IsEntitySet)
            {
                return $"[ODataEntitySet(\"{entityType.EntityPath}\")]";
            }

            return "";
        }

        private string GetEntitySetInterface()
        {
            if (entityType.IsEntitySet)
            {
                return $" : IODataEntitySet";
            }

            return "";
        }


        private string GetDerivedAttributes()
        {
            if (!derivedTypes.Any())
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[JsonPolymorphic(TypeDiscriminatorPropertyName = \"@odata.type\", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]");
            sb.AppendLine($"[JsonDerivedType(typeof({entityType.Name}))]");
            foreach (var derivedType in derivedTypes)
            {
                sb.AppendLine($"[JsonDerivedType(typeof({derivedType.Name}), \"#{metadataNamespace}.{derivedType.Name}\")]");
            }
            return sb.ToString();
        }

    }
}
