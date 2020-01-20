namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Log")]
    public partial class Log
    {
        public int LogId { get; set; }

        [Required]
        [StringLength(50)]
        public string Ip { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; }

        public int? ActorUserId { get; set; }

        [StringLength(50)]
        public string ActorUserEmail { get; set; }

        [Required]
        [StringLength(50)]
        public string Controller { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; }

        [Required]
        [StringLength(5)]
        public string MethodType { get; set; }

        [StringLength(50)]
        public string EntityType { get; set; }

        [StringLength(50)]
        public string ActionType { get; set; }

        public int? EntityId { get; set; }

        [StringLength(255)]
        public string EntityName { get; set; }

        public int? EventId { get; set; }

        [StringLength(60)]
        public string EventName { get; set; }

        [StringLength(1000)]
        public string AdditionalInfo { get; set; }

        public bool IsImpersonated { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
