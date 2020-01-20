using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.EditionTranslation
{
    public class EditionTranslationEditModel
    {
        public int Id { get; set; }
        
        public int EditionId { get; set; }

        [Display(Name = "Language", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string LanguageCode { get; set; }

        [Display(Name = "EditionName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string EventName { get; set; }

        [Display(Name = "EditionVenue", ResourceType = typeof(Resources.Resources))]
        public string VenueName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resources.Resources))]
        public string EditionDescription { get; set; }

        [Display(Name = "Summary", ResourceType = typeof(Resources.Resources))]
        public string EditionSummary { get; set; }

        [Display(Name = "BookStandUrl", ResourceType = typeof(Resources.Resources))]
        public string BookStandUrl { get; set; }

        [Display(Name = "OnlineInvitationUrl", ResourceType = typeof(Resources.Resources))]
        public string OnlineInvitationUrl { get; set; }

        //[Display(Name = "DisplayDate", ResourceType = typeof(Resources.Resources))]
        //public string DisplayDate { get; set; }
    }
}