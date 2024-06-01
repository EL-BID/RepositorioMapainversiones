using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.FundingType.title)]
    public class FundingType
    {

        [Key]
        [DisplayName(ModelsTexts.FundingType.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.FundingType.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;
    }
}
