using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Generator.Models
{
    public class FileEntry
    {
        public required string FileName { get; set; }
        public required string FolderPath { get; set; }
        public required string Content { get; set; }
    }
}
