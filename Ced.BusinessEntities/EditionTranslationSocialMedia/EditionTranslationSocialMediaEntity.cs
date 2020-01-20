using System;

namespace Ced.BusinessEntities
{
    public class EditionTranslationSocialMediaEntity : ICloneable
    {
        public int EditionTranslationSocialMediaId { get; set; }
        
        public int EditionTranslationId { get; set; }
        
        public int EditionId { get; set; }

        public string SocialMediaId { get; set; }
        
        public string AccountName { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public int CreatedBy { get; set; }
        
        public DateTime UpdatedOn { get; set; }
        
        public int UpdatedBy { get; set; }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}