using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Web.Models.ConfigSetting;
using ITE.Utility.Extensions;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class ConfigSettingController : GlobalController
    {
        public ConfigSettingController(
            BusinessServices.Auth.IUserServices authUserServices,
            BusinessServices.Auth.IRoleServices roleServices,
            BusinessServices.Auth.IApplicationServices applicationServices,
            BusinessServices.Auth.IIndustryServices industryServices,
            BusinessServices.Auth.IRegionServices regionServices,
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

        public ActionResult Index()
        {
            var model = new ConfigSettingListModel
            {
                ConfigSettings = GetConfigSettingListItems()
            };
            return View(model);
        }

        #region HELPER METHODS

        private IList<ConfigSettingListItemModel> GetConfigSettingListItems(string key = null)
        {
            var webConfigs = new List<ConfigSettingListItemModel>();

            foreach (var k in ConfigurationManager.AppSettings.AllKeys)
            {
                webConfigs.Add(new ConfigSettingListItemModel
                {
                    Key = k,
                    Value = ConfigurationManager.AppSettings[k],
                    Type = ConfigSettingType.AppSetting
                });
            }

            webConfigs.Add(new ConfigSettingListItemModel
            {
                Key = "CedContext",
                Value = ConfigurationManager.ConnectionStrings["CedContext"].ConnectionString.Replace(";", "; "),
                Type = ConfigSettingType.ConnectionString
            });

            webConfigs.Add(new ConfigSettingListItemModel
            {
                Key = "Ced.DWStagingEntities",
                Value = ConfigurationManager.ConnectionStrings["Ced.DWStagingEntities"].ConnectionString.Replace(";", "; "),
                Type = ConfigSettingType.ConnectionString
            });

            // TODO:
            webConfigs.Add(new ConfigSettingListItemModel
            {
                Key = "ApplicationPath",
                Value = Url.RequestContext.HttpContext.Request.ApplicationPath,
                Type = ConfigSettingType.Other
            });
            webConfigs.Add(new ConfigSettingListItemModel
            {
                Key = "SampleActionUrl",
                Value = Url.Action("Edit", "Edition", new { id = 1300, name = "TURKEYBUILD ISTANBUL 2017".ToUrlString() }),
                Type = ConfigSettingType.Other
            });

            return webConfigs;
        }

        #endregion
    }
}