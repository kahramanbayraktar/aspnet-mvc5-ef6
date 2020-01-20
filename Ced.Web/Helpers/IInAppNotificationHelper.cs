using Ced.BusinessEntities;
using Ced.Web.Models.Notification;
using System.Collections.Generic;

namespace Ced.Web.Helpers
{
    public interface IInAppNotificationHelper
    {
        string GetDescription(EditionEntity edition, NotificationType notificationType);
        string GetDescription(NotificationEntity notification);
        IList<NotificationListItemModel> GetNotificationViewModelItems(IList<NotificationEntity> notifs);
        string GetTitle(EditionEntity edition, NotificationType notificationType, bool isHtml);
        string GetTitle(EventEntity @event, NotificationType notificationType, bool isHtml);
        string GetEditionName(EditionEntity edition, bool isHtml);
        string GetEventName(EventEntity @event, bool isHtml);
    }
}