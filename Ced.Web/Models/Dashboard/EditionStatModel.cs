using System.Collections.Generic;
using Ced.BusinessServices.Models;

namespace Ced.Web.Models.Dashboard
{
    public class EditionStatModel
    {
        public List<string> ExhibitorCountries { get; set; }
        
        public List<string> VisitorCountries { get; set; }
        
        public List<string> DelegateCountries { get; set; }

        public string BookAStandUrl { get; set; }

        public string VisitorETicketUrl { get; set; }

        public IList<EditionStat> NumberOfInternationalPavilions { get; set; }

        public IList<EditionStat> NumberOfExhibitorCountries { get; set; }

        public IList<EditionStat> NumberOfVisitorCountries { get; set; }
        
        public IList<EditionStat> LocalExhibitorRetentionRates { get; set; }

        public IList<EditionStat> InternationalExhibitorRetentionRates { get; set; }
    }
}