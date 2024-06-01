using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Subsector.title)]
    public class Subsector
    {

        [Key]
        [DisplayName(ModelsTexts.Subsector.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Subsector.SectorTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Sector { get; set; }

        [DisplayName(ModelsTexts.Subsector.NameTitle)]
        [StringLength(100)]
        public string? Name { get; set; }

        // linked fields
        [ForeignKey("Sector")]
        virtual public Sector? Sector_info { get; set; }
    }
}
