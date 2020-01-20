namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EditionCountry")]
    public partial class EditionCountry
    {
        public int EditionCountryId { get; set; }

        public int EditionId { get; set; }

        [Required]
        [StringLength(3)]
        public string CountryCode { get; set; }

        public byte RelationType { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Country Country { get; set; }

        public virtual EditionCountryRelationType EditionCountryRelationType { get; set; }
    }
}
