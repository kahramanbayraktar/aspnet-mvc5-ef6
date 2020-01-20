using System;
using System.Linq;
using System.Web.Mvc;
using Ced.BusinessEntities;
using Ced.Utility;
using Ced.Utility.MVC;
using Ced.Utility.Web;
using Ced.Web.Controllers;
using ITE.Logger;
using ITE.Utility.Extensions;
using FilterAttribute = System.Web.Http.Filters.FilterAttribute;
using IExceptionFilter = System.Web.Mvc.IExceptionFilter;

namespace Ced.Web.Filters
{
    public class CedErrorHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            string message;
            if (WebConfigHelper.IsLocal || WebConfigHelper.IsTest)
                message = filterContext.Exception.GetFullMessage();
            else
                message = "An error occured!";

            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            // DETAILED LOG
            if (!(filterContext.Controller is GlobalController))
                return;

            var controller = filterContext.RouteData.Values["controller"].ToString().FirstLetterToUpperCase();
            var action = filterContext.RouteData.Values["action"].ToString();

            var actionDescriptor = MvcHelper.GetActionDescriptor(filterContext.HttpContext, controller, action);
            var actionAttr = actionDescriptor.GetCustomAttributes(typeof(CedActionAttribute), false).Cast<CedActionAttribute>().SingleOrDefault();
            if (actionAttr == null)
                return;

            if (!actionAttr.Loggable)
                return;

            var actionTypeAttr = actionAttr.ActionType.GetAttribute<ActionTypeAttribute>();
            EntityType? entityType = null;
            if (actionTypeAttr != null)
                entityType = actionTypeAttr.EntityType;

            int? entityId = null;
            int? eventId = null;

            if (filterContext.RouteData.Values["id"] != null)
                entityId = Convert.ToInt32(filterContext.RouteData.Values["id"]);

            if (entityId == null)
            {
                if (filterContext.RequestContext.HttpContext.Request.Params["EditionId"] != null)
                    entityId = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Params["EditionId"]);

                if (filterContext.RequestContext.HttpContext.Request.Params["EventId"] != null)
                    eventId = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Params["EventId"]);
            }

            var logMessage = "Unhandled error! " + filterContext.Exception.GetFullMessage();
            logMessage += "\r\nController: " + controller;
            logMessage += "\r\nAction: " + action;
            logMessage += "\r\nEntityId : " + entityId;
            logMessage += "\r\nEventId: " + eventId;
            logMessage += "\r\nEntityType: " + entityType;

            BusinessServices.Helpers.ExternalLogHelper.Log(logMessage, LoggingEventType.Error);
        }
    }
}