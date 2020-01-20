using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ced.Utility.MVC
{
    public static class MvcHelper
    {
        public static ActionDescriptor GetActionDescriptor(HttpContextBase context, string controllerName, string actionName)
        {
            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();

            var controller = (ControllerBase)controllerFactory.CreateController(new RequestContext(context, new RouteData()), controllerName);

            var controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());

            var controllerContext = new ControllerContext(context, new RouteData(), controller);

            var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

            return actionDescriptor;
        }
    }
}