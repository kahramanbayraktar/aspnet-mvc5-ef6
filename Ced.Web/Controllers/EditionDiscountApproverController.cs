using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Utility.MVC;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models.EditionDiscountApprover;
using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EditionDiscountApproverController : GlobalController
    {
        public EditionDiscountApproverController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionDiscountApproverServices editionDiscountApproverServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, editionDiscountApproverServices, eventServices, eventDirectorServices,
                logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _AddEditionDiscountApprover(EditionDiscountApproverAddModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var existingDiscountApprover = EditionDiscountApproverServices.Get(model.EditionId, model.ApprovingUser);

            if (existingDiscountApprover != null)
                return Json(
                    new
                    {
                        success = false,
                        message =
                            $"Discount approver already exists: {existingDiscountApprover.ApprovingUser}"
                    }, JsonRequestBehavior.AllowGet);

            var discountApprover = new EditionDiscountApproverEntity
            {
                EditionId = edition.EditionId,
                ApprovingUser = model.ApprovingUser,
                ApprovalLowerPercentage = model.ApprovalLowerPercentage.GetValueOrDefault(),
                ApprovalUpperPercentage = model.ApprovalUpperPercentage.GetValueOrDefault()
            };
            EditionDiscountApproverServices.Create(discountApprover, CurrentCedUser.CurrentUser.UserId);

            var scopeName = "Edition Discount Approver";

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance>
            {
                new Variance
                {
                    Prop = scopeName, ValA = null,
                    ValB = discountApprover.ApprovingUser
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
                        $"Discount approver has been added: {discountApprover.ApprovingUser}"
                }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _DeleteEditionDiscountApprover(int id)
        {
            var discountApprover = EditionDiscountApproverServices.GetById(id);
            if (discountApprover == null)
                return Json(new { success = false, message = "Discount approver doesn't exist!" }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(discountApprover.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var deleted = EditionDiscountApproverServices.Delete(id);
            if (!deleted)
                return Json(new { success = false, message = $"Discount approver could not be deleted: {discountApprover.ApprovingUser}" }, JsonRequestBehavior.AllowGet);

            var scopeName = "Edition Discount Approver";

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = scopeName, ValA = discountApprover.ApprovingUser, ValB = null } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson(scopeName, new List<Variance> { new Variance { Prop = scopeName } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new { success = true, message = $"Discount approver has been deleted: {discountApprover.ApprovingUser}" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _GetEditionDiscountApprovers(int editionId)
        {
            var discountApprovers = EditionDiscountApproverServices.GetByEdition(editionId);

            var model = new EditionDiscountApproverListModel
            {
                EditionId = editionId,
                EditionDiscountApprovers = discountApprovers.OrderBy(x => x.ApprovingUser).ToList()
            };

            return PartialView("_EditionDiscountApproverList", model);
        }
    }
}