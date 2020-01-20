using System;
using System.Collections.Generic;

namespace Ced.BusinessEntities
{
    public class EditionEntityLight
    {
        public int EditionId { get; set; }

        public string EditionName { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public int EditionNo { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string DirectorEmail { get; set; }

        public string DirectorFullName { get; set; }

        public string EventTypeCode { get; set; }

        public string EventActivity { get; set; }

        public byte Status { get; set; }

        public IEnumerable<string> DirectorEmails { get; set; }
    }
}
