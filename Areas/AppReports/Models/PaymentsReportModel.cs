using System.ComponentModel;

namespace IMRepo.Areas.AppReports.Models
{
    public class PaymentsReportModel
    {
        [DisplayName("Id Proyecto")]
        public string projectCode { get; set; } = string.Empty;
        [DisplayName("Proyecto")]
        public string projectName { get; set; } = string.Empty;
        [DisplayName("Id Pago")]
        public string paymentCode { get; set; } = string.Empty;
        [DisplayName("Etapa")]
        public string stageName { get; set; } = string.Empty;
        [DisplayName("Periodo")]
        public DateTime? reportedMonth { get; set; } = null;
        [DisplayName("Valor")]
        public double? paymentValue { get; set; } = null;
        [DisplayName("Avance Fisico")]
        public float? PhysicalAdvance { get; set; } = null;
        [DisplayName("Presentado")]
        public DateTime? DateDelivery { get; set; } = null;
        [DisplayName("Aprobado")]
        public DateTime? DateApproved { get; set; } = null;
        [DisplayName("Pagado")]
        public DateTime? DatePayed { get; set; } = null;
        [DisplayName("Soporte Avance?")]
        public string attachedMedicion { get; set; } = string.Empty;
        [DisplayName("Soporte Pago?")]
        public string attachedOrden { get; set; } = string.Empty;
        [DisplayName("Adjuntos")]
        public string otherAttachments { get; set; } = string.Empty;
        [DisplayName("Días trámite")]
        public double? PaymentDelay { get; set; } = null;


    }
}
