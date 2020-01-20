using System;
using System.ComponentModel.DataAnnotations;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionListModel
    {
        public int EditionId { get; set; }

        [Display(Name = "EditionName", ResourceType = typeof(Resources.Resources))]
        public string EditionName { get; set; }

        public int EventId { get; set; }

        public string ReportingName { get; set; }

        [Display(Name = "EditionNo", ResourceType = typeof(Resources.Resources))]
        public int EditionNo { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Director", ResourceType = typeof(Resources.Resources))]
        public string DirectorFullName { get; set; }

        public string DirectorEmail { get; set; }

        public bool IsEditable { get; set; }

        public string IsEditableDesc { get; set; }

        public bool IsClonable { get; set; }

        public string IsClonableDesc { get; set; }

        public EditionStatusType Status { get; set; }

        public EventActivity EventActivity { get; set; }
    }
}