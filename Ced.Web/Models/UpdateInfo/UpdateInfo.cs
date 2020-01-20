using System.Collections.Generic;

namespace Ced.Web.Models.UpdateInfo
{
    public class UpdateInfo // : IUpdateInfo
    {
        public static string CombineUpdateInfos(IList<string> updateInfo, UpdateDisplayType displayType)
        {
            var connector = "";

            switch (displayType)
            {
                case UpdateDisplayType.Json:
                case UpdateDisplayType.DetailedJson:
                    connector = ",";
                    break;
                case UpdateDisplayType.Html:
                case UpdateDisplayType.DetailedHtml:
                    connector = "<br/><br/>";
                    break;
            }

            var result = "";

            foreach (var ui in updateInfo)
            {
                if (!string.IsNullOrWhiteSpace(ui))
                    result += ui + connector;
            }

            return result;
        }
    }
}