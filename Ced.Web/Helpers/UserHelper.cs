using System;
using System.Web;
using Ced.Utility;
using Ced.Utility.Web;

namespace Ced.Web.Helpers
{
    public class UserHelper
    {
        public static string GetJwtCookie()
        {
            return HttpContext.Current.Request.Cookies[WebConfigHelper.JwtCookieName]?.Value;
        }

        public static void SetJwtCookie(string token)
        {
            // TODO: Do we need to set an expiration for the cookie? Because jwt has one itself.
            var cookie = new HttpCookie(WebConfigHelper.JwtCookieName, token)
            {
                Expires = DateTime.Now.AddMinutes(WebConfigHelper.AuthCookieLifeSpan),
                HttpOnly = true // TODO: ?
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void DeleteJwtCookie()
        {
            var currentUserCookie = HttpContext.Current.Request.Cookies[WebConfigHelper.JwtCookieName];
            HttpContext.Current.Response.Cookies.Remove(WebConfigHelper.JwtCookieName);
            if (currentUserCookie != null)
            {
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                HttpContext.Current.Response.SetCookie(currentUserCookie);
            }
        }
    }
}