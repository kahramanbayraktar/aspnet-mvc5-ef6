using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.EditionSection
{
    public class EditionSectionAddModel
    {
        public int EditionId { get; set; }

        [Required]
        [StringLength(500)]
        public string Sections { get; set; }
    }
}