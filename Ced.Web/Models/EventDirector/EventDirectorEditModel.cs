using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.EventDirector
{
    public class EventDirectorEditModel
    {
        public int EventDirectorId { get; set; }

        public IList<BusinessEntities.Auth.ApplicationEntity> Applications { get; set; }

        public int? EventId { get; set; }

        public string EventName { get; set; }

        public string UserEmail { get; set; }

        public int AppId { get; set; }

        public string AppName { get; set; }

        public int[] ApplicationId { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsAssistant { get; set; }
    }
}