namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EditionVisitor")]
    public partial class EditionVisitor
    {
        public int EditionVisitorId { get; set; }

        public int EditionId { get; set; }

        public byte DayNumber { get; set; }

        public short VisitorCount { get; set; }

        public short? RepeatVisitCount { get; set; }

        public short? OldVisitorCount { get; set; }

        public short? NewVisitorCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Edition Edition { get; set; }
    }
}
