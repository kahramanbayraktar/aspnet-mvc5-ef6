using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.User
{
    public class LoginModel
    {
        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessage = @"You must enter your Email / Username")]
        public string Email { get; set; }
        
        [Display(Name = "Password", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = @"You must enter your Password")]
        public string Password { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}