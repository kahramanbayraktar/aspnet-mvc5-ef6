using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IEditionCohostServices
    {
        EditionCohostEntity GetEditionCohostById(int editionCohostId);

        bool EditionCohostExists(int firstEditionId, int secondEditionId);

        IList<EditionCohostEntity> GetEditionCohosts(int editionId);

        int CreateEditionCohost(EditionCohostEntity editionCohostEntity, int userId);

        bool DeleteEditionCohost(int editionCohostId);

        bool DeleteAllEditionCohosts(int editionId);
    }
}