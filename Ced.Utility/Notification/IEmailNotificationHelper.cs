using Ced.BusinessEntities;
using Ced.Utility.Email;
using System.Collections.Generic;

namespace Ced.Utility.Notification
{
    public interface IEmailNotificationHelper
    {
        string GetSubject(EditionEntity edition, NotificationType notificationType);
        string GetSubject(EventEntity @event, NotificationType notificationType);

        EmailResult Send(EditionEntity edition, EditionTranslationEntity editionTranslation, NotificationType notificationType,
            string recipientFullName, string body, string recipients, string buttonUrl, string unsubscriptionUrl);

        bool Send(NotificationType notificationType, string subject, string body, string recipients);
        List<int> GetCheckDays(NotificationType notificationType);
        string GetTitle(EditionEntity edition, NotificationType notificationType, bool isHtml);
        string GetTitle(EventEntity @event, NotificationType notificationType, bool isHtml);
        string GetEditionName(EditionEntity edition, bool isHtml);
        string GetEventName(EventEntity @event, bool isHtml);
    }
}