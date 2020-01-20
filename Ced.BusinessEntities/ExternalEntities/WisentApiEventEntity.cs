using System;

namespace Ced.BusinessEntities.ExternalEntities
{
    public class WisentApiEventEntity
    {
        public string EventBEID { get; set; } // Edition.EventBEID

        public string EventName { get; set; } // Edition.InternationalName

        public string StartDate { get; set; } // Edition.StartDate

        public string EndDate { get; set; } // Edition.EndDate

        public string Country { get; set; } // Edition.Country

        public string CountryCode { get; set; } // Edition.CountryCode

        public string City { get; set; } // Event.City

        public short FinancialYearStart { get; set; } // Edition.FinancialYearStart
    }
}
