using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionEditGeneralInfoModel : EditionBaseModel
    {
        [Display(Name = "EditionName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string EditionName { get; set; }

        //[Display(Name = "ReportingName", ResourceType = typeof(Resources.Resources))]
        //[Required]
        public string ReportingName { get; set; }

        //[Display(Name = "LocalName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string LocalName { get; set; }

        //[Display(Name = "InternationalName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string InternationalName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string CountryLocalName { get; set; }

        public string CityLocalName { get; set; }

        public string InternationalDate { get; set; }

        public string LocalDate { get; set; }

        [Required]
        [Display(Name = "EditionVenue", ResourceType = typeof(Resources.Resources))]
        public string VenueName { get; set; }

        public string MapVenueFullAddress { get; set; }

        [AllowHtml]
        [Display(Name = "Summary", ResourceType = typeof(Resources.Resources))]
        [MaxLength(215)]
        public string Summary { get; set; }

        [AllowHtml]
        [Display(Name = "Description", ResourceType = typeof(Resources.Resources))]
        [MaxLength(5000)]
        public string Description { get; set; }

        //[Display(Name = "ExhibitorProfile", ResourceType = typeof(Resources.Resources))]
        [MaxLength(500)]
        public string ExhibitorProfile { get; set; }

        //[Display(Name = "VisitorProfile", ResourceType = typeof(Resources.Resources))]
        [MaxLength(500)]
        public string VisitorProfile { get; set; }

        [Range(1, 100)]
        [Display(Name = "EditionNo", ResourceType = typeof(Resources.Resources))]
        [Required]
        public int EditionNo { get; set; }

        [Required]
        public string ManagingOfficePhone { get; set; }

        [EmailAddress]
        [Required]
        public string ManagingOfficeEmail { get; set; }

        [Required]
        public string EventWebSite { get; set; }

        public string MarketoPreferenceCenterLink { get; set; }

        [Display(Name = "VenueCoordinates", ResourceType = typeof(Resources.Resources))]
        public string VenueCoordinates { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Resources.Resources))]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Resources.Resources))]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "VisitStartTime", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? VisitStartTime { get; set; }

        [Display(Name = "VisitEndTime", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? VisitEndTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CoolOffPeriodStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CoolOffPeriodEndDate { get; set; }

        [Display(Name = "AllDayEvent", ResourceType = typeof(Resources.Resources))]
        public bool AllDayEvent { get; set; }

        public bool Promoted { get; set; }

        //[Display(Name = "TradeShowConnectDisplay", ResourceType = typeof(Resources.Resources))]
        public byte? TradeShowConnectDisplay { get; set; }

        [Display(Name = "DisplayOnITEAsia", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnIteAsia { get; set; }

        [Display(Name = "DisplayOnITEI", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnIteI { get; set; }

        //[Display(Name = "DisplayOnIteGermany", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnIteGermany { get; set; }

        [Display(Name = "DisplayOnITETurkey", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnIteTurkey { get; set; }

        public bool DisplayOnIteRussia { get; set; }

        public bool DisplayOnIteEurasia { get; set; }

        [Display(Name = "DisplayOnTradeLink", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnTradeLink { get; set; }

        //[Display(Name = "DisplayOnITEPoland", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnItePoland { get; set; }

        [Display(Name = "DisplayOnITEModa", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnIteModa { get; set; }
        
        //[Display(Name = "DisplayOnIteUkraine", ResourceType = typeof(Resources.Resources))]
        public bool DisplayOnIteUkraine { get; set; }

        public bool DisplayOnIteBuildInteriors { get; set; }

        public bool DisplayOnIteFoodDrink { get; set; }

        public bool DisplayOnIteOilGas { get; set; }

        public bool DisplayOnIteTravelTourism { get; set; }

        public bool DisplayOnIteTransportLogistics { get; set; }

        public bool DisplayOnIteFashion { get; set; }

        public bool DisplayOnIteSecurity { get; set; }

        public bool DisplayOnIteBeauty { get; set; }

        public bool DisplayOnIteHealthCare { get; set; }

        public bool DisplayOnIteMining { get; set; }

        public bool DisplayOnIteEngineeringIndustrial { get; set; }

        public bool HiddenFromWebSites { get; set; }

        //[Display(Name = "WebLogoFileName", ResourceType = typeof(Resources.Resources))]
        public string WebLogoFileName { get; set; }

        public HttpPostedFileBase WebLogoFile { get; set; }

        //[Display(Name = "PeopleImageFileName", ResourceType = typeof(Resources.Resources))]
        public string PeopleImageFileName { get; set; }

        public HttpPostedFileBase PeopleImageFile { get; set; }

        [Display(Name = "EventFlagPictureFileName", ResourceType = typeof(Resources.Resources))]
        public string EventFlagPictureFileName { get; set; }

        [Display(Name = "BookStandUrl", ResourceType = typeof(Resources.Resources))]
        public string BookStandUrl { get; set; }

        [Display(Name = "OnlineInvitationUrl", ResourceType = typeof(Resources.Resources))]
        public string OnlineInvitationUrl { get; set; }

        public bool DisplayOnTypeFieldsMappable { get; set; }


        public IList<EditionTranslationSocialMediaEntity> SocialMedias { get; set; }
    }
}