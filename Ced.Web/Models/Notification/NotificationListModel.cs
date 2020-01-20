using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Notification
{
    public class NotificationListModel
    {
        public IList<NotificationType> NotificationTypes { get; set; }

        public int? DayRange { get; set; }

        public IList<NotificationListItemModel> Notifications { get; set; }
    }
}