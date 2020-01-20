using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IEditionCountryServices
    {
        EditionCountryEntity GetEditionCountryById(int editionCountryId);

        IList<EditionCountryEntity> GetEditionCountriesByEdition(int editionId, EditionCountryRelationType relationType);

        int CreateEditionCountry(EditionCountryEntity editionCountryEntity, int userId);

        bool UpdateEditionCountry(int editionCountryId, EditionCountryEntity editionCountryEntity, int userId);

        bool DeleteEditionCountry(int editionCountryId);
        
        bool DeleteAllEditionCountriesByEdition(int editionId);
    }
}