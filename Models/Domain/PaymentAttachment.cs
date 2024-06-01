using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.PaymentAttachment.title)]
    public class PaymentAttachment
    {

        [Key]
        [DisplayName(ModelsTexts.PaymentAttachment.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.PaymentAttachment.PaymentTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Payment { get; set; }

        [DisplayName(ModelsTexts.PaymentAttachment.TitleTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.PaymentAttachment.FileTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public string File { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.PaymentAttachment.DateAttachedTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? DateAttached { get; set; }

        // linked fields
        [ForeignKey("Payment")]
        virtual public Payment? Payment_info { get; set; }

        // View fields for file upload (input type="file"
        [NotMapped]
        public IFormFile? FileInput { get; set; }
    }
}
