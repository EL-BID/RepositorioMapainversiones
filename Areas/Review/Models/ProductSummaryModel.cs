using IMRepo.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IMRepo.Areas.Review.Models
{
    public class ProductSummaryModel
    {
        public int ProductId { get; set; }

        [DisplayName("Producto")]
        public string name { get; set; } = string.Empty;

        [DisplayName("Monto Original")]
        public double? originalValue { get; set; }

        [DisplayName("Monto Actual")]
        public double? programmedValue { get; set; }

        [DisplayName("Monto Pagado")]
        public double? actualValue { get; set; }
        [DisplayName("% Avance Físico")]
        public double? advanced { get; set; }
    }
}
