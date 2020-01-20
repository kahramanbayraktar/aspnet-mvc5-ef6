using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.BusinessEntities
{
    public class EditionDiscountApproverEntity
    {
        public int EditionDiscountApproverId { get; set; }

        public int EditionId { get; set; }

        [Required]
        [StringLength(100)]
        public string ApprovingUser { get; set; }

        [Required]
        [Range(0, 100)]
        public int ApprovalLowerPercentage { get; set; }

        [Required]
        [Range(0, 100)]
        public int ApprovalUpperPercentage { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }


        public EditionEntity Edition { get; set; }
    }
}
