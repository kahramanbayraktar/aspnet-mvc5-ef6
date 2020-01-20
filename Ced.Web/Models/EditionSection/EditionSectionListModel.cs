using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.Web.Models.EditionSection
{
    public class EditionSectionListModel
    {
        public int EditionId { get; set; }

        public IEnumerable<EditionSectionEntity> EditionSections { get; set; }
    }
}