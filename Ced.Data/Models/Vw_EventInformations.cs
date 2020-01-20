namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vw_EventInformations
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AxEventId { get; set; }

        [StringLength(255)]
        public string EventName { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(60)]
        public string MasterName { get; set; }

        [StringLength(60)]
        public string MasterCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(60)]
        public string EventType { get; set; }

        [StringLength(60)]
        public string EventTypeCode { get; set; }

        public string EventSummary { get; set; }

        public string EventDetails { get; set; }

        [StringLength(500)]
        public string ExhibitorProfile { get; set; }

        [StringLength(500)]
        public string VisitorProfile { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EventDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EventEndDate { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(1)]
        public string EventDisplayDate { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool EventAllDay { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(1)]
        public string InternationalDial { get; set; }

        [StringLength(50)]
        public string Telephone { get; set; }

        [StringLength(80)]
        public string EmailAddress { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [StringLength(500)]
        public string BookTicketLink { get; set; }

        [StringLength(255)]
        public string MarketoPreferenceCenterLink { get; set; }

        [StringLength(320)]
        public string EventImage { get; set; }

        [StringLength(324)]
        public string EventBackGroundImage { get; set; }

        [StringLength(324)]
        public string ProductImageFileName { get; set; }

        [StringLength(322)]
        public string MasterLogoFileName { get; set; }

        [StringLength(316)]
        public string IconFileName { get; set; }

        public bool? GiMA { get; set; }

        public bool? ASIA { get; set; }

        public bool? ITEI { get; set; }

        public bool? MODA { get; set; }

        public bool? Turkey { get; set; }

        public bool? TradeLink { get; set; }

        public bool? Poland { get; set; }

        public bool? Ukraine { get; set; }

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

        [Key]
        [Column(Order = 6)]
        [StringLength(5)]
        public string DocumentCulture { get; set; }

        public byte? TradeShowConnectDisplay { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Orginiser { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(500)]
        public string OrginiserName { get; set; }

        [StringLength(3)]
        public string CountryISO { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CountryID { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(500)]
        public string Country { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RegionID { get; set; }

        [Key]
        [Column(Order = 12)]
        [StringLength(500)]
        public string RegionName { get; set; }

        [Key]
        [Column(Order = 13)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VenueLocationID { get; set; }

        [Key]
        [Column(Order = 14)]
        [StringLength(500)]
        public string VenueName { get; set; }

        [Key]
        [Column(Order = 15)]
        [StringLength(500)]
        public string VenueLocation { get; set; }

        [StringLength(255)]
        public string MapVenueFullAddress { get; set; }

        [StringLength(60)]
        public string CEDVenueName { get; set; }

        [StringLength(200)]
        public string VenueCoordinates { get; set; }

        [Key]
        [Column(Order = 16)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IndustrySectorID { get; set; }

        [Key]
        [Column(Order = 17)]
        [StringLength(500)]
        public string IndustryName { get; set; }

        [StringLength(100)]
        public string SubIndustryName { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        [StringLength(60)]
        public string EventActivity { get; set; }

        public DateTime? UpdateTime { get; set; }

        [Key]
        [Column(Order = 18)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EditionId { get; set; }

        [Key]
        [Column(Order = 19)]
        public byte Status { get; set; }

        [Key]
        [Column(Order = 20)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DisplayOnMarketo { get; set; }
    }
}
