using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Web.Filters;
using Ced.Web.Models.Log;
using System.Collections.Generic;
using System.Web.Mvc;
using Ced.Utility.MVC;

namespace Ced.Web.Controllers
{
    public class LogController : GlobalController
    {
        public LogController(
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
            IUserRoleServices userRoleServices):
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
        }

        [CedAuthorize(Roles = "Super Admin")]
        public ActionResult Index()
        {
            var model = new LogListModel
            {
                Logs = new List<LogEntityLight>()
            };
            return View(model);
        }

        [AjaxOnly]
        public ActionResult _GetLogs(LogSearchModel searchModel)
        {
            if (string.IsNullOrWhiteSpace(searchModel.UserEmail)
                && searchModel.EventId == null)
                ModelState.AddModelError("FilterOptions", "At least one filter option must be applied.");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var userEmail = searchModel.UserEmail?.Split('|')[1];

            var logs = LogServices.GetLogs(searchModel.EventId, userEmail, searchModel.DayRange);
            return PartialView("_List", logs);
        }

        [AjaxOnly]
        [CedAuthorize(Roles = "Super Admin")]
        public ActionResult _Details(int id)
        {
            var log = LogServices.GetLogById(id);
            var model = Mapper.Map<LogEntity, LogDetailsModel>(log);
            return PartialView(model);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.LogDelete, Loggable = true)]
        public ActionResult _DeleteLog(int id)
        {
            var log = LogServices.GetLogById(id);

            if (log == null)
                return Json(new { success = false, message = "Log not found." }, JsonRequestBehavior.AllowGet);

            var deleted = LogServices.DeleteLog(id);

            if (deleted)
                return Json(new { success = true, message = "Log has been deleted." }, JsonRequestBehavior.AllowGet);
            return Json(new { success = false, message = "Log could not be deleted." }, JsonRequestBehavior.AllowGet);
        }
    }
}