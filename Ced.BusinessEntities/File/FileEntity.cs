using System;
using ITE.Utility.ObjectComparison;

namespace Ced.BusinessEntities
{
    public class FileEntity
    {
        public int FileId { get; set; }
        
        [Comparable]
        public string FileName { get; set; }
        
        public string FileExtension { get; set; }

        public int EntityId { get; set; }
        
        public string EntityType { get; set; }
        
        public EditionFileType EditionFileType { get; set; }

        public string FileTypeIcon { get; set; }

        public string LanguageCode { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public int CreatedBy { get; set; }
        
        public string CreatedByFullName { get; set; }
        
        public string CreatedByEmail { get; set; }
        
        public DateTime? UpdatedOn { get; set; }
        
        public int UpdatedBy { get; set; }

        public string UpdatedByFullName { get; set; }

        public string UpdatedByEmail { get; set; }
    }
}