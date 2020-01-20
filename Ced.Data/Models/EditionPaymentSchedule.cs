using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ced.Data.Models
{
    [Table("EditionPaymentSchedule")]
    public class EditionPaymentSchedule
    {
        public int EditionPaymentScheduleId { get; set; }

        public int EditionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime ActivationDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

        [Range(0, 100)]
        public int Installment1Percentage { get; set; }

        [Column(TypeName = "date")]
        public DateTime Installment1DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment2Percentage { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Installment2DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment3Percentage { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Installment3DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment4Percentage { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Installment4DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment5Percentage { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Installment5DueDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }


        public virtual Edition Edition { get; set; }
    }
}
