using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Web.Filters;
using Ced.Web.Models.Subscription;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class SubscriptionController : GlobalController
    {
        public SubscriptionController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEventDirectorServices eventDirectorServices,
            IEventServices eventServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionSubscribe)]
        public ActionResult _Subscribe(int editionId)
        {
            var subscription = SubscriptionServices.CreateSubscription(editionId, CurrentCedUser.CurrentUser.Email, CurrentCedUser.CurrentUser.UserId);

            if (!(subscription > 0))
                return Json(new { success = false, message = "Subscription failed." }, JsonRequestBehavior.AllowGet);
            return Json(new { success = true, message = "You have subscribed to this event edition. You will reveive notifications when any update is applied to it." }, JsonRequestBehavior.AllowGet);
        }

        [CedAction(ActionType = ActionType.EditionSubscribe)]
        public ActionResult Unsubscribe(int editionId)
        {
            return Request.IsAjaxRequest() ? _Unsubscribe(editionId) : Unsubscribe_(editionId);
        }

        //[AjaxOnly]
        public ActionResult _Unsubscribe(int editionId)
        {
            var existingSubscription = SubscriptionServices.GetSubscription(editionId, CurrentCedUser.CurrentUser.Email);

            if (existingSubscription == null)
                return JsonResult(false, "You are already unsubscribed.");

            var succeeded = SubscriptionServices.DeleteSubscription(editionId, CurrentCedUser.CurrentUser.Email);

            if (!succeeded)
                return JsonResult(false, "Unsubscription has been failed.");
            return JsonResult(true, "You have unsubscribed from this event edition. You will no longer receive notifications for updates applied to it.");
        }

        public ActionResult Unsubscribe_(int editionId)
        {
            var existingSubscription = SubscriptionServices.GetSubscription(editionId, CurrentCedUser.CurrentUser.Email);

            if (existingSubscription == null)
                return ActionResult(false, "You are already unsubscribed.", editionId);

            var succeeded = SubscriptionServices.DeleteSubscription(editionId, CurrentCedUser.CurrentUser.Email);

            if (!succeeded)
                return ActionResult(false, "Unsubscription has been failed.", editionId);
            return ActionResult(true, "You have unsubscribed from this event edition. You will no longer receive notifications for updates applied to it.", editionId);
        }

        private JsonResult JsonResult(bool success, string message)
        {
            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult ActionResult(bool success, string message, int editionId)
        {
            return View("_Unsubscription", new UnsubscriptionModel { EditionId = editionId, Message = message });
        }
    }
}