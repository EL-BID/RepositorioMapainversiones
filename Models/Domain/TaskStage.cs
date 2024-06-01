using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.TaskStage.title)]
    public class TaskStage
    {

        [Key]
        [DisplayName(ModelsTexts.TaskStage.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.TaskStage.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(20)]
        public string Name { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.TaskStage.OrderTitle)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public int? Order { get; set; }
    }
}
