namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Edition")]
    public partial class Edition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Edition()
        {
            EditionCohosts = new HashSet<EditionCohost>();
            EditionCohosts1 = new HashSet<EditionCohost>();
            EditionKeyVisitors = new HashSet<EditionKeyVisitor>();
            EditionTranslations = new HashSet<EditionTranslation>();
            EditionVisitors = new HashSet<EditionVisitor>();
            Notifications = new HashSet<Notification>();
        }

        public int EditionId { get; set; }

        public int EventId { get; set; }

        public int AxEventId { get; set; }

        public int DwEventID { get; set; }

        [StringLength(300)]
        public string EditionName { get; set; }

        [StringLength(255)]
        public string ReportingName { get; set; }

        [StringLength(255)]
        public string LocalName { get; set; }

        [StringLength(255)]
        public string InternationalName { get; set; }

        public int EditionNo { get; set; }

        [StringLength(60)]
        public string Frequency { get; set; }

        [StringLength(255)]
        public string Country { get; set; }

        [StringLength(3)]
        public string CountryCode { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string CountryLocalName { get; set; }

        [StringLength(100)]
        public string CityLocalName { get; set; }

        [StringLength(200)]
        public string VenueCoordinates { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public TimeSpan VisitStartTime { get; set; }

        public TimeSpan VisitEndTime { get; set; }

        public int? FinancialYearStart { get; set; }

        public int? FinancialYearEnd { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CoolOffPeriodStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CoolOffPeriodEndDate { get; set; }

        [StringLength(100)]
        public string InternationalDate { get; set; }

        [StringLength(100)]
        public string LocalDate { get; set; }

        public int? StartWeekOfYearDiff { get; set; }

        public int? StartDayOfYearDiff { get; set; }

        [StringLength(60)]
        public string DirectorFullName { get; set; }

        [StringLength(60)]
        public string DirectorEmail { get; set; }

        [StringLength(60)]
        public string EventTypeCode { get; set; }

        [StringLength(60)]
        public string Classification { get; set; }

        public bool AllDayEvent { get; set; }

        public bool CoHostedEvent { get; set; }

        public int? CoHostedEventCount { get; set; }

        public bool Promoted { get; set; }

        public byte? TradeShowConnectDisplay { get; set; }

        [StringLength(60)]
        public string EventActivity { get; set; }

        [StringLength(60)]
        public string ManagingOfficeName { get; set; }

        [StringLength(50)]
        public string ManagingOfficePhone { get; set; }

        [StringLength(80)]
        public string ManagingOfficeEmail { get; set; }

        [StringLength(100)]
        public string ManagingOfficeWebsite { get; set; }

        [StringLength(60)]
        public string DirectorManagingOfficeName { get; set; }

        [StringLength(100)]
        public string EventWebSite { get; set; }

        [StringLength(500)]
        public string EventFlagPictureFileName { get; set; }

        [StringLength(255)]
        public string MarketoPreferenceCenterLink { get; set; }

        public decimal? LocalSqmSold { get; set; }

        public decimal? InternationalSqmSold { get; set; }

        public decimal? SqmSold { get; set; }

        public int? LocalExhibitorCount { get; set; }

        public int? InternationalExhibitorCount { get; set; }

        public int? ExhibitorCount { get; set; }

        public int? LocalDelegateCount { get; set; }

        public int? InternationalDelegateCount { get; set; }

        public int? LocalPaidDelegateCount { get; set; }

        public int? InternationalPaidDelegateCount { get; set; }

        public byte? SponsorCount { get; set; }

        public int? LocalVisitorCount { get; set; }

        public int? InternationalVisitorCount { get; set; }

        public int? LocalRepeatVisitCount { get; set; }

        public int? InternationalRepeatVisitCount { get; set; }

        public int? RepeatVisitCount { get; set; }

        public int? VisitorCountryCount { get; set; }

        public short? NationalGroupCount { get; set; }

        public short? ExhibitorCountryCount { get; set; }

        public int? OnlineRegistrationCount { get; set; }

        public int? OnlineRegisteredVisitorCount { get; set; }

        public int? OnlineRegisteredBuyerVisitorCount { get; set; }

        public decimal? LocalExhibitorRetentionRate { get; set; }

        public decimal? InternationalExhibitorRetentionRate { get; set; }

        public decimal? ExhibitorRetentionRate { get; set; }

        public decimal? NPSScoreVisitor { get; set; }

        public decimal? NPSScoreExhibitor { get; set; }

        public decimal? NPSSatisfactionVisitor { get; set; }

        public decimal? NPSSatisfactionExhibitor { get; set; }

        public decimal? NetEasyScoreVisitor { get; set; }

        public decimal? NetEasyScoreExhibitor { get; set; }

        public int? PreviousInstanceDwEventId { get; set; }

        public bool? DisplayOnIteGermany { get; set; }

        public bool? DisplayOnIteAsia { get; set; }

        public bool? DisplayOnIteI { get; set; }

        public bool? DisplayOnItePoland { get; set; }

        public bool? DisplayOnIteModa { get; set; }

        public bool? DisplayOnIteTurkey { get; set; }

        public bool? DisplayOnIteRussia { get; set; }

        public bool? DisplayOnIteEurasia { get; set; }

        public bool? DisplayOnTradeLink { get; set; }

        public bool? DisplayOnIteUkraine { get; set; }

        public bool? DisplayOnIteBuildInteriors { get; set; }

        public bool? DisplayOnIteFoodDrink { get; set; }

        public bool? DisplayOnIteOilGas { get; set; }

        public bool? DisplayOnIteTravelTourism { get; set; }

        public bool? DisplayOnIteTransportLogistics { get; set; }

        public bool? DisplayOnIteFashion { get; set; }

        public bool? DisplayOnIteSecurity { get; set; }

        public bool? DisplayOnIteBeauty { get; set; }

        public bool? DisplayOnIteHealthCare { get; set; }

        public bool? DisplayOnIteMining { get; set; }

        public bool? DisplayOnIteEngineeringIndustrial { get; set; }

        public bool? HiddenFromWebSites { get; set; }

        [StringLength(10)]
        public string EventOwnershipBEID { get; set; }

        [StringLength(60)]
        public string EventOwnership { get; set; }

        public DateTime CreateTime { get; set; }

        public int? CreateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int UpdateUser { get; set; }

        public DateTime? UpdateTimeByAutoIntegration { get; set; }

        public int MatchedKenticoEventId { get; set; }

        public DateTime? MatchedOn { get; set; }

        public byte Status { get; set; }

        public DateTime? StatusUpdateTime { get; set; }

        public virtual Country Country1 { get; set; }

        public virtual Event Event { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EditionCohost> EditionCohosts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EditionCohost> EditionCohosts1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EditionKeyVisitor> EditionKeyVisitors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EditionTranslation> EditionTranslations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EditionVisitor> EditionVisitors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
