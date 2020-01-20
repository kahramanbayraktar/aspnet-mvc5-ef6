using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Auth;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.BusinessServices.Helpers;
using Ced.Data.UnitOfWork;
using Ced.Utility;
using Ced.Utility.Azure;
using Ced.Utility.Edition;
using Ced.Utility.Email;
using Ced.Utility.MVC;
using Ced.Utility.Notification;
using Ced.Utility.Web;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Hubs;
using Ced.Web.Models;
using Ced.Web.Models.UpdateInfo;
using Ced.Web.Models.User;
using ITE.Logger;
using ITE.Utility.Extensions;
using ITE.Utility.ObjectComparison;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class GlobalController : Controller
    {
        private IList<ApplicationEntity> _applications;
        protected IList<ApplicationEntity> Applications => _applications ?? (_applications = ApplicationServices.GetAllApplications());

        private IList<RoleEntity> _roles;
        protected IList<RoleEntity> Roles => _roles ?? (_roles = RoleServices.GetAllRoles(WebConfigHelper.ApplicationIdCed).OrderBy(x => x.Name).ToList());

        private IList<RegionEntity> _regions;
        protected IList<RegionEntity> Regions => _regions ?? (_regions = RegionServices.GetAllRegions().OrderBy(x => x.Name).ToList());

        private IList<CountryEntity> _countries;
        protected IList<CountryEntity> Countries => _countries ?? (_countries = new CountryServices(new UnitOfWork()).GetAllCountries());

        private IList<IndustryEntity> _industries;
        protected IList<IndustryEntity> Industries => _industries ?? (_industries = IndustryServices.GetAllIndustries().OrderBy(x => x.Name).ToList());

        private static IList<UserEntity> _approvers;
        protected IList<UserEntity> Approvers => _approvers ?? (_approvers = UserServices.GetUsersByRole(WebConfigHelper.ApplicationIdCed, "Approver"));

        // AUTH
        protected readonly IApplicationServices ApplicationServices;
        protected readonly IIndustryServices IndustryServices;
        protected readonly IRegionServices RegionServices;
        protected readonly IRoleServices RoleServices;
        protected readonly IUserServices UserServices;
        protected readonly IUserRoleServices UserRoleServices;

        protected readonly IEditionServices EditionServices;
        protected readonly IEditionCohostServices EditionCoHostServices;
        protected readonly IEditionCountryServices EditionCountryServices;
        protected readonly IEditionDiscountApproverServices EditionDiscountApproverServices;
        protected readonly IEditionKeyVisitorServices EditionKeyVisitorServices;
        protected readonly IEditionPaymentScheduleServices EditionPaymentScheduleServices;
        protected readonly IEditionSectionServices EditionSectionServices;
        protected readonly IEditionTranslationServices EditionTranslationServices;
        protected readonly IEditionTranslationSocialMediaServices EditionTranslationSocialMediaServices;
        protected readonly IEditionVisitorServices EditionVisitorServices;
        protected readonly IEventServices EventServices;
        protected readonly IEventDirectorServices EventDirectorServices;
        protected readonly IFileServices FileServices;
        protected readonly IKeyVisitorServices KeyVisitorServices;
        public readonly ILogServices LogServices;
        protected readonly INotificationServices NotificationServices;
        protected readonly ISocialMediaServices SocialMediaServices;
        protected readonly IStatisticServices StatisticServices;
        protected readonly ISubscriptionServices SubscriptionServices;
        protected readonly ITaskServices TaskServices;

        //private readonly IEmailHelper _emailHelper = new EmailHelper();
        public IEmailHelper EmailHelper { private get; set; }

        private readonly IEmailNotificationHelper _emailNotificationHelper = new EmailNotificationHelper();
        private readonly IEditionHelper _editionHelper = new EditionHelper();
        private readonly IInAppNotificationHelper _inAppNotificationHelper = new InAppNotificationHelper(new EditionHelper());

        public CedUser CurrentCedUser { get; set; }

        // BASE CTOR
        public GlobalController(
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
            IUserRoleServices userRoleServices)
        {
            UserServices = authUserServices;
            RoleServices = roleServices;
            ApplicationServices = applicationServices;
            IndustryServices = industryServices;
            RegionServices = regionServices;

            EventServices = eventServices; /*must be included in all constructors*/
            EventDirectorServices = eventDirectorServices; /*must be included in all constructors*/
            LogServices = logServices; /*must be included in all constructors*/
            NotificationServices = notificationServices; /*must be included in all constructors*/

            UserServices = userServices; /*must be included in all constructors*/
            UserRoleServices = userRoleServices; /*must be included in all constructors*/

            EmailHelper = new EmailHelper();

            CreateEmailLogFunc = CreateEmailLog;
        }

        public GlobalController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionCountryServices editionCountryServices,
            IEditionServices editionServices,
            IEditionTranslationServices editionTranslationServices,
            IEventDirectorServices eventDirectorServices,
            IEventServices eventServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IStatisticServices statisticServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionCountryServices = editionCountryServices;
            EditionServices = editionServices;
            EditionTranslationServices = editionTranslationServices;
            StatisticServices = statisticServices;
        }

        public GlobalController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionCohostServices editionCoHostServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IFileServices fileServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionCoHostServices = editionCoHostServices;
            FileServices = fileServices;
        }

        public GlobalController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            ICountryServices countryServices,
            IEditionServices editionServices,
            IEditionCohostServices editionCoHostServices,
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
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionCoHostServices = editionCoHostServices;
            EditionCountryServices = editionCountryServices;
            EditionKeyVisitorServices = editionKeyVisitorServices;
            EditionTranslationServices = editionTranslationServices;
            EditionTranslationSocialMediaServices = editionTranslationSocialMediaServices;
            EditionVisitorServices = editionVisitorServices;
            FileServices = fileServices;
            KeyVisitorServices = keyVisitorServices;
            SubscriptionServices = subscriptionServices;
        }

        public GlobalController(
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
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionKeyVisitorServices = editionKeyVisitorServices;
            KeyVisitorServices = keyVisitorServices;
            SubscriptionServices = subscriptionServices;
        }

        public GlobalController(
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
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionTranslationSocialMediaServices = editionTranslationSocialMediaServices;
            EditionTranslationServices = editionTranslationServices;
            SocialMediaServices = socialMediaServices;
        }

        public GlobalController(
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
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionSectionServices = editionSectionServices;
        }

        public GlobalController(
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
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionDiscountApproverServices = editionDiscountApproverServices;
        }

        public GlobalController(
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
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, subscriptionServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionPaymentScheduleServices = editionPaymentScheduleServices;
        }

        public GlobalController(
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
            ITaskServices taskServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            TaskServices = taskServices;
        }

        public GlobalController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            SubscriptionServices = subscriptionServices;
        }

        public GlobalController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionTranslationServices editionTranslationServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IFileServices fileServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
            EditionTranslationServices = editionTranslationServices;
            FileServices = fileServices;
            SubscriptionServices = subscriptionServices;
        }

        public GlobalController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IKeyVisitorServices keyVisitorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            KeyVisitorServices = keyVisitorServices;
        }

        public GlobalController(
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
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            this(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
            EditionServices = editionServices;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cedActionAttr = (CedActionAttribute) filterContext.ActionDescriptor
                .GetCustomAttributes(typeof(CedActionAttribute), false).SingleOrDefault();
            var noNeedForUserDataAttr = (NoNeedForUserDataAttribute) filterContext.ActionDescriptor
                .GetCustomAttributes(typeof(NoNeedForUserDataAttribute), false).SingleOrDefault();

            var pullUser = true;
            if (noNeedForUserDataAttr != null)
                pullUser = false;
            else if (cedActionAttr != null && cedActionAttr.External)
                pullUser = false;
            if (!pullUser)
                return;

            if (SetUserSso())
            {
                SetInAppNotifications();
                SetUnreadInAppNotificationsCount();
                SetRecentlyViewedEvents();

                ViewBag.CurrentCedUser = CurrentCedUser;
            }
            else
            {
                // TODO: What should it be here? No user found or added! Where should it be redirected?
            }
        }

        protected readonly UserEntity AutoIntegrationUser = new UserEntity { UserId = BusinessServices.Helpers.Constants.AutoIntegrationUserId };

        public Func<EditionEntity, ActionType, string, string, string, LogEntity> CreateEmailLogFunc { get; set; }

        [CedAction]
        public PartialViewResult _Navigation()
        {
            UserViewModel model;

            if (CurrentCedUser != null)
            {
                var roles = CurrentCedUser.Roles.ToList().Select(x => x.Name);

                var mustBePrimaryDirector = !CurrentCedUser.IsSuperAdmin;
                var draftsCount = EditionServices.GetEditionsCount(
                    CurrentCedUser.CurrentUser.Email,
                    null,
                    mustBePrimaryDirector,
                    WebConfigHelper.MinFinancialYear,
                    new List<EditionStatusType>{ EditionStatusType.Draft, EditionStatusType.PreDraft }.ToArray(),
                    Utility.Constants.ValidEventTypesForCed,
                    Utility.Constants.ValidEventActivitiesForCed);

                mustBePrimaryDirector = !(CurrentCedUser.IsSuperAdmin || CurrentCedUser.IsApprover);
                var waitingForApprovalsCount = EditionServices.GetEditionsCount(
                    CurrentCedUser.CurrentUser.Email,
                    null,
                    mustBePrimaryDirector,
                    WebConfigHelper.MinFinancialYear,
                    new List<EditionStatusType>{ EditionStatusType.WaitingForApproval }.ToArray(),
                    Utility.Constants.ValidEventTypesForCed,
                    Utility.Constants.ValidEventActivitiesForCed);

                var approvedsCount = EditionServices.GetEditionsCount(
                    CurrentCedUser.CurrentUser.Email,
                    null,
                    mustBePrimaryDirector,
                    WebConfigHelper.MinFinancialYear,
                    new List<EditionStatusType> { EditionStatusType.Approved }.ToArray(),
                    Utility.Constants.ValidEventTypesForCed,
                    Utility.Constants.ValidEventActivitiesForCed);

                var profilePictureUrl = UserServices.GetProfilePictureUrl(CurrentCedUser.CurrentUser.UserId);
                var emptyProfilePictureUrl = AzureStorageHelper.ImageBlobDirectory + Utility.Constants.NoProfilePicFileName;

                model = new UserViewModel
                {
                    UserId = CurrentCedUser.CurrentUser.UserId,
                    FullName = CurrentCedUser.CurrentUser.Name + " " + CurrentCedUser.CurrentUser.Surname,
                    RoleNames = string.Join(",", roles),
                    ProfilePictureUrl = profilePictureUrl,
                    EmptyProfilePictureUrl = emptyProfilePictureUrl,
                    DraftCount = draftsCount,
                    ApprovalCount = waitingForApprovalsCount,
                    ApprovedCount = approvedsCount,
                    CurrentUser = CurrentCedUser,
                    Approvers = string.Join("; ", Approvers.Select(x => x.Email))
                };
            }
            else
            {
                model = new UserViewModel
                {
                    Approvers = string.Join("; ", Approvers.Select(x => x.Email))
                };
            }

            return PartialView("_Navigation", model);
        }

        [AjaxOnly]
        [HttpPost]
        [CedAction(Loggable = true, ActionType = ActionType.HelpRequest)]
        public ActionResult _RequestHelp(HelpRequestModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            const string subject = "CED Help Request";
            var recipient = WebConfigHelper.AdminEmails; // WebConfigHelper.HelpDeskUserName;
            var actorUserEmail = CurrentCedUser.CurrentUser.Email;
            var body = model.Message + "<p><b>Sender:</b> " + actorUserEmail + "</p>";

            try
            {
                EmailHelper.SendEmail(subject, body, recipient);

                // EMAIL LOGGING
                LogEmail(null, recipient, body, actorUserEmail, "_RequestHelp");

                return Json(new { success = true });
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return Json(new { success = false, message = exc.Message });
            }
        }

        #region HELPER METHODS

        internal bool SetUserSso()
        {
            if (!User.Identity.IsAuthenticated) return false;

            string email = null;
            foreach (var claim in ((ClaimsPrincipal)User).Claims)
            {
                if (claim.Type == "preferred_username")
                {
                    email = claim.Value;
                    break;
                }
            }

            var user = UserServices.GetUser(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    Email = email,
                    Name = email,
                    Surname = email,
                    AdLogonName = email
                };
                user = UserServices.CreateUser(user);

                if (user != null)
                {
                    UserRoleServices.CreateUserRole(new UserRoleEntity
                    {
                        UserId = user.UserId,
                        RoleId = 3
                    });
                }
            }

            if (user == null) return false;

            var userRoles = GetUserRoles(user.UserId);

            User.AddUpdateClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Email.ToLower());
            User.AddUpdateClaim("sub", user.Email.ToLower());

            foreach (var userRole in userRoles)
            {
                User.AddClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", userRole.RoleName);
            }

            var list = ((ClaimsPrincipal)User).Claims.Where(claim => claim.Type.EndsWith("role")).Select(claim =>
            {
                var roleEntity = new RoleEntity
                {
                    RoleId = 1,
                    Name = claim.Value,
                    ApplicationId = WebConfigHelper.ApplicationIdCed,
                    ApplicationName = "ITE.CED"
                };
                return roleEntity;
            }).ToList();

            CurrentCedUser = new CedUser(user, list);
            CurrentCedUser.IsPrimaryDirector = EventDirectorServices.IsPrimaryDirector(CurrentCedUser.CurrentUser.Email, null, WebConfigHelper.ApplicationIdCed);
            CurrentCedUser.IsAssistantDirector = EventDirectorServices.IsAssistantDirector(CurrentCedUser.CurrentUser.Email, null, WebConfigHelper.ApplicationIdCed);

            return true;
        }

        private IEnumerable<UserRoleEntity> GetUserRoles(int userId)
        {
            return UserRoleServices.Get(WebConfigHelper.ApplicationIdCed, userId);
        }

        /* RECENTLY VIEWED */
        internal void SetRecentlyViewedEvents()
        {
            var recentlyViewedEventEditions = EventServices.GetLastViewedEvents(CurrentCedUser.CurrentUser.UserId, 10).OrderBy(x => x.MasterName).ToList();

            recentlyViewedEventEditions = recentlyViewedEventEditions
                .GroupBy(x => x.EventId, (key, xs) => xs.OrderByDescending(x => x.Logo).First())
                .ToList();

            foreach (var recentlyViewedEventEdition in recentlyViewedEventEditions)
            {
                dynamic o = JsonConvert.DeserializeObject(recentlyViewedEventEdition.Logo);
                recentlyViewedEventEdition.Logo = o.WebLogoFileName;
            }

            var recentLogsModel = Mapper.Map<IList<EventEditionCustomType>, List<RecentViewListModel>>(recentlyViewedEventEditions);

            ViewBag.RecentViews = recentLogsModel;
        }

        protected bool IsDirectorAuthorizedOnEvent(int eventId)
        {
            var authorized = EventDirectorServices.IsAuthorized(CurrentCedUser.CurrentUser.Email, eventId, WebConfigHelper.ApplicationIdCed);
            return authorized;
        }

        protected IList<EventDirectorEntity> GetPrimaryDirectors(int eventId)
        {
            var primaryDirectors = EventDirectorServices.GetPrimaryDirectors(eventId, WebConfigHelper.ApplicationIdCed);
            return primaryDirectors;
        }

        private string GetSubscriberEmails(int editionId)
        {
            var subscriberEmails = "";

            var subscribers = SubscriptionServices.GetSubscriptions(editionId);

            if (subscribers.Any())
                subscriberEmails = string.Join(",", subscribers.Select(x => x.UserEmail));

            return subscriberEmails;
        }

        /* LOG */
        protected LogEntity CreateInternalLog(string message, UserEntity actor = null)
        {
            if (HttpContext?.Request == null)
                return null;

            var controller = ControllerContext.RouteData.Values["controller"].ToString().FirstLetterToUpperCase();
            var action = ControllerContext.RouteData.Values["action"].ToString();

            var actionDescriptor = MvcHelper.GetActionDescriptor(HttpContext, controller, action);
            var actionAttr = actionDescriptor.GetCustomAttributes(typeof(CedActionAttribute), false).Cast<CedActionAttribute>().SingleOrDefault();

            EntityType? entityType = null;

            var actionTypeAttr = actionAttr?.ActionType.GetAttribute<ActionTypeAttribute>();
            if (actionTypeAttr != null)
                entityType = actionTypeAttr.EntityType;

            var log = new LogEntity
            {
                Url = HttpContext.Request.RawUrl,
                ActorUserId = actor?.UserId,
                ActorUserEmail = actor?.Email,
                Controller = controller,
                Action = action,
                MethodType = ControllerContext.RequestContext.HttpContext.Request.HttpMethod,
                Ip = HttpContext.Request.UserHostAddress,
                ActionType = actionAttr?.ActionType,
                EntityType = entityType,
                AdditionalInfo = message,
                CreatedOn = DateTime.Now
            };
            log.LogId = LogServices.CreateLog(log);
            return log;
        }

        protected LogEntity CreateInternalLog(Exception exc, string extraInfo, UserEntity actor = null)
        {
            var message = exc.GetFullMessage();
            if (!string.IsNullOrWhiteSpace(extraInfo))
                message = extraInfo + " | " + message;

            var log = CreateInternalLog(message, actor);
            return log;
        }

        protected LogEntity CreateInternalLog(Exception exc, UserEntity actor = null)
        {
            var log = CreateInternalLog(exc, null, actor);
            return log;
        }

        protected void UpdateLogInMemory(EditionEntity edition, string updatedFields = null)
        {
            if (TempData["Log"] is LogEntity log)
            {
               LogControllerHelper.UpdateLogInMemory(log, edition, updatedFields);
            }
        }

        protected void UpdateLogInMemory(EditionEntity currentEdition, EditionEntity edition, EditionTranslationEntity currentEditionTranslation, EditionTranslationEntity editionTranslation)
        {
            var updatedFields = LogControllerHelper.GetUpdatedFields(currentEdition, edition, currentEditionTranslation, editionTranslation, UpdateDisplayType.Json);

            if (string.IsNullOrWhiteSpace(updatedFields)) return;

            UpdateLogInMemory(edition, updatedFields);
        }

        /* IN-APP NOTIFICATION */
        private int GetUnreadInAppNotificationsCount(string recipient)
        {
            var unreadNotifCount = NotificationServices.GetNotificationCount(recipient, false);
            return unreadNotifCount;
        }

        private void SetUnreadInAppNotificationsCount()
        {
            var unreadNotifCount = GetUnreadInAppNotificationsCount(CurrentCedUser.CurrentUser.Email);
            ViewBag.UnreadNotificationCount = unreadNotifCount;
        }

        private void SetInAppNotifications()
        {
            var notifications = NotificationServices.GetNotificationsByRecipient(CurrentCedUser.CurrentUser.Email, 5);
            var notificationsModel = _inAppNotificationHelper.GetNotificationViewModelItems(notifications);
            ViewBag.Notifications = notificationsModel;
        }

        protected void CreateInAppNotification(EditionEntity edition, NotificationType notificationType, string recipients, string actorUserEmail)
        {
            var notifications = InAppNotificationHelper.CreateInAppNotifications(edition, notificationType, recipients, actorUserEmail);

            if (notifications == null)
                return;

            NotificationServices.CreateNotifications(notifications, 0);
        }

        protected void PushRealTimeInAppNotification(EditionEntity edition, NotificationType notificationType, string recipients, string actorUserEmail)
        {
            // TODO: If there are no recipients then notifications are getting pushed to Admins.
            if (string.IsNullOrWhiteSpace(recipients))
                recipients = WebConfigHelper.AdminEmails;

            var notifDesc = _inAppNotificationHelper.GetDescription(edition, notificationType);

            if (notificationType == NotificationType.EditionUpdated)
                notifDesc = notifDesc + " by " + CurrentCedUser.CurrentUser.FullName + " on " + DateTime.Now;

            if (WebConfigHelper.RemoveActorUserFromNotificationRecipients)
                recipients = NotificationControllerHelper.RemoveCurrentUserFromRecipients(recipients, actorUserEmail);

            NotificationHub.PushNotification(recipients.Split(',').ToList(), notifDesc, null);
        }

        /* EMAIL NOTIFICATION */
        public EmailResult SendEmailNotification(EditionEntity edition, NotificationType notificationType, string recipients, UserEntity actor, string body,
            string buttonUrl, string unsubscribingUrl = null)
        {
            //if (string.IsNullOrWhiteSpace(recipients))
            //{
            //    var message = $"{notificationType} type of notification email could not be sent since edition {edition.EditionId} - {edition.EditionName} has no recipients.";
            //    var log = CreateInternalLog(message, actor);
            //    ExternalLogHelper.Log(log, LoggingEventType.Warning);
            //    return new EmailResult { Sent = false, ErrorMessage = message };
            //}

            if (WebConfigHelper.RemoveActorUserFromNotificationRecipients)
                recipients = NotificationControllerHelper.RemoveCurrentUserFromRecipients(recipients, actor?.Email);

            //if (string.IsNullOrWhiteSpace(recipients))
            //    return false;

            try
            {
                var recipientFullName = _editionHelper.GetRecipientFullName(edition);

                var sendEmailAttr = notificationType.GetAttribute<NotificationAttribute>().SendEmail;
                if (sendEmailAttr)
                {
                    var editionTranslation =
                        edition.EditionTranslations.SingleOrDefault(x => string.Equals(x.LanguageCode,
                            LanguageHelper.GetBaseLanguageCultureName(), StringComparison.CurrentCultureIgnoreCase));

                    var emailResult = _emailNotificationHelper.Send(edition, editionTranslation, notificationType, recipientFullName, body, recipients,
                        buttonUrl, unsubscribingUrl);

                    //// EMAIL LOGGING
                    //if (sent)
                    //LogEmail(edition.EditionId, recipients, body, actor?.Email, notificationType.ToString());

                    return emailResult;
                }
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc, actor);
                ExternalLogHelper.Log(log, LoggingEventType.Error);
            }

            return new EmailResult { Sent= false, ErrorMessage = "" };
        }

        internal bool LogEmail(int? editionId, string recipients, string body, string actorUserEmail, string actionName, ActionType? actionType = ActionType.NotificationEmailSend)
        {
            if (string.IsNullOrWhiteSpace(recipients)) return false;

            EditionEntity edition = null;
            if (editionId.HasValue)
                edition = EditionServices.GetEditionById(editionId.Value);

            foreach (var recipient in recipients.Split(','))
            {
                var additionalInfo = "Recipient: " + recipient + " | Body: " + body;
                additionalInfo = additionalInfo.StripHtml().Substr(500);

                var emailLog = CreateEmailLogFunc(edition, actionType.GetValueOrDefault(), actorUserEmail, actionName, additionalInfo);
                LogServices.CreateLog(emailLog);
            }

            return true;
        }

        private LogEntity CreateEmailLog(EditionEntity edition, ActionType actionType, string actorUserEmail, string action, string additionalInfo)
        {
            var emailLog = new LogEntity
            {
                EntityId = edition?.EditionId,
                EntityName = edition?.EditionName,
                EventId = edition?.EventId,
                EventName = edition?.Event.MasterName,
                ActionType = actionType,
                EntityType = EntityType.Email,
                Url = HttpContext.Request.RawUrl,
                ActorUserId = null,
                ActorUserEmail = actorUserEmail,
                Controller = ControllerContext.RouteData.Values["controller"].ToString().FirstLetterToUpperCase(),
                Action = action,
                MethodType = ControllerContext.RequestContext.HttpContext.Request.HttpMethod,
                Ip = HttpContext.Request.UserHostAddress,
                AdditionalInfo = additionalInfo,
                CreatedOn = DateTime.Now
            };

            return emailLog;
        }

        /* NOTIFICATION - COMMON */
        protected void PushEditionUpdateNotifications(EditionEntity edition, string updatedFields)
        {
            if (WebConfigHelper.TrackEditionUpdate && !string.IsNullOrWhiteSpace(updatedFields))
            {
                var subscribers = GetSubscriberEmails(edition.EditionId);
                var inAppNotificationRecipients = subscribers;
                var emailRecipients = WebConfigHelper.TrackEditionUpdateUseMockRecipients
                    ? WebConfigHelper.AdminEmails
                    : subscribers;

                var body = $"Field(s) updated by {CurrentCedUser.CurrentUser.FullName} on {edition.UpdateTime}:<br/><br/>";
                body += updatedFields;

                const NotificationType notificationType = NotificationType.EditionUpdated;
                var notifAttr = notificationType.GetAttribute<NotificationAttribute>();
                var buttonUrl = _editionHelper.GetEditionUrl(edition);
                var unsubscriptionUrl = notifAttr.Unsubscribable ? EmailNotificationHelper.GetUnsubscriptionUrl(edition) : string.Empty;

                CreateInAppNotification(edition, notificationType, inAppNotificationRecipients, CurrentCedUser.CurrentUser.Email);

                PushRealTimeInAppNotification(edition, notificationType, inAppNotificationRecipients, CurrentCedUser.CurrentUser.Email);

                var emailResult = SendEmailNotification(edition, notificationType, emailRecipients, CurrentCedUser.CurrentUser, body,
                    buttonUrl, unsubscriptionUrl);

                // STEP 5: LOG EMAIL
                if (emailResult.Sent)
                    LogEmail(edition.EditionId, inAppNotificationRecipients, body, CurrentCedUser.CurrentUser.Email, notificationType.ToString());
                else
                    LogEmail(edition.EditionId, WebConfigHelper.AdminEmails, emailResult.ErrorMessage, null, notificationType.ToString());
            }
        }

        protected void UpdateEditionUpdateInfo(EditionEntity edition)
        {
            EditionServices.UpdateEdition(edition.EditionId, edition, CurrentCedUser.CurrentUser.UserId);
        }

        protected void OnEditionUpdated(EditionEntity edition, List<Variance> diff)
        {
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsDetailedHtml(diff);

            PushEditionUpdateNotifications(edition, updatedFields);
        }

        #endregion
    }
}