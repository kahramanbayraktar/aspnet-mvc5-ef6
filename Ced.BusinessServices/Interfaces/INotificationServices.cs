using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.BusinessServices
{
    public interface INotificationServices
    {
        NotificationEntity GetNotification(int notificationId);

        IList<NotificationEntity> GetNotificationsByRecipient(string recipient, int? count = null, bool? displayed = null);

        IList<NotificationEntity> GetNotificationsByEdition(int editionId);

        IList<NotificationEntity> GetNotifications(string recipient, NotificationType[] notificationTypes, int? dayRange);

        int GetNotificationCount(string recipient, bool? displayed = null);

        NotificationEntity CreateNotification(NotificationEntity notificationEntity, int userId);

        void CreateNotifications(IList<NotificationEntity> notifications, int userId);

        void DisableNotifications(string recipient);

        void DeleteNotification(int notificationId);
    }
}
