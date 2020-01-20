namespace Ced.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Clnd_KenticoEvents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KenticoEventID { get; set; }

        public int? EventBEID { get; set; }

        [StringLength(150)]
        public string EventName { get; set; }

        [StringLength(150)]
        public string City { get; set; }

        [StringLength(150)]
        public string Country { get; set; }

        public int? EventDateInt { get; set; }

        public int? EventEndDateInt { get; set; }

        public DateTime? EventDate { get; set; }

        public DateTime? EventEndDate { get; set; }

        [StringLength(4000)]
        public string EventSummary { get; set; }

        [StringLength(150)]
        public string EventDisplayDate { get; set; }

        public int? CountryID { get; set; }

        public int? IndustrySectorID { get; set; }

        public int? IndustrySectorID2 { get; set; }

        [StringLength(350)]
        public string EventImage { get; set; }

        [StringLength(350)]
        public string EventBackGroundImage { get; set; }

        [StringLength(150)]
        public string Telephone { get; set; }

        [StringLength(150)]
        public string Fax { get; set; }

        [StringLength(150)]
        public string Website { get; set; }

        [StringLength(150)]
        public string BookTicketLink { get; set; }

        [StringLength(150)]
        public string Organiser { get; set; }

        [StringLength(4000)]
        public string EventDetails { get; set; }

        [StringLength(150)]
        public string IndustrySector2 { get; set; }

        [StringLength(150)]
        public string InternationalDial { get; set; }

        [StringLength(150)]
        public string EmailAddress { get; set; }

        public int? EventAllDay { get; set; }

        [StringLength(150)]
        public string EventLocation { get; set; }

        public int? VenueLocation { get; set; }

        public byte? ITEI { get; set; }

        public byte? GiMA { get; set; }

        public byte? ASIA { get; set; }

        public byte? Turkey { get; set; }

        public byte? TradeLink { get; set; }

        public byte? MODA { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [StringLength(500)]
        public string Desc { get; set; }
    }
}
