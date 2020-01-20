using Microsoft.Owin.Security;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Ced.Web.Helpers
{
    public static class ClaimsIdentityExtensions
    {
        public static void AddClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            ClaimsIdentity identity;
            if ((identity = currentPrincipal.Identity as ClaimsIdentity) == null)
                return;
            identity.AddClaim(new Claim(key, value));
            HttpContext.Current.GetOwinContext().Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties
            {
                IsPersistent = true
            });
        }

        public static void AddUpdateClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            ClaimsIdentity identity;
            if ((identity = currentPrincipal.Identity as ClaimsIdentity) == null)
                return;
            Claim first = identity.FindFirst(key);
            if (first != null)
                identity.RemoveClaim(first);
            identity.AddClaim(new Claim(key, value));
            HttpContext.Current.GetOwinContext().Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties
            {
                IsPersistent = true
            });
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            ClaimsIdentity identity = currentPrincipal.Identity as ClaimsIdentity;
            return identity?.Claims.FirstOrDefault(c => c.Type == key)?.Value;
        }
    }
}