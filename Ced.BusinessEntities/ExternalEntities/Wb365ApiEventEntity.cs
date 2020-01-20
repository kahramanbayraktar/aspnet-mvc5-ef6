namespace Ced.BusinessEntities.ExternalEntities
{
    public class Wb365ApiEventEntity
    {
        public string EditionTranslationId { get; set; } // EditionTranslation.EditionTranslationId
        public string EditionId { get; set; } // Edition.EditionId
        public string EventId { get; set; } // Edition.EventId
        public string Name { get; set; } // Edition.EditionName
        public string ShortDescription { get; set; } // EditionTranslation.Summary
        public string StartDate { get; set; } // Edition.StartDate
        public string EndDate { get; set; } // Edition.EndDate
        public string Country { get; set; } // Edition.Country
        public string CountryCode { get; set; } // Edition.CountryCode
        public string City { get; set; } // Edition.City
        public string Venue { get; set; } // EditionTranslation.VenueName
        public string WebsiteLink { get; set; } // Edition.EventWebSite
        public string HeroImage { get; set; } // EditionTranslation.PeopleImage
        public byte TcDisplay { get; set; } // Edition.TradeShowConnectDisplay
        public string LangCode { get; set; } // EditionTranslation.LanguageCode
        public byte LangId { get; set; } // EditionTranslation
    }
}
