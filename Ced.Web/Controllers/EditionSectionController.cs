using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models.EditionSection;
using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ced.Utility.MVC;
using EditionSectionListModel = Ced.Web.Models.EditionSection.EditionSectionListModel;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EditionSectionController : GlobalController
    {
        public EditionSectionController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionSectionServices editionSectionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, editionSectionServices, eventServices, eventDirectorServices,
                logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _AddEditionSection(EditionSectionAddModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var existingEditionSection = EditionSectionServices.Get(model.EditionId, model.Sections);

            if (existingEditionSection != null)
                return Json(
                    new
                    {
                        success = false,
                        message =
                            $"Edition Section already exists: {existingEditionSection.Sections}"
                    }, JsonRequestBehavior.AllowGet);

            var editionSection = new EditionSectionEntity
            {
                EditionId = edition.EditionId,
                Sections = model.Sections
            };
            EditionSectionServices.Create(editionSection, CurrentCedUser.CurrentUser.UserId);

            var scopeName = "Edition Section";

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance>
            {
                new Variance
                {
                    Prop = scopeName, ValA = null,
                    ValB = editionSection.Sections
                }
            };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson(scopeName,
                new List<Variance> {new Variance {Prop = scopeName}});
            UpdateLogInMemory(edition, updatedFields);

            return Json(
                new
                {
                    success = true,
                    message =
                        $"Edition Section has been added: {editionSection.Sections}"
                }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _DeleteEditionSection(int editionSectionId)
        {
            var editionSection = EditionSectionServices.GetById(editionSectionId);
            if (editionSection == null)
                return Json(new { success = false, message = "Edition Section doesn't exist!" }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(editionSection.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var deleted = EditionSectionServices.Delete(editionSectionId);
            if (!deleted)
                return Json(new { success = false, message = $"Edition Section could not be deleted: {editionSection.Sections}" }, JsonRequestBehavior.AllowGet);

            var scopeName = "Edition Section";

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = scopeName, ValA = editionSection.Sections, ValB = null } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson(scopeName, new List<Variance> { new Variance { Prop = scopeName } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new { success = true, message = $"Edition Section has been deleted: {editionSection.Sections}" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _GetEditionSections(int editionId)
        {
            var editionSections = EditionSectionServices.GetByEdition(editionId);

            var model = new EditionSectionListModel
            {
                EditionId = editionId,
                EditionSections = editionSections.OrderBy(x => x.Sections).ToList()
            };

            return PartialView("_EditionSectionList", model);
        }
    }
}