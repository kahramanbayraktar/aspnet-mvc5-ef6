using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.EditionDiscountApprover
{
    public class EditionDiscountApproverAddModel
    {
        public int EditionId { get; set; }

        [Required]
        [StringLength(100)]
        public string ApprovingUser { get; set; }

        [Required]
        [Range(0, 100)]
        public int? ApprovalLowerPercentage { get; set; }

        [Required]
        [Range(0, 100)]
        public int? ApprovalUpperPercentage { get; set; }
    }
}