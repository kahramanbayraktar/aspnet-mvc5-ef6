using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IKeyVisitorServices
    {
        KeyVisitorEntity GetKeyVisitorById(int keyVisitorId);

        IList<KeyVisitorEntity> SearchKeyVisitors(string searchTerm, int pageSize, int pageNum);

        IList<KeyVisitorEntity> GetAllKeyVisitors();
    }
}
