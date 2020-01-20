using Ced.BusinessEntities;
using Ced.Utility.Azure;
using Ced.Utility.Email;
using Ced.Utility.Web;
using ITE.Utility.Extensions;
using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Ced.Utility.Notification
{
    public class EmailNotificationHelper : NotificationHelper, IEmailNotificationHelper
    {
        private static string _emailSubjectStart = "CED: ";
        private static string _emailSubjectMarkForLocal = "(CEDLocal)";
        private static string _emailSubjectMarkForTest = "(CEDTest)";

        public string GetSubject(EditionEntity edition, NotificationType notificationType)
        {
            return _emailSubjectStart + GetTitle(edition, notificationType, false);
        }

        public string GetSubject(EventEntity @event, NotificationType notificationType)
        {
            return _emailSubjectStart + GetTitle(@event, notificationType, false);
        }

        public static string GetUnsubscriptionUrl(EditionEntity edition)
        {
            var urlHelper = new UrlHelperHelper().UrlHelper;
            return urlHelper.AbsoluteAction("Unsubscribe", "Edition", new { editionId = edition.EditionId });
        }

        public EmailResult Send(EditionEntity edition, EditionTranslationEntity editionTranslation, NotificationType notificationType,
            string recipientFullName, string body, string recipients, string buttonUrl, string unsubscriptionUrl)
        {
            if (string.IsNullOrWhiteSpace(recipients))
            {
                var message = $"{notificationType} type of notification email could not be sent since edition {edition.EditionId} - {edition.EditionName} has no recipients.";
                return new EmailResult { Sent = false, ErrorMessage = message };
            }

            var sendEmail = notificationType.GetAttribute<NotificationAttribute>().SendEmail;
            if (sendEmail)
            {
                MvcMailMessage mailMessage = null;
                var webLogoUrl = EditionImageType.WebLogo.BlobFullUrl(editionTranslation);
                var displayDate = DateHelper.GetDisplayDate(edition.StartDate, edition.EndDate);
                var subject = GetSubject(edition, notificationType);

                subject += WebConfigHelper.IsLocal ? " " + _emailSubjectMarkForLocal : (WebConfigHelper.IsTest ? " " + _emailSubjectMarkForTest : null);

                var notfAttr = notificationType.GetAttribute<NotificationAttribute>();

                if (notfAttr == null)
                    throw new Exception("Notification Attribute not found!");

                var editionMailTemplate = new EditionMailTemplate
                {
                    EditionId = edition.EditionId,
                    EventId = edition.EventId,
                    AxEventId = edition.AxEventId,
                    EditionName = edition.EditionName,
                    EventName = edition.Event.MasterName,
                    WebLogoUrl = webLogoUrl,
                    Recipients = recipients,
                    RecipientFullName = recipientFullName,
                    DisplayDate = displayDate,
                    VenueName = editionTranslation.VenueName,
                    Subject = subject,
                    Body = body,
                    ButtonText = notfAttr.ButtonText,
                    ButtonUrl = buttonUrl,
                    UnsubscriptionUrl = unsubscriptionUrl,
                    GuideUrl = WebConfigHelper.CedLogoUrl,
                    PartialViewName = notfAttr.ViewName
                };

                var mailer = new UserMailer.UserMailer();
                mailMessage = mailer.Send(editionMailTemplate, notfAttr.PrivateEmail);
                return new EmailResult { Sent = true, ErrorMessage = "" };
            }

            return new EmailResult { Sent = false, ErrorMessage = "Unknown error" };
        }

        public bool Send(NotificationType notificationType, string subject, string body, string recipients)
        {
            var sendEmail = notificationType.GetAttribute<NotificationAttribute>().SendEmail;
            if (sendEmail)
            {
                subject = _emailSubjectStart + subject;
                subject += WebConfigHelper.IsLocal ? " " + _emailSubjectMarkForLocal : (WebConfigHelper.IsTest ? " " + _emailSubjectMarkForTest : null);

                var notfAttr = notificationType.GetAttribute<NotificationAttribute>();

                if (notfAttr == null)
                    throw new Exception("Notification Attribute not found!");

                var editionMailTemplate = new EditionMailTemplate
                {
                    Recipients = recipients,
                    Subject = subject,
                    Body = body,
                    ButtonText = notfAttr.ButtonText,
                    PartialViewName = notfAttr.ViewName
                };

                var mailer = new UserMailer.UserMailer();
                mailer.Send(editionMailTemplate, notfAttr.PrivateEmail);
                return true;
            }
            return false;
        }

        public List<int> GetCheckDays(NotificationType notificationType)
        {
            var val = ConfigurationManager.AppSettings[notificationType.ToString()];
            var list = val.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            return list;
        }
    }
}