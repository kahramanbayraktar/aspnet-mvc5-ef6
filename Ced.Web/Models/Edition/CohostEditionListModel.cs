using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class CohostEditionListModel
    {
        public int EditionId { get; set; }

        public IEnumerable<EditionCohostEntity> EditionCohosts { get; set; }
    }
}