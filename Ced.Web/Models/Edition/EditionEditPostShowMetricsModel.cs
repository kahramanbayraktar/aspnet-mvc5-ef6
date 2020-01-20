using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.Edition
{
    public class EditionEditPostShowMetricsModel : EditionBaseModel
    {
        [Display(Name = "NPSScoreVisitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NPSScoreVisitor { get; set; }

        [Display(Name = "NPSScoreExhibitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NPSScoreExhibitor { get; set; }

        [Display(Name = "NPSSatisfactionVisitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NPSSatisfactionVisitor { get; set; }

        [Display(Name = "NPSSatisfactionExhibitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NPSSatisfactionExhibitor { get; set; }

        [Display(Name = "NPSAverageVisitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NPSAverageVisitor { get; set; }

        [Display(Name = "NPSAverageExhibitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NPSAverageExhibitor { get; set; }

        //[Display(Name = "NetEasyScoreVisitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NetEasyScoreVisitor { get; set; }

        //[Display(Name = "NetEasyScoreExhibitor", ResourceType = typeof(Resources.Resources))]
        public decimal? NetEasyScoreExhibitor { get; set; }
    }
}