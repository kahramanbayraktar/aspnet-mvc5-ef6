using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Web.Filters;
using Ced.Web.Models.Select2;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class KeyVisitor : GlobalController
    {
        public KeyVisitor(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IKeyVisitorServices keyVisitorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, keyVisitorServices, logServices, notificationServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        public ActionResult _SearchKeyVisitors() //string searchTerm, int pageSize, int pageNum)
        {
            //var keyVisitors = KeyVisitorServices.SearchKeyVisitors(searchTerm.Trim(), pageSize, pageNum);
            var pagedKeyVisitors = new Select2PagedResult(); // KeyVisitorsToSelect2Format(keyVisitors, pageSize);

            return new JsonResult
            {
                Data = pagedKeyVisitors,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static Select2PagedResult KeyVisitorsToSelect2Format(IEnumerable<KeyVisitorEntity> keyVisitors, int totalKeyVisitors)
        {
            var jsonKeyVisitors = new Select2PagedResult
            {
                Results = new List<Select2Result>()
            };

            foreach (var kv in keyVisitors)
            {
                jsonKeyVisitors.Results.Add(new Select2Result { id = kv.KeyVisitorId.ToString(), text = kv.Name });
            }

            jsonKeyVisitors.Total = totalKeyVisitors;
            return jsonKeyVisitors;
        }
    }
}