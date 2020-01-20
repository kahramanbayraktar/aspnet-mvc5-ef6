using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility.MVC;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models.Notification;
using ITE.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class NotificationController : GlobalController
    {
        private readonly IInAppNotificationHelper _inAppNotificationHelper;

        public Func<LatestNotificationsModel, string> ViewEngineResultFunc { get; set; }

        public NotificationController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices,
            IInAppNotificationHelper inAppNotificationHelper) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            _inAppNotificationHelper = inAppNotificationHelper;

            ViewEngineResultFunc = RenderView;
        }

        public ViewResult Index()
        {
            NotificationServices.DisableNotifications(CurrentCedUser.CurrentUser.Email);

            var notifications = GetNotificationListItems(null, 1);

            var model = new NotificationListModel
            {
                NotificationTypes = GetAllNotificationTypes(),
                DayRange = 1,
                Notifications = notifications
            };
            return View(model);
        }

        [AjaxOnly]
        [HttpPost]
        public PartialViewResult _SearchNotifications(NotificationType[] notificationType, int? dayRange)
        {
            var model = GetNotificationListItems(notificationType, dayRange);
            return PartialView("_List", model);
        }

        [AjaxOnly]
        [HttpPost]
        public JsonResult _ReadAllNotifications()
        {
            NotificationServices.DisableNotifications(CurrentCedUser.CurrentUser.Email);
            return Json(new {success = true});
        }

        [AjaxOnly]
        [HttpPost]
        public JsonResult _GetNotifications()
        {
            var latestNotificationEntities = NotificationServices.GetNotificationsByRecipient(CurrentCedUser.CurrentUser.Email, 5);

            var latestNotificationsModel = _inAppNotificationHelper.GetNotificationViewModelItems(latestNotificationEntities);

            var unreadNotifCount = NotificationServices.GetNotificationCount(CurrentCedUser.CurrentUser.Email, false);

            var model = new LatestNotificationsModel
            {
                Notifications = latestNotificationsModel,
                Count = unreadNotifCount
            };

            var viewString = ViewEngineResultFunc.Invoke(model);

            return Json(new { data = viewString, count = unreadNotifCount });
        }

        public IList<NotificationListItemModel> GetNotificationListItems(NotificationType[] notificationType, int? dayRange)
        {
            var notifications = NotificationServices.GetNotifications(CurrentCedUser.CurrentUser.Email, notificationType, dayRange);

            if (notifications != null)
            {
                foreach (var notif in notifications)
                    notif.Description = _inAppNotificationHelper.GetDescription(notif);
            }

            var listItems = Mapper.Map<IList<NotificationEntity>, IList<NotificationListItemModel>>(notifications);
            return listItems;
        }

        private IList<NotificationType> GetAllNotificationTypes()
        {
            return EnumExtensions.GetValues<NotificationType>().ToList();
        }

        private string RenderView(LatestNotificationsModel model)
        {
            return this.RenderRazorViewToString("_Notifications", model);
        }
    }
}