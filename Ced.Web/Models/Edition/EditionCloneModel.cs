using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ced.BusinessEntities;
using Ced.Web.Models.ValidationAttributes;

namespace Ced.Web.Models.Edition
{
    public class EditionCloneModel : BaseModel
    {
        public int EditionId { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; }

        public int EditionTranslationId { get; set; }

        public string LanguageCode { get; set; }
        
        [Display(Name = "EditionName", ResourceType = typeof(Resources.Resources))]
        [Required]
        [CleanedEditionName]
        public string EditionName { get; set; }

        [Required]
        [CleanedLocalName]
        public string LocalName { get; set; }

        [Required]
        [CleanedInternationalName]
        public string InternationalName { get; set; }

        [Required]
        [Display(Name = "EditionVenue", ResourceType = typeof(Resources.Resources))]
        public string VenueName { get; set; }

        public string MapVenueFullAddress { get; set; }

        [Display(Name = "EditionNo", ResourceType = typeof(Resources.Resources))]
        [Required]
        [Range(1, 100)]
        public int EditionNo { get; set; }

        public string ManagingOfficePhone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string ManagingOfficeEmail { get; set; }

        public string EventWebSite { get; set; }

        [Display(Name = "VenueCoordinates", ResourceType = typeof(Resources.Resources))]
        public string VenueCoordinates { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime? StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime? EndDate { get; set; }

        [Display(Name = "VisitStartTime", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? VisitStartTime { get; set; }

        [Display(Name = "VisitEndTime", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? VisitEndTime { get; set; }

        public string Industry { get; set; }

        public string SubIndustry { get; set; }

        public string Frequency { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string EventType { get; set; }

        public string DirectorFullName { get; set; }

        public string EventOwnership { get; set; }

        public bool CohostedEvent { get; set; }

        public int? CohostedEventCount { get; set; }

        public IEnumerable<EditionCohostEntity> Cohosts { get; set; }

        public EditionStatusType Status { get; set; }

        public bool IsUpdatable { get; set; }

        public bool IsSendableForApproval { get; set; }
    }
}