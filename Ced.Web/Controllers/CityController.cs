using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Web.Filters;
using Ced.Web.Models.Select2;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class CityController : GlobalController
    {
        public CityController(
            BusinessServices.Auth.IUserServices authUserServices,
            BusinessServices.Auth.IRoleServices roleServices,
            BusinessServices.Auth.IApplicationServices applicationServices,
            BusinessServices.Auth.IIndustryServices industryServices,
            BusinessServices.Auth.IRegionServices regionServices,
            IEditionServices editionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        public ActionResult _SearchCities(string searchTerm, string countryCode)
        {
            var cities = EditionServices.GetCities(searchTerm, countryCode);
            var pagedEvents = CitiesToSelect2Format(cities, 15);

            return new JsonResult
            {
                Data = pagedEvents,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static Select2PagedResult CitiesToSelect2Format(IEnumerable<string> cities, int totalCities)
        {
            var jsonCities = new Select2PagedResult
            {
                Results = new List<Select2Result>()
            };

            foreach (var c in cities)
            {
                jsonCities.Results.Add(new Select2Result { id = c, text = c });
            }

            jsonCities.Total = totalCities;
            return jsonCities;
        }
    }
}