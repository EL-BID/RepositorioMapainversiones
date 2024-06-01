using System.ComponentModel.DataAnnotations;
using IMRepo.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using NPOI.SS.Formula.PTG;
using IMRepo.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace JaosLib.Areas.UserAdmin.Models
{
    public class UserSetupModel
    {
        public IdentityUser? user { get; set; }
        public UserProfile? profile { get; set; }
        public string? roleName { get; set; }
    }
}
