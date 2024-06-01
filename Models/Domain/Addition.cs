using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Addition.title)]
    public class Addition : IValidatableObject
    {

        [Key]
        [DisplayName(ModelsTexts.Addition.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Addition.CodeTitle)]
        [StringLength(15)]
        public string? Code { get; set; }

        [DisplayName(ModelsTexts.Addition.ProductTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Product { get; set; }

        [DisplayName(ModelsTexts.Addition.ValueTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public double? Value { get; set; }
        [DisplayName(ModelsTexts.Addition.StageTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Stage { get; set; }

        [DisplayName(ModelsTexts.Addition.DateDeliveryTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateDelivery { get; set; }
        [DisplayName(ModelsTexts.Addition.DateApprovedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateApproved { get; set; }

        [DisplayName(ModelsTexts.Addition.NotesTitle)]
        public string? Notes { get; set; }

        [DisplayName(ModelsTexts.Addition.AttachmentTitle)]
        public string? Attachment { get; set; }


        // linked fields
        [ForeignKey("Product")]
        virtual public Product? Product_info { get; set; }
        [ForeignKey("Stage")]
        virtual public TaskStage? Stage_info { get; set; }

        // children
        [DisplayName(ModelsTexts.AdditionAttachment.titlePlural)]
        public ICollection<AdditionAttachment>? AdditionAttachments { get; set; }

        // View fields for file upload (input type="file"
        [NotMapped]
        public IFormFile? AttachmentInput { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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

            // Validaciones fechas y estados
            if ((Stage == ProjectGlobals.TaskStage.Aprobada) && (DateApproved == null || DateApproved <= ProjectGlobals.MinValidDate))
                        {
                            yield return new ValidationResult(
                                errorMessage: "Fecha Aprobado requerida para estado Aprobada.",
                                memberNames: new[] { "DateApproved" }
                           );
                        }
            else
                        if ((Stage < ProjectGlobals.TaskStage.Aprobada) && (DateApproved != null && DateApproved > ProjectGlobals.MinValidDate))
                        {
                            yield return new ValidationResult(
                                errorMessage: "Si tiene fecha Aprobada, debe pasar a Estado Aprobada.",
                                memberNames: new[] { "Stage" }
                           );
                        }

        }
    }
}
