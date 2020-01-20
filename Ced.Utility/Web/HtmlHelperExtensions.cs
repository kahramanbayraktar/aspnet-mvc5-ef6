using System.Web.Mvc;

namespace Ced.Utility.Web
{
    public static class HtmlHelperExtensions
    {
        private const string CssClass = "active";

        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            var viewContext = html.ViewContext.IsChildAction
                ? html.ViewContext.ParentActionViewContext
                : html.ViewContext;
            var currentAction = (string)viewContext.RouteData.Values["action"];
            var currentController = (string)viewContext.RouteData.Values["controller"];

            if (string.IsNullOrEmpty(controller))
                controller = currentController;

            if (string.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                CssClass : string.Empty;
        }

        public static string IsSelectedByRouteName(this HtmlHelper html, string routeName)
        {
            var viewContext = html.ViewContext.IsChildAction
                ? html.ViewContext.ParentActionViewContext
                : html.ViewContext;

            var currentRouteName = viewContext.RouteData.Values[nameof(routeName)]?.ToString();

            return routeName == currentRouteName ? CssClass : string.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            var currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string AbsoluteAction(this UrlHelper url, string actionName, string controllerName, object routeValues = null)
        {
            var retUrl = url.Action(actionName, controllerName, routeValues);
            //if (!WebConfigHelper.IsLocal)
            //{
            //    return WebConfigHelper.ApplicationAbsolutePath + retUrl;
            //}
            //return retUrl;
            return WebConfigHelper.ApplicationAbsolutePath + retUrl;
        }
    }
}