using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public class EventEntity : ICloneable
    {
        public int EventId { get; set; }

        [StagingDbComparable]
        public string MasterCode { get; set; }
        
        [StagingDbComparable]
        public string MasterName { get; set; }

        public long AxRecId { get; set; }
        
        public int RegionId { get; set; }

        [StagingDbComparable]
        public string Region { get; set; }

        [StagingDbComparable]
        public string EventType { get; set; }

        [StagingDbComparable]
        public string EventTypeCode { get; set; }

        public int IndustryId { get; set; }

        [StagingDbComparable]
        public string Industry { get; set; }

        [StagingDbComparable]
        public string SubIndustry { get; set; }

        [StagingDbComparable]
        public string EventBusinessClassification { get; set; }

        public string DisplayDate { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public int? CreateUser { get; set; }
        
        public DateTime? UpdateTime { get; set; }
        
        public int? UpdateUser { get; set; }

        public IList<EventDirectorEntity> Directors { get; set; }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public enum EventType
    {
        [Description("CONF")]
        Conference,

        [Description("CONFEX")]
        ConferenceExhibition,

        [Description("EXH")]
        Exhibition,
        
        [Description("BOTH")]
        ExhibitionConference,

        [Description("IOH")]
        IndirectOverheads,
        
        [Description("NON")]
        NonEvent,

        [Description("PUB")]
        Publication,

        [Description("WEB")]
        Website
    }

    public enum EventBusinessClassification
    {
        [Description("Event")]
        Event,

        [Description("Publication")]
        Publication,

        [Description("Other")]
        Other
    }
}
