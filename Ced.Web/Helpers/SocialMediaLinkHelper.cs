using Ced.BusinessEntities;
using ITE.Utility.Extensions;

namespace Ced.Web.Helpers
{
    public class SocialMediaLinkHelper
    {
        public static string GetAccountLink(string socialMediaId, string accountName)
        {
            var socialMediaType = socialMediaId.ToEnumFromDescription<SocialMediaType>();
            var accountRootUrl = socialMediaType.GetAttribute<SocialMediaAttribute>().AccountUrlRoot;
            return string.Format(accountRootUrl, accountName);
        }

        public static string GetAccountName(string socialMediaId, string link)
        {
            if (!string.IsNullOrWhiteSpace(link))
            {
                link = link.Replace("https://", "http://");
                link = link.Replace("www.", "");

                var socialMediaType = socialMediaId.ToEnumFromDescription<SocialMediaType>();
                var accountRootUrl = socialMediaType.GetAttribute<SocialMediaAttribute>().AccountUrlRoot;
                return link.Replace(accountRootUrl.Replace("{0}", ""), "");
            }
            return "";
        }
    }
}