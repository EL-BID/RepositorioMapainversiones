using IMRepo.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IMRepo.Areas.Review.Models.Approval
{
    [DisplayName("Aprobaciones Pendientes")]
    public class GeneralApprovalModel
    {
        public int ContractId { get; set; }

        [DisplayName("Tipo")]
        public string tipo { get; set; } = string.Empty;

        [DisplayName("Cantidad")]
        public int cantidad { get; set; }

        public string action { get; set; } = string.Empty;

    }
}
