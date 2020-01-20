using System.IO;
using System.Web.Mvc;

namespace Ced.Utility.MVC
{
    public static class MvcExtensions
    {
        public static string GetErrors(this ModelStateDictionary modelState)
        {
            var errorMessage = "";
            foreach (var val in modelState.Values)
            {
                foreach (var error in val.Errors)
                {
                    errorMessage += error.ErrorMessage + "<br>";
                }
            }
            return errorMessage;
        }

        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}