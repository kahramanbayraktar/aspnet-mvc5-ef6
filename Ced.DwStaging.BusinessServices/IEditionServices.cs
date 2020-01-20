using System.Collections.Generic;
using System.Linq;

namespace Ced.DWStaging.BusinessServices
{
    public interface IEditionServices
    {
        IQueryable<Edition> GetEditionsQueryable();

        IList<Edition> GetEditions();

        IList<Edition> GetEditionsByEventBeId(string eventBeId);

        IList<Edition> GetEditionsByMasterCode(string[] masterCodes);
    }
}
