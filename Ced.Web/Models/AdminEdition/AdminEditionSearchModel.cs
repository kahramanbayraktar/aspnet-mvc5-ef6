using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.Web.Models.AdminEdition
{
    public class AdminEditionSearchModel
    {
        public int? EventId { get; set; }

        public string DirectorEmail { get; set; }

        public string[] RegionNames { get; set; }

        public string CountryCode { get; set; }

        public string CityName { get; set; }

        public int[] EditionStatusTypeIds { get; set; }

        public int[] EventActivityIds { get; set; }

        public int[] EventTypeIds { get; set; }

        //public IList<RegionEntity> Regions { get; set; }
        public IList<BusinessEntities.Auth.RegionEntity> Regions { get; set; }

        public IList<EditionStatusType> EditionStatusTypes { get; set; }

        public IList<EventActivity> EventActivities { get; set; }

        public IList<EventType> EventTypes { get; set; }
    }
}