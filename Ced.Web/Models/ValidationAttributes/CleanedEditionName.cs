using System.ComponentModel.DataAnnotations;
using Ced.Web.Models.Edition;

namespace Ced.Web.Models.ValidationAttributes
{
    public class CleanedEditionName : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var editionCloneModel = (EditionCloneModel) validationContext.ObjectInstance;

            if (editionCloneModel.EditionName.ToLower().Contains("copy of "))
                return new ValidationResult("You must reset Edition Name properly.");
            return ValidationResult.Success;

        }
    }
}