using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.ProjectFunding.title)]
    public class ProjectFunding
    {

        [Key]
        [DisplayName(ModelsTexts.ProjectFunding.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.ProjectFunding.ProjectTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Project { get; set; }

        [DisplayName(ModelsTexts.ProjectFunding.TypeTitle)]
        public int? Type { get; set; }

        [DisplayName(ModelsTexts.ProjectFunding.SourceTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Source { get; set; }

        [DisplayName(ModelsTexts.ProjectFunding.ValueTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public double? Value { get; set; }
        // linked fields
        [ForeignKey("Project")]
        virtual public Project? Project_info { get; set; }
        [ForeignKey("Type")]
        virtual public FundingType? Type_info { get; set; }
        [ForeignKey("Source")]
        virtual public FundingAgency? Source_info { get; set; }
    }
}
