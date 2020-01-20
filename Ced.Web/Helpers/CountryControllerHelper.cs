using Ced.Web.Models.Country;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Helpers
{
    public static class CountryControllerHelper
    {
        public static IEnumerable<SelectListItem> CountryList
        {
            get
            {
                var countries = new List<CountryListModel>
                {
                    new CountryListModel {Code = "GBP", Name = "£"},
                    new CountryListModel {Code = "RUB", Name = "₽"},
                    new CountryListModel {Code = "TRL", Name = "₺"},
                    new CountryListModel {Code = "USD", Name = "€"}
                };
                return countries.Select(s => new SelectListItem { Value = s.Code.ToString(), Text = s.Name });
            }
        }
    }
}