namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notification")]
    public partial class Notification
    {
        public int NotificationId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public byte NotificationType { get; set; }

        public int? ReceiverId { get; set; }

        [StringLength(50)]
        public string ReceiverEmail { get; set; }

        public bool? Displayed { get; set; }

        public bool? SentByEmail { get; set; }

        public DateTime? EmailedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DisplayedOn { get; set; }

        public int? EventId { get; set; }

        public int? EditionId { get; set; }

        public virtual Edition Edition { get; set; }

        public virtual Event Event { get; set; }
    }
}
