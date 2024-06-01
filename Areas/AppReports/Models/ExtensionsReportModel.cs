using System.ComponentModel;

namespace IMRepo.Areas.AppReports.Models
{
    public class ExtensionsReportModel
    {

        [DisplayName("Id Proyecto")]
        public string projectCode { get; set; } = string.Empty;
        [DisplayName("Proyecto")]
        public string projectName { get; set; } = string.Empty;
        [DisplayName("Id Axtensión")]
        public string extensionCode { get; set; } = string.Empty;
        [DisplayName("Estado")]
        public string stageName { get; set; } = string.Empty;
        [DisplayName("Días de Extensión")]
        public int? days { get; set; } = null;
        [DisplayName("Presentada")]
        public DateTime? DateDelivery { get; set; } = null;
        [DisplayName("Aprobada")]
        public DateTime? DateApproved { get; set; } = null;
        [DisplayName("Adjunta Extensión")]
        public string attached { get; set; } = string.Empty;
        [DisplayName("Otros Adjuntos")]
        public string otherAttachments { get; set; } = string.Empty;
        [DisplayName("Notas")]
        public string motivo { get; set; } = string.Empty;
        [DisplayName("Tiempo Trámite")]
        public int? extensionDelay { get; set; } = null;


    }
}
