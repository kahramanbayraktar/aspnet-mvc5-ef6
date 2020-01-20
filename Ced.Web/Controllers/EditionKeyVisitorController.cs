using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models.Edition;
using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EditionKeyVisitorController : GlobalController
    {
        public EditionKeyVisitorController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionKeyVisitorServices editionKeyVisitorServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IKeyVisitorServices keyVisitorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, editionKeyVisitorServices, eventServices, eventDirectorServices, keyVisitorServices,
                logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
        }

        public ActionResult _GetEditionKeyVisitors(int editionId)
        {
            var editionKeyVisitors = EditionKeyVisitorServices.GetEditionKeyVisitors(editionId);

            var model = new EditionKeyVisitorListModel
            {
                EditionId = editionId,
                EditionKeyVisitors = editionKeyVisitors
            };

            return PartialView("_EditionKeyVisitors", model);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _AddEditionKeyVisitor(int editionId, int keyVisitorId, string value)
        {
            var edition = EditionServices.GetEditionById(editionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            //var keyVisitor = KeyVisitorServices.GetKeyVisitorById(keyVisitorId);
            //if (keyVisitor == null)
            //    return Json(new { success = false, message = "KeyVisitor doesn't exist." }, JsonRequestBehavior.AllowGet);

            var editionKeyVisitorId = EditionKeyVisitorServices.CreateEditionKeyVisitor(new
                    EditionKeyVisitorEntity
                    {
                        EditionId = editionId,
                        EventBEID = edition.AxEventId,
                        KeyVisitorId = keyVisitorId,
                        Value = value.Trim()
                    },
                CurrentCedUser.CurrentUser.UserId
            );

            // TODO: KeyVisitor entity is not loaded into EditionKeyVisitor entity.
            var editionKeyVisitor = EditionKeyVisitorServices.GetEditionKeyVisitorById(editionKeyVisitorId);
            var keyVisitor = KeyVisitorServices.GetKeyVisitorById(editionKeyVisitor.KeyVisitorId);

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = "KeyVisitor", ValA = null, ValB = keyVisitor.Name + ": " + editionKeyVisitor.Value } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson("KeyVisitor", new List<Variance> { new Variance { Prop = "KeyVisitor" } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new { success = true, message = "KeyVisitor has been added." /*, editionKeyVisitorCount = editionKeyVisitorCount*/ }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _DeleteEditionKeyVisitor(int editionKeyVisitorId)
        {
            var editionKeyVisitor = EditionKeyVisitorServices.GetEditionKeyVisitorById(editionKeyVisitorId);
            if (editionKeyVisitor == null)
                return Json(new {success = false, message = "KeyVisitor doesn't exist." }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(editionKeyVisitor.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var deleted = EditionKeyVisitorServices.DeleteEditionKeyVisitor(editionKeyVisitorId);
            if (!deleted)
                return Json(new { success = false, message = "KeyVisitor could not be removed." }, JsonRequestBehavior.AllowGet);

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = "KeyVisitor", ValA = editionKeyVisitor.KeyVisitor.Name + ": " + editionKeyVisitor.Value, ValB = null } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson("KeyVisitor", new List<Variance> { new Variance { Prop = "KeyVisitor" } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new {success = true, message = "KeyVisitor has been removed." /*, editionKeyVisitorCount = editionKeyVisitorCount*/ }, JsonRequestBehavior.AllowGet);
        }
    }
}