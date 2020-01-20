using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.Edition
{
    public class EditionRejectionModel
    {
        public int EditionId { get; set; }

        [Required]
        public string Reason { get; set; }
    }
}