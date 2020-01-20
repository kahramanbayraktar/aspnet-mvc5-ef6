using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility.MVC;
using Ced.Web.Filters;
using Ced.Web.Models.AdminEdition;
using ITE.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class AdminEditionController : GlobalController
    {
        public AdminEditionController(
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
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
        }

        public ActionResult Index()
        {
            var model = new AdminEditionListModel
            {
                Editions = new List<AdminEditionListItemModel>()
            };
            return View(model);
        }

        public ActionResult _Search()
        {
            var model = new AdminEditionSearchModel
            {
                Regions = Regions,
                EditionStatusTypes = EditionStatusType.Approved.ToEnumList().OrderBy(x => x.ToString()).ToList(),
                EventActivities = EventActivity.ShowCancelled.ToEnumList().OrderBy(x => x.ToString()).ToList(),
                EventTypes = EventType.Conference.ToEnumList().OrderBy(x => x.ToString()).ToList()
            };

            return PartialView(model);
        }

        [AjaxOnly]
        [HttpPost]
        public ActionResult _Search(AdminEditionSearchModel searchModel)
        {
            if (
                string.IsNullOrWhiteSpace(searchModel.DirectorEmail) &&
                searchModel.EventId == null &&
                searchModel.EditionStatusTypeIds == null &&
                searchModel.EventActivityIds == null &&
                searchModel.EventTypes == null)
                ModelState.AddModelError("FilterOptions", "At least one filter option must be applied.");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var directorEmail = searchModel.DirectorEmail?.Split('|')[1];

            var editionCount = GetEditionListItemsCount(searchModel.EventId, directorEmail, searchModel.RegionNames, searchModel.CountryCode, searchModel.CityName,
                searchModel.EditionStatusTypeIds, searchModel.EventActivityIds, searchModel.EventTypeIds);

            if (editionCount > 1000)
                return Json(new { success = false, message = $"Query returned more than 1000 ({editionCount}) items. Make a more specific search." });

            var model = GetEditionListItems(searchModel.EventId, directorEmail, searchModel.RegionNames, searchModel.CountryCode, searchModel.CityName,
                searchModel.EditionStatusTypeIds, searchModel.EventActivityIds, searchModel.EventTypeIds);

            var json = Json(new { success = true, data = this.RenderRazorViewToString("_List", model) });
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [AjaxOnly]
        [CedAuthorize(Roles = "Super Admin")]
        public ActionResult _Details(int id)
        {
            var edition = EditionServices.GetEditionById(id);
            var model = Mapper.Map<EditionEntity, AdminEditionDetailsModel>(edition);
            return PartialView(model);
        }

        private int GetEditionListItemsCount(int? eventId, string directorEmail, string[] regions, string country, string city, int[] statusId, int[] eventActivityId, int[] eventTypeId)
        {
            var statusTypes = GetEditionStatusTypes(statusId);
            var eventActivities = GetEventActivities(eventActivityId);
            var eventTypes = GetEventTypes(eventTypeId);
            var editionsCount = EditionServices.GetEditionsCount2(directorEmail, eventId, false, null, statusTypes, eventTypes, eventActivities, null, country, city);
            return editionsCount;
        }

        private IList<AdminEditionListItemModel> GetEditionListItems(int? eventId, string directorEmail, string[] regions, string country, string city, int[] statusId, int[] eventActivityId, int[] eventTypeId)
        {
            var statusTypes = GetEditionStatusTypes(statusId);
            var eventActivities = GetEventActivities(eventActivityId);
            var eventTypes = GetEventTypes(eventTypeId);

            var editions = EditionServices.GetEditions2(directorEmail, eventId, false, null, statusTypes, eventTypes, eventActivities, null, country, city);
            var editionListItems = Mapper.Map<IList<EditionEntityLight>, IList<AdminEditionListItemModel>>(editions);
            return editionListItems;
        }

        private EditionStatusType[] GetEditionStatusTypes(int[] statusId)
        {
            EditionStatusType[] statusTypes = null;
            if (statusId != null)
            {
                statusTypes = new EditionStatusType[statusId.Length];
                for (var i = 0; i < statusId.Length; i++)
                    statusTypes[i] = statusId[i].ToEnum<EditionStatusType>();
            }
            return statusTypes;
        }

        private string[] GetEventActivities(int[] eventActivityId)
        {
            string[] eventActivities = null;
            if (eventActivityId != null)
            {
                eventActivities = new string[eventActivityId.Length];
                for (var i = 0; i < eventActivityId.Length; i++)
                    eventActivities[i] = eventActivityId[i].ToEnum<EventActivity>().GetDescription();
            }
            return eventActivities;
        }

        private string[] GetEventTypes(int[] eventTypeId)
        {
            string[] eventTypes = null;
            if (eventTypeId != null)
            {
                eventTypes = new string[eventTypeId.Length];
                for (var i = 0; i < eventTypeId.Length; i++)
                    eventTypes[i] = eventTypeId[i].ToEnum<EventType>().GetDescription();
            }
            return eventTypes;
        }
    }
}