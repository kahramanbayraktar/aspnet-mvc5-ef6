using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace Ced.Web.Filters
{
    public class CedGlobalizationAttribute : ActionFilterAttribute
    {
        // Globalization hatası oluşuyor!
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var language = filterContext.RouteData.Values["language"] ?? "en";
        //    var culture = filterContext.RouteData.Values["culture"] ?? "GB";

        //    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(string.Format("{0}-{1}", language, culture));
        //    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        //}
    }
}