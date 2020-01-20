using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Ced.Utility.Web
{
    public class WebConfigHelper
    {
        public static int ApplicationIdCed => Convert.ToInt32(ConfigurationManager.AppSettings["AppId-CED"]);

        public static IList<int> EditionNotificationEditionExistence
        {
            get
            {
                var val = ConfigurationManager.AppSettings["EditionNotification_EditionExistence"];
                return string.IsNullOrWhiteSpace(val) ? new List<int>() : val.Split(',').Select(int.Parse).ToList();
            }
        }

        public static IList<int> EditionNotificationDaysLeftToStartForGeneralInfo
        {
            get
            {
                var val = ConfigurationManager.AppSettings["EditionNotification_DaysLeftToStart_ForGeneralInfo"];
                return string.IsNullOrWhiteSpace(val) ? new List<int>() : val.Split(',').Select(int.Parse).ToList();
            }
        }

        public static IList<int> EditionNotificationDaysPassedAfterEndForPostShowMetricsInfo
        {
            get
            {
                var val = ConfigurationManager.AppSettings["EditionNotification_DaysPassedAfterEnd_ForPostShowMetricsInfo"];
                return string.IsNullOrWhiteSpace(val) ? new List<int>() : val.Split(',').Select(int.Parse).ToList();
            }
        }

        public static IList<int> EditionNotificationDaysPassedAfterEndForPostShowMetricsInfo2
        {
            get
            {
                var val = ConfigurationManager.AppSettings["EditionNotification_DaysPassedAfterEnd_ForPostShowMetricsInfo2"];
                return string.IsNullOrWhiteSpace(val) ? new List<int>() : val.Split(',').Select(int.Parse).ToList();
            }
        }

        public static int EditionNotificationDeviationInDays => Convert.ToInt32(ConfigurationManager.AppSettings["EditionNotification_DeviationInDays"]);

        public static int EditionLifeSpan => Convert.ToInt32(ConfigurationManager.AppSettings["EditionLifeSpan"]);

        public static int MinFinancialYear => Convert.ToInt32(ConfigurationManager.AppSettings["MinFinancialYear"]);

        public static int CohostEditionsAcceptanceNumberOfDays => Convert.ToInt32(ConfigurationManager.AppSettings["CohostEditionsAcceptanceNumberOfDays"]);

        public static string TaskSchedulerSecretKey => ConfigurationManager.AppSettings["TaskSchedulerSecretKey"];

        public static bool CloningAllowed => Convert.ToBoolean(ConfigurationManager.AppSettings["CloningAllowed"]);

        public static string HelpDeskUserName => ConfigurationManager.AppSettings["HelpDeskUserName"];

        public static string AdminEmails => ConfigurationManager.AppSettings["AdminEmails"];

        public static string MarketingAdminEmails => ConfigurationManager.AppSettings["MarketingAdminEmails"];

        public static string NewEventNotificationRecipients => ConfigurationManager.AppSettings["NewEventNotificationRecipients"];

        public static string ApplicationAbsolutePath => ConfigurationManager.AppSettings["ApplicationAbsolutePath"];

        public static bool IsLocal => Convert.ToBoolean(ConfigurationManager.AppSettings["IsLocal"]);

        public static bool IsTest => Convert.ToBoolean(ConfigurationManager.AppSettings["IsTest"]);

        public static bool PrimaryDirectorNotifications => Convert.ToBoolean(ConfigurationManager.AppSettings["PrimaryDirectorNotifications"]);

        public static bool PrimaryDirectorNotificationsUseMockRecipients => Convert.ToBoolean(ConfigurationManager.AppSettings["PrimaryDirectorNotifications_UseMockRecipients"]);

        public static bool TrackDraftEditionStatusChange => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackDraftEditionStatusChange"]);

        public static bool TrackDraftEditionStatusChangeUseMockRecipients => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackDraftEditionStatusChange_UseMockRecipients"]);

        public static bool TrackEditionUpdate => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackEditionUpdate"]);

        public static bool TrackEditionUpdateUseMockRecipients => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackEditionUpdate_UseMockRecipients"]);

        public static bool TrackEditionNameUpdate => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackEditionNameUpdate"]);

        public static bool TrackEditionNameUpdateMockRecipients => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackEditionNameUpdateMockRecipients"]);

        public static string TrackEditionNameUpdateAdditionalRecipients => ConfigurationManager.AppSettings["TrackEditionNameUpdate_AdditionalRecipients"];

        public static bool TrackEditionLocationUpdate => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackEditionLocationUpdate"]);

        public static bool TrackEditionLocationUpdateUseMockRecipients => Convert.ToBoolean(ConfigurationManager.AppSettings["TrackEditionLocationUpdate_UseMockRecipients"]);

        public static bool RemoveActorUserFromNotificationRecipients => Convert.ToBoolean(ConfigurationManager.AppSettings["RemoveActorUserFromNotificationRecipients"]);

        public static string CedLogoUrl => ConfigurationManager.AppSettings["CedLogoUrl"];

        public static string JwtCookieName => ConfigurationManager.AppSettings["JwtCookieName"];

        public static int AuthCookieLifeSpan => Convert.ToInt32(ConfigurationManager.AppSettings["AuthCookieLifeSpanInMins"]);

        public static string WebApiKey => ConfigurationManager.AppSettings["WebApiKey"];

        public static string QuickStartGuideFilePath => ConfigurationManager.AppSettings["QuickStartGuideFilePath"];
    }
}