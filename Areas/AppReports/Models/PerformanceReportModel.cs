using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Areas.AppReports.Models
{
    public class PerformanceReportModel
    {
        public int projectId { get; set; } = 0;
        [DisplayName("Código")]
        public string projectCode { get; set; } = string.Empty;
        [DisplayName("Proyecto")]
        public string projectName { get; set; } = string.Empty;
        [DisplayName("Costo Planeado")]
        public double? plannedCost { get; set; } = null;
        [DisplayName("Costo Programado")]
        public double? programmedCost { get; set; } = null;
        [DisplayName("Valor Pagado")]
        public double? paymentsTotal { get; set; } = null;
        [DisplayName("% avance financiero")]
        public float? financialRate { get; set; } = null;
        [DisplayName("% avance físico")]
        public float? physicalRate { get; set; } = null;
        [DisplayName("Inicio real")]
        public DateTime? actualStartDate { get; set; } = null;
        [DisplayName("Fin real")]
        public DateTime? actualEndDate { get; set; } = null;
        [DisplayName("Fin Programado")]
        public DateTime? ProgrammedEndDate { get; set; } = null;
        [DisplayName("Transcurrido")]
        public int? elapsedTime { get; set; } = null;
        [DisplayName("% Tiempo")]
        public float? timeRate { get; set; } = null;
    }
}
