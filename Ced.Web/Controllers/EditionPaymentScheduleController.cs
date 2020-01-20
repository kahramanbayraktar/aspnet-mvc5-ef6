using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Utility.MVC;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models.EditionPaymentSchedule;
using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EditionPaymentScheduleController : GlobalController
    {
        public EditionPaymentScheduleController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionPaymentScheduleServices editionPaymentScheduleServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, editionPaymentScheduleServices, eventServices, eventDirectorServices,
                logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _AddEditionPaymentSchedule(EditionPaymentScheduleAddModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var existingEditionPaymentSchedule = EditionPaymentScheduleServices.Get(model.EditionId, model.Name);

            if (existingEditionPaymentSchedule != null)
                return Json(
                    new
                    {
                        success = false,
                        message =
                            $"Edition payment schedule already exists: {existingEditionPaymentSchedule.Name}"
                    }, JsonRequestBehavior.AllowGet);

            var editionPaymentSchedule = new EditionPaymentScheduleEntity
            {
                EditionId = edition.EditionId,
                Name = model.Name,
                ActivationDate = model.ActivationDate,
                ExpiryDate = model.ExpiryDate,
                Installment1DueDate = model.Installment1DueDate,
                Installment1Percentage = model.Installment1Percentage,
                Installment2DueDate = model.Installment2DueDate,
                Installment2Percentage = model.Installment2Percentage,
                Installment3DueDate = model.Installment3DueDate,
                Installment3Percentage = model.Installment3Percentage,
                Installment4DueDate = model.Installment4DueDate,
                Installment4Percentage = model.Installment4Percentage,
                Installment5DueDate = model.Installment5DueDate,
                Installment5Percentage = model.Installment5Percentage,
            };
            EditionPaymentScheduleServices.Create(editionPaymentSchedule, CurrentCedUser.CurrentUser.UserId);

            var scopeName = "Edition Payment Schedule";

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance>
            {
                new Variance
                {
                    Prop = scopeName, ValA = null,
                    ValB = editionPaymentSchedule.Name
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
                        $"Edition payment schedule has been added: {editionPaymentSchedule.Name}"
                }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _DeleteEditionPaymentSchedule(int editionPaymentScheduleId)
        {
            var editionPaymentSchedule = EditionPaymentScheduleServices.GetById(editionPaymentScheduleId);
            if (editionPaymentSchedule == null)
                return Json(new { success = false, message = "Edition payment schedule doesn't exist!" }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(editionPaymentSchedule.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var deleted = EditionPaymentScheduleServices.Delete(editionPaymentScheduleId);
            if (!deleted)
                return Json(new { success = false, message = $"Edition payment schedule could not be deleted: {editionPaymentSchedule.Name}" }, JsonRequestBehavior.AllowGet);

            var scopeName = "Edition Payment Schedule";

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = scopeName, ValA = editionPaymentSchedule.Name, ValB = null } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson(scopeName, new List<Variance> { new Variance { Prop = scopeName } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new { success = true, message = $"Edition payment schedule has been deleted: {editionPaymentSchedule.Name}" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _GetEditionPaymentSchedules(int editionId)
        {
            var editionPaymentSchedules = EditionPaymentScheduleServices.GetByEdition(editionId);

            var model = new EditionPaymentScheduleListModel
            {
                EditionId = editionId,
                EditionPaymentSchedules = editionPaymentSchedules.OrderBy(x => x.ActivationDate).ToList()
            };

            return PartialView("_EditionPaymentScheduleList", model);
        }
    }
}