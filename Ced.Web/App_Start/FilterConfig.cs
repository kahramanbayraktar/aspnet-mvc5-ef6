using System.Web.Mvc;
using Ced.Data.UnitOfWork;
using Ced.Web.Filters;

namespace Ced.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CedGlobalizationAttribute());
            filters.Add(new CedAuthorizeAttribute());
            filters.Add(new CedLogAttribute(new UnitOfWork()));
            filters.Add(new CedErrorHandlerAttribute());
        }
    }
}
