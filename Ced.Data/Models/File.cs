namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("File")]
    public partial class File
    {
        public int FileId { get; set; }

        [Required]
        [StringLength(100)]
        public string FileName { get; set; }

        public int EntityId { get; set; }

        [Required]
        [StringLength(20)]
        public string EntityType { get; set; }

        [Required]
        [StringLength(20)]
        public string FileType { get; set; }

        [StringLength(5)]
        public string LanguageCode { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedByEmail { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedByFullName { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        [StringLength(50)]
        public string UpdatedByEmail { get; set; }

        [StringLength(100)]
        public string UpdatedByFullName { get; set; }
    }
}
