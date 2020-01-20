using System;

namespace Ced.BusinessEntities
{
    public class FileTypeAttribute : Attribute
    {
        public string[] AllowedExtensions { get; set; }

        public string FolderName { get; set; }

        public bool Private { get; set; }
    }
}