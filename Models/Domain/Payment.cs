using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Payment.title)]
    public class Payment : IValidatableObject
    {

        [Key]
        [DisplayName(ModelsTexts.Payment.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Payment.CodeTitle)]
        [StringLength(15)]
        public string? Code { get; set; }

        [DisplayName(ModelsTexts.Payment.ProductTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Product { get; set; }

        [DisplayName(ModelsTexts.Payment.FundingSourceTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int FundingSource { get; set; }

        [DisplayName(ModelsTexts.Payment.ReportedMonthTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? ReportedMonth { get; set; }

        [DisplayName(ModelsTexts.Payment.PaymentAmountTitle)]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public double? PaymentAmount { get; set; }

        [DisplayName(ModelsTexts.Payment.PhysicalAdvanceTitle)]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 100, ErrorMessage = "{0} debe ser un número entre 0 y 100.")]
        public float? PhysicalAdvance { get; set; }

        [DisplayName(ModelsTexts.Payment.StageTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Stage { get; set; }

        [DisplayName(ModelsTexts.Payment.DateDeliveryTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateDelivery { get; set; }
        [DisplayName(ModelsTexts.Payment.DateApprovedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        [NotSmallerThan("DateDelivery")]
        [NotGreaterThan("DatePayed")]
        public DateTime? DateApproved { get; set; }

        [DisplayName(ModelsTexts.Payment.DatePayedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        [NotSmallerThan("DateDelivery")]
        public DateTime? DatePayed { get; set; }

        [DisplayName(ModelsTexts.Payment.AttachmentAdvanceTitle)]
        public string? AttachmentAdvance { get; set; }

        [DisplayName(ModelsTexts.Payment.AttachmentPaymentTitle)]
        public string? AttachmentPayment { get; set; }

        // linked fields
        [ForeignKey("Product")]
        virtual public Product? Product_info { get; set; }
        [ForeignKey("FundingSource")]
        virtual public ProjectFunding? FundingSource_info { get; set; }
        [ForeignKey("Stage")]
        virtual public PaymentStage? Stage_info { get; set; }

        // children
        [DisplayName(ModelsTexts.PaymentAttachment.titlePlural)]
        public ICollection<PaymentAttachment>? PaymentAttachments { get; set; }

        // View fields for file upload (input type="file"
        [NotMapped]
        public IFormFile? AttachmentAdvanceInput { get; set; }
        [NotMapped]
        public IFormFile? AttachmentPaymentInput { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReportedMonth > DateTime.Now)
            {
                yield return new ValidationResult(
                    errorMessage: "Mes de Medición no admite fechas futuras",
                    memberNames: new[] { "ReportedMonth" }
               );
            }
            if (DateDelivery > DateTime.Now)
            {
                yield return new ValidationResult(
                    errorMessage: "Fecha presentación no admite fechas futuras",
                    memberNames: new[] { "DateDelivery" }
               );
            }
            if (DateApproved > DateTime.Now)
            {
                yield return new ValidationResult(
                    errorMessage: "Fecha aprobación no admite fechas futuras",
                    memberNames: new[] { "DateApproved" }
               );
            }
            if (DatePayed > DateTime.Now)
            {
                yield return new ValidationResult(
                    errorMessage: "Fecha pagado no admite fechas futuras",
                    memberNames: new[] { "DatePayed" }
               );
            }

            // Validaciones fechas y estados
            if ((Stage == ProjectGlobals.PaymentStage.Pagado) && (DatePayed == null || DatePayed <= ProjectGlobals.MinValidDate))
            {
                yield return new ValidationResult(
                    errorMessage: "Fecha pagado requerida para estado Pagado.",
                    memberNames: new[] { "DatePayed" }
               );
            }
            if ((Stage < ProjectGlobals.PaymentStage.Pagado) && (DatePayed != null && DatePayed > ProjectGlobals.MinValidDate))
            {
                yield return new ValidationResult(
                    errorMessage: "Si tiene Fecha pagado, debe pasar a estado Pagado.",
                    memberNames: new[] { "Stage" }
               );
            }

        }
    }
}
