namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clnd_EventDirectors
    {
        [Key]
        public int EventDirectorID { get; set; }

        public int EventID { get; set; }

        public int EditionID { get; set; }

        public int DwEventID { get; set; }

        [Required]
        [StringLength(100)]
        public string EventBEId { get; set; }

        [StringLength(60)]
        public string MasterCode { get; set; }

        [StringLength(255)]
        public string EditionName { get; set; }

        [Required]
        [StringLength(60)]
        public string MasterName { get; set; }

        [Required]
        [StringLength(60)]
        public string DirectorFullName { get; set; }

        [StringLength(60)]
        public string DirectorManagingOfficeName { get; set; }
    }
}
