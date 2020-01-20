using System;

namespace Ced.BusinessEntities
{
    public class EventInformationEntity
    {
        public int AxEventId { get; set; }
        public string EventName { get; set; }
        public string MasterName { get; set; }
        public string MasterCode { get; set; }
        public string EventType { get; set; }
        public string EventTypeCode { get; set; }
        public string EventSummary { get; set; }
        public string EventDetails { get; set; }
        public string ExhibitorProfile { get; set; }
        public string VisitorProfile { get; set; }
        public string City { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string EventDisplayDate { get; set; }
        public bool EventAllDay { get; set; }
        public string InternationalDial { get; set; }
        public string Telephone { get; set; }
        public string EmailAddress { get; set; }
        public string Website { get; set; }
        public string BookTicketLink { get; set; }
        public string EventImage { get; set; }
        public string EventBackGroundImage { get; set; }
        public string ProductImageFileName { get; set; }
        public string MasterLogoFileName { get; set; }
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
        public string DocumentCulture { get; set; }
        public byte? TradeShowConnectDisplay { get; set; }
        public int Orginiser { get; set; }
        public string OrginiserName { get; set; }
        public string CountryISO { get; set; }
        public int CountryID { get; set; }
        public string Country { get; set; }
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public int VenueLocationID { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public string MapVenueFullAddress { get; set; }
        public string CEDVenueName { get; set; }
        public string VenueCoordinates { get; set; }
        public int IndustrySectorID { get; set; }
        public string IndustryName { get; set; }
        public string SubIndustryName { get; set; }
        public string Brand { get; set; }
        public string EventActivity { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int EditionId { get; set; }
        public byte Status { get; set; }
    }
}
