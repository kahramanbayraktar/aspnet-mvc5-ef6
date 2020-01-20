using System;

namespace Ced.BusinessEntities
{
    public class NotificationEntity
    {
        public int NotificationId { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string Url { get; set; }
        
        public NotificationType NotificationType { get; set; }

        public int ReceiverId { get; set; }

        public string ReceiverEmail { get; set; }

       public bool Displayed { get; set; }

        public bool SentByEmail { get; set; }

        public DateTime? EmailedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int EventId { get; set; }

        public int EditionId { get; set; }

        public EditionEntity Edition { get; set; }
    }
}
