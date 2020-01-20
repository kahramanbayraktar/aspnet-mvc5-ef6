using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.Web.Models.EditionPaymentSchedule
{
    public class EditionPaymentScheduleListModel
    {
        public int EditionId { get; set; }

        public IEnumerable<EditionPaymentScheduleEntity> EditionPaymentSchedules { get; set; }
    }
}