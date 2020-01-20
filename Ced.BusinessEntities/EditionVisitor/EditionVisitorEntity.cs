using System;
using ITE.Utility.ObjectComparison;

namespace Ced.BusinessEntities
{
    public class EditionVisitorEntity : ICloneable
    {
        public int EditionVisitorId { get; set; }

        public int EditionId { get; set; }

        public byte DayNumber { get; set; }

        [Comparable]
        public short VisitorCount { get; set; }

        [Comparable]
        public short? RepeatVisitCount { get; set; }

        [Comparable]
        public short? OldVisitorCount { get; set; }

        [Comparable]
        public short? NewVisitorCount { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
