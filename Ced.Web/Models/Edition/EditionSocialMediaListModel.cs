using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionSocialMediaListModel
    {
        public int EditionTranslationId { get; set; }

        public int EditionId { get; set; }

        public string LanguageCode { get; set; }

        public IList<EditionTranslationSocialMediaEntity> SocialMedias { get; set; }
    }
}