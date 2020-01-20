namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            Editions = new HashSet<Edition>();
            EventDirectors = new HashSet<EventDirector>();
            Notifications = new HashSet<Notification>();
        }

        public int EventId { get; set; }

        [Required]
        [StringLength(60)]
        public string MasterName { get; set; }

        [StringLength(60)]
        public string MasterCode { get; set; }

        [StringLength(100)]
        public string Region { get; set; }

        [Required]
        [StringLength(60)]
        public string EventType { get; set; }

        [StringLength(60)]
        public string EventTypeCode { get; set; }

        [StringLength(100)]
        public string Industry { get; set; }

        [StringLength(100)]
        public string SubIndustry { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        [StringLength(60)]
        public string EventBusinessClassification { get; set; }

        public DateTime CreateTime { get; set; }

        public int? CreateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int? UpdateUser { get; set; }

        public DateTime? UpdateTimeByAutoIntegration { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Edition> Editions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventDirector> EventDirectors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
