using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Agency.title)]
    public class Agency
    {

        [Key]
        [DisplayName(ModelsTexts.Agency.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Agency.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.Agency.AcronymTitle)]
        [StringLength(50)]
        public string? Acronym { get; set; }

        [DisplayName(ModelsTexts.Agency.OfficialIDTitle)]
        [StringLength(25)]
        public string? OfficialID { get; set; }
    }
}
