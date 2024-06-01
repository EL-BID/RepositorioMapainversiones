using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using IMRepo;
using IMRepo.Areas.Identity.Models;
using IMRepo.Data;
using IMRepo.Models.Domain;
using System.Security.Claims;


namespace JaosLib.Services.Utilities
{
    public class UserService : IUserService
    {
        private readonly IMRepoDbContext context;

        public UserService(IMRepoDbContext context)
        {
            this.context = context;
        }



        public List<IdentityRole> getRoles(ClaimsPrincipal user)
        {
            List<IdentityRole> roles = new List<IdentityRole>();
            if (user != null)
                if (user.IsInRole(ProjectGlobals.RoleAdmin))
                {
                    {
                        SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                        try
                        {
                            sqlConnection.Open();
                            var result = sqlConnection.Query<IdentityRole>($"select * from AspNetRoles");
                            if (result.Any())
                            {
                                roles = result.ToList();
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            sqlConnection.Close();
                        }
                    }
                }
            return roles;
        }



        public List<AspNetUsers> getUsers(ClaimsPrincipal user)
        {
            List<AspNetUsers> users = new List<AspNetUsers>();
            if (user?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.Query<AspNetUsers>($"select * from AspNetUsers");
                    if (result.Any())
                    {
                        users = result.ToList();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return users;
        }


        async public Task setRole(ClaimsPrincipal user, string userId, int roleId)
        {
            if (user?.IsInRole(ProjectGlobals.RoleAdmin) == true)
                if (!string.IsNullOrEmpty(userId))
                {
                    SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                    try
                    {
                        sqlConnection.Open();
                        var result = await sqlConnection.QueryAsync<int>($"select RoleId from AspNetUserRoles where UserId='{userId}'");
                        int r = result.FirstOrDefault();
                        if (r != roleId || result.Count() > 1) // remove this delete to allow for multiple roles in a user.
                        {
                            await deleteRolesForUser(user, userId);
                            if (roleId > 0)
                            {
                                string sql = $"insert into AspNetUserRoles (RoleId, UserId) values ({roleId}, '{userId}');";
                                await sqlConnection.ExecuteAsync(sql);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
        }

        async public Task deleteRolesForUser(ClaimsPrincipal user, string userId)
        {
            if (user?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.Query($"select * from AspNetUserRoles where UserId='{userId}'");
                    if (result.Any())
                    {
                        string sql = $"delete from AspNetUserRoles where UserId ='{userId}';";
                        await sqlConnection.ExecuteAsync(sql);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }



        async public Task<int> getRoleIdFor(string userId)
        {
            int result;
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                result = await sqlConnection.QueryFirstOrDefaultAsync<int>($"select RoleId from AspNetUserRoles where UserId='{userId}'");
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }


        async public Task<string> getRoleNameFor(string userId)
        {
            string result;
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                result = await sqlConnection.QueryFirstOrDefaultAsync<string>($"select Name from AspNetUserRoles left join AspNetRoles on AspNetRoles.Id = AspNetUserRoles.RoleId where UserId='{userId}'");
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }



        // ------------------- User Profile

        async public Task<UserProfile?> getUserProfile(string userId)
        {
            UserProfile? profile;
            try
            {
                profile = await context.UserProfile.FirstOrDefaultAsync(r => r.AspNetUserId == userId);
            }
            catch
            {
                throw;
            }
            return profile;
        }






        // -------------------------- User Identity
        //=============================================================
        //
        //         USER IDENTITY
        //
        //-------------------------------------------------------------
        public string? getUserId(ClaimsPrincipal user)
        {
            string id = "";
            if (user != null && user.Identity != null)
            {
                SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.Query<string>($"select id from AspNetUsers where UserName = '{user.Identity.Name}'");
                    if (result.Any())
                    {
                        id = result.First();
                    }
                }
                catch
                {

                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return id;
        }





    }
}
