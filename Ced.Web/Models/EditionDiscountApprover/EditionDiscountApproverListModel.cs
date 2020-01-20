using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.Web.Models.EditionDiscountApprover
{
    public class EditionDiscountApproverListModel
    {
        public int EditionId { get; set; }

        public IEnumerable<EditionDiscountApproverEntity> EditionDiscountApprovers { get; set; }
    }
}