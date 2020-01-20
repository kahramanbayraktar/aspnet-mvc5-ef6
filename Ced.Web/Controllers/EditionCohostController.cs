using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models.Edition;
using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EditionCohostController : GlobalController
    {
        public EditionCohostController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionCohostServices editionCohostServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IFileServices fileServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, editionCohostServices, eventServices, eventDirectorServices, fileServices, logServices,
                notificationServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        public ActionResult _GetCohostEditions(int editionId)
        {
            var cohosts = EditionCoHostServices.GetEditionCohosts(editionId);

            var model = new CohostEditionListModel
            {
                EditionId = editionId,
                EditionCohosts = cohosts
            };

            return PartialView("_CohostEditions", model);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _AddCohostEdition(int editionId, int cohostEditionId)
        {
            var edition = EditionServices.GetEditionById(editionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var cohostEdition = EditionServices.GetEditionById(cohostEditionId);
            if (cohostEdition == null)
                return Json(new { success = false, message = "Co-host edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            if (EditionCoHostServices.EditionCohostExists(editionId, cohostEditionId))
                return Json(new { success = false, message = "Co-host already exists." }, JsonRequestBehavior.AllowGet);

            if (cohostEditionId == editionId)
                return Json(new { success = false, message = "You cannot add the edition itself as its co-host." }, JsonRequestBehavior.AllowGet);

            EditionCoHostServices.CreateEditionCohost(new
                    EditionCohostEntity
                    {
                        FirstEditionId = editionId,
                        SecondEditionId = cohostEditionId
                    }
                , CurrentCedUser.CurrentUser.UserId
            );

            var cohostCount = SetEditionCohostedEventStatus(editionId);
            SetEditionCohostedEventStatus(cohostEditionId);

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = "Co-host", ValA = null, ValB = cohostEdition.EditionName } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson("Co-host", new List<Variance> { new Variance { Prop = "Co-host" } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new { success = true, message = "Co-host has been added.", cohostCount = cohostCount }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _DeleteCohostEdition(int cohostEditionId)
        {
            var cohostEdition = EditionCoHostServices.GetEditionCohostById(cohostEditionId);
            if (cohostEdition == null)
                return Json(new {success = false, message = "Co-host doesn't exist."}, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(cohostEdition.FirstEditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var deleted = EditionCoHostServices.DeleteEditionCohost(cohostEditionId);
            if (!deleted)
                return Json(new { success = false, message = "Co-host could not be removed." }, JsonRequestBehavior.AllowGet);

            var cohostCount = SetEditionCohostedEventStatus(cohostEdition.FirstEditionId);

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = "Co-host", ValA = null, ValB = cohostEdition.FirstEdition.EditionName } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson("Co-host", new List<Variance> { new Variance { Prop = "Co-host" } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new {success = true, message = "Co-host has been removed.", cohostCount = cohostCount }, JsonRequestBehavior.AllowGet);
        }

        #region HELPER METHODS

        private int SetEditionCohostedEventStatus(int editionId)
        {
            var cohosts = EditionCoHostServices.GetEditionCohosts(editionId);
            if (!cohosts.Any() || cohosts.Count == 1)
            {
                var edition = EditionServices.GetEditionById(editionId);
                var currentEdition = (EditionEntity)edition.Clone();

                edition.CohostedEvent = cohosts.Any();
                EditionServices.UpdateEdition(editionId, edition, CurrentCedUser.CurrentUser.UserId);

                UpdateLogInMemory(currentEdition, edition, null, null);
            }

            return cohosts.Count;
        }

        #endregion
    }
}