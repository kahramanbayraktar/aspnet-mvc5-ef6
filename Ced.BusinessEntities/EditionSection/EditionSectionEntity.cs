using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.BusinessEntities
{
    public class EditionSectionEntity
    {
        public int EditionSectionId { get; set; }

        [Required]
        public int EditionId { get; set; }

        [Required]
        public string Sections { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }


        public EditionEntity Edition { get; set; }
    }
}
