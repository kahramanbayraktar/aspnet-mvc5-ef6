using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.BusinessEntities
{
    public class EditionPaymentScheduleEntity
    {
        public int EditionPaymentScheduleId { get; set; }

        public int EditionId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public DateTime ActivationDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        [Range(0, 100)]
        public int Installment1Percentage { get; set; }

        public DateTime Installment1DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment2Percentage { get; set; }

        public DateTime? Installment2DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment3Percentage { get; set; }

        public DateTime? Installment3DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment4Percentage { get; set; }

        public DateTime? Installment4DueDate { get; set; }

        [Range(0, 100)]
        public int? Installment5Percentage { get; set; }

        public DateTime? Installment5DueDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }


        public EditionEntity Edition { get; set; }
    }
}
