using System.Web.Mvc;
using Ced.Web.Models;

namespace Ced.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Error(string aspxErrorPath)
        {
            ViewBag.ReturnUrl = aspxErrorPath;
            return View("~/Views/Shared/Error.cshtml", new ErrorModel());
        }

        public ActionResult NotFound(string aspxErrorPath)
        {
            ViewBag.ReturnUrl = aspxErrorPath;
            return View("~/Views/Shared/NotFound.cshtml");
        }

        public ActionResult Unauthorized(string aspxErrorPath)
        {
            ViewBag.ReturnUrl = aspxErrorPath;
            return View("~/Views/Shared/Unauthorized.cshtml");
        }
    }
}