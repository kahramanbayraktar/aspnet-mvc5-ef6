namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EditionTranslationSocialMedia")]
    public partial class EditionTranslationSocialMedia
    {
        public int EditionTranslationSocialMediaId { get; set; }

        public int EditionTranslationId { get; set; }

        public int EditionId { get; set; }

        [Required]
        [StringLength(20)]
        public string SocialMediaId { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual EditionTranslation EditionTranslation { get; set; }

        public virtual SocialMedia SocialMedia { get; set; }
    }
}
