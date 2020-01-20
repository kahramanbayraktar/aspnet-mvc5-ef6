using System.Collections.Generic;
using System.Linq;

namespace Ced.DWStaging.BusinessServices
{
    public class EditionServices : IEditionServices
    {
        private readonly CedDWStagingEntities _db = new CedDWStagingEntities();

        public IQueryable<Edition> GetEditionsQueryable()
        {
            var editions = _db.Editions.AsQueryable();
            return editions;
        }

        public IList<Edition> GetEditions()
        {
            var editions = _db.Editions.ToList();
            return editions;
        }

        public IList<Edition> GetEditionsByEventBeId(string eventBeId)
        {
            var editions = _db.Editions.Where(x => x.EventBEID == eventBeId).ToList();
            return editions;
        }

        public IList<Edition> GetEditionsByMasterCode(string[] masterCodes)
        {
            var editions = _db.Editions.Where(x => masterCodes.Contains(x.EventMasterCode)).ToList();
            return editions;
        }
    }
}
