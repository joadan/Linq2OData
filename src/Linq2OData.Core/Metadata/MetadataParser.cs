using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Core.Metadata
{
    public static class MetadataParser
    {
        public static ODataMetadata Parse(string content)
        {
            var xDocument = System.Xml.Linq.XDocument.Parse(content);

            if (xDocument.Root is null) 
            { 
                throw new InvalidOperationException("Invalid OData metadata document."); 
            }
               
            // Determine OData version from the Edmx Version attribute
            var edmxVersionAttribute = xDocument.Root.Attribute("Version");
            
            if (edmxVersionAttribute != null)
            {
                var version = edmxVersionAttribute.Value;
                
                // OData v4.0 uses Edmx Version="4.0"
                if (version.StartsWith("4"))
                {
                    return MetadataParserVersion4.Parse(xDocument);
                }
            }
            
            // For OData v2, the Edmx Version is "1.0" and DataServiceVersion is "2.0"
            // Check DataServiceVersion as a fallback
            var dataServicesElement = xDocument.Root.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "DataServices");
                
            if (dataServicesElement != null)
            {
                var dataServiceVersionAttribute = dataServicesElement.Attributes()
                    .FirstOrDefault(a => a.Name.LocalName == "DataServiceVersion");
                    
                if (dataServiceVersionAttribute != null && dataServiceVersionAttribute.Value.StartsWith("2"))
                {
                    return MetadataParserVersion2.Parse(xDocument);
                }
            }

            // Default to OData 2.0 parser for backward compatibility
            return MetadataParserVersion2.Parse(xDocument);
        }

    }
}
