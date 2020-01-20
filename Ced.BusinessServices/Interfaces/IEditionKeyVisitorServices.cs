using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IEditionKeyVisitorServices
    {
        EditionKeyVisitorEntity GetEditionKeyVisitorById(int editionKeyVisitorId);

        bool EditionKeyVisitorExists(int editionId, int keyVisitorId);

        IList<EditionKeyVisitorEntity> GetEditionKeyVisitors(int editionId);

        int CreateEditionKeyVisitor(EditionKeyVisitorEntity editionKeyVisitorEntity, int userId);

        bool DeleteEditionKeyVisitor(int editionKeyVisitorId);

        bool DeleteAllEditionKeyVisitors(int editionId);
    }
}