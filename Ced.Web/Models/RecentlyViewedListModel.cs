using System;
using Ced.BusinessEntities;

namespace Ced.Web.Models
{
    public class RecentViewListModel
    {
        public int EntityId { get; set; }

        public EntityType EntityType { get; set; }
        
        public string Title { get; set; }
        
        public string Logo { get; set; }

        public string Url { get; set; }
        
        public ActionType ActionType { get; set; }

        public DateTime CreateTime { get; set; }
    }
}