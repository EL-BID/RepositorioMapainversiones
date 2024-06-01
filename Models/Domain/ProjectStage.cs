using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.ProjectStage.title)]
    public class ProjectStage
    {

        [Key]
        [DisplayName(ModelsTexts.ProjectStage.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.ProjectStage.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.ProjectStage.OrderTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public int? Order { get; set; }    }
}
