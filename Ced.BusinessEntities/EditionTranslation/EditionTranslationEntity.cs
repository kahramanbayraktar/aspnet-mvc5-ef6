using ITE.Utility.ObjectComparison;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public class EditionTranslationEntity : ICloneable
    {
        public int EditionTranslationId { get; set; }
        
        public int EditionId { get; set; }

        public string LanguageCode { get; set; }

        [Comparable]
        [StagingDbComparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string VenueName { get; set; }

        public string MapVenueFullAddress { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string Summary { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string Description { get; set; }

        [Comparable]
        [Description("Exhibitor Profile")]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string ExhibitorProfile { get; set; }

        [Comparable]
        [Description("Visitor Profile")]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string VisitorProfile { get; set; }

        [Comparable]
        [Description("Web Logo")]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string WebLogoFileName { get; set; }

        [Comparable]
        [Description("People Image")]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string PeopleImageFileName { get; set; }

        [Comparable]
        [Description("Product Image")]
        public string ProductImageFileName { get; set; }

        [Comparable]
        [Description("Master Logo")]
        public string MasterLogoFileName { get; set; }

        [Comparable]
        [Description("CRM Logo")]
        public string CrmLogoFileName { get; set; }

        [Comparable]
        [Description("Icon")]
        public string IconFileName { get; set; }

        [Comparable]
        [Description("Promoted Logo")]
        public string PromotedLogoFileName { get; set; }

        [Comparable]
        [Description("Details Image")]
        public string DetailsImageFileName { get; set; }

        [Comparable]
        public string BookStandUrl { get; set; }

        [Comparable]
        public string OnlineInvitationUrl { get; set; }

        public DateTime CreateTime { get; set; }

        public int? CreateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int? UpdateUser { get; set; }

        public IList<EditionTranslationSocialMediaEntity> SocialMedias { get; set; }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
