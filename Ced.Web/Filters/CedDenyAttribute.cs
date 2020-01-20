using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ced.Web.Filters
{
    public class CedDenyAttribute : AuthorizeAttribute
    {
        private const string IsAuthorized = "isAuthorized";

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            bool isAuthorized = true;

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                isAuthorized = false;
            }

            if (Users.Length > 0 && Users.Split(',').Any(u => u.Trim() == user.Identity.Name))
            {
                isAuthorized = false;
            }

            if (Roles.Length > 0 && Roles.Split(',').Any(u => user.IsInRole(u.Trim())))
            {
                isAuthorized = false;
            }

            httpContext.Items.Add(IsAuthorized, isAuthorized);

            return isAuthorized;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var isAuthorized = filterContext.HttpContext.Items[IsAuthorized] != null && Convert.ToBoolean(filterContext.HttpContext.Items[IsAuthorized]);

            if (!isAuthorized && filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var authenticatedUnauthorizedRouteValues = new RouteValueDictionary(new { controller = "Error", action = "Unauthorized" });
                filterContext.Result = new RedirectToRouteResult(authenticatedUnauthorizedRouteValues);
                //filterContext.RequestContext.HttpContext.Response.Redirect(RedirectUrl);
            }
        }
    }
}