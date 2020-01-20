using System.ComponentModel.DataAnnotations;
using Ced.Web.Models.Edition;

namespace Ced.Web.Models.ValidationAttributes
{
    public class CleanedInternationalName : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var editionCloneModel = (EditionCloneModel)validationContext.ObjectInstance;

            if (editionCloneModel.InternationalName.ToLower().Contains("copy of "))
                return new ValidationResult("You must reset International Name properly.");
            return ValidationResult.Success;
        }
    }
}