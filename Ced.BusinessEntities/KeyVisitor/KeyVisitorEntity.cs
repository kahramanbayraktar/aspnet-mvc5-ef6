using System;

namespace Ced.BusinessEntities
{
    public class KeyVisitorEntity
    {
        public int KeyVisitorId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
