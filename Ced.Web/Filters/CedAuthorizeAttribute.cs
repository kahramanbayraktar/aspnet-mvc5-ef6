using System.Web.Mvc;
using System.Web.Routing;

namespace Ced.Web.Filters
{
    public class CedAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "Your session timed out. Log in again to continue."
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "auth"
                    && filterContext.ActionDescriptor.ActionName.ToLower() == "signin"
                    || filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "error"
                    || filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "elmah")
                {
                    return;
                }

                if (filterContext.HttpContext.Request.IsAuthenticated)
                {
                    //filterContext.Result = new HttpUnauthorizedResult();
                    var authenticatedUnauthorizedRouteValues = new RouteValueDictionary(new { controller = "Error", action = "Unauthorized" });
                    filterContext.Result = new RedirectToRouteResult(authenticatedUnauthorizedRouteValues);
                    return;
                }

                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var url = urlHelper.Action("SignIn", "Auth", new { returnUrl = filterContext.RequestContext.HttpContext.Request.Url });
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}