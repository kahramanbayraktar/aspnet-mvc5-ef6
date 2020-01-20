using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility.Edition;
using Ced.Utility.Web;
using ITE.Utility.Extensions;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class UserMailerController : GlobalController
    {
        private readonly IEditionHelper _editionHelper;

        public UserMailerController(
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
            IEditionHelper editionHelper,
            IUserServices userServices,
            IUserRoleServices userRoleServices):
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            _editionHelper = editionHelper;
        }

        public ActionResult GeneralInfoCompleteness(int editionId)
        {
            var edition = EditionServices.GetEditionById(editionId);
            var notificationAttr = NotificationType.PostShowMetricsInfoCompleteness.GetAttribute<NotificationAttribute>();
            var buttonUrl = _editionHelper.GetEditionUrl(edition, notificationAttr.Fragment);

            SendEmailNotification(edition, NotificationType.PostShowMetricsInfoCompleteness, WebConfigHelper.AdminEmails, CurrentCedUser.CurrentUser, "test", buttonUrl);

            return View();
        }

        public ActionResult PostShowMetricsInfoCompleteness2(int editionId)
        {
            var edition = EditionServices.GetEditionById(editionId);
            var notificationAttr = NotificationType.PostShowMetricsInfoCompleteness2.GetAttribute<NotificationAttribute>();
            var buttonUrl = _editionHelper.GetEditionUrl(edition, notificationAttr.Fragment);

            SendEmailNotification(edition, NotificationType.PostShowMetricsInfoCompleteness2, WebConfigHelper.AdminEmails, CurrentCedUser.CurrentUser, "test", buttonUrl);

            return View();
        }

        public ActionResult EditionUpdate(int editionId)
        {
            var edition = EditionServices.GetEditionById(editionId);
            var buttonUrl = _editionHelper.GetEditionUrl(edition);

            SendEmailNotification(edition, NotificationType.EditionUpdated, WebConfigHelper.AdminEmails, CurrentCedUser.CurrentUser, "test", buttonUrl);

            return View();
        }
    }
}