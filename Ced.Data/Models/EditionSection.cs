using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ced.Data.Models
{
    [Table("EditionSection")]
    public class EditionSection
    {
        public int EditionSectionId { get; set; }

        public int EditionId { get; set; }

        [Required]
        [StringLength(500)]
        public string Sections { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        
        public virtual Edition Edition { get; set; }
    }
}
