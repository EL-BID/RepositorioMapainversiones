using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.ProjectAttachment.title)]
    public class ProjectAttachment
    {

        [Key]
        [DisplayName(ModelsTexts.ProjectAttachment.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.ProjectAttachment.ProjectTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Project { get; set; }

        [DisplayName(ModelsTexts.ProjectAttachment.TitleTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.ProjectAttachment.FileNameTitle)]
        public string? FileName { get; set; }

        [DisplayName(ModelsTexts.ProjectAttachment.DateAttachedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateAttached { get; set; }

        // linked fields
        [ForeignKey("Project")]
        virtual public Project? Project_info { get; set; }

        // View fields for file upload (input type="file"
        [NotMapped]
        public IFormFile? FileNameInput { get; set; }
    }
}
