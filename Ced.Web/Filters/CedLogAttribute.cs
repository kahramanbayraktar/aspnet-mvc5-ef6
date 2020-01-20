using System;
using System.Linq;
using System.Web.Mvc;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.Data.UnitOfWork;
using Ced.Utility;
using Ced.Web.Controllers;
using ITE.Utility.Extensions;

namespace Ced.Web.Filters
{
    public class CedLogAttribute : ActionFilterAttribute
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IEditionServices _editionServices;
        private readonly IEventServices _eventServices;

        public CedLogAttribute(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _editionServices = new EditionServices(_unitOfWork);
            _eventServices = new EventServices(_unitOfWork);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;

            var ip = request.UserHostAddress;
            var url = request.RawUrl;

            if (!(filterContext.Controller is GlobalController controller))
                return;

            var actionAttr = filterContext.ActionDescriptor.GetCustomAttributes(typeof(CedActionAttribute), false).Cast<CedActionAttribute>().SingleOrDefault();
            if (actionAttr == null)
                return;

            if (!actionAttr.Loggable)
                return;

            var actionTypeAttr = actionAttr.ActionType.GetAttribute<ActionTypeAttribute>();
            EntityType? entityType = null;
            if (actionTypeAttr != null)
                entityType = actionTypeAttr.EntityType;

            int? entityId = null;
            string entityName = null;
            int? eventId = null;
            string eventName = null;

            if (filterContext.RouteData.Values["id"] != null)
                entityId = Convert.ToInt32(filterContext.RouteData.Values["id"]);

            if (entityId == null)
            {
                if (filterContext.RequestContext.HttpContext.Request.Params["EditionId"] != null)
                    entityId = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Params["EditionId"]);

                if (!string.IsNullOrWhiteSpace(filterContext.RequestContext.HttpContext.Request.Params["EventId"]))
                    eventId = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Params["EventId"]);
            }

            if (entityId.HasValue)
            {
                if (entityType == EntityType.Edition)
                {
                    var edition = _editionServices.GetEditionById(entityId.Value, Constants.ValidEventTypesForCed);
                    if (edition != null)
                    {
                        entityId = edition.EditionId;
                        entityName = edition.EditionName;
                        eventId = edition.EventId;
                        eventName = edition.Event.MasterName;
                    }
                }
                else if (entityType == EntityType.Event)
                {
                    var @event = _eventServices.GetEventById(entityId.Value, Constants.ValidEventTypesForCed);
                    if (@event != null)
                    {
                        entityId = @event.EventId;
                        entityName = @event.MasterName;
                        eventId = @event.EventId;
                        eventName = @event.MasterName;
                    }
                }
            }

            var log = new LogEntity
            {
                Ip = ip,
                Url = url,
                ActorUserId = controller.CurrentCedUser?.CurrentUser.UserId ?? 0,
                ActorUserEmail = controller.CurrentCedUser?.CurrentUser.Email,
                Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Action = filterContext.ActionDescriptor.ActionName,
                MethodType = request.HttpMethod,
                ActionType = actionAttr.ActionType,
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                EventId = eventId,
                EventName = eventName,
                IsImpersonated = controller.CurrentCedUser?.IsImpersonated ?? false
            };
            filterContext.Controller.TempData["Log"] = log;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            if (!(filterContext.Controller is GlobalController controller))
                return;

            var actionAttr = filterContext.ActionDescriptor.GetCustomAttributes(typeof(CedActionAttribute), false).Cast<CedActionAttribute>().SingleOrDefault();
            if (actionAttr == null)
                return;

            if (filterContext.Controller.TempData["Log"] is LogEntity log)
                controller.LogServices.CreateLog(log);
        }
    }
}