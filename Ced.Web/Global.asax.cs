using Ced.Utility.Web;
using Ced.Web.Helpers;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ced.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();

            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();

            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Register();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Check for the jwt cookie.
            var jwtToken = Request.Cookies[WebConfigHelper.JwtCookieName];
            // Add jwt token to the request as a header.
            if (jwtToken != null)
                Request.Headers.Add("Authorization", "Bearer " + jwtToken.Value);

            // This code has to stay here. ActionFilterAttribute (CedGlobalizationAttribute) doesn't provide globalization.
            HttpContextBase currentContext = new HttpContextWrapper(HttpContext.Current);
            RouteData routeData = RouteTable.Routes.GetRouteData(currentContext);

            if (routeData != null)
            {
                //var language = routeData.Values["language"] ?? "en";
                //var culture = routeData.Values["culture"] ?? "gb";
                var language = "en";
                var culture = "gb";

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo($"{language}-{culture}");
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
        }
    }
}