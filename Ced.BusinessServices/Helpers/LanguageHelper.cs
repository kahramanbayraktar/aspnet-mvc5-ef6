using System;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using ITE.Utility.Extensions;

namespace Ced.BusinessServices.Helpers
{
    //public class LanguageHelper
    //{
    //    public static string GetCurrentLanguageCultureName()
    //    {
    //        return Thread.CurrentThread.CurrentCulture.Name;
    //    }

    //    public static Languages GetCurrentLanguage()
    //    {
    //        return GetCurrentLanguageCultureName().ToLower().ToEnumFromDescription<Languages>();
    //    }

    //    public static string GetBaseLanguageCultureName()
    //    {
    //        return ConfigurationManager.AppSettings["BaseLanguageCode"].ToLower();
    //    }

    //    public static Languages GetBasedLanguage()
    //    {
    //        return GetBaseLanguageCultureName().ToEnumFromDescription<Languages>();
    //    }

    //    public enum Languages
    //    {
    //        [Description("en-gb")]
    //        [Language(LanguageCode = "en-gb", Name = "English", LongName = "English")]
    //        English,

    //        [Description("ru-ru")]
    //        [Language(LanguageCode = "ru-ru", Name = "Russian", LongName = "Russian")]
    //        Russian,

    //        [Description("tr-tr")]
    //        [Language(LanguageCode = "tr-tr", Name = "Turkish", LongName = "Turkish")]
    //        Turkish,

    //        [Description("zn-ch")]
    //        [Language(LanguageCode = "zn-ch", Name = "Chinese", LongName = "Chinese (Simplified)")]
    //        Chinese
    //    }

    //    [AttributeUsage(AttributeTargets.Field)]
    //    public class LanguageAttribute : Attribute
    //    {
    //        public string LanguageCode { get; set; }

    //        public string Name { get; set; }

    //        public string LongName { get; set; }
    //    }
    //}
}