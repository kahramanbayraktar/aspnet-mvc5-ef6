using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Data.UnitOfWork;
using Ced.Utility.Edition;
using Ced.Utility.Notification;
using Ced.Web.Helpers;
using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;

namespace Ced.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<ICountryServices, CountryServices>();
            container.RegisterType<IEditionServices, EditionServices>();
            container.RegisterType<IEditionCohostServices, EditionCohostServices>();
            container.RegisterType<IEditionCountryServices, EditionCountryServices>();
            container.RegisterType<IEditionDiscountApproverServices, EditionDiscountApproverServices>();
            container.RegisterType<IEditionKeyVisitorServices, EditionKeyVisitorServices>();
            container.RegisterType<IEditionSectionServices, EditionSectionServices>();
            container.RegisterType<IEditionPaymentScheduleServices, EditionPaymentScheduleServices>();
            container.RegisterType<IEditionTranslationServices, EditionTranslationServices>();
            container.RegisterType<IEditionTranslationSocialMediaServices, EditionTranslationSocialMediaServices>();
            container.RegisterType<IEditionVisitorServices, EditionVisitorServices>();
            container.RegisterType<IEventServices, EventServices>();
            container.RegisterType<IEventDirectorServices, EventDirectorServices>();
            container.RegisterType<IFileServices, FileServices>();
            container.RegisterType<IKeyVisitorServices, KeyVisitorServices>();
            container.RegisterType<ILogServices, LogServices>();
            container.RegisterType<INotificationServices, NotificationServices>();
            container.RegisterType<IStatisticServices, StatisticServices>();
            container.RegisterType<ISocialMediaServices, SocialMediaServices>();
            container.RegisterType<ISubscriptionServices, SubscriptionServices>();
            container.RegisterType<ITaskServices, TaskServices>();
            //container.RegisterType<IUserServices, UserServices>();
            container.RegisterType<IEditionHelper, EditionHelper>();
            container.RegisterType<IEmailNotificationHelper, EmailNotificationHelper>();
            container.RegisterType<IInAppNotificationHelper, InAppNotificationHelper>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();

            // AUTH
            container.RegisterType<IUserServices, UserServices>();
            container.RegisterType<IRoleServices, RoleServices>();
            container.RegisterType<IApplicationServices, ApplicationServices>();
            container.RegisterType<IIndustryServices, IndustryServices>();
            container.RegisterType<IRegionServices,  RegionServices>();
            container.RegisterType<IUserRoleServices, UserRoleServices>();

            // ITE.Auth
            //container.RegisterType<Auth.DataModel.UnitOfWork.IUnitOfWork, Auth.DataModel.UnitOfWork.UnitOfWork>();
            //var unitOfWork = container.Resolve<Auth.DataModel.UnitOfWork.UnitOfWork>();
            //container.RegisterType<Auth.BusinessServices.IUserServices, Auth.BusinessServices.UserServices>(
            //    new InjectionProperty("AuthUserServices", new Auth.BusinessServices.UserServices(unitOfWork)));

            DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}