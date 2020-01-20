using System;

namespace Ced.BusinessEntities
{
    public class EventDirectorEntity
    {
        public int EventDirectorId { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public string DirectorEmail { get; set; }
        
        public string DirectorFullName { get; set; }

        public string ADLogonName { get; set; }

        public bool? IsPrimary { get; set; }

        public bool? IsAssistant { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public int CreatedBy { get; set; }
        
        public DateTime? UpdatedOn { get; set; }
        
        public int? UpdatedBy { get; set; }
    }
}
