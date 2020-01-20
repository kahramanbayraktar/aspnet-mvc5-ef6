using ITE.Utility.ObjectComparison;
using System;
using System.Collections.Generic;

namespace Ced.BusinessEntities
{
    public class EditionEntity : ICloneable
    {
        public int EditionId { get; set; }

        public int EventId { get; set; }

        public int DwEventId { get; set; }
        
        public int AxEventId { get; set; }

        [StagingDbComparable]
        public string EditionName { get; set; }

        public string ReportingName { get; set; }

        [Comparable]
        public string LocalName { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string InternationalName { get; set; }

        [StagingDbComparable]
        [Comparable]
        public int EditionNo { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string CountryLocalName { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string CityLocalName { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string VenueCoordinates { get; set; }

        [StagingDbComparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public DateTime? StartDate { get; set; }

        [StagingDbComparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public DateTime? EndDate { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public TimeSpan? VisitStartTime { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public TimeSpan? VisitEndTime { get; set; }

        [StagingDbComparable]
        public int? FinancialYearStart { get; set; }

        [StagingDbComparable]
        public int? FinancialYearEnd { get; set; }

        public int? StartWeekOfYearDiff { get; set; }

        public int? StartDayOfYearDiff { get; set; }

        [Comparable]
        public DateTime? CoolOffPeriodStartDate { get; set; }

        [Comparable]
        public DateTime? CoolOffPeriodEndDate { get; set; }

        [Comparable]
        public string InternationalDate { get; set; }

        [Comparable]
        public string LocalDate { get; set; }

        [StagingDbComparable]
        public string Frequency { get; set; }

        [StagingDbComparable]
        public string Country { get; set; }

        [StagingDbComparable]
        public string CountryCode { get; set; }

        [StagingDbComparable]
        public string City { get; set; }

        [StagingDbComparable]
        public string ManagingOfficeName { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string ManagingOfficePhone { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string ManagingOfficeEmail { get; set; }

        public string ManagingOfficeWebsite { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.GeneralInfo)]
        public string EventWebSite { get; set; }

        public string MarketoPreferenceCenterLink { get; set; }

        [StagingDbComparable]
        public string DirectorManagingOfficeName { get; set; }

        //[StagingDbComparable]
        //public string DirectorFullName { get; set; }

        //[StagingDbComparable]
        public string DirectorEmail { get; set; }

        // PATCH#001
        [StagingDbComparable]
        public string EventTypeCode { get; set; }

        [StagingDbComparable]
        public string Classification { get; set; }

        [Comparable]
        public bool AllDayEvent { get; set; }

        [Comparable]
        public bool CohostedEvent { get; set; }
        
        public int? CohostedEventCount { get; set; }

        [Comparable]
        public bool Promoted { get; set; }

        [Comparable]
        public byte? TradeShowConnectDisplay { get; set; }

        [StagingDbComparable]
        public string EventActivity { get; set; }

        [Comparable]
        public bool DisplayOnIteAsia { get; set; }
        
        [Comparable]
        public bool DisplayOnIteI { get; set; }

        [Comparable]
        public bool DisplayOnIteGermany { get; set; }

        [Comparable]
        public bool DisplayOnIteTurkey { get; set; }

        [Comparable]
        public bool DisplayOnIteRussia { get; set; }

        [Comparable]
        public bool DisplayOnIteEurasia { get; set; }

        [Comparable]
        public bool DisplayOnTradeLink { get; set; }

        [Comparable]
        public bool DisplayOnItePoland { get; set; }

        [Comparable]
        public bool DisplayOnIteModa { get; set; }

        [Comparable]
        public bool DisplayOnIteUkraine { get; set; }

        [Comparable]
        public bool DisplayOnIteBuildInteriors { get; set; }

        [Comparable]
        public bool DisplayOnIteFoodDrink { get; set; }

        [Comparable]
        public bool DisplayOnIteOilGas { get; set; }

        [Comparable]
        public bool DisplayOnIteTravelTourism { get; set; }

        [Comparable]
        public bool DisplayOnIteTransportLogistics { get; set; }

        [Comparable]
        public bool DisplayOnIteFashion { get; set; }

        [Comparable]
        public bool DisplayOnIteSecurity { get; set; }

        [Comparable]
        public bool DisplayOnIteBeauty { get; set; }

        [Comparable]
        public bool DisplayOnIteHealthCare { get; set; }

        [Comparable]
        public bool DisplayOnIteMining { get; set; }

        [Comparable]
        public bool DisplayOnIteEngineeringIndustrial { get; set; }

        [Comparable]
        public bool HiddenFromWebSites { get; set; }

        [StagingDbComparable]
        public string EventOwnershipBEID { get; set; }

        [StagingDbComparable]
        public string EventOwnership { get; set; }

        public string EventFlagPictureFileName { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public decimal? LocalSqmSold { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public decimal? InternationalSqmSold { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public decimal? SqmSold { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? LocalExhibitorCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? InternationalExhibitorCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? ExhibitorCount { get; set; }

        [Comparable]
        public int? LocalDelegateCount { get; set; }

        [Comparable]
        public int? InternationalDelegateCount { get; set; }

        [Comparable]
        public int? LocalPaidDelegateCount { get; set; }

        [Comparable]
        public int? InternationalPaidDelegateCount { get; set; }

        [Comparable]
        public byte? SponsorCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? LocalVisitorCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? InternationalVisitorCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? LocalRepeatVisitCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? InternationalRepeatVisitCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? RepeatVisitCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? VisitorCountryCount { get; set; }

        [Comparable]
        public short? NationalGroupCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public short? ExhibitorCountryCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? OnlineRegistrationCount { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.PostShowMetricsInfo)]
        public int? OnlineRegisteredVisitorCount { get; set; }

        [Comparable]
        public int? OnlineRegisteredBuyerVisitorCount { get; set; }

        [Comparable]
        public decimal? LocalExhibitorRetentionRate { get; set; }

        [Comparable]
        public decimal? InternationalExhibitorRetentionRate { get; set; }

        [Comparable]
        public decimal? ExhibitorRetentionRate { get; set; }

        [Comparable]
        //[EditionField(EditionInfoType.SurveyResultsInfo)]
        public decimal? NPSScoreVisitor { get; set; }

        [Comparable]
        //[EditionField(EditionInfoType.SurveyResultsInfo)]
        public decimal? NPSScoreExhibitor { get; set; }

        [Comparable]
        //[EditionField(EditionInfoType.SurveyResultsInfo)]
        public decimal? NPSSatisfactionVisitor { get; set; }

        [Comparable]
        //[EditionField(EditionInfoType.SurveyResultsInfo)]
        public decimal? NPSSatisfactionExhibitor { get; set; }

        [Comparable]
        public decimal? NPSAverageVisitor { get; set; }

        [Comparable]
        public decimal? NPSAverageExhibitor { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.SurveyResultsInfo)]
        public decimal? NetEasyScoreVisitor { get; set; }

        [Comparable]
        [EditionField(EditionInfoType.SurveyResultsInfo)]
        public decimal? NetEasyScoreExhibitor { get; set; }

        public int? PreviousInstanceDwEventId { get; set; }

        [Comparable]
        public string TopExhibitorCountries { get; set; }

        [Comparable]
        public string TopVisitorCountries { get; set; }

        [Comparable]
        public string DelegateCountries { get; set; }

        public DateTime CreateTime { get; set; }
        
        public int? CreateUser { get; set; }
        
        public DateTime? UpdateTime { get; set; }

        public DateTime? UpdateTimeByAutoIntegration { get; set; }
        
        public int UpdateUser { get; set; }

        public EditionStatusType Status { get; set; }

        public DateTime? StatusUpdateTime { get; set; }

        public IEnumerable<string> DirectorEmails { get; set; }


        public IList<EditionTranslationEntity> EditionTranslations { get; set; }

        public EventEntity Event { get; set; }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}