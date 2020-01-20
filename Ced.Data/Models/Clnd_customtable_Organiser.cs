namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clnd_customtable_Organiser
    {
        [Key]
        public int ItemID { get; set; }

        [Required]
        [StringLength(500)]
        public string OrganiserName { get; set; }

        public int? ItemOrder { get; set; }

        public Guid ItemGUID { get; set; }
    }
}
