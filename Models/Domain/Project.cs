using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.Project.title)]
    public class Project
    {

        [Key]
        [DisplayName(ModelsTexts.Project.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.Project.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.Project.CodeTitle)]
        [StringLength(20)]
        public string? Code { get; set; }

        [DisplayName(ModelsTexts.Project.SectorTitle)]
        public int? Sector { get; set; }

        [DisplayName(ModelsTexts.Project.SubsectorTitle)]
        public int? Subsector { get; set; }

        [DisplayName(ModelsTexts.Project.OfficeTitle)]
        public int? Office { get; set; }

        [DisplayName(ModelsTexts.Project.ExecutingAgencyTitle)]
        public int? ExecutingAgency { get; set; }

        [DisplayName(ModelsTexts.Project.StageTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccionar {0}")]
        public int Stage { get; set; }

        [DisplayName(ModelsTexts.Project.SdgTitle)]
        public int? Sdg { get; set; }

        [DisplayName(ModelsTexts.Project.PlannedCostTitle)]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public double? PlannedCost { get; set; }

        [DisplayName(ModelsTexts.Project.PlannedDurationTitle)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "{0} debe ser un número válido.")]
        public int? PlannedDuration { get; set; }

        [DisplayName(ModelsTexts.Project.PlannedStartDateTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? PlannedStartDate { get; set; }

        [DisplayName(ModelsTexts.Project.ActualStartDateTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? ActualStartDate { get; set; }

        [DisplayName(ModelsTexts.Project.ActualEndDateTitle)]
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime? ActualEndDate { get; set; }

        [DisplayName(ModelsTexts.Project.DescriptionTitle)]
        public string? Description { get; set; }

        [DisplayName(ModelsTexts.Project.ObjectivesTitle)]
        public string? Objectives { get; set; }

        [DisplayName(ModelsTexts.Project.LocationTitle)]
        public string? Location { get; set; }



        // linked fields
        [ForeignKey("Sector")]
        virtual public Sector? Sector_info { get; set; }
        [ForeignKey("Subsector")]
        virtual public Subsector? Subsector_info { get; set; }
        [ForeignKey("Office")]
        virtual public Office? Office_info { get; set; }
        [ForeignKey("ExecutingAgency")]
        virtual public Agency? ExecutingAgency_info { get; set; }
        [ForeignKey("Stage")]
        virtual public ProjectStage? Stage_info { get; set; }
        [ForeignKey("Sdg")]
        virtual public Sdg? Sdg_info { get; set; }

        // children
        [DisplayName(ModelsTexts.ProjectAttachment.titlePlural)]
        public ICollection<ProjectAttachment>? ProjectAttachments { get; set; }
        [DisplayName(ModelsTexts.Extension.titlePlural)]
        public ICollection<Extension>? Extensions { get; set; }
        [DisplayName(ModelsTexts.ProjectFunding.titlePlural)]
        public ICollection<ProjectFunding>? ProjectFundings { get; set; }
        [DisplayName(ModelsTexts.ProjectImage.titlePlural)]
        public ICollection<ProjectImage>? ProjectImages { get; set; }
        [DisplayName(ModelsTexts.Product.titlePlural)]
        public ICollection<Product>? Products { get; set; }
        [DisplayName(ModelsTexts.ProjectVideo.titlePlural)]
        public ICollection<ProjectVideo>? ProjectVideos { get; set; }
    }
}
