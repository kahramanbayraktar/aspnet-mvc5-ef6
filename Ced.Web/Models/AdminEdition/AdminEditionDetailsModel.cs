using System;
using System.ComponentModel.DataAnnotations;
using Ced.BusinessEntities;

namespace Ced.Web.Models.AdminEdition
{
    public class AdminEditionDetailsModel
    {
        public int EditionId { get; set; }

        public int EventId { get; set; }

        public int DwEventId { get; set; }

        public int AxEventId { get; set; }

        public string EditionName { get; set; }

        public string ReportingName { get; set; }

        public string LocalName { get; set; }

        public string InternationalName { get; set; }

        public int EditionNo { get; set; }

        public string EventName { get; set; }

        public string DirectorEmail { get; set; }

        public string VenueCoordinates { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? EndDate { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? VisitStartTime { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? VisitEndTime { get; set; }

        public int? FinancialYearStart { get; set; }

        public int? FinancialYearEnd { get; set; }

        public int? StartWeekOfYearDiff { get; set; }

        public int? StartDayOfYearDiff { get; set; }

        public string Frequency { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string ManagingOfficeName { get; set; }

        public string ManagingOfficePhone { get; set; }

        public string ManagingOfficeEmail { get; set; }

        public string ManagingOfficeWebsite { get; set; }

        public string EventWebSite { get; set; }

        public string DirectorManagingOfficeName { get; set; }

        public string Classification { get; set; }

        public bool AllDayEvent { get; set; }

        public bool CohostedEvent { get; set; }

        public int? CohostedEventCount { get; set; }

        public byte? TradeShowConnectDisplay { get; set; }

        public string EventActivity { get; set; }

        public EventType EventType { get; set; }

        public bool DisplayOnIteAsia { get; set; }

        public bool DisplayOnIteI { get; set; }

        public bool DisplayOnIteGermany { get; set; }

        public bool DisplayOnIteTurkey { get; set; }

        public bool DisplayOnTradeLink { get; set; }

        public bool DisplayOnItePoland { get; set; }

        public bool DisplayOnIteModa { get; set; }

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

        public string EventOwnershipBEID { get; set; }

        public string EventOwnership { get; set; }

        public string EventFlagPictureFileName { get; set; }

        public decimal? LocalSqmSold { get; set; }

        public decimal? InternationalSqmSold { get; set; }

        public int? LocalExhibitorCount { get; set; }

        public int? InternationalExhibitorCount { get; set; }

        public int? LocalDelegateCount { get; set; }

        public int? InternationalDelegateCount { get; set; }

        public int? LocalPaidDelegateCount { get; set; }

        public int? InternationalPaidDelegateCount { get; set; }

        public byte? SponsorCount { get; set; }

        public int? LocalVisitorCount { get; set; }

        public int? InternationalVisitorCount { get; set; }

        public int? LocalRepeatVisitCount { get; set; }

        public int? InternationalRepeatVisitCount { get; set; }

        public int? VisitorCountryCount { get; set; }

        public short? NationalGroupCount { get; set; }

        public short? ExhibitorCountryCount { get; set; }

        public int? OnlineRegistrationCount { get; set; }

        public int? OnlineRegisteredVisitorCount { get; set; }

        public decimal? LocalExhibitorRetentionRate { get; set; }

        public decimal? InternationalExhibitorRetentionRate { get; set; }

        public decimal? NPSScoreVisitor { get; set; }

        public decimal? NPSScoreExhibitor { get; set; }

        public decimal? NPSSatisfactionVisitor { get; set; }

        public decimal? NPSSatisfactionExhibitor { get; set; }

        public decimal? NPSAverageVisitor { get; set; }

        public decimal? NPSAverageExhibitor { get; set; }

        public int? PreviousInstanceDwEventId { get; set; }

        public string TopExhibitorCountries { get; set; }

        public string TopVisitorCountries { get; set; }

        public string DelegateCountries { get; set; }

        public DateTime CreateTime { get; set; }

        public int? CreateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? UpdateTimeByAutoIntegration { get; set; }

        public int UpdateUser { get; set; }

        public EditionStatusType Status { get; set; }

        public DateTime? StatusUpdateTime { get; set; }
    }
}