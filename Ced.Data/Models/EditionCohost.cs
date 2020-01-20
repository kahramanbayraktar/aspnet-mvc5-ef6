namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EditionCohost")]
    public partial class EditionCohost
    {
        public int EditionCohostId { get; set; }

        public int FirstEditionId { get; set; }

        public int SecondEditionId { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public virtual Edition Edition { get; set; }

        public virtual Edition Edition1 { get; set; }
    }
}
