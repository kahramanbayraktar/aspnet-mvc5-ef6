namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EditionKeyVisitor")]
    public partial class EditionKeyVisitor
    {
        public int EditionKeyVisitorId { get; set; }

        public int EditionId { get; set; }

        public int KeyVisitorId { get; set; }

        [Required]
        [StringLength(500)]
        public string Value { get; set; }

        public int EventBEID { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Edition Edition { get; set; }

        public virtual KeyVisitor KeyVisitor { get; set; }
    }
}
