using System.Collections.Generic;

namespace Ced.Web.Models.Select2
{
    // Extra class to format the results the way the select2 dropdown wants them
    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<Select2Result> Results { get; set; }
    }
}