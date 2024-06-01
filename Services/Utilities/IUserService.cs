using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using IMRepo.Models.Domain;
using System.Security.Claims;

namespace JaosLib.Services.Utilities
{
    public interface IUserService
    {
        List<IdentityRole> getRoles(ClaimsPrincipal user);
        Task setRole(ClaimsPrincipal user, string userId, int roleId);
        Task deleteRolesForUser(ClaimsPrincipal user, string userId);
        Task<int> getRoleIdFor(string userId);
        Task<string> getRoleNameFor(string userId);

        // ------------------- User Profile
        Task<UserProfile?> getUserProfile(string userId);
    }
}
