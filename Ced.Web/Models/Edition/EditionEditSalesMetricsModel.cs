using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.Edition
{
    public class EditionEditSalesMetricsModel : EditionBaseModel
    {
        [Display(Name = "LocalSqmSold", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public decimal? LocalSqmSold { get; set; }

        [Display(Name = "InternationalSqmSold", ResourceType = typeof(Resources.Resources))]
        public decimal? InternationalSqmSold { get; set; }

        //[Display(Name = "SqmSold", ResourceType = typeof(Resources.Resources))]
        public decimal? SqmSold { get; set; }

        [Display(Name = "NumberOfSponsors", ResourceType = typeof(Resources.Resources))]
        public byte? SponsorCount { get; set; }


    }
}