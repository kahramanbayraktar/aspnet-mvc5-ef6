namespace Ced.Web.Models.Edition
{
    public class EditionApprovalModel
    {
        public int EditionId { get; set; }

        public string EditionName { get; set; }

        public string MasterName { get; set; }

        public string MasterCode { get; set; }

        public string PreviousEditionName { get; set; }

        public int PreviousEditionAxEventId { get; set; }

        public string LocalName { get; set; }

        public string InternationalName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string VisitStartTime { get; set; }

        public string VisitEndTime { get; set; }

        public string EventType { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string VenueName { get; set; }

        public string MapVenueFullAddress { get; set; }

        public string VenueCoordinates { get; set; }

        public string DirectorFullName { get; set; }

        public string Frequency { get; set; }

        public string EventOwnership { get; set; }

        public string Industry { get; set; }
        
        public int EditionNo { get; set; }

        public string ManagingOfficePhone { get; set; }

        public string ManagingOfficeEmail { get; set; }

        public string EventWebSite { get; set; }
    }
}
