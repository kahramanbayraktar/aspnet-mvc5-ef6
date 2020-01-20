using System.ComponentModel.DataAnnotations;
using Ced.Web.Models.Edition;

namespace Ced.Web.Models.ValidationAttributes
{
    public class CleanedLocalName : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var editionCloneModel = (EditionCloneModel) validationContext.ObjectInstance;

            if (editionCloneModel.LocalName.ToLower().Contains("copy of "))
                return new ValidationResult("You must reset Local Name properly.");
            return ValidationResult.Success;
        }
    }
}