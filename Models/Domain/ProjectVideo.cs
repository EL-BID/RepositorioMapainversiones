using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.ProjectVideo.title)]
    public class ProjectVideo : IValidatableObject
    {

        [Key]
        [DisplayName(ModelsTexts.ProjectVideo.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.ProjectVideo.ProjectTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Project { get; set; }

        [DisplayName(ModelsTexts.ProjectVideo.LinkTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(255)]
        public string Link { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.ProjectVideo.DescriptionTitle)]
        public string? Description { get; set; }

        [DisplayName(ModelsTexts.ProjectVideo.VideoDateTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? VideoDate { get; set; }

        [DisplayName(ModelsTexts.ProjectVideo.UploadDateTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? UploadDate { get; set; }

        // linked fields
        [ForeignKey("Project")]
        virtual public Project? Project_info { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (VideoDate > DateTime.Now)
            {
                yield return new ValidationResult(
                    errorMessage: "Fecha del video no admite fechas futuras",
                    memberNames: new[] { "VideoDate" }
               );
            }
        }
    }
}
