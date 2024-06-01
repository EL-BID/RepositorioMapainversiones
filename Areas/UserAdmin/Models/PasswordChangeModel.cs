using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JaosLib.Areas.UserAdmin.Models
{
    public class PasswordChangeModel
    {
        public string userId = string.Empty;
        
        [DisplayName("Nueva Contraseña")]
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} debe tener mínimo {2} caracteres.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "At least 8 characters. Must contain: upper case, lower case, number and special character (!@#$%^&*)")]
        public string new_password { get; set; } =  string.Empty;
        
        [Compare("new_password", ErrorMessage = "Las Contraseñas son diferentes")]
        [DisplayName("Repetir Nueva Contraseña")]
        [DataType(DataType.Password)]
        public string repeat_password { get; set; } = string.Empty;
    }
}
