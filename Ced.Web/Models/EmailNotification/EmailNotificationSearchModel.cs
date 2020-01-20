using System;
using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.Web.Models.EmailNotification
{
    public class EmailNotificationSearchModel
    {
        public int? EventId { get; set; }

        public int[] NotificationTypeIds { get; set; }

        public int? DayRange { get; set; }

        public DateTime? EmailSendingDate { get; set; }

        public IList<NotificationType> NotificationTypes { get; set; }
    }
}