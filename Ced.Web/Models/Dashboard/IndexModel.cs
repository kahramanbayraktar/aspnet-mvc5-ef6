using System;
using System.Collections.Generic;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Event;

namespace Ced.Web.Models.Dashboard
{
    public class IndexModel
    {
        public int EventId { get; set; }
        
        public string EventName { get; set; }

        public EventType EventType { get; set; }

        public string EventDisplayDate { get; set; }

        public int SelectedEditionId { get; set; }
        
        public string SelectedEditionName { get; set; }

        public DateTime? NextEditionStartDate { get; set; }

        public List<EventEditionCustomType> EventEditionList { get; set; }

        public EditionStatModel EditionStatModel { get; set; }
    }
}