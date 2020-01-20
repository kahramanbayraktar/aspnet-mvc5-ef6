using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.BusinessEntities
{
    public class EditionKeyVisitorEntity
    {
        public int EditionKeyVisitorId { get; set; }

        [Required]
        public int EditionId { get; set; }

        public int EventBEID { get; set; }

        [Required]
        public int KeyVisitorId { get; set; }

        [Required]
        public string Value { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }


        public EditionEntity Edition { get; set; }

        public KeyVisitorEntity KeyVisitor { get; set; }
    }
}
