using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionKeyVisitorListModel
    {
        public int EditionId { get; set; }

        public IEnumerable<EditionKeyVisitorEntity> EditionKeyVisitors { get; set; }
    }
}