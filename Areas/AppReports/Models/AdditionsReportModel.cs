using System.ComponentModel;

namespace IMRepo.Areas.AppReports.Models
{
    public class AdditionsReportModel
    {

        [DisplayName("Id Proyecto")]
        public string projectCode { get; set; } = string.Empty;
        [DisplayName("Proyecto")]
        public string projectName { get; set; } = string.Empty;
        [DisplayName("Id Adición")]
        public string additionCode { get; set; } = string.Empty;
        [DisplayName("Estado")]
        public string stageName { get; set; } = string.Empty;

        [DisplayName("Valor")]
        public double? additionValue { get; set; } = null;
        [DisplayName("Presentado")]
        public DateTime? DateDelivery { get; set; } = null;
        [DisplayName("Aprobado")]
        public DateTime? DateApproved { get; set; } = null;
        [DisplayName("Anexo Adición")]
        public string attached { get; set; } = string.Empty;
        [DisplayName("Otros Anexos")]
        public string otherAttachments { get; set; } = string.Empty;
        [DisplayName("Días Trámite")]
        public int? AdditionDelay { get; set; } = null;


    }
}
