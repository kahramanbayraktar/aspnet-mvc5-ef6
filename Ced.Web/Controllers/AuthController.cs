using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    // Single Sign-On Authentication
    public class AuthController : Controller
    {
        public void SignIn()
        {
            if (Request.IsAuthenticated)
                return;

            var authentication = HttpContext.GetOwinContext().Authentication;

            var properties = new AuthenticationProperties
            {
                RedirectUri = "/"
            };

            var strArray = new[] { "OpenIdConnect" };

            authentication.Challenge(properties, strArray);
        }

        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut("OpenIdConnect", "Cookies");
        }
    }
}