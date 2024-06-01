using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.PaymentStage.title)]
    public class PaymentStage
    {

        [Key]
        [DisplayName(ModelsTexts.PaymentStage.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.PaymentStage.TitleTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(30)]
        public string Title { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.PaymentStage.SortOrderTitle)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public int? SortOrder { get; set; }
    }
}
