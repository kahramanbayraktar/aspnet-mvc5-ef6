using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Utility.MVC;
using Ced.Web.Filters;
using Ced.Web.Models.EventDirector;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [Authorize(Roles = "Super Admin")]
    [CedAuthorize]
    public class EventDirectorController : GlobalController
    {
        public EventDirectorController(
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
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
        }

        public ActionResult Index()
        {
            var model = new EventDirectorListModel
            {
                EventDirectors = new List<EventDirectorListItemModel>()
            };
            return View(model);
        }

        public ActionResult _Search()
        {
            var model = new EventDirectorSearchModel
            {
                Applications = Applications
            };

            return PartialView(model);
        }

        [AjaxOnly]
        [HttpPost]
        public ActionResult _Search(EventDirectorSearchModel searchModel)
        {
            if ((searchModel.ApplicationIds == null || !searchModel.ApplicationIds.Any())
                && string.IsNullOrWhiteSpace(searchModel.UserEmail)
                && searchModel.EventId == null)
                ModelState.AddModelError("FilterOptions", "At least one filter option must be applied.");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var userEmail = searchModel.UserEmail?.Split('|')[1];

            var eventDirectorCount = GetEventDirectorListItemsCount(searchModel.EventId, userEmail, searchModel.ApplicationIds, searchModel.IsPrimary, searchModel.IsAssistant);

            var maxAllowedCount = 5000;
            if (eventDirectorCount > maxAllowedCount)
                return Json(new { success = false, message = $"Query returned more than {maxAllowedCount} ({eventDirectorCount}) items. Make a more specific search." });

            var model = GetEventDirectorListItems(searchModel.EventId, userEmail, searchModel.ApplicationIds, searchModel.IsPrimary, searchModel.IsAssistant);
            var json = Json(new { success = true, data = this.RenderRazorViewToString("_List", model) });
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public ActionResult _Add()
        {
            var model = new EventDirectorEditModel
            {
                Applications = Applications,
                IsPrimary = true
            };

            return PartialView(model);
        }

        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EventDirectorAdd, Loggable = true)]
        public ActionResult _Add(EventDirectorEditModel model)
        {
            string title = "None added";

            if (model.EventId == null || model.EventId <= 0)
                return Json(new { success = false, title = title, message = "An Event must be selected." }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrWhiteSpace(model.UserEmail))
                return Json(new { success = false, title = title, message = "A User Email must be selected." }, JsonRequestBehavior.AllowGet);
            if (model.ApplicationId == null || !model.ApplicationId.Any())
                return Json(new { success = false, title = title, message = "An Application must be selected." }, JsonRequestBehavior.AllowGet);

            var userEmail = model.UserEmail?.Split('|')[1];
            var user = UserServices.GetUser(userEmail);

            var addedAppCount = 0;
            var detailedReport = "";

            foreach (var appId in model.ApplicationId)
            {
                var existing = EventDirectorServices.GetEventDirector(model.EventId.Value, user.Email, appId);
                if (existing != null)
                {
                    detailedReport += "Event director already exists. -> EventId: " + model.EventId + " | User: " + user.Email + " | AppId: " + appId + "<br/>";
                }

                if (existing == null)
                {
                    var eventDirector = new EventDirectorEntity
                    {
                        EventId = model.EventId.Value,
                        DirectorEmail = user.Email,
                        ADLogonName = user.AdLogonName,
                        ApplicationId = appId,
                        DirectorFullName = user.FullName,
                        IsPrimary = model.IsPrimary,
                        IsAssistant = model.IsAssistant
                    };

                    var id = EventDirectorServices.CreateEventDirector(eventDirector, CurrentCedUser.CurrentUser.UserId);
                    if (id < 0)
                    {
                        detailedReport += "Event director could not be added. -> EventId: " + model.EventId + " | User: " + user.Email + " | AppId: " + appId + "<br/>";
                    }
                    else
                    {
                        addedAppCount++;
                        detailedReport += "Event director has been added. -> EventId: " + model.EventId + " | User: " + user.Email + " | AppId: " + appId + "<br/>";
                    }
                }
            }

            bool? success;
            if (addedAppCount == model.ApplicationId.Length)
            {
                success = true;
                title = "All Added";
            }
            else if (addedAppCount == 0)
            {
                success = false;
                title = "None Added";
            }
            else
            {
                success = null;
                title = "Partly Added";
            }
            return Json(new { success = success, title = title, message = detailedReport }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Edit(int id)
        {
            var eventDirector = EventDirectorServices.GetEventDirectorById(id);

            if (eventDirector == null)
                return Json(new { success = false, message = "Event Director not found!" }, JsonRequestBehavior.AllowGet);

            eventDirector.ApplicationName = Applications.Single(x => x.ApplicationId == eventDirector.ApplicationId).Name;

            if (eventDirector == null)
                return Json(new { success = false, message = "Event Director not found!" });

            var model = new EventDirectorEditModel
            {
                EventDirectorId = eventDirector.EventDirectorId,
                EventId = eventDirector.EventId,
                EventName = eventDirector.EventName,
                UserEmail = eventDirector.DirectorEmail,
                AppId = eventDirector.ApplicationId,
                AppName = eventDirector.ApplicationName,
                Applications = Applications,
                IsPrimary = eventDirector.IsPrimary.GetValueOrDefault(),
                IsAssistant = eventDirector.IsAssistant.GetValueOrDefault()
            };

            return PartialView(model);
        }

        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EventDirectorEdit, Loggable = true)]
        public ActionResult _Edit(EventDirectorEditModel model)
        {
            var eventDirectorEntity = new EventDirectorEntity
            {
                DirectorEmail= model.UserEmail,
                ApplicationId = model.AppId,
                EventDirectorId = model.EventDirectorId,
                IsPrimary = model.IsPrimary,
                IsAssistant = model.IsAssistant
            };

            var success = EventDirectorServices.UpdateEventDirector(model.EventDirectorId, eventDirectorEntity, CurrentCedUser.CurrentUser.UserId);

            var title = success ? "Saved" : "Not saved";

            return Json(new { success = success, title = title, message = "Event Director has been updated." }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EventDirectorDelete, Loggable = true)]
        public ActionResult _Delete(int id)
        {
            var eventDirector = EventDirectorServices.GetEventDirectorById(id);

            if (eventDirector == null)
                return Json(new { success = false, message = "Event Director not found." }, JsonRequestBehavior.AllowGet);

            var deleted = EventDirectorServices.DeleteEventDirector(id);

            if (deleted)
                return Json(new {success = true, message = "Event Director has been deleted."}, JsonRequestBehavior.AllowGet);
            return Json(new { success = false, message = "Event Director could not be deleted." }, JsonRequestBehavior.AllowGet);
        }

        private int GetEventDirectorListItemsCount(int? eventId, string userEmail, int[] applicationIds, bool? isPrimary, bool? isAssistant)
        {
            var eventDirectorsCount = EventDirectorServices.GetEventDirectorsCount(eventId, userEmail, applicationIds, isPrimary, isAssistant);
            return eventDirectorsCount;
        }

        private IList<EventDirectorListItemModel> GetEventDirectorListItems(int? eventId, string userEmail, int[] applicationIds, bool? isPrimary, bool? isAssistant)
        {
            if (eventId == null && string.IsNullOrWhiteSpace(userEmail))
                return new List<EventDirectorListItemModel>();

            var eventDirectors = EventDirectorServices.GetEventDirectors(eventId, userEmail, applicationIds, isPrimary, isAssistant);
            var eventDirectorListItems = Mapper.Map<IList<EventDirectorEntity>, IList<EventDirectorListItemModel>>(eventDirectors);

            foreach (var item in eventDirectorListItems)
                item.ApplicationName = GetApplicationCode(item.ApplicationId);

            return eventDirectorListItems;
        }

        private string GetApplicationCode(int applicationId)
        {
            var app = Applications.SingleOrDefault(x => x.ApplicationId == applicationId);
            return app?.Code;
        }
    }
}