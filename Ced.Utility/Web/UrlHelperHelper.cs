using System.Web;
using System.Web.Mvc;

namespace Ced.Utility.Web
{
    public class UrlHelperHelper
    {
        public UrlHelperHelper()
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            UrlHelper = urlHelper;
        }

        public UrlHelperHelper(UrlHelper urlHelper)
        {
            UrlHelper = urlHelper;
        }

        public UrlHelper UrlHelper { get; }
    }
}