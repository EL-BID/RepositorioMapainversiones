using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.AdditionAttachment.title)]
    public class AdditionAttachment
    {

        [Key]
        [DisplayName(ModelsTexts.AdditionAttachment.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.AdditionAttachment.AdditionTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Addition { get; set; }

        [DisplayName(ModelsTexts.AdditionAttachment.TitleTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.AdditionAttachment.FileNameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public string FileName { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.AdditionAttachment.DateAttachedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateAttached { get; set; }

        // linked fields
        [ForeignKey("Addition")]
        virtual public Addition? Addition_info { get; set; }

        // View fields for file upload (input type="file"
        [NotMapped]
        public IFormFile? FileNameInput { get; set; }
    }
}
