using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Sector.title)]
    public class Sector
    {

        [Key]
        [DisplayName(ModelsTexts.Sector.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Sector.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // children
        [DisplayName(ModelsTexts.Subsector.titlePlural)]
        public ICollection<Subsector>? Subsectors { get; set; }
    }
}
