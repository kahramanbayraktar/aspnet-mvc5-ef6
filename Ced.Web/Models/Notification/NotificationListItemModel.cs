using System;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Notification
{
    public class NotificationListItemModel
    {
        public int NotificationId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public string Url { get; set; }

        public NotificationType Type { get; set; }
        
        public string FaIcon { get; set; }
        
        public string TextClass { get; set; }

        public bool Displayed { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}