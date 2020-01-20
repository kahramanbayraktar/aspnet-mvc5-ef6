using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IEditionVisitorServices
    {
        EditionVisitorEntity GetEditionVisitorById(int editionVisitorId);

        IList<EditionVisitorEntity> GetEditionVisitors(int editionId);

        int CreateEditionVisitor(EditionVisitorEntity editionVisitorEntity, int userId);

        bool UpdateEditionVisitor(EditionVisitorEntity editionVisitorEntity, int userId);

        void CreateOrUpdateEditionVisitor(EditionVisitorEntity editionVisitorEntity, int userId);

        void CreateOrUpdateEditionVisitors(IList<EditionVisitorEntity> editionVisitorEntities, int userId);
    }
}