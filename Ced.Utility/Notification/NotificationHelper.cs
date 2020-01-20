using Ced.BusinessEntities;
using Ced.Utility.Edition;
using ITE.Utility.Extensions;

namespace Ced.Utility.Notification
{
    public class NotificationHelper : INotificationHelper
    {
        private readonly IEditionHelper _editionHelper = new EditionHelper();

        public string GetTitle(EditionEntity edition, NotificationType notificationType, bool isHtml)
        {
            var description = notificationType.GetDescription();
            var name = GetEditionName(edition, isHtml);
            var nameWithNumber = _editionHelper.GetNameWithEditionNo(edition.EditionNo, edition.EditionName);
            
            var title = edition.EditionNo > 0
                ? string.Format(description, nameWithNumber)
                : string.Format(description, name);

            return title;
        }

        public string GetTitle(EventEntity @event, NotificationType notificationType, bool isHtml)
        {
            var description = notificationType.GetDescription();
            var name = GetEventName(@event, isHtml);

            var title = string.Format(description, name);

            return title;
        }

        public string GetEditionName(EditionEntity edition, bool isHtml)
        {
            var name = isHtml
                ? "<b>" + edition.EditionName + "</b>"
                : edition.EditionName;
            return name;
        }

        public string GetEventName(EventEntity @event, bool isHtml)
        {
            var name = isHtml
                ? "<b>" + @event.MasterName + "</b>"
                : @event.MasterName;
            return name;
        }
    }
}