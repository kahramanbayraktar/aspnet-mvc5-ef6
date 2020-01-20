namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Subscription")]
    public partial class Subscription
    {
        public int SubscriptionId { get; set; }

        public int EditionId { get; set; }

        [StringLength(80)]
        public string UserEmail { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }
    }
}
