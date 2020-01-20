using System.Collections.Generic;

namespace Ced.Web.Models.Notification
{
    public class LatestNotificationsModel
    {
        public IList<NotificationListItemModel> Notifications { get; set; }

        public int Count { get; set; }
    }
}