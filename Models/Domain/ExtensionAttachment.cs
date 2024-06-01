using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.ExtensionAttachment.title)]
    public class ExtensionAttachment
    {

        [Key]
        [DisplayName(ModelsTexts.ExtensionAttachment.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.ExtensionAttachment.ExtensionTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Extension { get; set; }

        [DisplayName(ModelsTexts.ExtensionAttachment.TitleTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.ExtensionAttachment.FileNameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public string FileName { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.ExtensionAttachment.DateAttachedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateAttached { get; set; }

        // linked fields
        [ForeignKey("Extension")]
        virtual public Extension? Extension_info { get; set; }

        // View fields for file upload (input type="file"
        [NotMapped]
        public IFormFile? FileNameInput { get; set; }
    }
}
