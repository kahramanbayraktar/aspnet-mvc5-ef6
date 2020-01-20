using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility.MVC;
using Ced.Utility.Notification;
using Ced.Utility.Web;
using Ced.Web.Filters;
using Ced.Web.Models.EmailNotification;
using ITE.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class EmailNotificationController : GlobalController
    {
        private readonly IEmailNotificationHelper _emailNotificationHelper;

        public EmailNotificationController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IEmailNotificationHelper emailNotificationHelper,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            _emailNotificationHelper = emailNotificationHelper;
        }

        public ActionResult Index()
        {
            var notifs = GetEmailNotificationListItems(new EmailNotificationSearchModel
            {
                EmailSendingDate = DateTime.Today
            });

            var model = new EmailNotificationListModel
            {
                NotificationTypes = EditionCompletenessNotificationTypes(),
                EmailSendingDate = DateTime.Today,
                SelectedNotificationTypes = null,
                EmailNotifications = notifs
            };

            return View(model);
        }

        public ActionResult _Search()
        {
            var model = new EmailNotificationSearchModel
            {
                NotificationTypes = EditionCompletenessNotificationTypes(),
                EmailSendingDate = DateTime.Today
            };

            return PartialView(model);
        }

        [AjaxOnly]
        [HttpPost]
        public ActionResult _Search(EmailNotificationSearchModel searchModel)
        {
            //if ((searchModel.NotificationTypeIds == null || !searchModel.NotificationTypeIds.Any()))
            //    ModelState.AddModelError("FilterOptions", "At least one filter option must be applied.");

            //// If more than one apps are selected then roles will be ignored in search.
            //if (searchModel.ApplicationIds != null && searchModel.ApplicationIds.Length > 1)
            //    searchModel.RoleIds = null;

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var model = GetEmailNotificationListItems(searchModel);
            return Json(new { success = true, data = this.RenderRazorViewToString("_List", model) });
        }

        #region HELPER METHODS

        private IList<NotificationType> EditionCompletenessNotificationTypes()
        {
            return EnumExtensions.GetValues<NotificationType>()
                .Where(notificationType => notificationType.GetAttribute<CompletenessNotificationTypeAttribute>() != null)
                .ToList();
        }

        private void AddEditionsToNotificationList(IList<EmailNotificationListItemModel> notifications, IList<EditionEntity> editions, NotificationType notificationType, int checkDay)
        {
            foreach (var edition in editions)
            {
                EditionEntity prevEdition = null;
                var sendingDate = DateTime.MinValue;

                switch (notificationType)
                {
                    case NotificationType.EditionExistence: // TODO:???
                        break;
                    case NotificationType.GeneralInfoCompleteness:
                        prevEdition = EditionServices.GetPreviousEdition(edition);
                        sendingDate = prevEdition.StartDate.Value.AddDays(-checkDay);
                        break;
                    case NotificationType.PostShowMetricsInfoCompleteness:
                    case NotificationType.PostShowMetricsInfoCompleteness2:
                        sendingDate = edition.EndDate.Value.AddDays(checkDay);
                        break;
                }

                var log = NotificationEmailSent(prevEdition ?? edition, notificationType, sendingDate);
                
                notifications.Add(new EmailNotificationListItemModel
                {
                    EmailSendingDate = sendingDate,
                    NotificationType = notificationType,
                    EditionName = edition.EditionName,
                    StartDate = edition.StartDate.GetValueOrDefault(),
                    EndDate = edition.EndDate.GetValueOrDefault(),
                    ReceiverEmail = log?.AdditionalInfo.Substring("Recipient: ".Length, log.AdditionalInfo.IndexOf("|") - "Recipient: ".Length),
                    SentByEmail = log != null,
                    CreatedOn = log?.CreatedOn
                });
            }
        }

        private LogEntity NotificationEmailSent(EditionEntity edition, NotificationType notificationType, DateTime notificationDate)
        {
            var logs = LogServices.GetLogsByEdition(edition.EditionId, EntityType.Email.ToString(), notificationType.ToString(), notificationDate);

            logs = ExtractSentNotificationEmailLogs(logs);

            return logs.Any() ? logs.OrderByDescending(x => x.CreatedOn).First() : null;
        }

        private IList<LogEntity> ExtractSentNotificationEmailLogs(IList<LogEntity> logs)
        {
            return logs.Where(x => x.ActionType == ActionType.NotificationEmailSend).ToList();
        }

        private IList<EmailNotificationListItemModel> GetEmailNotificationListItems(EmailNotificationSearchModel model)
        {
            var notifications = new List<EmailNotificationListItemModel>();

            if (model.NotificationTypes == null)
                model.NotificationTypes = EditionCompletenessNotificationTypes().ToArray();

            if (model.DayRange > 0)
                model.EmailSendingDate = null;

            foreach (var notifType in model.NotificationTypes)
            {
                var deviationInDays = Convert.ToInt32((DateTime.Today - model.EmailSendingDate).GetValueOrDefault().TotalDays);
                if (notifType.GetAttribute<NotificationAttribute>().CheckDaysType == NotificationAttribute.CheckDaysTypes.Passed)
                    deviationInDays *= -1;

                if (notifType.GetAttribute<CompletenessNotificationTypeAttribute>() != null)
                {
                    var minFinancialYear = WebConfigHelper.MinFinancialYear;
                    var statuses = Utility.Constants.DefaultValidEditionStatusesForCed;
                    var eventTypes = notifType.GetAttribute<NotificationAttribute>().EventTypes.Select(x => x.GetDescription()).ToArray();
                    var eventActivities = Utility.Constants.ValidEventActivitiesToNotify;
                    var checkDays = _emailNotificationHelper.GetCheckDays(notifType);

                    foreach (var checkDay in checkDays)
                    {
                        var editionsToNotify = EditionServices.GetEditionsByNotificationType(checkDay - deviationInDays, model.DayRange, notifType, minFinancialYear, statuses, eventTypes, eventActivities, model.EventId);

                        AddEditionsToNotificationList(notifications, editionsToNotify, notifType, checkDay);
                    }
                }
            }

            return notifications;
        }

        #endregion
    }
}