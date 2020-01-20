using Ced.Utility.Web;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Ced.Web.Filters
{
    public class ApiAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple { get; }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            const string apiKeyName = "key";
            var query = context.Request.RequestUri.Query;

            if (query.StartsWith("?"))
                query = query.Substring(1);

            if (string.IsNullOrWhiteSpace(query))
            {
                context.ErrorResult = new BadRequestErrorMessageResult("Api key is missing.", (ApiController)context.ActionContext.ControllerContext.Controller);
                return Task.FromResult(0);
            }

            var queryParams = query.Split('&');

            var apiKeyParamFound = queryParams.Any(x => x.Split('=')[0] == apiKeyName);
            if (!apiKeyParamFound)
            {
                context.ErrorResult = new BadRequestErrorMessageResult("Api key is missing.", (ApiController)context.ActionContext.ControllerContext.Controller);
                return Task.FromResult(0);
            }

            string apiKey = null;
            try
            {
                var apiKeyStartIndex = query.IndexOf(apiKeyName + "=");
                if (apiKeyStartIndex > -1)
                    apiKey = query.Substring(apiKeyStartIndex + ("?" + apiKeyName + "=").Length - 1, 32);
            }
            catch
            {
            }

            if (apiKey != WebConfigHelper.WebApiKey)
            {
                context.ErrorResult = new BadRequestErrorMessageResult("Api key is invalid.", (ApiController)context.ActionContext.ControllerContext.Controller);
                return Task.FromResult(0);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}