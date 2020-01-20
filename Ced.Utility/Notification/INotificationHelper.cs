using Ced.BusinessEntities;

namespace Ced.Utility.Notification
{
    public interface INotificationHelper
    {
        string GetTitle(EditionEntity edition, NotificationType notificationType, bool isHtml);
        string GetTitle(EventEntity @event, NotificationType notificationType, bool isHtml);
        string GetEditionName(EditionEntity edition, bool isHtml);
        string GetEventName(EventEntity @event, bool isHtml);
    }
}