using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models.Edition;
using ITE.Utility.Extensions;
using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EditionSocialMediaController : GlobalController
    {
        public EditionSocialMediaController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionTranslationSocialMediaServices editionTranslationSocialMediaServices,
            IEditionTranslationServices editionTranslationServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISocialMediaServices socialMediaServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, editionTranslationSocialMediaServices, editionTranslationServices, eventServices, eventDirectorServices,
                logServices, notificationServices, socialMediaServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _AddSocialMedia(int editionTranslationId, string socialMediaId, string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
                return Json(new { success = false, message = "Account name cannot be empty!" }, JsonRequestBehavior.AllowGet);

            var editionTranslation = EditionTranslationServices.GetEditionTranslationById(editionTranslationId);
            if (editionTranslation == null)
                return Json(new { success = false, message = "Edition translation must be saved for this action." }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(editionTranslation.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var existingSocialMedia = SocialMediaServices.GetSocialMediaById(socialMediaId);

            if (existingSocialMedia == null)
                socialMediaId = SocialMediaServices.CreateSocialMedia(socialMediaId);

            if (!string.IsNullOrWhiteSpace(socialMediaId))
            {
                var existingEditionSocialMedia = EditionTranslationSocialMediaServices.Get(editionTranslationId, socialMediaId);

                if (existingEditionSocialMedia != null)
                    return Json(new { success = false, message = $"<i class='fa fa-{existingEditionSocialMedia.SocialMediaId}'></i> {existingEditionSocialMedia.SocialMediaId.ToEnumFromDescription<SocialMediaType>()} account already exists!" }, JsonRequestBehavior.AllowGet);

                var editionSocialMedia = new EditionTranslationSocialMediaEntity
                {
                    EditionId = editionTranslation.EditionId,
                    EditionTranslationId = editionTranslation.EditionTranslationId,
                    SocialMediaId = socialMediaId,
                    AccountName = accountName
                };
                EditionTranslationSocialMediaServices.Create(editionSocialMedia, CurrentCedUser.CurrentUser.UserId);


                var scopeName = "Social Media Account";

                // UPDATE EDITION
                UpdateEditionUpdateInfo(edition);

                // DIFF
                var diff = new List<Variance> { new Variance { Prop = scopeName, ValA = null, ValB = editionSocialMedia.SocialMediaId + "/" + editionSocialMedia.AccountName } };

                OnEditionUpdated(edition, diff);

                // UPDATE LOG
                var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson(scopeName, new List<Variance> { new Variance { Prop = scopeName } });
                UpdateLogInMemory(edition, updatedFields);

                return Json(new { success = true, message = $"<i class='fa fa-{editionSocialMedia.SocialMediaId}'></i> {editionSocialMedia.SocialMediaId.ToEnumFromDescription<SocialMediaType>()} account has been added." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "<i class='fa fa-{editionSocialMedia.SocialMediaId}'></i> {editionSocialMedia.SocialMediaId.ToEnum<SocialMediaType>()} account could not be added!" }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _DeleteSocialMedia(int editionSocialMediaId)
        {
            var editionSocialMedia = EditionTranslationSocialMediaServices.GetById(editionSocialMediaId);
            if (editionSocialMedia == null)
                return Json(new { success = false, message = "Social media account doesn't exist!" }, JsonRequestBehavior.AllowGet);

            var edition = EditionServices.GetEditionById(editionSocialMedia.EditionId);
            if (edition == null)
                return Json(new { success = false, message = "Edition doesn't exist." }, JsonRequestBehavior.AllowGet);

            var deleted = EditionTranslationSocialMediaServices.Delete(editionSocialMediaId);
            if (!deleted)
                return Json(new { success = false, message = $"<i class='fa fa-{editionSocialMedia.SocialMediaId}'></i> {editionSocialMedia.SocialMediaId.ToEnumFromDescription<SocialMediaType>()} account could not be deleted." }, JsonRequestBehavior.AllowGet);

            var scopeName = "Social Media Account";

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // DIFF
            var diff = new List<Variance> { new Variance { Prop = scopeName, ValA = editionSocialMedia.SocialMediaId + "/" + editionSocialMedia.AccountName, ValB = null } };

            OnEditionUpdated(edition, diff);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson(scopeName, new List<Variance> { new Variance { Prop = scopeName } });
            UpdateLogInMemory(edition, updatedFields);

            return Json(new { success = true, message = $"<i class='fa fa-{editionSocialMedia.SocialMediaId}'></i> {editionSocialMedia.SocialMediaId.ToEnumFromDescription<SocialMediaType>()} account has been deleted." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _GetSocialMedias(int editionTranslationId)
        {
            var editionTranslation = EditionTranslationServices.GetEditionTranslationById(editionTranslationId);

            if (editionTranslation == null)
                return PartialView("_EditionSocialMedias", new EditionSocialMediaListModel());

            var socialMedias = EditionTranslationSocialMediaServices.GetByEdition(editionTranslation.EditionId, editionTranslation.LanguageCode);

            var model = new EditionSocialMediaListModel
            {
                EditionTranslationId = editionTranslation.EditionTranslationId,
                EditionId = editionTranslation.EditionId,
                SocialMedias = socialMedias.OrderBy(x => x.SocialMediaId).ToList()
            };

            ViewBag.SocialMediaTypes = SocialMediaServices.GetAllSocialMedias().OrderBy(x => x.SocialMediaId);

            return PartialView("_EditionSocialMedias", model);
        }
    }
}