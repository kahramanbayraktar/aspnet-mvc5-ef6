using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionDetailsModel
    {
        public int EditionId { get; set; }

        public int EventId { get; set; }

        [Display(Name = "EditionName", ResourceType = typeof(Resources.Resources))]
        public string EditionName { get; set; }

        //[Display(Name = "AxEventId", ResourceType = typeof(Resources.Resources))]
        public int AxEventId { get; set; }

        [Display(Name = "EventName", ResourceType = typeof(Resources.Resources))]
        public string EventName { get; set; }

        [Display(Name = "Director", ResourceType = typeof(Resources.Resources))]
        public string Director { get; set; }

        [Display(Name = "Classification", ResourceType = typeof(Resources.Resources))]
        public string Classification { get; set; }

        [Display(Name = "EventType", ResourceType = typeof(Resources.Resources))]
        public EventType EventType { get; set; }

        [Display(Name = "Frequency", ResourceType = typeof(Resources.Resources))]
        public string Frequency { get; set; }
        
        public string LanguageCode { get; set; }

        public string EventActivity { get; set; }

        public string Status { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime UpdateTimeByAutoIntegration { get; set; }

        #region GENERAL INFO

        // SUMMARY / DESCRIPTION

        public string ReportingName { get; set; }

        public string InternationalName { get; set; }

        public string LocalName { get; set; }

        public string EditionSummary { get; set; }

        public string EditionDescription { get; set; }

        public string ExhibitorProfile { get; set; }
        
        public string VisitorProfile { get; set; }

        public string ManagingOfficeEmail { get; set; }

        public string ManagingOfficePhone { get; set; }
        
        public string EventWebsite { get; set; }

        public string MarketoPreferenceCenterLink { get; set; }

        [Display(Name = "EditionNo", ResourceType = typeof(Resources.Resources))]
        public int EditionNo { get; set; }

        //[Display(Name = "WebLogoFileName", ResourceType = typeof(Resources.Resources))]
        public string WebLogoFileName { get; set; }

        //[Display(Name = "PeopleImageFileName", ResourceType = typeof(Resources.Resources))]
        public string PeopleImageFileName { get; set; }

        [Display(Name = "EventFlagPictureFileName", ResourceType = typeof(Resources.Resources))]
        public string EventFlagPictureFileName { get; set; }

        [Display(Name = "NumberOfLocalExhibitors", ResourceType = typeof(Resources.Resources))]
        public int? LocalExhibitorCount { get; set; }

        [Display(Name = "NumberOfInternationalExhibitors", ResourceType = typeof(Resources.Resources))]
        public int? InternationalExhibitorCount { get; set; }

        //[Display(Name = "NumberOfExhibitors", ResourceType = typeof(Resources.Resources))]
        public int? ExhibitorCount { get; set; }

        // DATE
        [Display(Name = "StartDate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "VisitStartTime", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? VisitStartTime { get; set; }

        [Display(Name = "VisitEndTime", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? VisitEndTime { get; set; }

        public string DisplayDate { get; set; }

        [Display(Name = "AllDayEvent", ResourceType = typeof(Resources.Resources))]
        public bool AllDayEvent { get; set; }

        [Display(Name = "CohostedEvent", ResourceType = typeof(Resources.Resources))]
        public bool CohostedEvent { get; set; }

        [Display(Name = "CohostedEvent", ResourceType = typeof(Resources.Resources))]
        public int? CohostedEventCount { get; set; }

        public bool Promoted { get; set; }

        // VENUE
        public string Country { get; set; }

        public string City { get; set; }

        public string VenueName { get; set; }

        [Display(Name = "VenueCoordinates", ResourceType = typeof(Resources.Resources))]
        public string VenueCoordinates { get; set; }

        // DISPLAY SETTINGS
        public bool DisplayOnIteGermany { get; set; }

        public bool DisplayOnIteAsia { get; set; }

        public bool DisplayOnIteI { get; set; }

        public bool DisplayOnItePoland { get; set; }

        public bool DisplayOnIteModa { get; set; }

        public bool DisplayOnIteTurkey { get; set; }

        public bool DisplayOnTradeLink { get; set; }

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

        #endregion

        #region SALES METRICS

        // SQM SALES
        [Display(Name = "LocalSqmSold", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? LocalSqmSold { get; set; }

        [Display(Name = "InternationalSqmSold", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? InternationalSqmSold { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? SqmSold { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? TotalSqmSold { get; set; }

        // SPONSORSHIP
        [Display(Name = "NumberOfSponsors", ResourceType = typeof(Resources.Resources))]
        public byte? SponsorCount { get; set; }
        
        #endregion

        #region EXHIBITOR / VISITOR

        // EXHIBITOR
        [Display(Name = "NumberOfExhibitingCountries", ResourceType = typeof(Resources.Resources))]
        public short? ExhibitorCountryCount { get; set; }

        public string TopExhibitorCountries { get; set; }

        // VISITOR
        [Display(Name = "LocalVisitorCount", ResourceType = typeof(Resources.Resources))]
        public int? LocalVisitorCount { get; set; }

        [Display(Name = "InternationalVisitorCount", ResourceType = typeof(Resources.Resources))]
        public int? InternationalVisitorCount { get; set; }

        //[Display(Name = "LocalRepeatVisitCount", ResourceType = typeof(Resources.Resources))]
        public int? LocalRepeatVisitCount { get; set; }

        //[Display(Name = "InternationalRepeatVisitCount", ResourceType = typeof(Resources.Resources))]
        public int? InternationalRepeatVisitCount { get; set; }

        public int? RepeatVisitCount { get; set; }

        [Display(Name = "NumberOfVisitorCountries", ResourceType = typeof(Resources.Resources))]
        public int? VisitorCountryCount { get; set; }

        [Display(Name = "NumberOfNationalGroups", ResourceType = typeof(Resources.Resources))]
        public short? NationalGroupCount { get; set; }

        public string TopVisitorCountries { get; set; }

        public IList<EditionVisitorEntity> EditionVisitors { get; set; }

        public int[] DailyVisitorCounts { get; set; }

        public int[] DailyRepeatVisits { get; set; }

        public int[] DailyOldVisitorCounts { get; set; }

        public int[] DailyNewVisitorCounts { get; set; }

        // E-TICKET
        public int? OnlineRegistrationCount { get; set; }

        public int? OnlineRegisteredVisitorCount { get; set; }

        public int? OnlineRegisteredBuyerVisitorCount { get; set; }

        // WEB LINKS
        public string BookStandUrl { get; set; }

        public string OnlineInvitationUrl { get; set; }

        // DELEGATE
        [Display(Name = "NumberOfLocalDelegates", ResourceType = typeof(Resources.Resources))]
        public int? LocalDelegateCount { get; set; }

        [Display(Name = "NumberOfInternationalDelegates", ResourceType = typeof(Resources.Resources))]
        public int? InternationalDelegateCount { get; set; }

        [Display(Name = "NumberOfLocalPaidDelegates", ResourceType = typeof(Resources.Resources))]
        public int? LocalPaidDelegateCount { get; set; }

        [Display(Name = "NumberOfInternationalPaidDelegates", ResourceType = typeof(Resources.Resources))]
        public int? InternationalPaidDelegateCount { get; set; }

        public string DelegateCountries { get; set; }

        #endregion

        #region POSTSHOW METRICS

        // RETENTION
        [Display(Name = "LocalExhibitorRetentionRate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? LocalExhibitorRetentionRate { get; set; }

        [Display(Name = "InternationalExhibitorRetentionRate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? InternationalExhibitorRetentionRate { get; set; }

        //[Display(Name = "ExhibitorRetentionRate", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? ExhibitorRetentionRate { get; set; }

        // NPS
        [Display(Name = "NPSScoreVisitor", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? NPSScoreVisitor { get; set; }

        [Display(Name = "NPSScoreExhibitor", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? NPSScoreExhibitor { get; set; }

        // NPS SATISFACTION
        [Display(Name = "NPSSatisfactionVisitor", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? NPSSatisfactionVisitor { get; set; }

        [Display(Name = "NPSSatisfactionExhibitor", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? NPSSatisfactionExhibitor { get; set; }

        // NPS AVERAGE
        //[Display(Name = "NPSAverageVisitor", ResourceType = typeof(Resources.Resources))]
        //[DisplayFormat(DataFormatString = "{0:N0}")]
        //public decimal? NPSAverageVisitor { get; set; }

        //[Display(Name = "NPSAverageExhibitor", ResourceType = typeof(Resources.Resources))]
        //[DisplayFormat(DataFormatString = "{0:N0}")]
        //public decimal? NPSAverageExhibitor { get; set; }

        #endregion

        public IList<EditionTranslationSocialMediaEntity> SocialMedias { get; set; }
    }
}