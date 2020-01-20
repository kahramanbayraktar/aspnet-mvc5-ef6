using ITE.Utility.ObjectComparison;
using System.Collections.Generic;
using System.Linq;

namespace Ced.Web.Models.UpdateInfo
{
    public class JsonUpdateInfo : IUpdateInfo
    {
        private readonly string _objName;

        private readonly IList<Variance> _diff;

        private readonly bool? _isDetailed;

        public JsonUpdateInfo(string objName, IList<Variance> diff, bool? isDetailed = false)
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
                    content = $"\"{_objName}\":[" + string.Join(",", _diff.Select(x => "\"" + x.Prop + "\"")) + "]";
                }
                else
                {
                    content = $"\"{_objName}\":[" + string.Join(",", _diff.Select(x => "\"" + x.Prop + "\"")) + "]";
                }
            }
            return content;
        }
    }
}