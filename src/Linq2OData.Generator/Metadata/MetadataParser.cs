using Linq2OData.Generator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Generator.Metadata
{
    public static class MetadataParser
    {
        public static ODataMetadata Parse(string content)
        {
    
            //For now we only support OData 2.0
            return MetadataParserVersion2.Parse(content);


        }

    }
}
