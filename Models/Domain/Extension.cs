using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Extension.title)]
    public class Extension : IValidatableObject
    {

        [Key]
        [DisplayName(ModelsTexts.Extension.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Extension.CodeTitle)]
        [StringLength(15)]
        public string? Code { get; set; }

        [DisplayName(ModelsTexts.Extension.ProjectTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Project { get; set; }

        [DisplayName(ModelsTexts.Extension.DaysTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public int? Days { get; set; }
        [DisplayName(ModelsTexts.Extension.StageTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Stage { get; set; }

        [DisplayName(ModelsTexts.Extension.DateDeliveryTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateDelivery { get; set; }
        [DisplayName(ModelsTexts.Extension.DateApprovedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateApproved { get; set; }

        [DisplayName(ModelsTexts.Extension.MotiveTitle)]
        [StringLength(250)]
        public string? Motive { get; set; }

        [DisplayName(ModelsTexts.Extension.AttachmentTitle)]
        public string? Attachment { get; set; }


        // linked fields
        [ForeignKey("Project")]
        virtual public Project? Project_info { get; set; }
        [ForeignKey("Stage")]
        virtual public TaskStage? Stage_info { get; set; }

        // children
        [DisplayName(ModelsTexts.ExtensionAttachment.titlePlural)]
        public ICollection<ExtensionAttachment>? ExtensionAttachments { get; set; }

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
