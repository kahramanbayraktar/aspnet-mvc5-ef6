using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.AdminEdition
{
    public class AdminEditionListItemModel
    {
        public int EditionId { get; set; }

        public string EditionName { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public int EditionNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? EndDate { get; set; }

        public string DirectorFullName { get; set; }

        public string DirectorEmail { get; set; }

        public string Status { get; set; }

        public string EventActivity { get; set; }
    }
}