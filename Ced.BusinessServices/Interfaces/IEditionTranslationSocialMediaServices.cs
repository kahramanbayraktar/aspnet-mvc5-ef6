using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IEditionTranslationSocialMediaServices
    {
        EditionTranslationSocialMediaEntity GetById(int id);

        EditionTranslationSocialMediaEntity Get(int editionTranslationId, string socialMediaId);

        IList<EditionTranslationSocialMediaEntity> GetByEdition(int editionId, string languageCode = null);

        int Create(EditionTranslationSocialMediaEntity entity, int userId);

        bool Update(int id, EditionTranslationSocialMediaEntity entity, int userId);

        bool Delete(int id);
    }
}