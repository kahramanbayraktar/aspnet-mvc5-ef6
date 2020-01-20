namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EditionTranslation")]
    public partial class EditionTranslation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EditionTranslation()
        {
            EditionTranslationSocialMedias = new HashSet<EditionTranslationSocialMedia>();
        }

        public int EditionTranslationId { get; set; }

        public int EditionId { get; set; }

        [Required]
        [StringLength(5)]
        public string LanguageCode { get; set; }

        [StringLength(60)]
        public string VenueName { get; set; }

        [StringLength(60)]
        public string MapVenueName { get; set; }

        [StringLength(255)]
        public string MapVenueFullAddress { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        [StringLength(500)]
        public string BookStandUrl { get; set; }

        [StringLength(500)]
        public string OnlineInvitationUrl { get; set; }

        [StringLength(255)]
        public string WebLogoFileName { get; set; }

        [StringLength(255)]
        public string PeopleImageFileName { get; set; }

        [StringLength(255)]
        public string ProductImageFileName { get; set; }

        [StringLength(255)]
        public string MasterLogoFileName { get; set; }

        [StringLength(255)]
        public string CrmLogoFileName { get; set; }

        [StringLength(255)]
        public string IconFileName { get; set; }

        [StringLength(255)]
        public string PromotedLogoFileName { get; set; }

        [StringLength(255)]
        public string DetailsImageFileName { get; set; }

        [StringLength(500)]
        public string ExhibitorProfile { get; set; }

        [StringLength(500)]
        public string VisitorProfile { get; set; }

        public DateTime CreateTime { get; set; }

        public int? CreateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int? UpdateUser { get; set; }

        public DateTime? UpdateTimeByAutoIntegration { get; set; }

        public virtual Edition Edition { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EditionTranslationSocialMedia> EditionTranslationSocialMedias { get; set; }
    }
}
