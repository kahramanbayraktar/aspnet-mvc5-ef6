using System.Web.Mvc;
using System.Web.Routing;

namespace Ced.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.Ignore("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            //routes.MapRoute("DefaultLocalized",
            //    "{language}-{culture}/{controller}/{action}/{id}",
            //    new
            //    {
            //        controller = "Dashboard",
            //        action = "Index",
            //        id = "",
            //        language = "en",
            //        culture = "GB"
            //    });

            routes.MapRoute(
                name: "Dashboard",
                url: "dashboard/{id}/{name}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = "\\d+" }
                );

            routes.MapRoute(
                name: "DashboardIndex",
                url: "dashboard",
                defaults: new { controller = "Dashboard", action = "Index" },
                constraints: new { }
            );

            routes.MapRoute(
                name: "EventIndex",
                url: "events",
                defaults: new { controller = "Event", action = "Index" }
                );

            routes.MapRoute(
                name: "EventDetails",
                url: "event/{id}/{name}",
                defaults: new { controller = "Event", action = "Details" }
                );

            routes.MapRoute(
                name: "EventAdd",
                url: "event/add",
                defaults: new { controller = "Event", action = "Add" }
                );

            routes.MapRoute(
                name: "EditionsByEvent",
                url: "editions/{eventId}/{name}",
                defaults: new { controller = "Edition", action = "Index" },
                constraints: new { eventId = @"\d+" }
                );

            routes.MapRoute(
                name: "EditionsByStatus",
                url: "editions/{status}",
                defaults: new { controller = "Edition", action = "Index" }
                );

            routes.MapRoute(
                name: "EditionsNew",
                url: "editions/newedition",
                defaults: new { controller = "Edition", action = "Index" }
                );

            routes.MapRoute(
                name: "EditionsIndex",
                url: "editions",
                defaults: new { controller = "Edition", action = "Index", routeName = "EditionIndex" }
                );

            routes.MapRoute(
                name: "EditionDetails",
                url: "edition/{id}/{name}/details",
                defaults: new { controller = "Edition", action = "Details" }
                );

            routes.MapRoute(
                name: "EditionEdit",
                url: "edition/{id}/{name}/edit",
                defaults: new { controller = "Edition", action = "Edit" }
                );

            routes.MapRoute(
                name: "EditionClone",
                url: "edition/{id}/{name}/clone",
                defaults: new { controller = "Edition", action = "_Clone" }
                );

            routes.MapRoute(
                name: "EditionDraft",
                url: "edition/{id}/{name}/draft",
                defaults: new { controller = "Edition", action = "Draft" }
                );

            routes.MapRoute(
                name: "EditionPdf",
                url: "edition/{id}/{name}/{lang}/pdf",
                defaults: new { controller = "Edition", action = "ExportToPdf" }
                );

            routes.MapRoute(
                name: "EventDirectorIndex",
                url: "eventdirectors",
                defaults: new { controller = "EventDirector", action = "Index" }
            );

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "User", action = "Login" }
                );

            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "User", action = "Logout" }
                );

            routes.MapRoute(
                name: "DeleteProfilePicture",
                url: "delete-profile-picture",
                defaults: new { controller = "User", action = "_DeleteProfilePicture" }
                );

            routes.MapRoute(
                name: "RequestAccess",
                url: "access-request",
                defaults: new { controller = "User", action = "RequestAccess" }
                );

            routes.MapRoute(
               name: "LogIndex",
               url: "logs",
               defaults: new { controller = "Log", action = "Index" }
               );

            routes.MapRoute(
                name: "NotificationIndex",
                url: "notifications",
                defaults: new { controller = "Notification", action = "Index" }
                );

            routes.MapRoute(
                name: "EmailNotifications",
                url: "email-notifications",
                defaults: new { controller = "EmailNotification", action = "Index" }
            );

            routes.MapRoute(
                name: "Tasks",
                url: "tasks",
                defaults: new { controller = "Task", action = "Index" }
            );

            routes.MapRoute(
                name: "UserIndex",
                url: "users",
                defaults: new { controller = "User", action = "Index" }
                );

            routes.MapRoute(
                name: "UserRoleIndex",
                url: "userroles",
                defaults: new { controller = "UserRole", action = "Index" }
                );

            routes.MapRoute(
                name: "ConfigSettings",
                url: "configsettings",
                defaults: new { controller = "ConfigSetting", action = "Index" }
            );

            routes.MapRoute(
                name: "AdminEditionIndex",
                url: "admin-editions",
                defaults: new { controller = "AdminEdition", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Edition", action = "Index" }
                );
        }
    }
}