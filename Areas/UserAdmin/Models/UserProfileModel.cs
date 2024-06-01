using System.ComponentModel.DataAnnotations;
using IMRepo.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using NPOI.SS.Formula.PTG;
using IMRepo.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using IMRepo;

namespace JaosLib.Areas.UserAdmin.Models
{
    public class UserProfileModel : UserProfile
    {
        [DisplayName("Rol")]
        public int? role { get; set; }
    }
}

