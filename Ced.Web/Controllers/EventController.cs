using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Event;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Utility.MVC;
using Ced.Utility.Web;
using Ced.Web.Filters;
using Ced.Web.Models.Event;
using Ced.Web.Models.Select2;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Constants = Ced.Utility.Constants;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EventController : GlobalController
    {
        public EventController(
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

        public ActionResult Index(int? count)
        {
            var events = EventServices.GetEventsByDirector(CurrentCedUser.CurrentUser.Email, Constants.ValidEventTypesForCed, WebConfigHelper.MinFinancialYear, count);
            var model = Mapper.Map<List<EventEntity>, List<EventListModel>>(events.ToList());

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var @event = EventServices.GetEventById(id, Constants.ValidEventTypesForCed);
            if (@event == null)
                return View("NotFound");

            if (!IsDirectorAuthorizedOnEvent(@event.EventId))
                return View("Unauthorized");

            var model = Mapper.Map<EventEntity, EventListModel>(@event);

            return View(model);
        }

        [CedAction(Loggable = true, ActionType = ActionType.EventEdit)]
        [HttpPost]
        public ActionResult Edit(EventEditModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            var eventEntity = Mapper.Map<EventEditModel, EventEntity>(model);
            var success = EventServices.UpdateEvent(model.EventId, eventEntity, CurrentCedUser.CurrentUser.UserId);

            if (!success)
                return Json(new { success = false, message = "Update failed!" });

            return Json(new { success = true, message = "You updated the event!" });
        }

        [AjaxOnly]
        public ActionResult _SearchEvents(string searchTerm, int pageSize, int pageNum)
        {
            var events = EventServices.SearchEvents(searchTerm.Trim(), pageSize, pageNum, CurrentCedUser.CurrentUser.Email, Constants.ValidEventTypesForCed, WebConfigHelper.MinFinancialYear);
            var pagedEvents = EventsToSelect2Format(events, pageSize);

            return new JsonResult
            {
                Data = pagedEvents,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [AjaxOnly]
        public ActionResult _SearchEventsAsAdmin(string searchTerm, int pageSize, int pageNum)
        {
            var events = EventServices.SearchEvents(searchTerm.Trim(), pageSize, pageNum, null, null, null);
            var pagedEvents = EventsToSelect2Format(events, pageSize);

            return new JsonResult
            {
                Data = pagedEvents,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static Select2PagedResult EventsToSelect2Format(IEnumerable<EventEntityLight> events, int totalEvents)
        {
            var jsonEvents = new Select2PagedResult
            {
                Results = new List<Select2Result>()
            };

            foreach (var a in events)
            {
                jsonEvents.Results.Add(new Select2Result { id = a.EventId.ToString(), text = a.MasterName });
            }

            jsonEvents.Total = totalEvents;
            return jsonEvents;
        }
    }
}