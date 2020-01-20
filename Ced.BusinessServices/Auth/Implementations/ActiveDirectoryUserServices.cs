using Ced.BusinessEntities;
using ITE.Utility.ActiveDirectory;

namespace Ced.BusinessServices.Auth
{
    public class ActiveDirectoryUserServices : IActiveDirectoryUserServices
    {
        private readonly string _domainName;
        private readonly string _adminUserName;
        private readonly string _adminPassword;

        public ActiveDirectoryUserServices(string domainName, string userName, string password)
        {
            _domainName = domainName;
            _adminUserName = userName;
            _adminPassword = password;
        }

        public UserEntity Authenticate(string userName, string password)
        {
            var adHelper = new ActiveDirectoryHelper(_domainName, _adminUserName, _adminPassword);

            var user = adHelper.FindUser(userName);
            if (user == null) return null;

            var validated = adHelper.Validate(user.UserPrincipalName, password);
            if (!validated) return null;
            
            return new UserEntity
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                UserPrincipalName = user.UserPrincipalName,
                AdLogonName = GetAdLogonName(user)
            };
        }

        public UserEntity FindUser(string userName)
        {
            var adHelper = new ActiveDirectoryHelper(_domainName, _adminUserName, _adminPassword);

            var user = adHelper.FindUser(userName);
            if (user == null) return null;

            return new UserEntity
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                UserPrincipalName = user.UserPrincipalName,
                AdLogonName = GetAdLogonName(user)
            };
        }

        private string GetAdLogonName(ActiveDirectoryHelper.SimpleAdUser user)
        {
            var adsPath = user.AdsPath.ToLower();
            var startIndex = adsPath.IndexOf("dc=");
            var dcList = adsPath.Substring(startIndex).Split(",".ToCharArray());

            var domain = "";
            foreach (var dc in dcList)
            {
                if (dc == "dc=itegroup" || dc == "dc=com") continue;
                domain = dc.Replace("dc=", "");
                break;
            }

            var indexOfAtSign = user.UserPrincipalName.IndexOf("@");
            var samAccountName = user.UserPrincipalName.Substring(0, indexOfAtSign);

            return domain + @"\" + samAccountName;
        }
    }
}
