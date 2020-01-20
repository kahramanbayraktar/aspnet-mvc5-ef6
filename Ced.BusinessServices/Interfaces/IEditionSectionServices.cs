using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.BusinessServices
{
    public interface IEditionSectionServices
    {
        EditionSectionEntity GetById(int id);

        EditionSectionEntity Get(int editionId, string sections);

        IList<EditionSectionEntity> GetByEdition(int editionId);

        int Create(EditionSectionEntity entity, int userId);

        bool Delete(int id);
    }
}