using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Areas.AppReports.Models
{
    public class ProjectGeneralReportModel
    {
        public int id_project { get; set; } = 0;
        [DisplayName("Código")]
        public string code_project { get; set; } = string.Empty;
        [DisplayName("Nombre")]
        public string name_project { get; set; } = string.Empty;
        [DisplayName("Área Responsable")]
        public string name_office { get; set; } = string.Empty;
        [DisplayName("Sector")]
        public string name_sector { get; set; } = string.Empty;
        [DisplayName("Subsector")]
        public string name_subsector { get; set; } = string.Empty;
        [DisplayName("Etapa")]
        public string name_projectStage { get; set; } = string.Empty;
    }
}
