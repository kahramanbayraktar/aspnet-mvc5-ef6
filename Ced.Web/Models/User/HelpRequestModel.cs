using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.User
{
    public class HelpRequestModel
    {
        //[Display(Name = "Message", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessage = "Question/Comment field is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Message { get; set; }
    }
}