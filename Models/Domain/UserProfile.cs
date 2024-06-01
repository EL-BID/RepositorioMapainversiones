using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    [DisplayName(ModelsTexts.UserProfile.title)]
    public class UserProfile
    {

        [Key]
        [DisplayName(ModelsTexts.UserProfile.IdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        public int Id { get; set; }

        [DisplayName(ModelsTexts.UserProfile.AspNetUserIdTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(255)]
        public string AspNetUserId { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.UserProfile.EmailTitle)]
        [StringLength(100)]
        public string? Email { get; set; }

        [DisplayName(ModelsTexts.UserProfile.NameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(25)]
        public string Name { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.UserProfile.SurnameTitle)]
        [Required(ErrorMessage = "{0}: campo obligatorio.")]
        [StringLength(25)]
        public string Surname { get; set; } = string.Empty;

        [DisplayName(ModelsTexts.UserProfile.OfficeTitle)]
        public int? Office { get; set; }

        [DisplayName(ModelsTexts.UserProfile.NotesTitle)]
        public string? Notes { get; set; }

        // linked fields
        [ForeignKey("Office")]
        virtual public Office? Office_info { get; set; }
    }
}
