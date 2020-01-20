using AutoMapper;
using Ced.BusinessEntities;
using Ced.Utility.Edition;
using Ced.Utility.Notification;
using Ced.Utility.Web;
using Ced.Web.Models.Notification;
using ITE.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.Web.Helpers
{
    public class InAppNotificationHelper : NotificationHelper, IInAppNotificationHelper
    {
        private readonly IEditionHelper _editionHelper;

        public InAppNotificationHelper(IEditionHelper editionHelper)
        {
            _editionHelper = editionHelper;
        }

        public string GetDescription(EditionEntity edition, NotificationType notificationType)
        {
            var url = notificationType == NotificationType.EditionExistence
                ? _editionHelper.GetEditionListUrl(edition.Event, notificationType.GetAttribute<NotificationAttribute>().Fragment)
                : _editionHelper.GetEditionUrl(edition, notificationType.GetAttribute<NotificationAttribute>().Fragment);
            var name = _editionHelper.GetNameWithEditionNo(edition);
            var link = $"<a href=\"{url}\" target=\"_blank\"><b>{name}</b></a>";
            var desc = notificationType.GetDescription();
            return string.Format(desc, link);
        }

        public string GetDescription(NotificationEntity notification)
        {
            return GetDescription(notification.Edition, notification.NotificationType);
        }

        public IList<NotificationListItemModel> GetNotificationViewModelItems(IList<NotificationEntity> notifs)
        {
            foreach (var notif in notifs)
            {
                notif.Title = GetTitle(notif.Edition, notif.NotificationType, true);

                var notifAttr = notif.NotificationType.GetAttribute<NotificationAttribute>();
                notif.Url = notif.NotificationType == NotificationType.EditionExistence
                    ? _editionHelper.GetEditionListUrl(notif.Edition.Event, notifAttr.Fragment)
                    : _editionHelper.GetEditionUrl(notif.Edition, notifAttr.Fragment);
            }

            var notifsModel = Mapper.Map<IList<NotificationEntity>, IList<NotificationListItemModel>>(notifs);
            return notifsModel;
        }

        public static IList<NotificationEntity> CreateInAppNotifications(EditionEntity edition, NotificationType notificationType, string recipients, string actorUserEmail)
        {
            if (WebConfigHelper.RemoveActorUserFromNotificationRecipients)
                recipients = NotificationControllerHelper.RemoveCurrentUserFromRecipients(recipients, actorUserEmail);

            if (string.IsNullOrWhiteSpace(recipients))
                return null;

            var recipientList = recipients.Split(Utility.Constants.EmailAddressSeparator).ToList();
            var notifications = new List<NotificationEntity>();

            foreach (var recipient in recipientList)
            {
                var notification = new NotificationEntity
                {
                    NotificationType = notificationType,
                    ReceiverEmail = recipient,
                    EventId = edition.EventId,
                    EditionId = edition.EditionId,
                    CreatedOn = DateTime.Now,
                    Displayed = false,
                    SentByEmail = false
                };

                notifications.Add(notification);
            }

            return notifications;
        }
    }
}