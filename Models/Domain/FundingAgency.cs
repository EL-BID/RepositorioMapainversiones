using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.FundingAgency.title)]
    public class FundingAgency
    {

        [Key]
        [DisplayName(ModelsTexts.FundingAgency.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.FundingAgency.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.FundingAgency.AcronymTitle)]
        [StringLength(20)]
        public string? Acronym { get; set; }
    }
}
