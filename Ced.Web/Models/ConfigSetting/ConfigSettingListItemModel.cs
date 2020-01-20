namespace Ced.Web.Models.ConfigSetting
{
    public class ConfigSettingListItemModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public ConfigSettingType Type { get; set; }
    }

    public enum ConfigSettingType
    {
        AppSetting,
        ConnectionString,
        Other
    }
}