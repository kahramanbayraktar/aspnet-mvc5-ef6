using Ced.BusinessEntities;
using Ced.Utility.Edition;
using Ced.Web.Models.UpdateInfo;
using ITE.Utility.Extensions;
using ITE.Utility.ObjectComparison;
using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.Web.Helpers
{
    public class NotificationControllerHelper
    {
        private static IEditionHelper _editionHelper = new EditionHelper();

        public static string GetUpdatedFields(object current, object updated, string objName, UpdateDisplayType displayType)
        {
            var diff = current.Compare(updated);

            if (!diff.Any()) return string.Empty;

            switch (displayType)
            {
                case UpdateDisplayType.Json:
                    return GetUpdatedFieldsAsJson(objName, diff);
                case UpdateDisplayType.Text:
                    return GetUpdatedFieldsAsText(objName, diff);
                case UpdateDisplayType.Html:
                    return GetUpdatedFieldsAsHtml(objName, diff);
                //case UpdatedFieldsDisplayType.DetailedJson:
                //    return GetUpdatedFields(objName, diff);
                case UpdateDisplayType.DetailedText:
                    return GetUpdatedFieldsAsDetailedText(objName, diff);
                case UpdateDisplayType.DetailedHtml:
                    return GetUpdatedFieldsAsDetailedHtml(diff);
            }

            return GetUpdatedFieldsAsJson("Edition", diff);
        }

        public static string GetUpdatedFieldsAsJson(string objName, List<Variance> diff)
        {
            if (diff.Count > 0)
                return $"\"{objName}\":[" + string.Join(",", diff.Select(x => "\"" + x.Prop + "\"")) + "]";
            return "";
        }

        private static string GetUpdatedFieldsAsText(string objName, List<Variance> diff)
        {
            if (diff.Count > 0)
                return $"{objName}: " + string.Join(",", diff.Select(x => x.Prop));
            return "";
        }

        private static string GetUpdatedFieldsAsHtml(string objName, List<Variance> diff)
        {
            if (diff.Count > 0)
                return $"<b>{objName}<b><br/>" + string.Join("<br/>", diff.Select(x => x.Prop));
            return "";
        }

        private static string GetUpdatedFieldsAsDetailedText(string objName, List<Variance> diff)
        {
            if (diff.Count > 0)
            {
                var result = objName + "<br/>";

                foreach (var d in diff)
                {
                    result += d.Prop + "<br/>";
                    result += "PREVIOUSLY:<br/>" + d.ValA + "<br/>";
                    result += "CURRENTLY:<br/>" + d.ValB + "<br/><br/>";
                }

                return result;
            }

            //if (diff.Count > 0)
            //    return $"{objName}: " + string.Join(",", diff.Select(x => x.Prop));
            return "";
        }

        public static string GetUpdatedFieldsAsDetailedHtml(List<Variance> diff)
        {
            if (diff.Count > 0)
            {
                var result = "";

                foreach (var d in diff)
                {
                    result += "<b>" + d.Prop + "</b><br/>";
                    if (!(d.ValA == null && d.ValB == null))
                    {
                        result += "<b>PREVIOUSLY:</b><br/>" + d.ValA + "<br/>";
                        result += "<b>CURRENTLY:</b><br/>" + d.ValB + "<br/><br/>";
                    }
                }

                return result;
            }

            //if (diff.Count > 0)
            //    return $"{objName}: " + string.Join(",", diff.Select(x => x.Prop));
            return "";
        }

        private static List<Variance> PrepareDiff(List<Variance> diff)
        {
            foreach (var d in diff)
            {
                if (d.ValB != null && d.ValB.ToString().IsImage())
                {
                    var dims = _editionHelper.GetImageSizes(d.Prop);

                    var actualWidth = dims.First();
                    var limitedWidth = actualWidth > 500 ? 500 : actualWidth;

                    if (d.ValB != null && !string.IsNullOrWhiteSpace(d.ValB.ToString()))
                    {
                        var html =
                            $"<!--[if (mso)|(IE)]><table align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\"><tr class=\"layout-fixed-width\" emb-background-style><td style=\"width: {limitedWidth}px\" valign=\"top\" class=\"w260\"><![endif]-->" +
                            $"<div class=\"column\" style=\"text-align: left; color: #888794; font-size: 14px; line-height: 21px; font-family: Roboto,Tahoma,sans-serif; float: left; max-width: {limitedWidth}px; min-width: {limitedWidth}px; width: {limitedWidth}px; width: calc(12300px - 2000%);\">" +
                            "<div style=\"margin-left: 20px; margin-right: 20px; margin-top: 12px;\">" +
                            "<div style=\"line-height:10px; font-size:1px\">&nbsp;</div>" +
                            "</div>" +
                            "<div style=\"margin-left: 20px; margin-right: 20px; margin-bottom: 12px;\">" +
                            "<div style=\"font-size: 12px; font-style: normal; font-weight: normal;\" align=\"left\">" +
                            $"<img style=\"border: 0; display: block; height: auto; width: 100%; max-width: {limitedWidth}px;\" alt=\"\" width=\"{limitedWidth}\" src=\"{d.ValB}\" />" +
                            "</div>" +
                            "</div>" +
                            "</div>" +
                            "<!--[if (mso)|(IE)]></td></tr></table><![endif]-->";
                        d.ValB = html;
                    }

                    d.IsFile = true;
                }
            }
            return diff;
        }

        public static string GetUpdatedContent(EditionEntity current, EditionEntity updated)
        {
            var diff = current.Compare(updated);
            diff = PrepareDiff(diff);
            var updateInfo = new HtmlUpdateInfo("Edition", diff, true);
            var updatedContent = updateInfo.ComposeContent();
            return updatedContent;
        }

        public static string GetUpdatedContent(EditionTranslationEntity current, EditionTranslationEntity updated)
        {
            var diff = current.Compare(updated);
            diff = PrepareDiff(diff);
            var updateInfo = new HtmlUpdateInfo("EditionTranslation", diff, true);
            var updatedContent = updateInfo.ComposeContent();
            return updatedContent;
        }

        public static string GetUpdatedContent(FileEntity current, FileEntity updated)
        {
            var diff = current.Compare(updated);
            diff = PrepareDiff(diff);
            var updateInfo = new HtmlUpdateInfo("File", diff, true);
            var updatedContent = updateInfo.ComposeContent();
            return updatedContent;
        }

        public static string GetUpdatedContent(IList<EditionVisitorEntity> currentEditionVisitors, IList<EditionVisitorEntity> updatedEditionVisitors)
        {
            var diffAvailable = false;
            if (!currentEditionVisitors.Any())
            {
                if (updatedEditionVisitors.Any()) // Being saved for the first time
                {
                    if (updatedEditionVisitors.Any(x => x.VisitorCount > 0 || x.RepeatVisitCount > 0 || x.OldVisitorCount > 0 || x.NewVisitorCount > 0)) // Update
                    {
                        diffAvailable = true;
                    }
                }
            }
            else // Records were saved before
            {
                if (updatedEditionVisitors.Any())
                {
                    diffAvailable = true;
                }
            }

            return diffAvailable ? CompareEditionVisitors(currentEditionVisitors, updatedEditionVisitors) : "";
        }

        private static string CompareEditionVisitors(IList<EditionVisitorEntity> currentEditionVisitors, IList<EditionVisitorEntity> updatedEditionVisitors)
        {
            var diffs = new List<Variance>();

            if (currentEditionVisitors == null || currentEditionVisitors.Count == 0)
            {
                currentEditionVisitors = CreateEmptyEditionVisitorsForComparison(updatedEditionVisitors);
            }

            foreach (var currentEditionVisitor in currentEditionVisitors)
            {
                var editionVisitor = updatedEditionVisitors.SingleOrDefault(x =>
                    x.EditionId == currentEditionVisitor.EditionId
                    && x.DayNumber == currentEditionVisitor.DayNumber);
                if (editionVisitor != null)
                {
                    var diff = currentEditionVisitor.Compare(editionVisitor);
                    diffs.AddRange(diff);
                }
            }

            var updateInfo = new HtmlUpdateInfo("EditionVisitor", diffs, true);
            var updatedContent = updateInfo.ComposeContent();
            return updatedContent;
        }

        private static IList<EditionVisitorEntity> CreateEmptyEditionVisitorsForComparison(IList<EditionVisitorEntity> updatedEditionVisitors)
        {
            var currentEditionVisitors = new List<EditionVisitorEntity>();
            updatedEditionVisitors.ForEach(item =>
            {
                currentEditionVisitors.Add((EditionVisitorEntity)item.Clone());
            });

            currentEditionVisitors.ForEach(x =>
            {
                x.VisitorCount = 0;
                x.RepeatVisitCount = x.RepeatVisitCount != null ? (short?)0 : null;
                x.OldVisitorCount = 0;
                x.NewVisitorCount = 0;
            });

            return currentEditionVisitors;
        }

        public static string RemoveCurrentUserFromRecipients(string recipients, string actorUserEmail)
        {
            if (!string.IsNullOrWhiteSpace(recipients) && !string.IsNullOrEmpty(actorUserEmail))
            {
                var recipientList = recipients.Split(Utility.Constants.EmailAddressSeparator).ToList();
                recipientList.Remove(recipientList.SingleOrDefault(x => string.Equals(x.ToString(), actorUserEmail, StringComparison.CurrentCultureIgnoreCase)));
                recipients = recipientList.JoinStrings(Utility.Constants.EmailAddressSeparator.ToString());
            }
            return recipients;
        }
    }
}