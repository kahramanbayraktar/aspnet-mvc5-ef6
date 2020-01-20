using System;
using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.EmailNotification
{
    public class EmailNotificationListModel
    {
        public DateTime? EmailSendingDate { get; set; }

        public IList<NotificationType> NotificationTypes { get; set; }

        //public IList<NotificationEntity> Notifications { get; set; }

        public NotificationType[] SelectedNotificationTypes { get; set; }

        public IList<EmailNotificationListItemModel> EmailNotifications { get; set; }
    }
}