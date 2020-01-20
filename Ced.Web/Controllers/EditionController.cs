using AutoMapper;
using ITE.AzureStorage;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.BusinessServices.Helpers;
using Ced.Utility;
using Ced.Utility.Azure;
using Ced.Utility.Edition;
using Ced.Utility.MVC;
using Ced.Utility.Web;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models;
using Ced.Web.Models.Edition;
using Ced.Web.Models.File;
using Ced.Web.Models.Select2;
using Ced.Web.Models.UpdateInfo;
using ITE.Logger;
using ITE.Utility.Extensions;
using Newtonsoft.Json.Linq;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICountryServices = Ced.BusinessServices.ICountryServices;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class EditionController : GlobalController
    {
        private readonly IEditionHelper _editionHelper;

        public EditionController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            ICountryServices countryServices,
            IEditionServices editionServices,
            IEditionCohostServices editionCohostServices,
            IEditionCountryServices editionCountryServices,
            IEditionKeyVisitorServices editionKeyVisitorServices,
            IEditionTranslationServices editionTranslationServices,
            IEditionTranslationSocialMediaServices editionTranslationSocialMediaServices,
            IEditionVisitorServices editionVisitorServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IFileServices fileServices,
            IKeyVisitorServices keyVisitorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices,
            IEditionHelper editionHelper) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                countryServices, editionServices, editionCohostServices, editionCountryServices, editionKeyVisitorServices,
                editionTranslationServices, editionTranslationSocialMediaServices, editionVisitorServices,
                eventServices, eventDirectorServices, fileServices, keyVisitorServices, logServices, notificationServices,
                subscriptionServices, userServices, userRoleServices)
        {
            _editionHelper = editionHelper;
        }

        public ViewResult Index(int? eventId = null, string status = null)
        {
            EventEntity @event = null;

            var statusList = new List<EditionStatusType>();
            if (status != null)
                statusList.AddRange(status.Split(',').Select(s => s.ToEnum(EditionStatusType.Published)));

            if (statusList.Contains(EditionStatusType.WaitingForApproval) && !(CurrentCedUser.IsSuperAdmin || CurrentCedUser.IsApprover))
                return View("Unauthorized", new ErrorModel { Message = Utility.Constants.NotAuthorizedToEditionsOfWaitingForApproval });

            if (eventId != null)
            {
                @event = EventServices.GetEventById(eventId.Value, Utility.Constants.ValidEventTypesForCed);
                if (@event == null)
                    return View("NotFound", new ErrorModel { Message = Utility.Constants.NoEventFoundWithThisId });
            }

            var mustBePrimaryDirector = false;

            if (statusList.Contains(EditionStatusType.Draft))
            {
                mustBePrimaryDirector = !CurrentCedUser.IsSuperAdmin;
                statusList = new List<EditionStatusType> { EditionStatusType.PreDraft, EditionStatusType.Draft };
            }
            else if (statusList.Contains(EditionStatusType.WaitingForApproval))
            {
                mustBePrimaryDirector = !(CurrentCedUser.IsSuperAdmin || CurrentCedUser.IsApprover);
                statusList = new List<EditionStatusType> {EditionStatusType.WaitingForApproval};
            }
            else if (statusList.Contains(EditionStatusType.Approved))
            {
                mustBePrimaryDirector = !(CurrentCedUser.IsSuperAdmin || CurrentCedUser.IsApprover);
                statusList = new List<EditionStatusType> { EditionStatusType.Approved };
            }
            else
            {
                statusList = Utility.Constants.ValidEditionStatusesToList;
            }

            var editions = EditionServices.GetEditions(
                CurrentCedUser.CurrentUser.Email,
                eventId,
                mustBePrimaryDirector,
                WebConfigHelper.MinFinancialYear,
                statusList.ToArray(),
                Utility.Constants.ValidEventTypesForCed,
                Utility.Constants.ValidEventActivitiesForCed);

            var isPrimaryDirector =
                @event != null &&
                EventDirectorServices.IsPrimaryDirector(CurrentCedUser.CurrentUser.Email, @event.EventId, WebConfigHelper.ApplicationIdCed);

            var editionsMapped = Mapper.Map<List<EditionEntityLight>, List<EditionListModel>>(
                editions.OrderByDescending(x => x.StartDate).ToList(), opts => opts.Items["CurrentUser"] = CurrentCedUser);

            var model = new EditionIndexModel
            {
                EventId = @event?.EventId,
                EventName = @event?.MasterName,
                IsPrimaryDirector = isPrimaryDirector,
                Editions = editionsMapped,
                CurrentUser = CurrentCedUser
            };

            return View(model);
        }

        [CedAction(ActionType = ActionType.EditionDetails, Loggable = true)]
        public ActionResult Details(int id, string lang)
        {
            // CHECK LANGCODE IN URL
            var corrected = _editionHelper.CorrectLanguageCodeInUrl(ref lang);
            if (corrected)
                return RedirectToAction("Details", new { id, lang });

            var edition = EditionServices.GetEditionById(id);
            if (edition == null)
                return View("NotFound");

            if (!(edition.Status == EditionStatusType.Published || edition.Status == EditionStatusType.Approved))
                return View("NotFound", new ErrorModel { Message = "An event must have already been published or approved to be displayed." });

            var model = GetEditionDetailsModel(edition, lang);

            return View(model);
        }

        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult Edit(int id, string lang)
        {
            // CHECK LANGCODE IN URL
            var corrected = _editionHelper.CorrectLanguageCodeInUrl(ref lang);
            if (corrected)
                return RedirectToAction("Edit", new { id, lang });

            // GET EDITON
            // TODO: Exclude EditionTranslations
            var edition = EditionServices.GetEditionById(id, Utility.Constants.ValidEventTypesForCed);
            if (edition == null)
                return View("NotFound");

            if (!(edition.Status == EditionStatusType.Published || edition.Status == EditionStatusType.Approved))
                return View("NotFound", new ErrorModel { Message = "An event must have been published or approved already to be edited." });

            // CHECK AUTHORIZATION
            if (!IsDirectorAuthorizedOnEvent(edition.EventId))
                return View("Unauthorized");

            // GET EDITIONTRANSLATION
            var editionTranslation = EditionTranslationServices.GetEditionTranslation(edition.EditionId, lang);
            if (editionTranslation == null)
                editionTranslation = new EditionTranslationEntity { LanguageCode = lang };

            if (editionTranslation.LanguageCode != LanguageHelper.GetBaseLanguageCultureName()) // tr/ru etc.
            {
                var baseEditionTranslation = edition.EditionTranslations.Single(x => x.LanguageCode == LanguageHelper.GetBaseLanguageCultureName());
                if (editionTranslation.EditionTranslationId == 0) // New EditionTranslation to be added
                {
                    editionTranslation.BookStandUrl = baseEditionTranslation.BookStandUrl;
                    editionTranslation.OnlineInvitationUrl = baseEditionTranslation.OnlineInvitationUrl;
                }
            }

            var isEditableForImages = edition.ImagesEditable(CurrentCedUser);
            var isUserSubscribed = SubscriptionServices.GetSubscription(edition.EditionId, CurrentCedUser.CurrentUser.Email) != null;

            var editionEditGeneralInfoModel = EditionControllerHelper.ComposeEditionEditGeneralInfoModel(CurrentCedUser, edition, editionTranslation, lang, isEditableForImages);
            var editionEditSalesMetricsModel = EditionControllerHelper.ComposeEditionEditSalesMetricsModel(CurrentCedUser, edition);
            var editionEditExhibitorVisitorStatsModel = ComposeEditionEditExhibitorVisitorStatsModel(edition, lang);
            var editionEditPostShowMetricsModel = ComposeEditionEditPostShowMetricsModel(edition);
            var editionEditFilesModel = ComposeFilesEditModel(edition, editionTranslation);
            var eEditionEditImagesModel = EditionControllerHelper.ComposeEditionEditImagesModel(CurrentCedUser, edition, editionTranslation, isEditableForImages);

            var model = ComposeEditionEditModel(edition, editionTranslation,
                editionEditGeneralInfoModel, editionEditSalesMetricsModel, editionEditExhibitorVisitorStatsModel,
                editionEditPostShowMetricsModel, editionEditFilesModel, eEditionEditImagesModel,
                isEditableForImages, isUserSubscribed);

            ViewBag.Countries = Countries;

            // Log objesi daha önceki bir aşamada (OnActionExecuting olabilir) oluşturulacak.
            // Log.Description alanına WebLogoFileName aktarılacak.
            // OnActionExecuted aşamasında da bu Log objesi kullanılacak.
            var log = TempData["Log"] as LogEntity;
            if (log != null)
            {
                log.AdditionalInfo = "{ 'WebLogoFileName': '" + editionTranslation.WebLogoFileName + "' }";
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var edition = EditionServices.GetEditionById(id);

            if (edition == null)
                return View("NotFound");

            if (edition.Status != EditionStatusType.PreDraft && edition.Status != EditionStatusType.Draft)
                return View("Unauthorized", new ErrorModel { Message = "Only PreDraft or Draft editions can be deleted." });

            var primaryDirector = GetPrimaryDirectors(edition.EventId).FirstOrDefault();

            if (!CurrentCedUser.IsSuperAdmin && (primaryDirector == null || CurrentCedUser.CurrentUser.Email.ToLower() != primaryDirector.DirectorEmail.ToLower()))
                return View("Unauthorized");

            var deleted = EditionServices.DeleteEdition(id);
            if (deleted)
            {
                switch (edition.Status)
                {
                    case EditionStatusType.PreDraft:
                    case EditionStatusType.Draft:
                        return RedirectToAction("Index", new { status = EditionStatusType.Draft.ToString().ToLower() });
                    case EditionStatusType.WaitingForApproval:
                        return RedirectToAction("Index", new { status = EditionStatusType.WaitingForApproval.ToString().ToLower() });
                }
                return RedirectToAction("Index");
            }
            return View("Error", new ErrorModel { Message = "Edition could not be deleted." });
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public JsonResult _SaveGeneralInfo(EditionEditGeneralInfoModel model)
        {
            // Check EventActivity Status (if canceled or not)
            if (model.EventActivity.ToEnumFromDescription<EventActivity>() == EventActivity.ShowCancelled)
                return Json(new { success = false, message = "This event is cancelled!" });

            // Set LanguageCode to the default if not exists
            if (string.IsNullOrWhiteSpace(model.LanguageCode))
                model.LanguageCode = LanguageHelper.GetBaseLanguageCultureName();

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var isEditable = edition.IsEditable(CurrentCedUser);
            if (!isEditable)
                return Json(new { success = false, message = "Edition not editable." });

            var currentEdition = (EditionEntity)edition.Clone();

            Mapper.Map(model, edition);

            EditionTranslationEntity currentEditionTranslation = null;
            var editionTranslation = EditionTranslationServices.GetEditionTranslation(model.EditionId, model.LanguageCode);
            if (editionTranslation == null)
            {
                editionTranslation = new EditionTranslationEntity
                {
                    EditionId = model.EditionId,
                    LanguageCode = model.LanguageCode,
                    VenueName = model.VenueName,
                    MapVenueFullAddress = model.MapVenueFullAddress,
                    Summary = HttpUtility.HtmlDecode(model.Summary),
                    Description = HttpUtility.HtmlDecode(model.Description),
                    ExhibitorProfile = model.ExhibitorProfile,
                    VisitorProfile = model.VisitorProfile,
                    BookStandUrl = model.BookStandUrl,
                    OnlineInvitationUrl = model.OnlineInvitationUrl
                };
            }
            else
            {
                currentEditionTranslation = (EditionTranslationEntity)editionTranslation.Clone();

                Mapper.Map(model, editionTranslation);
            }

            if (model.SocialMedias != null)
            {
                foreach (var socialMedia in model.SocialMedias)
                {
                    socialMedia.AccountName = SocialMediaLinkHelper.GetAccountName(socialMedia.SocialMediaId, socialMedia.AccountName);
                }
            }

            var success = EditionServices.UpdateEditionGeneralInfo(model.EditionId, edition,
                editionTranslation.EditionTranslationId, editionTranslation, model.SocialMedias,
                CurrentCedUser.CurrentUser.UserId);

            if (!success)
                return Json(new { success = false, message = "Update failed!" });

            UpdateLogInMemory(currentEdition, edition, currentEditionTranslation, editionTranslation);

            // UPDATEDCONENT
            var updatedContentEdition = NotificationControllerHelper.GetUpdatedContent(currentEdition, edition);
            var updatedContentEditionTranslation = NotificationControllerHelper.GetUpdatedContent(currentEditionTranslation, editionTranslation);
            var updatedContent = UpdateInfo.CombineUpdateInfos(
                new List<string>
                {
                    updatedContentEdition,
                    updatedContentEditionTranslation
                }, UpdateDisplayType.DetailedHtml);

            PushEditionUpdateNotifications(edition, updatedContent);

            var message = model.EditionTranslationId > 0
                ? "You updated <b>General Info</b>!"
                : $"You created the <b>{model.LanguageCode.ToEnumFromDescription<LanguageHelper.Languages>()}</b> version of the event";
            return Json(new { success = true, message = message });
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _SaveSalesMetrics(EditionEditSalesMetricsModel model)
        {
            if (model.EventActivity.ToEnumFromDescription<EventActivity>() == EventActivity.ShowCancelled)
                return Json(new { success = false, message = "This event is cancelled!" });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var isEditable = edition.IsEditable(CurrentCedUser);
            if (!isEditable)
                return Json(new { success = false, message = "Edition not editable." });

            var currentEdition = (EditionEntity)edition.Clone();

            Mapper.Map(model, edition);

            var success = EditionServices.UpdateEditionSalesMetrics(model.EditionId, edition, CurrentCedUser.CurrentUser.UserId);

            if (!success)
                return Json(new { success = false, message = "Update failed." });

            UpdateLogInMemory(currentEdition, edition, null, null);

            // UPDATEDCONTENT
            var updatedContent = NotificationControllerHelper.GetUpdatedContent(currentEdition, edition);

            PushEditionUpdateNotifications(edition, updatedContent);

            return Json(new { success = true, message = "You updated <b>Sales Metrics</b>!" });
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _SaveExhibitiorVisitorStats(EditionEditExhibitorVisitorStatsModel model)
        {
            if (model.EventActivity.ToEnumFromDescription<EventActivity>() == EventActivity.ShowCancelled)
                return Json(new { success = false, message = "This event is cancelled!" });

            if (string.IsNullOrWhiteSpace(model.LanguageCode))
                model.LanguageCode = LanguageHelper.GetBaseLanguageCultureName();

            // CHECK COHOSTING STATE IS VALID
            if (model.CohostedEvent)
            {
                //if (model.CohostedEventCount < 2)
                //    ModelState.AddModelError("CohostedEventCount", "Co-hosted Event Count must be at least 2 when Co-hosted Event checked.");
                // GET COHOST EVENTS FROM DB
                var cohosts = EditionCoHostServices.GetEditionCohosts(model.EditionId);
                if (cohosts == null || !cohosts.Any())
                    ModelState.AddModelError("Cohosts", "You must select Co-hosted Event(s) when Co-hosted Event checked.");
            }

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            // EDITION
            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var isEditable = edition.IsEditable(CurrentCedUser);
            if (!isEditable)
                return Json(new { success = false, message = "Edition not editable." });

            if (model.TopExhibitorCountries != null)
                model.TopExhibitorCountries = model.TopExhibitorCountries.OrderBy(x => x.ToString()).ToArray();
            if (model.TopVisitorCountries != null)
                model.TopVisitorCountries = model.TopVisitorCountries.OrderBy(x => x.ToString()).ToArray();
            if (model.DelegateCountries != null)
                model.DelegateCountries = model.DelegateCountries.OrderBy(x => x.ToString()).ToArray();

            edition.TopExhibitorCountries = GetEditionCountryCodes(edition, EditionCountryRelationType.Exhibitor).ToCommaSeparatedString();
            edition.TopVisitorCountries = GetEditionCountryCodes(edition, EditionCountryRelationType.Visitor).ToCommaSeparatedString();
            edition.DelegateCountries = GetEditionCountryCodes(edition, EditionCountryRelationType.Delegate).ToCommaSeparatedString();

            var currentEdition = (EditionEntity)edition.Clone();

            Mapper.Map(model, edition);

            // EDITION TRANSLATION
            var editionTranslation = edition.EditionTranslations.SingleOrDefault(x => x.LanguageCode == model.LanguageCode);
            if (editionTranslation == null)
            {
                editionTranslation = new EditionTranslationEntity
                {
                    EditionId = model.EditionId,
                    LanguageCode = model.LanguageCode,
                };
            }
            else
            {
                Mapper.Map(model, editionTranslation);
            }

            var currentEditionTranslation = (EditionTranslationEntity)editionTranslation.Clone();

            // TODO: TransactionScope?
            var success = EditionServices.UpdateEditionExhibitorVisitorStats(model.EditionId, edition, editionTranslation, CurrentCedUser.CurrentUser.UserId);

            if (!success)
                return Json(new { success = false, message = "Update failed." });

            // EDITION VISITOR
            var currentEditionVisitors = EditionVisitorServices.GetEditionVisitors(edition.EditionId);
            var updatedEditionVisitors = ComposeEditionVisitorsFromModel(model);

            // COMPARE EDITIONVISITORS
            //var editionVisitorDiff = GetUpdatedContentForEditionVisitors(currentEditionVisitors, updatedEditionVisitors);

            EditionVisitorServices.CreateOrUpdateEditionVisitors(updatedEditionVisitors, CurrentCedUser.CurrentUser.UserId);

            // DELETE COHOSTS IF NEEDED
            if (!model.CohostedEvent)
                EditionCoHostServices.DeleteAllEditionCohosts(edition.EditionId);

            UpdateLogInMemory(currentEdition, edition, currentEditionTranslation, editionTranslation);

            // UPDATEDCONTENT
            var updatedContentEdition = NotificationControllerHelper.GetUpdatedContent(currentEdition, edition);
            var updatedContentEditionTranslation = NotificationControllerHelper.GetUpdatedContent(currentEditionTranslation, editionTranslation);
            var updatedContentEditionVisitor = NotificationControllerHelper.GetUpdatedContent(currentEditionVisitors, updatedEditionVisitors);
            //var updatedContent = UpdateInfo.CombineUpdateInfos(updatedContentEdition, updatedContentEditionTranslation, UpdateDisplayType.DetailedHtml);
            var updatedContent = UpdateInfo.CombineUpdateInfos(
                new List<string>
                {
                    updatedContentEdition,
                    updatedContentEditionTranslation,
                    updatedContentEditionVisitor
                }, UpdateDisplayType.DetailedHtml);

            PushEditionUpdateNotifications(edition, updatedContent);

            return Json(new { success = true, message = "You updated <b>Exhibitor / Visitor Stats</b>!" });
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _SavePostShowMetrics(EditionEditPostShowMetricsModel model)
        {
            if (model.EventActivity.ToEnumFromDescription<EventActivity>() == EventActivity.ShowCancelled)
                return Json(new { success = false, message = "This event is cancelled!" });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var isEditable = edition.IsEditable(CurrentCedUser);
            if (!isEditable)
                return Json(new { success = false, message = "Edition not editable." });

            var currentEdition = (EditionEntity)edition.Clone();

            Mapper.Map(model, edition);

            var success = EditionServices.UpdateEditionPostShowMetrics(model.EditionId, edition, CurrentCedUser.CurrentUser.UserId);

            if (!success)
                return Json(new { success = false, message = "Update failed." });

            UpdateLogInMemory(currentEdition, edition, null, null);

            // UPDATEDCONTENT
            var updatedContent = NotificationControllerHelper.GetUpdatedContent(currentEdition, edition);
            PushEditionUpdateNotifications(edition, updatedContent);

            return Json(new { success = true, message = "You updated <b>Survey Results</b>!" });
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _SaveImage()
        {
            var editionId = Convert.ToInt32(Request.Params["EditionEditGeneralInfoModel.EditionId"]);
            if (editionId == 0)
                editionId = Convert.ToInt32(Request.Params["EditionId"]);

            if (editionId == 0)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var edition = EditionServices.GetEditionById(editionId);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var isEditable = edition.IsEditable(CurrentCedUser);
            if (!isEditable)
                return Json(new { success = false, message = "Edition not editable." });

            var currentEdition = (EditionEntity) edition.Clone();

            var langCode = Request.Params["EditionEditGeneralInfoModel.LanguageCode"];
            if (string.IsNullOrWhiteSpace(langCode))
                langCode = Request.Params["LanguageCode"];

            var editionTranslation = EditionTranslationServices.GetEditionTranslation(edition.EditionId, langCode);
            if (editionTranslation == null)
                return Json(new { success = false, message = Utility.Constants.EditionTranslationNotFound });

            var currentEditionTranslation = (EditionTranslationEntity)editionTranslation.Clone();

            var imageType = Request.Params["EditionImageType"].ToEnum<EditionImageType>();
            var oldFileName = editionTranslation.GetFileName(imageType);
            string newFileName = null;

            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var imageValidationResult = IsImageValid(file, imageType);
                    if (imageValidationResult != null)
                        return imageValidationResult;

                    var azureStorageService = new Service();

                    // TODO: If an error occurs after deletion event will be left without any image.
                    // DELETE OLD FILE
                    try
                    {
                        azureStorageService.DeleteFile(imageType.BlobFullName(oldFileName));
                    }
                    catch (Exception exc)
                    {
                        var extLog = CreateInternalLog(exc, Utility.Constants.ErrorWhileDeletingFileOnAzureStorage);
                        ExternalLogHelper.Log(extLog, LoggingEventType.Error);
                    }

                    newFileName = EditionServiceHelper.ComposeImageName(edition.EditionId, edition.EditionName, editionTranslation.LanguageCode, Path.GetExtension(file.FileName));

                    //var fileFullPath = azureStorageService.UploadFileAsync("edition/images/" + imageType.GetAttribute<EditionImageTypeAttr>().Key.ToLower() + "/" + newFileName, file.ContentType, file.InputStream);
                    var azureFileUploadResult = azureStorageService.UploadFile("edition/images/" + imageType.GetAttribute<ImageAttribute>().Key.ToLower() + "/" + newFileName, file.ContentType, file.InputStream);

                    if (azureFileUploadResult.Result == OperationResult.Failed)
                    {
                        var intLog = CreateInternalLog(azureFileUploadResult.Message);
                        ExternalLogHelper.Log(intLog, LoggingEventType.Error);

                        return Json(new {success = false, message = azureFileUploadResult.Message});
                    }

                    // UPDATE EDITON & EDITONTRANSLATION
                    editionTranslation.SetFileName(imageType, newFileName);
                    EditionTranslationServices.UpdateEditionTranslation(editionTranslation, CurrentCedUser.CurrentUser.UserId);
                    UpdateEditionUpdateInfo(edition);

                    // UPDATE LOG
                    UpdateLogInMemory(currentEdition, edition, currentEditionTranslation, editionTranslation);

                    // UPDATEDCONTENT
                    var currentEditionTranslationForComparison = (EditionTranslationEntity) currentEditionTranslation.Clone();
                    currentEditionTranslationForComparison.SetFullUrlOfImages();
                    var editionTranslationForComparison = (EditionTranslationEntity) editionTranslation.Clone();
                    editionTranslationForComparison.SetFullUrlOfImages();
                    var updatedContent = NotificationControllerHelper.GetUpdatedContent(currentEditionTranslationForComparison, editionTranslationForComparison);

                    PushEditionUpdateNotifications(edition, updatedContent);
                }
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = Utility.Constants.ErrorWhileSavingFile + (CurrentCedUser.IsSuperAdmin ? " Reason: " + exc.GetFullMessage() : "")
                });
            }

            return Json(new { success = true, fileName = newFileName });
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [CedAction(ActionType = ActionType.EditionEdit, Loggable = true)]
        public ActionResult _DeleteImage(int editionTranslationId, EditionImageType imageType)
        {
            var editionTranslation = EditionTranslationServices.GetEditionTranslationById(editionTranslationId);
            if (editionTranslation == null)
                return Json(new { success = false, message = Utility.Constants.EditionTranslationNotFound });

            var currentEditionTranslation = (EditionTranslationEntity) editionTranslation.Clone();

            var edition = EditionServices.GetEditionById(editionTranslation.EditionId);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var currentEdition = (EditionEntity) edition.Clone();

            var succeeded = true;

            try
            {
                editionTranslation.SetFileName(imageType, string.Empty);

                var success = EditionTranslationServices.UpdateEditionTranslation(editionTranslation, CurrentCedUser.CurrentUser.UserId);

                if (!success)
                    return Json(new {success = false, message = "Error while deleting image"});
            }
            catch (Exception exc)
            {
                succeeded = false;
            }

            if (!succeeded)
                return Json(new {success = false, message = "Error while deleting image"});

            UpdateEditionUpdateInfo(edition);

            // UPDATE LOG
            UpdateLogInMemory(currentEdition, edition, currentEditionTranslation, editionTranslation);

            // UPDATEDCONTENT
            var currentEditionTranslationForComparison = (EditionTranslationEntity)currentEditionTranslation.Clone();
            currentEditionTranslationForComparison.SetFullUrlOfImages();
            var editionTranslationForComparison = (EditionTranslationEntity)editionTranslation.Clone();
            editionTranslationForComparison.SetFullUrlOfImages();
            var updatedContent = NotificationControllerHelper.GetUpdatedContent(currentEditionTranslationForComparison, editionTranslationForComparison);
            PushEditionUpdateNotifications(edition, updatedContent);

            var attr = imageType.GetAttribute<ImageAttribute>();

            return Json(
                new
                {
                    success = true,
                    message = imageType + " deleted.",
                    imgType = imageType,
                    imgId = "img" + attr.Key,
                    defImgPath = imageType.EditionDefaultImageUrl(),
                    delBtnId = "del" + attr.Key
                });
        }

        [CedAction(ActionType = ActionType.EditionPdf, Loggable = true)]
        public ActionResult ExportToPdf(int id, string lang)
        {
            try
            {
                var edition = EditionServices.GetEditionById(id);
                if (edition == null)
                    return View("NotFound");

                var model = GetEditionDetailsModel(edition, lang);

                return new ViewAsPdf("Details", model)
                {
                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    FileName = model.EditionName.ToUrlString() + "-" + model.LanguageCode + ".pdf"
                };
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return null;
            }
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionClone, Loggable = true)]
        public ActionResult _Clone(int id)
        {
            var edition = EditionServices.GetEditionById(id);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            var isClonable = edition.IsClonable(CurrentCedUser);
            if (!isClonable)
                return View("Unauthorized", new ErrorModel { Title = "Not Clonable", Message = $"<b>{edition.EditionName}</b> is not clonable." });

            var isPrimaryDirector = EventDirectorServices.IsPrimaryDirector(CurrentCedUser.CurrentUser.Email, edition.EventId, WebConfigHelper.ApplicationIdCed);
            if (!isPrimaryDirector && !CurrentCedUser.IsSuperAdmin)
                return View("Unauthorized", new ErrorModel { Title = "Not Clonable", Message = $"You don't have privilege to clone <b>{edition.EditionName}</b>." });

            var clonedEditionAlreadyExists = EditionServices.ClonedEditionAlreadyExists(edition.EventId);
            if (clonedEditionAlreadyExists)
            {
                return Json(new
                {
                    success = false,
                    message = $"You cannot clone this edition since another edition of <b>{edition.Event.MasterName}</b> is already cloned and has not been published yet."
                });
            }

            var newEditionId = EditionServices.CloneEdition(id, CurrentCedUser.CurrentUser.UserId);
            if (newEditionId > 0)
            {
                var newEdition = EditionServices.GetEditionById(newEditionId);
                var returnUrl = Url.Action("Draft", new {id = newEditionId, name = newEdition.EditionName.ToUrlString()});
                return Json(new { success = true, message = "You have cloned the edition. Click the button to edit.", returnUrl = returnUrl });
            }

            var message = $"Edition could not be cloned. EditionId: {id}";
            var log = CreateInternalLog(message);
            ExternalLogHelper.Log(log, LoggingEventType.Error);

            return Json(new { success = false, message = "Edition could not be cloned." });
        }

        public ActionResult Draft(int id)
        {
            var edition = EditionServices.GetEditionById(id);
            if (edition == null)
                return View("NotFound");

            var isPrimaryDirector = EventDirectorServices.IsPrimaryDirector(CurrentCedUser.CurrentUser.Email, edition.EventId, WebConfigHelper.ApplicationIdCed);
            var isViewable = false;

            switch (edition.Status)
            {
                case EditionStatusType.PreDraft:
                case EditionStatusType.Draft:
                    isViewable = isPrimaryDirector || CurrentCedUser.IsSuperAdmin;
                    break;
                case EditionStatusType.WaitingForApproval:
                    isViewable = CurrentCedUser.IsApprover || CurrentCedUser.IsSuperAdmin;
                    break;
                case EditionStatusType.Approved:
                    isViewable = isPrimaryDirector || CurrentCedUser.IsSuperAdmin || CurrentCedUser.IsApprover;
                    break;
            }

            if (!isViewable)
                return View("Unauthorized");

            if (edition.Status == EditionStatusType.Published)
                return View("NotFound", new ErrorModel { Message = "An edition must not have been published to be edited on draft page." });

            var model = Mapper.Map<EditionEntity, EditionCloneModel>(edition);

            var baseEditionTranslation = edition.EditionTranslations.Single(x => x.LanguageCode.ToLower() == LanguageHelper.GetBaseLanguageCultureName());
            var primaryDirectors = GetPrimaryDirectors(edition.EventId);

            var cohosts = EditionCoHostServices.GetEditionCohosts(edition.EditionId);

            model.EditionId = baseEditionTranslation.EditionId;
            model.EventId = edition.EventId;
            model.EventName = edition.Event.MasterName;
            model.EditionTranslationId = baseEditionTranslation.EditionTranslationId;
            model.LanguageCode = baseEditionTranslation.LanguageCode;
            model.VenueName = baseEditionTranslation.VenueName;
            model.Industry = edition.Event.Industry;
            model.SubIndustry = edition.Event.SubIndustry;
            model.Frequency = edition.Frequency;
            model.Country = edition.Country;
            model.City = edition.City;
            model.EventType = edition.Event.EventType;
            model.DirectorFullName = primaryDirectors != null && primaryDirectors.Any() ? primaryDirectors.First().DirectorFullName : null;
            model.CurrentUser = CurrentCedUser;
            model.Cohosts = cohosts;
            model.CohostedEvent = edition.CohostedEvent;
            model.CohostedEventCount = edition.CohostedEventCount;
            model.Status = edition.Status;
            model.IsUpdatable = new List<EditionStatusType> { EditionStatusType.Draft, EditionStatusType.PreDraft }.Contains(edition.Status);
            model.IsSendableForApproval = edition.Status == EditionStatusType.Draft;

            return View(model);
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionDraftEdit, Loggable = true)]
        public ActionResult _SaveDraft(EditionCloneModel model)
        {
            UniqueNameValidation(model);

            // CHECK COHOSTING STATE IS VALID
            if (model.CohostedEvent)
            {
                //if (model.CohostedEventCount < 2)
                //    ModelState.AddModelError("CohostedEventCount", "Co-hosted Event Count must be at least 2 when Co-hosted Event checked.");
                // GET COHOST EVENTS FROM DB
                var cohosts = EditionCoHostServices.GetEditionCohosts(model.EditionId);
                if (cohosts == null || !cohosts.Any())
                    ModelState.AddModelError("Cohosts", "You must select Co-hosted Event(s) when Co-hosted Event checked.");
            }

            if ((model.StartDate - model.EndDate).GetValueOrDefault().TotalDays > 0)
                ModelState.AddModelError("Dates", "StartDate cannot be later than EndDate");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            var isPrimaryDirector = EventDirectorServices.IsPrimaryDirector(CurrentCedUser.CurrentUser.Email, model.EventId, WebConfigHelper.ApplicationIdCed);
            if (!isPrimaryDirector && !CurrentCedUser.IsSuperAdmin && !CurrentCedUser.IsApprover)
                return Json(new { success = false, message = "You are not authorized to perform this operation." });

            var edition = Mapper.Map<EditionCloneModel, EditionEntity>(model);

            var editionTranslation = EditionTranslationServices.GetEditionTranslationById(model.EditionTranslationId);
            Mapper.Map(model, editionTranslation);

            if (edition.Status == EditionStatusType.PreDraft)
                edition.Status = EditionStatusType.Draft;
            var updated = EditionServices.UpdateEditionGeneralInfo(model.EditionId, edition, model.EditionTranslationId, editionTranslation, null, CurrentCedUser.CurrentUser.UserId);

            if (updated)
            {
                // UPDATE IMAGE FILE NAMES
                EditionServiceHelper.RenameEditionImages(edition.EditionId, edition.EditionName, editionTranslation);

                EditionTranslationServices.UpdateEditionTranslation(editionTranslation, CurrentCedUser.CurrentUser.UserId);

                var returnUrl = Url.Action("Draft", new { id = edition.EditionId, name = edition.EditionName.ToUrlString() });
                return Json(new {success = true, message = "You have saved the draft edition.", returnUrl = returnUrl });
            }
            return Json(new { success = false, message = "Draft edition could not be saved." });
        }

        [CedDeny(Roles = "Viewer, Super Viewer")]
        [CedAction(ActionType = ActionType.EditionDraftSendForApproval, Loggable = true)]
        [AjaxOnly]
        [HttpPost]
        public ActionResult _SendForApproval(int id)
        {
            var edition = EditionServices.GetEditionById(id);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            if (edition.Status == EditionStatusType.PreDraft)
                return View("NotFound", new ErrorModel { Message = "An event's status must be Draft to be sent for approval." });

            if (edition.Status != EditionStatusType.Draft)
                return View("NotFound", new ErrorModel { Message = "You must save this PreDraft edition to send it for approval." });

            var isPrimaryDirector = EventDirectorServices.IsPrimaryDirector(CurrentCedUser.CurrentUser.Email, edition.EventId, WebConfigHelper.ApplicationIdCed);
            if (!isPrimaryDirector && !CurrentCedUser.IsSuperAdmin)
                return View("Unauthorized");

            edition.Status = EditionStatusType.WaitingForApproval;
            edition.StatusUpdateTime = DateTime.Now;
            EditionServices.UpdateEdition(edition.EditionId, edition, CurrentCedUser.CurrentUser.UserId);

            CreateNotifications(edition, EditionStatusType.WaitingForApproval);

            var message = $"Edition has been sent for approval. EditionId: {edition.EditionId} EditionName: {edition.EditionName}";
            var log = CreateInternalLog(message);
            ExternalLogHelper.Log(log, LoggingEventType.Information);

            if (WebConfigHelper.TrackDraftEditionStatusChange)
            {
                try
                {
                    var approvalModel = GetEditionReview(edition);
                    var htmlTable = ToHTmlTable(approvalModel);
                    var extendedName = _editionHelper.GetNameWithEditionNo(edition);
                    var body = $"{extendedName} is pending your approval with the values below." + "<br/><br/>" + htmlTable;

                    var recipients = WebConfigHelper.TrackDraftEditionStatusChangeUseMockRecipients
                        ? WebConfigHelper.AdminEmails
                        : string.Join(",", Approvers.Select(x => x.Email));

                    var buttonUrl = _editionHelper.GetEditionUrl(edition);

                    var emailResult = SendEmailNotification(edition, NotificationType.DraftEditionWaitingForApproval, recipients, CurrentCedUser.CurrentUser, body, buttonUrl);

                    LogEmail(edition.EditionId, recipients, emailResult.ErrorMessage, null, NotificationType.DraftEditionWaitingForApproval.ToString());
                }
                catch (Exception exc)
                {
                    log = CreateInternalLog(exc);
                    ExternalLogHelper.Log(log, LoggingEventType.Error);
                }
            }

            var returnUrl = Url.Action("Index", "Edition", new { status = EditionStatusType.Draft.ToString().ToLower() });
            return Json(new
            {
                success = true,
                message = @"Thank you for submitting your new event edition setup request. 
                            London HQ Finance teams will now review your request.<br/>
                            A notification email with further instructions will be sent out about the status of your request.",
                returnUrl = returnUrl
            },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Approver")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionDraftApprove, Loggable = true)]
        public ActionResult _Approve(int id)
        {
            var edition = EditionServices.GetEditionById(id);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            if (edition.Status != EditionStatusType.WaitingForApproval)
                return View("Unauthorized", new ErrorModel { Message = "An event's status must be Waiting For Approval to be able to get approved." });

            edition.Status = EditionStatusType.Approved;
            edition.StatusUpdateTime = DateTime.Now;
            EditionServices.UpdateEdition(edition.EditionId, edition, CurrentCedUser.CurrentUser.UserId);

            CreateNotifications(edition, EditionStatusType.Approved);

            var message = $"Edition approved. EditionId: {edition.EditionId} EditionName: {edition.EditionName}";
            var log = CreateInternalLog(message);
            ExternalLogHelper.Log(log, LoggingEventType.Information);

            if (WebConfigHelper.TrackDraftEditionStatusChange)
            {
                try
                {
                    var primaryDirectors = GetPrimaryDirectors(edition.EventId);

                    var body = "Your event application has been approved.<br><br>" +
                               "You will receive another notification from us in the coming days guiding you through necessary steps" +
                               " to publish your event’s information on ITE’s websites and calendars.";

                    var recipients = WebConfigHelper.TrackDraftEditionStatusChangeUseMockRecipients
                        ? WebConfigHelper.AdminEmails
                        : string.Join(",", primaryDirectors.Select(x => x.DirectorEmail));

                    var buttonUrl = _editionHelper.GetEditionUrl(edition);

                    var emailResult = SendEmailNotification(edition, NotificationType.DraftEditionApproved, recipients, CurrentCedUser.CurrentUser, body, buttonUrl);

                    LogEmail(edition.EditionId, recipients, emailResult.ErrorMessage, null, NotificationType.DraftEditionApproved.ToString());
                }
                catch (Exception exc)
                {
                    log = CreateInternalLog(exc);
                    ExternalLogHelper.Log(log, LoggingEventType.Error);
                }
            }

            var approvalModel = GetEditionReview(edition);
            var htmlTable = "<h2 style=\"line-height: 30px\">Approved with the values below.</h2><br/>";
            htmlTable += "<div id=\"divEdition\">" + ToHTmlTableNaked(approvalModel) + "</div>";
            htmlTable += "<br/><button id=\"copyButton\" type=\"button\" class=\"btn btn-orange\">Copy to Clipboard</button>";

            var returnUrl = Url.Action("Index", "Edition", new { status = EditionStatusType.WaitingForApproval.ToString().ToLower() });
            return Json(new
            {
                success = true,
                title = "You have approved the event with the values below",
                message = htmlTable,
                returnUrl = returnUrl
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Approver")]
        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.EditionDraftReject, Loggable = true)]
        public ActionResult _Reject(EditionRejectionModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            var edition = EditionServices.GetEditionById(model.EditionId);
            if (edition == null)
                return Json(new { success = false, message = Utility.Constants.EditionNotFound });

            if (edition.Status != EditionStatusType.WaitingForApproval)
                return View("Unauthorized", new ErrorModel { Message = "An event's status must be Waiting For Approval to be able to get rejected." });

            edition.Status = EditionStatusType.PreDraft;
            edition.StatusUpdateTime = DateTime.Now;
            EditionServices.UpdateEdition(edition.EditionId, edition, CurrentCedUser.CurrentUser.UserId);

            CreateNotifications(edition, EditionStatusType.PreDraft);

            var message = $"Edition rejected. EditionId: {edition.EditionId} EditionName: {edition.EditionName} Reason: {model.Reason}";
            var log = CreateInternalLog(message);
            ExternalLogHelper.Log(log, LoggingEventType.Information);

            if (WebConfigHelper.TrackDraftEditionStatusChange)
            {
                try
                {
                    var extendedName = _editionHelper.GetNameWithEditionNo(edition);
                    var body = $"Your event setup request in regard to {extendedName} has been rejected. Reason for rejection:<br/><br/>";
                    body += $"{model.Reason}";

                    var recipients = WebConfigHelper.TrackDraftEditionStatusChangeUseMockRecipients
                        ? WebConfigHelper.AdminEmails
                        : string.Join(",", Approvers.Select(x => x.Email));

                    var buttonUrl = _editionHelper.GetEditionUrl(edition);

                    var emailResult = SendEmailNotification(edition, NotificationType.DraftEditionRejected, recipients, CurrentCedUser.CurrentUser, body, buttonUrl);

                    LogEmail(edition.EditionId, recipients, emailResult.ErrorMessage, null, NotificationType.DraftEditionRejected.ToString());
                }
                catch (Exception exc)
                {
                    log = CreateInternalLog(exc);
                    ExternalLogHelper.Log(log, LoggingEventType.Error);
                }
            }

            var returnUrl = Url.Action("Index", "Edition", new { status = EditionStatusType.WaitingForApproval.ToString().ToLower() });
            return Json(new
            {
                success = true,
                message = "You have rejected the draft edition.",
                returnUrl = returnUrl
            },
            JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        public ActionResult _SearchEditions(int editionId, string searchTerm, int pageSize, int pageNum)
        {
            var editions = EditionServices.SearchEditionsAsCohost(editionId, searchTerm, pageSize, pageNum);
            var pagedEditions = ToSelect2Format(editions, 15);

            return new JsonResult
            {
                Data = pagedEditions,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [AjaxOnly]
        public ActionResult _SearchVenues(int editionId, string searchTerm, int pageSize, int pageNum)
        {
            var edition = EditionServices.GetEditionById(editionId);
            //var venues = EditionTranslationServices.SearchVenues(eventId, searchTerm, pageSize, pageNum);
            var venues = EditionTranslationServices.SearchVenues(edition, searchTerm, pageSize, pageNum);
            var pagedVenues = ToSelect2Format(venues, 15);

            return new JsonResult
            {
                Data = pagedVenues,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region HELPER METHODS

        private EditionEditModel ComposeEditionEditModel(EditionEntity edition, EditionTranslationEntity editionTranslation,
            EditionEditGeneralInfoModel editionEditGeneralInfoModel, EditionEditSalesMetricsModel editionEditSalesMetricsModel,
            EditionEditExhibitorVisitorStatsModel editionEditExhibitorVisitorStatsModel, EditionEditPostShowMetricsModel editionEditPostShowMetricsModel,
            FilesEditModel editionEditFilesModel, EditionEditImagesModel editionEditImagesModel,
            bool isEditableForImages, bool isUserSubscribed)
        {
            var primaryDirectors = GetPrimaryDirectors(edition.EventId);

            var model = new EditionEditModel
            {
                EditionTranslationId = editionTranslation.EditionTranslationId,
                EditionId = edition.EditionId,
                LanguageCode = editionTranslation.LanguageCode,
                AxEventId = edition.AxEventId,
                EventId = edition.Event.EventId,
                EventMasterName = edition.Event.MasterName,
                DirectorFullName = primaryDirectors != null && primaryDirectors.Any() ? primaryDirectors.First().DirectorFullName : "",
                Classification = edition.Classification,
                Frequency = edition.Frequency,
                EventType = edition.Event.EventTypeCode.ToEnumFromDescription<EventType>(),
                Status = edition.Status,
                UpdateTime = edition.UpdateTime.GetValueOrDefault(),
                UpdateTimeByAutoIntegration = edition.UpdateTimeByAutoIntegration.GetValueOrDefault(),
                IsAlive = edition.IsAlive(),
                IsCancelled = edition.IsCancelled(),
                IsEditableForImages = isEditableForImages,
                IsUserSubscribed = isUserSubscribed,
                EditionEditGeneralInfoModel = editionEditGeneralInfoModel,
                EditionEditSalesMetricsModel = editionEditSalesMetricsModel,
                EditionEditExhibitorVisitorStatsModel = editionEditExhibitorVisitorStatsModel,
                EditionEditPostShowMetricsModel = editionEditPostShowMetricsModel,
                EditionEditFilesModel = editionEditFilesModel,
                EditionEditImagesModel = editionEditImagesModel,
                CountrySelectList = new SelectList(CountryControllerHelper.CountryList, "Value", "Text")
            };
            return model;
        }

        private EditionEditExhibitorVisitorStatsModel ComposeEditionEditExhibitorVisitorStatsModel(EditionEntity edition, string lang)
        {
            string[] delegateCountries = null;
            if (Utility.Constants.ConferenceEventTypes.Contains(edition.Event.EventType))
            {
                delegateCountries = GetEditionCountryCodes(edition, EditionCountryRelationType.Delegate);
            }

            var cohosts = EditionCoHostServices.GetEditionCohosts(edition.EditionId);
            var editionKeyVisitors = EditionKeyVisitorServices.GetEditionKeyVisitors(edition.EditionId);
            var editionVisitors = new EditionVisitorControllerHelper(EditionVisitorServices).GetEditionVisitors(edition);
            var keyVisitors = KeyVisitorServices.GetAllKeyVisitors();

            var model = new EditionEditExhibitorVisitorStatsModel
            {
                LanguageCode = lang,
                EditionId = edition.EditionId,
                EventId = edition.EventId,
                EventType = edition.Event.EventTypeCode.ToEnumFromDescription<EventType>(),
                // EXHIBITOR
                ExhibitorCountryCount = edition.ExhibitorCountryCount,
                LocalExhibitorCount = edition.LocalExhibitorCount,
                InternationalExhibitorCount = edition.InternationalExhibitorCount,
                ExhibitorCount = edition.ExhibitorCount,
                LocalExhibitorRetentionRate = edition.LocalExhibitorRetentionRate,
                InternationalExhibitorRetentionRate = edition.InternationalExhibitorRetentionRate,
                ExhibitorRetentionRate = edition.ExhibitorRetentionRate,
                TopExhibitorCountries = GetEditionCountryCodes(edition, EditionCountryRelationType.Exhibitor),
                // VISITOR
                LocalVisitorCount = edition.LocalVisitorCount,
                InternationalVisitorCount = edition.InternationalVisitorCount,
                LocalRepeatVisitCount = edition.LocalRepeatVisitCount,
                InternationalRepeatVisitCount = edition.InternationalRepeatVisitCount,
                RepeatVisitCount = edition.RepeatVisitCount,
                OnlineRegistrationCount = edition.OnlineRegistrationCount,
                OnlineRegisteredVisitorCount = edition.OnlineRegisteredVisitorCount,
                OnlineRegisteredBuyerVisitorCount = edition.OnlineRegisteredBuyerVisitorCount,
                NationalGroupCount = edition.NationalGroupCount,
                VisitorCountryCount = edition.VisitorCountryCount,
                TopVisitorCountries = GetEditionCountryCodes(edition, EditionCountryRelationType.Visitor),
                EditionVisitors = editionVisitors,
                // DELEGATE
                InternationalDelegateCount = edition.InternationalDelegateCount,
                InternationalPaidDelegateCount = edition.InternationalPaidDelegateCount,
                LocalDelegateCount = edition.LocalDelegateCount,
                LocalPaidDelegateCount = edition.LocalPaidDelegateCount,
                DelegateCountries = delegateCountries,
                EventActivity = edition.EventActivity,
                CohostedEvent = edition.CohostedEvent,
                CohostedEventCount = edition.CohostedEventCount,
                Cohosts = cohosts,
                EditionKeyVisitors = editionKeyVisitors,
                IsAlive = edition.IsAlive(),
                IsCancelled = edition.IsCancelled(),
                KeyVisitors = keyVisitors.OrderBy(x => x.Name).ToList(),
                CurrentUser = CurrentCedUser
            };
            return model;
        }

        private FilesEditModel ComposeFilesEditModel(EditionEntity edition, EditionTranslationEntity editionTranslation)
        {
            var files = FileServices.GetFilesByEntity(edition.EditionId, EntityType.Edition.GetDescription(), null, editionTranslation.LanguageCode).OrderByDescending(x => x.CreatedOn).ToList();

            var model = new FilesEditModel
            {
                EntityId = edition.EditionId,
                EntityType = EntityType.Edition,
                EventType = edition.Event.EventTypeCode.ToEnumFromDescription<EventType>(),
                Files = files,
                LanguageCode = editionTranslation.LanguageCode,
                EventActivity = edition.EventActivity,
                IsAlive = edition.IsAlive(),
                IsCancelled = edition.IsCancelled(),
                CurrentUser = CurrentCedUser
            };
            return model;
        }

        private EditionEditPostShowMetricsModel ComposeEditionEditPostShowMetricsModel(EditionEntity edition)
        {
            var model = new EditionEditPostShowMetricsModel
            {
                EditionId = edition.EditionId,
                EventId = edition.EventId,
                EventType = edition.Event.EventTypeCode.ToEnumFromDescription<EventType>(),
                NPSAverageExhibitor = edition.NPSAverageExhibitor,
                NPSAverageVisitor = edition.NPSAverageVisitor,
                NPSSatisfactionExhibitor = edition.NPSSatisfactionExhibitor,
                NPSSatisfactionVisitor = edition.NPSSatisfactionVisitor,
                NPSScoreExhibitor = edition.NPSScoreExhibitor,
                NPSScoreVisitor = edition.NPSScoreVisitor,
                NetEasyScoreExhibitor = edition.NetEasyScoreExhibitor,
                NetEasyScoreVisitor = edition.NetEasyScoreVisitor,
                EventActivity = edition.EventActivity,
                IsAlive = edition.IsAlive(),
                IsCancelled = edition.IsCancelled(),
                CurrentUser = CurrentCedUser
            };
            return model;
        }

        private string[] GetEditionCountryCodes(EditionEntity edition, EditionCountryRelationType relationType)
        {
            var editionCountries = EditionCountryServices.GetEditionCountriesByEdition(edition.EditionId, relationType);
            return editionCountries.Any() ? editionCountries.Select(x => x.CountryCode).ToArray() : null;
        }

        private IEnumerable<string> GetEditionCountryNames(EditionEntity edition, EditionCountryRelationType relationType)
        {
            var editionCountries = EditionCountryServices.GetEditionCountriesByEdition(edition.EditionId, relationType);

            IEnumerable<string> countryNames = editionCountries.Any()
                ? Countries.Where(c => editionCountries.Any(ec => ec.CountryCode == c.CountryCode))
                    .Select(c => c.CountryName)
                    .ToList()
                : null;

            return countryNames;
        }

        private EditionDetailsModel GetEditionDetailsModel(EditionEntity edition, string lang)
        {
            var editionTranslation = EditionTranslationServices.GetEditionTranslation(edition.EditionId, lang);
            if (editionTranslation == null)
                editionTranslation = new EditionTranslationEntity();
            //var editionTranslationEditModel = Mapper.Map<EditionTranslationEntity, EditionTranslationEditModel>(editionTranslation);

            var exhibitorCountryNames = GetEditionCountryNames(edition, EditionCountryRelationType.Exhibitor);
            var visitorCountryNames = GetEditionCountryNames(edition, EditionCountryRelationType.Visitor);

            var editionVisitors = new EditionVisitorControllerHelper(EditionVisitorServices).GetEditionVisitors(edition);

            IEnumerable<string> delegateCountryNames = null;
            if (Utility.Constants.ConferenceEventTypes.Contains(edition.Event.EventType))
                delegateCountryNames = GetEditionCountryNames(edition, EditionCountryRelationType.Delegate);

            var socialMedias = EditionTranslationSocialMediaServices.GetByEdition(edition.EditionId, editionTranslation.LanguageCode);

            var primaryDirectors = GetPrimaryDirectors(edition.EventId);

            var model = new EditionDetailsModel
            {
                EditionId = edition.EditionId,
                EventId = edition.EventId,
                AxEventId = edition.AxEventId,
                EditionName = edition.EditionName,
                ReportingName = edition.ReportingName,
                InternationalName = edition.InternationalName,
                LocalName = edition.LocalName,
                EventName = edition.Event.MasterName,
                Director = primaryDirectors != null && primaryDirectors.Any() ? primaryDirectors.First().DirectorFullName: null,
                Classification = edition.Classification,
                EventType = edition.Event.EventTypeCode.ToEnumFromDescription<EventType>(),
                LanguageCode = lang,
                Frequency = edition.Frequency,
                Country = edition.Country,
                City = edition.City,

                WebLogoFileName = editionTranslation.WebLogoFileName,
                PeopleImageFileName = editionTranslation.PeopleImageFileName,
                EventFlagPictureFileName = edition.EventFlagPictureFileName,

                EventActivity = edition.EventActivity,
                Status = edition.Status.GetDescription(),

                UpdateTime = edition.UpdateTime.GetValueOrDefault(),
                UpdateTimeByAutoIntegration = edition.UpdateTimeByAutoIntegration.GetValueOrDefault(),

                // GENERAL INFO
                EditionSummary = editionTranslation.Summary,
                EditionDescription = editionTranslation.Description,
                ExhibitorProfile = editionTranslation.ExhibitorProfile,
                VisitorProfile = editionTranslation.VisitorProfile,
                StartDate = edition.StartDate,
                EndDate = edition.EndDate,
                VisitEndTime = edition.VisitEndTime,
                VisitStartTime = edition.VisitStartTime,
                EditionNo = edition.EditionNo,
                DisplayDate = DateHelper.GetDisplayDate(edition.StartDate, edition.EndDate),
                AllDayEvent = edition.AllDayEvent,
                CohostedEvent = edition.CohostedEvent,
                CohostedEventCount = edition.CohostedEventCount,
                Promoted = edition.Promoted,
                VenueName = editionTranslation.VenueName,
                //MapVenueName = editionTranslation.MapVenueName,
                VenueCoordinates = edition.VenueCoordinates,
                ManagingOfficeEmail = edition.ManagingOfficeEmail,
                ManagingOfficePhone = edition.ManagingOfficePhone,
                EventWebsite = edition.EventWebSite,
                MarketoPreferenceCenterLink = edition.MarketoPreferenceCenterLink,
                DisplayOnIteGermany = edition.DisplayOnIteGermany,
                DisplayOnIteAsia = edition.DisplayOnIteAsia,
                DisplayOnIteI = edition.DisplayOnIteI,
                DisplayOnItePoland = edition.DisplayOnItePoland,
                DisplayOnIteModa = edition.DisplayOnIteModa,
                DisplayOnIteTurkey = edition.DisplayOnIteTurkey,
                DisplayOnTradeLink = edition.DisplayOnTradeLink,
                DisplayOnIteUkraine = edition.DisplayOnIteUkraine,
                DisplayOnIteBuildInteriors = edition.DisplayOnIteBuildInteriors,
                DisplayOnIteFoodDrink = edition.DisplayOnIteFoodDrink,
                DisplayOnIteOilGas = edition.DisplayOnIteOilGas,
                DisplayOnIteTravelTourism = edition.DisplayOnIteTravelTourism,
                DisplayOnIteTransportLogistics = edition.DisplayOnIteTransportLogistics,
                DisplayOnIteFashion = edition.DisplayOnIteFashion,
                DisplayOnIteSecurity = edition.DisplayOnIteSecurity,
                DisplayOnIteBeauty = edition.DisplayOnIteBeauty,
                DisplayOnIteHealthCare = edition.DisplayOnIteHealthCare,
                DisplayOnIteMining = edition.DisplayOnIteMining,
                DisplayOnIteEngineeringIndustrial = edition.DisplayOnIteEngineeringIndustrial,

                // SALES METRICS
                LocalSqmSold = edition.LocalSqmSold,
                InternationalSqmSold = edition.InternationalSqmSold,
                SqmSold = edition.SqmSold,
                TotalSqmSold = edition.LocalSqmSold + edition.InternationalSqmSold,
                SponsorCount = edition.SponsorCount,

                // EXHIBITOR / VISITOR
                LocalExhibitorCount = edition.LocalExhibitorCount,
                InternationalExhibitorCount = edition.InternationalExhibitorCount,
                ExhibitorCount = edition.ExhibitorCount,
                ExhibitorCountryCount = edition.ExhibitorCountryCount,
                NationalGroupCount = edition.NationalGroupCount,
                TopExhibitorCountries = exhibitorCountryNames.ToCommaSeparatedString(true),

                LocalVisitorCount = edition.LocalVisitorCount,
                InternationalVisitorCount = edition.InternationalVisitorCount,
                LocalRepeatVisitCount = edition.LocalRepeatVisitCount,
                InternationalRepeatVisitCount = edition.InternationalRepeatVisitCount,
                RepeatVisitCount = edition.RepeatVisitCount,
                VisitorCountryCount = edition.VisitorCountryCount,
                TopVisitorCountries = visitorCountryNames.ToCommaSeparatedString(true),

                EditionVisitors = editionVisitors,

                OnlineRegistrationCount = edition.OnlineRegistrationCount,
                OnlineRegisteredVisitorCount = edition.OnlineRegisteredVisitorCount,
                OnlineRegisteredBuyerVisitorCount = edition.OnlineRegisteredBuyerVisitorCount,

                BookStandUrl = editionTranslation.BookStandUrl,
                OnlineInvitationUrl = editionTranslation.OnlineInvitationUrl,

                LocalDelegateCount = edition.LocalDelegateCount,
                InternationalDelegateCount = edition.InternationalDelegateCount,
                LocalPaidDelegateCount = edition.LocalPaidDelegateCount,
                InternationalPaidDelegateCount = edition.InternationalPaidDelegateCount,
                DelegateCountries = delegateCountryNames.ToCommaSeparatedString(true),

                // POSTSHOW METRICS
                LocalExhibitorRetentionRate = edition.LocalExhibitorRetentionRate,
                InternationalExhibitorRetentionRate = edition.InternationalExhibitorRetentionRate,
                ExhibitorRetentionRate = edition.ExhibitorRetentionRate,
                NPSScoreVisitor = edition.NPSScoreVisitor,
                NPSScoreExhibitor = edition.NPSScoreExhibitor,
                NPSSatisfactionVisitor = edition.NPSSatisfactionVisitor,
                NPSSatisfactionExhibitor = edition.NPSSatisfactionExhibitor,
                //NPSAverageVisitor = edition.NPSAverageVisitor,
                //NPSAverageExhibitor = edition.NPSAverageExhibitor

                SocialMedias = socialMedias
            };

            return model;
        }

        private void UniqueNameValidation(EditionCloneModel model)
        {
            if (!EditionServices.IsEditionNameUnique(model.EditionId, model.EditionName))
                ModelState.AddModelError("EditionName", "Edition Name must be unique.");

            if (!EditionServices.IsInternationalNameUnique(model.EditionId, model.InternationalName))
                ModelState.AddModelError("InternationalName", "International Name must be unique.");

            if (!EditionServices.IsLocalNameUnique(model.EditionId, model.LocalName))
                ModelState.AddModelError("LocalName", "Local Name must be unique.");
        }

        private EditionApprovalModel GetEditionReview(EditionEntity edition)
        {
            var editionCloneModel = Mapper.Map<EditionEntity, EditionCloneModel>(edition);
            var editionTranslation =
                edition.EditionTranslations.Single(
                    x =>
                        x.EditionId == edition.EditionId &&
                        x.LanguageCode == LanguageHelper.GetBaseLanguageCultureName());

            var editionApprovalModel = Mapper.Map<EditionCloneModel, EditionApprovalModel>(editionCloneModel);
            editionApprovalModel.VenueName = editionTranslation.VenueName;
            editionApprovalModel.MapVenueFullAddress = editionTranslation.MapVenueFullAddress;

            editionApprovalModel.MasterName = edition.Event.MasterName;
            editionApprovalModel.MasterCode = edition.Event.MasterCode;

            var lastEdition = EditionServices.GetLastEdition(edition.EventId, Utility.Constants.DefaultValidEditionStatusesForCed, Utility.Constants.ValidEventTypesForCed);
            editionApprovalModel.PreviousEditionName = lastEdition.EditionName;
            editionApprovalModel.PreviousEditionAxEventId = lastEdition.AxEventId;

            editionApprovalModel.DirectorFullName = _editionHelper.GetEventDirectorFullName(edition);

            return editionApprovalModel;
        }

        private static string ToHTmlTable(object obj)
        {
            var table = "<table cellpadding='5' cellspacing='5'>";
            var json = JObject.FromObject(obj);
            foreach (var item in json)
            {
                table += "<tr>";
                table += "<td class='font-lato' style='font-size: 14px; color: #888794; text-align: left'>" + item.Key + "</td>";
                table += "<td class='font-lato' style='font-size: 14px; color: #888794'>" + ":" + "</td>";
                table += "<td class='font-lato' style='font-size: 14px; color: #888794; text-align: left'>" + item.Value + "</td>";
                table += "</tr>";
            }
            table += "</table>";
            return table;
        }

        private static string ToHTmlTableNaked(object obj)
        {
            var table = "<table>";
            var json = JObject.FromObject(obj);
            foreach (var item in json)
            {
                table += "<tr>";
                table += "<td style='text-align: left; padding-right: 5px'>" + item.Key + "</td>";
                table += "<td style='padding-right: 5px'>" + ":" + "</td>";
                table += "<td style='text-align: left; padding-right: 5px'>" + item.Value + "</td>";
                table += "</tr>";
            }
            table += "</table>";
            return table;
        }

        private string GetEmailButtonActionName(EditionEntity edition)
        {
            var action = "Edit";
            switch (edition.Status)
            {
                case EditionStatusType.Draft:
                case EditionStatusType.PreDraft:
                case EditionStatusType.WaitingForApproval:
                case EditionStatusType.Approved:
                    action = "Draft";
                    break;
                case EditionStatusType.Published:
                    action = "Edit";
                    break;
            }
            return action;
        }

        private string GetEmailTitle(EditionEntity edition, NotificationType notificationType)
        {
            return string.Format(notificationType.GetDescription(), _editionHelper.GetNameWithEditionNo(edition));
        }

        private string GetEmailButtonUrl(EditionEntity edition)
        {
            var action = GetEmailButtonActionName(edition);
            return Url.AbsoluteAction(action, "Edition", new { id = edition.EditionId, name = edition.EditionName.ToUrlString() });
        }

        private string[] GetNotificationRecipients(EditionEntity edition, EditionStatusType editionStatus)
        {
            string[] recipients;
            if (editionStatus == EditionStatusType.WaitingForApproval)
            {
                recipients = Approvers.Select(x => x.Email).ToArray();
            }
            else
            {
                recipients = GetPrimaryDirectors(edition.EventId)
                    .Select(x => x.DirectorEmail)
                    .ToArray();
            }
            return recipients;
        }
        private NotificationType? GetNotificationType(EditionStatusType editionStatus)
        {
            switch (editionStatus)
            {
                case EditionStatusType.Approved:
                    return NotificationType.DraftEditionApproved;
                case EditionStatusType.WaitingForApproval:
                    return NotificationType.DraftEditionWaitingForApproval;
                case EditionStatusType.PreDraft:
                    return NotificationType.DraftEditionRejected;
            }
            return null;
        }

        private void CreateNotifications(EditionEntity edition, EditionStatusType editionStatus)
        {
            var notificationType = GetNotificationType(editionStatus);
            var title = GetEmailTitle(edition, notificationType.GetValueOrDefault());
            var url = GetEmailButtonUrl(edition);
            var recipients = GetNotificationRecipients(edition, editionStatus);

            var notifications = new List<NotificationEntity>();
            foreach (var recipient in recipients)
            {
                notifications.Add(new NotificationEntity
                {
                    Title = title,
                    Url = url,
                    NotificationType = notificationType.GetValueOrDefault(),
                    EventId = edition.EventId,
                    EditionId = edition.EditionId,
                    ReceiverEmail = recipient
                });
            }
            NotificationServices.CreateNotifications(notifications, CurrentCedUser.CurrentUser.UserId);
        }

        private static Select2PagedResult ToSelect2Format(IEnumerable<EditionEntityLight> editions, int totalEvents)
        {
            var jsonEditions = new Select2PagedResult
            {
                Results = new List<Select2Result>()
            };

            foreach (var e in editions)
            {
                jsonEditions.Results.Add(new Select2Result { id = e.EditionId.ToString(), text = e.EditionName });
            }

            jsonEditions.Total = totalEvents;
            return jsonEditions;
        }

        private static Select2PagedResult ToSelect2Format(IEnumerable<EditionVenue> venues, int total)
        {
            var jsonEditions = new Select2PagedResult
            {
                Results = new List<Select2Result>()
            };

            foreach (var v in venues)
            {
                jsonEditions.Results.Add(new Select2Result { id = v.VenueName, text = v.VenueName });
            }

            jsonEditions.Total = total;
            return jsonEditions;
        }

        private JsonResult IsImageValid(HttpPostedFileBase file, EditionImageType imageType)
        {
            // CHECK EXTENSION
            var extension = Path.GetExtension(file.FileName);
            var allowedExtensions = imageType.GetAttribute<ImageAttribute>().AllowedExtensions;
            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = Utility.Constants.InvalidFileExtension });
            }

            // CHECK LENGTH
            var minMaxLengths = imageType.GetAttribute<ImageAttribute>().MinMaxLengths;
            if (file.ContentLength < minMaxLengths[0] || file.ContentLength > minMaxLengths[1] * 1024)
            {
                return Json(new { success = false, message = Utility.Constants.InvalidFileSize });
            }

            // CHECK SIZES (WIDTH AND HEIGHT)
            var allowedWidth = imageType.GetAttribute<ImageAttribute>().Width;
            var allowedHeight = imageType.GetAttribute<ImageAttribute>().Height;
            if (allowedWidth > 0 && allowedHeight > 0)
            {
                var imgFile = System.Drawing.Image.FromStream(file.InputStream);

                if (imgFile.PhysicalDimension.Width > allowedWidth ||
                    imgFile.PhysicalDimension.Width < allowedWidth
                    || imgFile.PhysicalDimension.Height > allowedHeight ||
                    imgFile.PhysicalDimension.Height < allowedHeight)
                {
                    return Json(new { success = false, message = Utility.Constants.InvalidFileDimension });
                }
            }
            return null;
        }

        private static IList<EditionVisitorEntity> ComposeEditionVisitorsFromModel(EditionEditExhibitorVisitorStatsModel model)
        {
            var editionVisitorEntities = new List<EditionVisitorEntity>();
            for (var i = 0; i < model.DailyVisitorCounts.Length; i++)
            {
                editionVisitorEntities.Add(new EditionVisitorEntity
                {
                    EditionId = model.EditionId,
                    DayNumber = (byte)(i + 1),
                    VisitorCount = (short)model.DailyVisitorCounts[i],
                    RepeatVisitCount = i > 0 ? (short?)model.DailyRepeatVisits[i - 1] : null,
                    OldVisitorCount = (short)model.DailyOldVisitorCounts[i],
                    NewVisitorCount = (short)model.DailyNewVisitorCounts[i]
                }
                );
            }
            return editionVisitorEntities;
        }

        #endregion
    }
}