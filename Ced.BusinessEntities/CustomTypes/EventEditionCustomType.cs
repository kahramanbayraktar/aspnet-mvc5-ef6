using System;

namespace Ced.BusinessEntities
{
    public class EventEditionCustomType
    {
        public int EventId { get; set; }

        public string MasterName { get; set; }

        public int EditionId { get; set; }
        
        public string EditionName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        
        public string DisplayDate { get; set; }
        
        public DateTime? LastVisitDate { get; set; }

        public string Logo { get; set; }
    }
}
