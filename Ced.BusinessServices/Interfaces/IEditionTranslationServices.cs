using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IEditionTranslationServices
    {
        EditionTranslationEntity GetEditionTranslationById(int editionTranslationId);

        EditionTranslationEntity GetEditionTranslation(int editionId, string languageCode);

        bool Exists(int editionId, string languageCode);

        EditionTranslationEntityLight GetEditionTranslationLight(int editionId, string languageCode);

        IList<EditionTranslationEntity> GetEditionTranslationsByEdition(int editionId);

        //IList<EditionVenue> SearchVenues(int eventId, string searchTerm, int pageSize, int pageNum);
        IList<EditionVenue> SearchVenues(EditionEntity edition, string searchTerm, int pageSize, int pageNum);

        int CreateEditionTranslation(EditionTranslationEntity editionTranslationEntity, int userId);

        bool UpdateEditionTranslation(EditionTranslationEntity editionTranslationEntity, int userId, bool? autoIntegration = false);

        bool DeleteEditionTranslation(int editionTranslationId);

        bool IsEditionTranslationInfoCompleted(EditionTranslationEntity editionTranslationEntity, EditionInfoType infoType);
        
        //bool DeleteImage(int editionTranslationId, EditionImageType imageType, int userId);
    }
}
