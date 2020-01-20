using System;
using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.EditionPaymentSchedule
{
    public class EditionPaymentScheduleAddModel
    {
        public int EditionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ActivationDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [Range(0, 100)]
        public int Installment1Percentage { get; set; }

        [Required]
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
    }
}