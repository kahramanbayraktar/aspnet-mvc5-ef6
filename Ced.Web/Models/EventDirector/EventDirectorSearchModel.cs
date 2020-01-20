using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.EventDirector
{
    public class EventDirectorSearchModel
    {
        public int? EventId { get; set; }

        public string UserEmail { get; set; }

        public int[] ApplicationIds { get; set; }

        public bool? IsPrimary { get; set; }

        public bool? IsAssistant { get; set; }

        public IList<BusinessEntities.Auth.ApplicationEntity> Applications { get; set; }
    }
}