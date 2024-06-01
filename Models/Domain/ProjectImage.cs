using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.ProjectImage.title)]
    public class ProjectImage : IValidatableObject
    {

        [Key]
        [DisplayName(ModelsTexts.ProjectImage.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.ProjectImage.ProjectTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Project { get; set; }

        [DisplayName(ModelsTexts.ProjectImage.FileTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public string File { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.ProjectImage.DescriptionTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public string Description { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.ProjectImage.ImageDateTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? ImageDate { get; set; }

        [DisplayName(ModelsTexts.ProjectImage.UploadDateTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? UploadDate { get; set; }

        // linked fields
        [ForeignKey("Project")]
        virtual public Project? Project_info { get; set; }

        // View fields for file upload (input type="file"
        [NotMapped]
        public IFormFile? FileInput { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ImageDate > DateTime.Now)
            {
                yield return new ValidationResult(
                    errorMessage: "Fecha no admite fechas futuras",
                    memberNames: new[] { "ImageDate" }
               );
            }
        }
    }
}
