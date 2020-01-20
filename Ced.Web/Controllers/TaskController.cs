using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.BusinessServices.Helpers;
using Ced.DWStaging;
using Ced.Utility;
using Ced.Utility.Edition;
using Ced.Utility.Notification;
using Ced.Utility.Web;
using Ced.Web.Hubs;
using Ced.Web.Models.Task;
using ITE.Logger;
using ITE.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class TaskController : GlobalController
    {
        private readonly IEditionHelper _editionHelper;
        private readonly IEmailNotificationHelper _emailNotificationHelper;

        public TaskController(
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
            ITaskServices taskServices,
            IEditionHelper editionHelper,
            IEmailNotificationHelper emailNotificationHelper,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, eventServices, eventDirectorServices, logServices, notificationServices, taskServices, userServices, userRoleServices)
        {
            _editionHelper = editionHelper;
            _emailNotificationHelper = emailNotificationHelper;
        }

        public ActionResult Index()
        {
            var model = new TaskListModel
            {
                Tasks = GetTaskListItems()
            };
            return View(model);
        }

        [AllowAnonymous]
        [CedAction(External = true)]
        public void UpdateEventsDirectorsNotifications(string key, string connectionId)
        {
            UpdateApprovedEditionsFromStagingDb(key, connectionId);

            UpdateEventsFromStagingDb(key, connectionId);

            UpdateEventDirectors(key, connectionId);

            UpdateNotifications(key, connectionId);

            NotifyAboutMissingEditionImages(key, connectionId);
        }

        [AllowAnonymous]
        [CedAction(Loggable = true, External = true)]
        public void UpdateApprovedEditionsFromStagingDb(string key, string connectionId)
        {
            if (key == WebConfigHelper.TaskSchedulerSecretKey)
            {
                try
                {
                    var approvedEditions = GetApprovedEditions();

                    var stagingEditions = GetStagingEditionsPossiblyHaveNewEditions(approvedEditions);

                    if (approvedEditions.Count > 0)
                    {
                        for (var i = 0; i < approvedEditions.Count; i++)
                        {
                            var approvedEdition = approvedEditions[i];

                            var siblingsOnCed = GetSiblingEditionsByApprovedEdition(approvedEdition);

                            var siblingsOnStaging = stagingEditions.Where(x => x.EventMasterCode == approvedEdition.Event.MasterCode).ToList();

                            if (siblingsOnStaging.Count == siblingsOnCed.Count)
                            {
                                var newlyAddedEdition = siblingsOnStaging.OrderByDescending(x => x.EventStartDate).Take(1).SingleOrDefault();
                                UpdateApprovedEdition(newlyAddedEdition, approvedEdition);
                                break;
                            }

                            if (siblingsOnStaging.Count > siblingsOnCed.Count)
                            {
                                var message = "In Staging database there are more editions of the event " +
                                              $"{approvedEdition.Event.MasterCode} - {approvedEdition.Event.MasterName} than in CED database." +
                                              "Both must be equal or the ones CED database must be lesser.";
                                var log = CreateInternalLog(message);
                                ExternalLogHelper.Log(log, LoggingEventType.Fatal);
                            }

                            if (!string.IsNullOrWhiteSpace(connectionId))
                                ProgressHub.SendProgress(connectionId, "Done", i + 1, approvedEditions.Count, TaskType.UpdateApprovedEditionsFromStagingDb.GetHashCode());
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(connectionId))
                            ProgressHub.SendProgress(connectionId, "Done", 1, 1, TaskType.UpdateApprovedEditionsFromStagingDb.GetHashCode());
                    }

                    var log2 = CreateInternalLog("The task UpdateApprovedEditionsFromStagingDb completed.", AutoIntegrationUser);
                    ExternalLogHelper.Log(log2, LoggingEventType.Information);
                }
                catch (Exception exc)
                {
                    var message = "The task UpdateApprovedEditionsFromStagingDb failed! " + exc.GetFullMessage();
                    ExternalLogHelper.Log(message, LoggingEventType.Fatal);
                }
            }
        }

        [AllowAnonymous]
        [CedAction(ActionType = ActionType.TaskEditionUpdateFromStaging, Loggable = true, External = true)]
        public JsonResult UpdateEventsFromStagingDb(string key, string connectionId)
        {
            if (key == WebConfigHelper.TaskSchedulerSecretKey)
            {
                try
                {
                    var stagingEditionServices = new DWStaging.BusinessServices.EditionServices();

                    var stagingEditions =
                        WebConfigHelper.IsLocal || WebConfigHelper.IsTest
                            //? stagingEditionServices.GetEditionsByEventBeId("101558") :
                            ? stagingEditionServices.GetEditionsByMasterCode(new[] { "1102" }) :
                            stagingEditionServices.GetEditions();

                    //var stagingEditions =
                    //    stagingEditionServices.GetEditionsQueryable()
                    //        //.Where(x => new[] { "10117", "10215", "941", "969", "980" }.Contains(x.EventMasterCode))
                    //        .Where(x => x.EventMaster.ToLower().Contains("agent"))
                    //        .ToList();

                    stagingEditions = stagingEditions.OrderBy(x => x.EventStartDate).ToList();

                    for (var i = 0; i < stagingEditions.Count; i++)
                    {
                        var stagingEdition = stagingEditions[i];

                        var axId = Convert.ToInt32(stagingEdition.EventBEID);

                        if (!DateTime.TryParseExact(stagingEdition.EventStartDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eventStartDate))
                        {
                            eventStartDate = new DateTime(1970, 1, 1);
                        }

                        if (!DateTime.TryParseExact(stagingEdition.EventEndDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eventEndDate))
                        {
                            eventEndDate = new DateTime(1970, 1, 1);
                        }

                        var existingEdition = EditionServices.GetEditionByAxId(axId);

                        if (existingEdition != null) // UPDATE
                        {
                            TaskServices.UpdateEventEditionFromStagingDb(existingEdition, stagingEdition, eventStartDate, eventEndDate);
                        }
                        else // CREATE
                        {
                            TaskServices.CreateEventEditionFromStagingDb(stagingEdition, eventStartDate, eventEndDate);
                        }

                        if (!string.IsNullOrWhiteSpace(connectionId))
                            ProgressHub.SendProgress(connectionId, stagingEdition.EventName, i + 1, stagingEditions.Count, TaskType.UpdateEventsFromStagingDb.GetHashCode());
                    }

                    var log = CreateInternalLog("The task UpdateEventsFromStagingDb completed.", AutoIntegrationUser);
                    ExternalLogHelper.Log(log, LoggingEventType.Information);

                    return Json("", JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    var message = "The task UpdateEventsFromStagingDb failed! " + exc.GetFullMessage();
                    ExternalLogHelper.Log(message, LoggingEventType.Fatal);

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [CedAction(ActionType = ActionType.TaskEventDirectorUpdate, Loggable = true, External = true)]
        public void UpdateEventDirectors(string key, string connectionId)
        {
            if (key == WebConfigHelper.TaskSchedulerSecretKey)
            {
                try
                {
                    TaskServices.UpdateEventDirectors();

                    var log = CreateInternalLog("The task UpdateEventDirectors completed.", AutoIntegrationUser);
                    ExternalLogHelper.Log(log, LoggingEventType.Information);

                    if (!string.IsNullOrWhiteSpace(connectionId))
                        ProgressHub.SendProgress(connectionId, "Done", 1, 1, TaskType.UpdateEventDirectors.GetHashCode());
                }
                catch (Exception exc)
                {
                    var log = CreateInternalLog("The task UpdateEventDirectors failed!", AutoIntegrationUser);
                    var message = log.AdditionalInfo + " | " + exc.GetFullMessage();
                    ExternalLogHelper.Log(message, LoggingEventType.Fatal);
                }
            }
        }

        [AllowAnonymous]
        [CedAction(ActionType = ActionType.TaskNotificationUpdate, Loggable = true, External = false /*true*/)]
        public JsonResult UpdateNotifications(string key, string connectionId)
        {
            if (key == WebConfigHelper.TaskSchedulerSecretKey)
            {
                try
                {
                    var notificationTypes = Enum.GetValues(typeof(NotificationType));

                    var allEditionNotifPairs = new Dictionary<EditionEntity, NotificationType>();
                    foreach (NotificationType notificationType in notificationTypes)
                    {
                        if (notificationType.GetAttribute<CompletenessNotificationTypeAttribute>() == null)
                            continue;

                        var editions = GetEditionsToNotify(notificationType);

                        foreach (var edition in editions)
                        {
                            allEditionNotifPairs.Add(edition, notificationType);
                        }                        
                    }

                    if (allEditionNotifPairs.Count > 0)
                    {
                        for (var i = 0; i < allEditionNotifPairs.Count; i++)
                        {
                            var pair = allEditionNotifPairs.ElementAt(i);

                            ProcessEditionNotifications(pair, i, allEditionNotifPairs.Count);

                            if (!string.IsNullOrWhiteSpace(connectionId))
                                ProgressHub.SendProgress(connectionId, pair.Value + " for " + pair.Key.EditionName, i + 1, allEditionNotifPairs.Count, TaskType.UpdateNotifications.GetHashCode());
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(connectionId))
                            ProgressHub.SendProgress(connectionId, "Done", 1, 1, TaskType.UpdateNotifications.GetHashCode());
                    }

                    var log = CreateInternalLog("The task UpdateNotifications completed.", AutoIntegrationUser);
                    ExternalLogHelper.Log(log, LoggingEventType.Information);

                    return Json("", JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    var message = "The task UpdateNotifications failed! " + exc.GetFullMessage();
                    ExternalLogHelper.Log(message, LoggingEventType.Fatal);

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        private void ProcessEditionNotifications(KeyValuePair<EditionEntity, NotificationType> editionNotification, int itemIndex, int totalItemsCount)
        {
            var edition = editionNotification.Key;
            var notificationType = editionNotification.Value;

            // NEEDLESS!
            // STEP 2: Does it REQUIRE a NOTIFICATION?
            var requires = EditionServices.RequiresNotification(edition, notificationType);

            if (requires)
            {
                // STEP 3: Does NOTIFICATION already EXIST?
                //if (!NotificationAlreadyExists(edition, notificationType))
                {
                    if (WebConfigHelper.PrimaryDirectorNotifications)
                    {
                        var recipients = WebConfigHelper.PrimaryDirectorNotificationsUseMockRecipients
                            ? WebConfigHelper.AdminEmails
                            : EventDirectorServices.GetRecipientEmails(edition);

                        // STEP 3: CREATE NOTIFICATION
                        CreateInAppNotification(edition, notificationType, recipients, null);

                        PushRealTimeInAppNotification(edition, notificationType, recipients, CurrentCedUser?.CurrentUser?.Email);

                        // STEP 4: SEND EMAIL
                        var notifAttr = notificationType.GetAttribute<NotificationAttribute>();
                        var buttonUrl = notificationType == NotificationType.EditionExistence
                            ? _editionHelper.GetEditionListUrl(edition.Event, notifAttr.Fragment)
                            : _editionHelper.GetEditionUrl(edition, notifAttr.Fragment);

                        var emailResult = SendEmailNotification(edition, notificationType, recipients, null, null, buttonUrl);

                        // STEP 5: LOG EMAIL
                        if (emailResult.Sent)
                            LogEmail(edition.EditionId, recipients, emailResult.ErrorMessage, null, notificationType.ToString());
                        else
                            LogEmail(edition.EditionId, WebConfigHelper.AdminEmails, emailResult.ErrorMessage, null, notificationType.ToString(), ActionType.NotificationEmailSendFailure);
                    }
                }
            }
        }

        [AllowAnonymous]
        [CedAction(ActionType = ActionType.TaskEditionUpdateFromKentico, Loggable = true, External = true)]
        public void UpdateEventsFromKentico(string key)
        {
            //if (key == WebConfigHelper.TaskSchedulerSecretKey)
            //{
            //    try
            //    {
            //        TaskServices.UpdateEventsFromKentico();

            //        var log = CreateInternalLog("One-time task UpdateEventsFromKentico completed.", true);
            //        ExternalLogHelper.Log(log, LoggingEventType.Information);
            //    }
            //    catch (Exception exc)
            //    {
            //        var message = "One-time task UpdateEventsFromKentico failed! " + exc.GetFullMessage();
            //        ExternalLogHelper.Log(message, LoggingEventType.Fatal);
            //    }
            //}
        }

        [AllowAnonymous]
        [CedAction(ActionType = ActionType.TaskNotifyAboutMissingEditionImages, Loggable = true, External = true)]
        public JsonResult NotifyAboutMissingEditionImages(string key, string connectionId)
        {
            if (key == WebConfigHelper.TaskSchedulerSecretKey)
            {
                try
                {
                    var editions = TaskServices.GetEditionsWithMissingImages();

                    var pattern = "<a href='{0}'>{1}</a>";

                    var body = "<table>";

                    for(var i = 0; i < editions.Count; i++)
                    {
                        var edition = editions[i];

                        var url = _editionHelper.GetEditionUrl(new EditionEntity
                        {
                            EditionId = edition.EditionId,
                            EditionName = edition.EventName,
                            Status = edition.Status.ToEnum<EditionStatusType>()
                        });

                        body += "<tr><td class='font-lato' style='font-size: 14px; color: #888794'>" + string.Format(pattern, url, edition.EventName);
                        body += edition.EventBackGroundImage == null ? " [People Image] " : "";
                        body += edition.EventImage == null ? " [Web Logo] " : "";
                        body += edition.IconFileName == null ? " [Icon] " : "";
                        body += edition.MasterLogoFileName == null ? " [Master Logo] " : "";
                        body += edition.ProductImageFileName == null ? " [Product Image] " : "";
                        body += "</td></tr>";

                        if (!string.IsNullOrWhiteSpace(connectionId))
                            ProgressHub.SendProgress(connectionId, "Detecting missing images...", i + 1, editions.Count, TaskType.NotifyAboutMissingEditionImages.GetHashCode());
                    }

                    body += "</table>";

                    var subject = "Events with Missing Images";
                    var recipients = WebConfigHelper.MarketingAdminEmails;

                    _emailNotificationHelper.Send(NotificationType.MissingEditionImages, subject, body, recipients);

                    var log = CreateInternalLog("The task NotifyAboutMissingEditionImages completed.", AutoIntegrationUser);
                    ExternalLogHelper.Log(log, LoggingEventType.Information);

                    return Json("", JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    var message = "The task NotifyAboutMissingEditionImages failed! " + exc.GetFullMessage();
                    ExternalLogHelper.Log(message, LoggingEventType.Error);

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public void ResetEditionStartDateDifferences(string key)
        {
            if (key == WebConfigHelper.TaskSchedulerSecretKey)
            {
                try
                {
                    TaskServices.ResetEditionStartDateDifferences();

                    var log = CreateInternalLog("One-time task ResetEditionStartDateDifferences completed.", AutoIntegrationUser);
                    ExternalLogHelper.Log(log, LoggingEventType.Information);
                }
                catch (Exception exc)
                {
                    var message = "One-time task ResetEditionStartDateDifferences failed! " + exc.GetFullMessage();
                    ExternalLogHelper.Log(message, LoggingEventType.Error);
                }
            }
        }

        #region HELPER METHODS

        private IList<TaskListItemModel> GetTaskListItems(string taskName = null)
        {
            return new List<TaskListItemModel>
            {
                new TaskListItemModel
                {
                    TaskId = TaskType.UpdateEventsDirectorsNotifications.GetHashCode(),
                    TaskName = "Update Events Directors Notifications",
                    Description = "Runs the tasks UpdateApprovedEditionsFromStagingDb, UpdateEventsFromStagingDb, UpdateEventDirectors and UpdateNotifications respectively.",
                    TaskUrl = Url.Action("UpdateEventsDirectorsNotifications"),
                    LastRunTime = LogServices.GetLatestLogByAction("Task", "UpdateEventsDirectorsNotifications")?.CreatedOn,
                    IsActive = false
                },
                new TaskListItemModel
                {
                    TaskId = TaskType.UpdateApprovedEditionsFromStagingDb.GetHashCode(),
                    TaskName = "Update Approved Editions From Staging Db",
                    Description = "Makes events published if they're assigned an EventBEID on AX side.",
                    TaskUrl = Url.Action("UpdateApprovedEditionsFromStagingDb"),
                    LastRunTime = LogServices.GetLatestLogByAction("Task", "UpdateApprovedEditionsFromStagingDb")?.CreatedOn,
                    IsActive = true
                },
                new TaskListItemModel
                {
                    TaskId = TaskType.UpdateEventsFromStagingDb.GetHashCode(),
                    TaskName = "Update Events From Staging Db",
                    Description = "Retrives current event data from AX database via Staging database.",
                    TaskUrl = Url.Action("UpdateEventsFromStagingDb"),
                    LastRunTime = LogServices.GetLatestLogByAction("Task", "UpdateEventsFromStagingDb")?.CreatedOn,
                    IsActive = true
                },
                new TaskListItemModel
                {
                    TaskId = TaskType.UpdateEventDirectors.GetHashCode(),
                    TaskName = "Update Event Directors",
                    Description = "Executes related stored procedures to create event director records.",
                    TaskUrl = Url.Action("UpdateEventDirectors"),
                    LastRunTime = LogServices.GetLatestLogByAction("Task", "UpdateEventDirectors")?.CreatedOn,
                    IsActive = true
                },
                new TaskListItemModel
                {
                    TaskId = TaskType.UpdateNotifications.GetHashCode(),
                    TaskName = "Update Notifications",
                    Description = "Sends primary event directors notifications relating to events' status.",
                    TaskUrl = Url.Action("UpdateNotifications"),
                    LastRunTime = LogServices.GetLatestLogByAction("Task", "UpdateNotifications")?.CreatedOn,
                    IsActive = true
                },
                new TaskListItemModel
                {
                    TaskId = TaskType.UpdateEventsFromKentico.GetHashCode(),
                    TaskName = "Update Events From Kentico",
                    Description = "Retrieved the event data in Kentico in the beginning of the project CED.",
                    TaskUrl = Url.Action("UpdateEventsFromKentico"),
                    LastRunTime = LogServices.GetLatestLogByAction("Task", "UpdateEventsFromKentico")?.CreatedOn,
                    IsActive = false
                },
                new TaskListItemModel
                {
                    TaskId = TaskType.NotifyAboutMissingEditionImages.GetHashCode(),
                    TaskName = "Notify About Editions With Missing Images",
                    Description = "Notifies about editions with missing images.",
                    TaskUrl = Url.Action("NotifyAboutMissingEditionImages"),
                    LastRunTime = LogServices.GetLatestLogByAction("Task", "NotifyAboutEditionsWithMissingImages")?.CreatedOn,
                    IsActive = true
                }
            }.OrderBy(x => x.TaskName).ToList();
        }

        public IList<EditionEntity> GetEditionsToNotify(NotificationType notificationType)
        {
            IList<EditionEntity> editions;
            if (WebConfigHelper.IsLocal || WebConfigHelper.IsTest)
            {
                editions = EditionServices.GetEditionsByEvent(248);
            }
            else
            {
                var deviationInDays = WebConfigHelper.EditionNotificationDeviationInDays;
                if (notificationType.GetAttribute<NotificationAttribute>().CheckDaysType == NotificationAttribute.CheckDaysTypes.Passed)
                    deviationInDays *= -1;
                var checkDays = _emailNotificationHelper.GetCheckDays(notificationType).Select(x => x - deviationInDays).ToList();

                var minFinancialYear = WebConfigHelper.MinFinancialYear;
                var statuses = Utility.Constants.DefaultValidEditionStatusesForCed;
                var eventTypes = notificationType.GetAttribute<NotificationAttribute>().EventTypes.Select(x => x.GetDescription()).ToArray();
                var eventActivities = Utility.Constants.ValidEventActivitiesToNotify;

                editions = EditionServices.GetEditionsByNotificationType(checkDays, null, notificationType, minFinancialYear, statuses, eventTypes, eventActivities);
            }

            return editions;
        }

        public IList<EditionEntity> GetApprovedEditions()
        {
            var approvedEditions = EditionServices.GetEditionsByStatus(new List<EditionStatusType> { EditionStatusType.Approved }.ToArray());
            return approvedEditions;
        }

        private IList<Edition> GetStagingEditionsPossiblyHaveNewEditions(IEnumerable<EditionEntity> approvedEditions)
        {
            var stagingEditionServices = new DWStaging.BusinessServices.EditionServices();

            var masterCodes = approvedEditions.Select(x => x.Event.MasterCode).ToArray();

            var stagingEditions = stagingEditionServices.GetEditionsByMasterCode(masterCodes);

            return stagingEditions;
        }

        private IList<EditionEntity> GetSiblingEditionsByApprovedEdition(EditionEntity approvedEdition)
        {
            var editions = EditionServices.GetEditionsByEvent(approvedEdition.EventId);
            return editions;
        }

        private void UpdateApprovedEdition(Edition stagingEdition, EditionEntity approvedEdition)
        {
            approvedEdition.AxEventId = Convert.ToInt32(stagingEdition.EventBEID);
            approvedEdition.DwEventId = stagingEdition.DWEventID;
            approvedEdition.Status = EditionStatusType.Published;

            EditionServices.UpdateEdition(approvedEdition.EditionId, approvedEdition, BusinessServices.Helpers.Constants.AutoIntegrationUserId, true);
        }

        #endregion
    }
}