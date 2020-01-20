using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.User
{
    public class AccessRequestModel
    {
        [Display(Name = "FirstName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "CorporateEmail", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        
        [Display(Name = "OfficeOrCompanyName", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string OfficeName { get; set; }

        [Display(Name = "EventNames", ResourceType = typeof(Resources.Resources))]
        [Required]
        public string EventNames { get; set; }

        [Display(Name = "AdditionalNotes", ResourceType = typeof(Resources.Resources))]
        public string AdditionalNotes { get; set; }
    }
}