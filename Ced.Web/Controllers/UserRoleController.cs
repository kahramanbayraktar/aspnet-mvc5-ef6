using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Auth;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Utility.MVC;
using Ced.Web.Filters;
using Ced.Web.Models.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize(Roles = "Super Admin")]
    public class UserRoleController : GlobalController
    {
        public UserRoleController(
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
            var model = new UserRoleListModel
            {
                UserRoles = new List<UserRoleListItemModel>()
            };
            return View(model);
        }

        public ActionResult _Search()
        {
            var model = new UserRoleSearchModel
            {
                Applications = Applications,
                Roles = Roles,
                Regions = Regions,
                Countries = Countries,
                Industries = Industries
            };

            return PartialView(model);
        }

        [AjaxOnly]
        [HttpPost]
        public ActionResult _Search(UserRoleSearchModel searchModel)
        {
            if ((searchModel.ApplicationIds == null || !searchModel.ApplicationIds.Any())
                && string.IsNullOrWhiteSpace(searchModel.UserEmail)
                && (searchModel.RoleIds == null || !searchModel.RoleIds.Any())
                && (searchModel.RegionIds == null || !searchModel.RegionIds.Any())
                && (searchModel.IndustryIds == null || !searchModel.IndustryIds.Any()))
                ModelState.AddModelError("FilterOptions", "At least one filter option must be applied.");

            // If more than one apps are selected then roles will be ignored in search.
            if (searchModel.ApplicationIds != null && searchModel.ApplicationIds.Length > 1)
                searchModel.RoleIds = null;

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var model = GetUserRoleListItems(searchModel);
            return Json(new { success = true, data = this.RenderRazorViewToString("_List", model) });
        }

        public ActionResult _Add()
        {
            var model = new UserRoleEditModel
            {
                Applications = Applications,
                Roles = Roles,
                Regions = Regions,
                Countries = Countries,
                Industries = Industries
            };

            return PartialView(model);
        }

        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.UserRoleAdd, Loggable = true)]
        public ActionResult _Add(UserRoleEditModel model)
        {
            switch (model.RoleId)
            {
                case 4: // Industry Director
                case 12:
                    model.RegionId = null;
                    model.CountryId = null;
                    break;
                case 5: // Region Director
                case 13:
                    model.IndustryId = null;
                    model.CountryId = null;
                    break;
                case 6: // Country Director
                case 14:
                    model.IndustryId = null;
                    model.RegionId = null;
                    break;
                default:
                    model.IndustryId = null;
                    model.RegionId = null;
                    model.CountryId = null;
                    break;
            }

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            var userRole = Mapper.Map<UserRoleEditModel, UserRoleEntity>(model);

            var userId = model.UserEmail.Split('|')[0];
            var userEmail = model.UserEmail.Split('|')[1];

            userRole.UserId = Convert.ToInt32(userId);
            userRole.UserEmail = userEmail;

            var added = UserRoleServices.CreateUserRole(userRole);
            switch (added)
            {
                case -1:
                    return Json(new { success = false, title = "Error!", message = "User role could not added!" });
                case 0:
                    return Json(new { success = false, title = "Already exists!", message = "User role could not added. It already exists." });
                default: // 1:
                    return Json(new { success = true, message = "User role added!" });
            }
        }

        [AjaxOnly]
        public ActionResult _Delete(int id)
        {
            var userRole = UserRoleServices.Get(id);
            if (userRole == null)
                return Json(new { success = false, message = "User role could not found!" }, JsonRequestBehavior.AllowGet);

            var deleted = UserRoleServices.DeleteUserRole(id);
            if (!deleted)
                return Json(new { success = false, message = "User role could not deleted!" }, JsonRequestBehavior.AllowGet);
            return Json(new { success = true, message = "Deleted!" }, JsonRequestBehavior.AllowGet);
        }

        #region HELPER METHODS

        private IList<UserRoleListItemModel> GetUserRoleListItems(UserRoleSearchModel model)
        {
            var userEmail = model.UserEmail?.Split('|')[1];

            var userRoles = UserRoleServices.Search(model.ApplicationIds, userEmail, model.RoleIds, model.RegionIds, model.CountryIds, model.IndustryIds);
            var userRoleListItems = Mapper.Map<IList<UserRoleEntity>, IList<UserRoleListItemModel>>(userRoles);

            return userRoleListItems;
        }

        #endregion
    }
}