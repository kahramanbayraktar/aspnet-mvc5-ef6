using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Linq;

namespace Ced.Web.Models.UpdateInfo
{
    public class HtmlUpdateInfo : IUpdateInfo
    {
        private readonly string _objName;

        private readonly IList<Variance> _diff;

        private readonly bool? _isDetailed;

        public HtmlUpdateInfo(string objName, IList<Variance> diff, bool? isDetailed = false)
        {
            _objName = objName;
            _diff = diff;
            _isDetailed = isDetailed;
        }

        public string ComposeContent()
        {
            var content = "";
            if (_diff.Count > 0)
            {
                if (_isDetailed == true)
                {
                    foreach (var d in _diff)
                    {
                        d.Prop = d.Prop.ToUpper();

                        if (d.IsFile)
                        {
                            if ((d.ValA == null || string.IsNullOrWhiteSpace(d.ValA.ToString()))
                                && (d.ValB != null && !string.IsNullOrWhiteSpace(d.ValB.ToString())))
                            {
                                content += "<b>" + d.Prop + " (added)</b><br/>";
                                content += d.ValB + "<br/><br/>";
                            }
                            else if ((d.ValA != null && !string.IsNullOrWhiteSpace(d.ValA.ToString()))
                                     && (d.ValB != null && !string.IsNullOrWhiteSpace(d.ValB.ToString())))
                            {
                                content += "<b>" + d.Prop + " (updated)</b><br/>";
                                content += d.ValB + "<br/><br/>";
                            }
                            else if ((d.ValA != null && !string.IsNullOrWhiteSpace(d.ValA.ToString()))
                                     && (d.ValB == null || string.IsNullOrWhiteSpace(d.ValB.ToString())))
                            {
                                content += "<b>" + d.Prop + " (deleted)</b><br/>";
                            }
                        }
                        else
                        {
                            content += "<b>" + d.Prop + "</b><br/>";
                            if (!(d.ValA == null && d.ValB == null))
                            {
                                content += "<b><i><u>Previously:</u></i></b><br/><i>" + d.ValA + "</i><br/>";
                                content += "<b><u>Currently:</u></b><br/>" + d.ValB + "<br/><br/>";
                            }
                        }
                    }
                }
                else
                {
                    content = $"<b>{_objName}<b><br/>" + string.Join("<br/>", _diff.Select(x => x.Prop));
                }
            }
            return content;
        }
    }
}