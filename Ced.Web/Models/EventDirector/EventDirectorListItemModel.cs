using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.EventDirector
{
    public class EventDirectorListItemModel
    {
        public int EventDirectorId { get; set; }

        public string EventName { get; set; }

        public string DirectorEmail { get; set; }

        public string ADLogonName { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CreatedOn { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsAssistant { get; set; }
    }
}