using Ced.BusinessEntities.Auth;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Web.Filters;
using Ced.Web.Models.Select2;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class CountryController : GlobalController
    {
        public CountryController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        public ActionResult _SearchCountries(string searchTerm)
        {
            var countries = Countries.Where(x => x.CountryName.ToLower().Contains(searchTerm));
            var pagedEvents = CountriesToSelect2Format(countries, 15);

            return new JsonResult
            {
                Data = pagedEvents,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static Select2PagedResult CountriesToSelect2Format(IEnumerable<CountryEntity> countries, int totalCountries)
        {
            var jsonCountries = new Select2PagedResult
            {
                Results = new List<Select2Result>()
            };

            foreach (var c in countries)
            {
                jsonCountries.Results.Add(new Select2Result { id = c.CountryCode, text = c.CountryName });
            }

            jsonCountries.Total = totalCountries;
            return jsonCountries;
        }
    }
}