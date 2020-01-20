using System;
using Ced.BusinessEntities;

namespace Ced.Web.Models.EmailNotification
{
    public class EmailNotificationListItemModel
    {
        public DateTime? EmailSendingDate { get; set; }

        public NotificationType NotificationType { get; set; }

        public string EditionName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ReceiverEmail { get; set; }

        public bool SentByEmail { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}