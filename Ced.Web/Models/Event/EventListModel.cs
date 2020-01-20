using System;
using System.ComponentModel.DataAnnotations;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Event;

namespace Ced.Web.Models.Event
{
    public class EventListModel
    {
        public int EventId { get; set; }

        [Display(Name = "MasterCode", ResourceType = typeof(Resources.Resources))]
        public string MasterCode { get; set; }

        [Display(Name = "MasterName", ResourceType = typeof(Resources.Resources))]
        public string MasterName { get; set; }
        
        public int RegionId { get; set; }

        [Display(Name = "Region", ResourceType = typeof(Resources.Resources))]
        public string Region { get; set; }
        
        public int CountryId { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Resources.Resources))]
        public string Country { get; set; }
        
        public int CityId { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resources.Resources))]
        public string City { get; set; }
        
        public EventType EventType { get; set; }

        // TODO: Enum
        [Display(Name = "EventType", ResourceType = typeof(Resources.Resources))]
        public string EventTypeName { get; set; }
        
        public int IndustryId { get; set; }

        [Display(Name = "Industry", ResourceType = typeof(Resources.Resources))]
        public string Industry { get; set; }

        //[Display(Name = "SubIndustry", ResourceType = typeof(Resources.Resources))]
        public string SubIndustry { get; set; }
        
        [Display(Name = "CreateTime", ResourceType = typeof(Resources.Resources))]
        public DateTime CreateTime { get; set; }

        [Display(Name = "UpdateTime", ResourceType = typeof(Resources.Resources))]
        public DateTime UpdateTime { get; set; }
    }
}