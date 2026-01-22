using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Generator.Models
{
    public class ClientRequest
    {
        public required string Name { get; set; }
        public required string Namespace { get; set; }

        public List<string> MetadataList { get; set; } = [];

    }
}
