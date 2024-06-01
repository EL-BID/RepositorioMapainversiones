using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Product.title)]
    public class Product
    {

        [Key]
        [DisplayName(ModelsTexts.Product.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Product.ProjectTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Project { get; set; }

        [DisplayName(ModelsTexts.Product.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.Product.CostTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public double? Cost { get; set; }
        [DisplayName(ModelsTexts.Product.DescriptionTitle)]
        public string? Description { get; set; }

        [DisplayName(ModelsTexts.Product.ObjectiveTitle)]
        public string? Objective { get; set; }

        // linked fields
        [ForeignKey("Project")]
        virtual public Project? Project_info { get; set; }

        // children
        [DisplayName(ModelsTexts.Addition.titlePlural)]
        public ICollection<Addition>? Additions { get; set; }
        [DisplayName(ModelsTexts.Payment.titlePlural)]
        public ICollection<Payment>? Payments { get; set; }
    }
}
