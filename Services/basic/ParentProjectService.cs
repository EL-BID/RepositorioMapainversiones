using System.Security.Claims;
using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using IMRepo.Areas.Identity.Models;
using IMRepo.Data;
using IMRepo.Models.Domain;

namespace IMRepo.Services.basic
{
    public partial class ParentProjectService : IParentProjectService
    {
        IMRepoDbContext context;
        public ParentProjectService(IMRepoDbContext context)
        {
            this.context = context;
        }

        public Project? getSessionProject(ISession session, dynamic ViewBag)
        {
            string? textProject = session.GetString("project");
            Project? project = (!string.IsNullOrEmpty(textProject)) ? JsonSerializer.Deserialize<Project > (textProject) : null;
            if ((project != null) && !(project?.Id > 0))
                project = null;
            if (ViewBag != null)
                ViewBag.ParentProject = project;
            return project;
        }
        
        public int? getSessionProjectId(ISession session)
        {
            if (int.TryParse(session.GetString("projectId"), out int id)) 
                return id;
            return null;
        }
        public async Task<Project?> getProjectFromIdOrSession(int? id, ClaimsPrincipal user, ISession session, dynamic ViewBag)
        {
            // use the id from parameter or from session variable.
            Project? project = null;
            if (id != null) // load info for id
            {
                project = await getProjectFromId(id, session, user);
                ViewBag.ParentProject = project;
            }
            else // retrieve from session variable
            {
                project = getSessionProject(session, ViewBag);
            }
            if (project == null)
                return null;
            if (context.Project != null)
                project = await context.Project
                            .Include(t => t.ProjectFundings!).ThenInclude(t => t.Type_info)
                            .Include(t => t.ProjectFundings!).ThenInclude(t => t.Source_info)
                            .Include(t => t.ProjectAttachments!)
                                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == project.Id);
            return project;
        }

        public async Task<Project?> getProjectFromId(int? id, ISession session, ClaimsPrincipal user)
        {
            Project? project = null;
            if (id != null) // load info for id
            {
                if (context.Project != null)
                {
                    project = await context.Project.FindAsync(id);
                    if (project != null)
                    {
                        context.Entry(project).State = EntityState.Detached;
                        project.ProjectAttachments = null;
                        project.Extensions = null;
                        project.ProjectFundings = null;
                        project.ProjectImages = null;
                        project.Products = null;
                        project.ProjectVideos = null;
                    }
                }
            }
            setSessionProject(project, session);
            return project;
        }
        public void setSessionProject(Project? project, ISession session)
        {
            if (project != null)
            {
                    session.SetString("projectId", project.Id.ToString());
                    session.SetString("project", JsonSerializer.Serialize(project));
            }
            else
            {
                session.SetString("project", "");
                session.SetString("projectId", "");
            }
        }
        public async Task<int?> checkSessionProject(int projectId, ISession session, ClaimsPrincipal user)
        {
            int? sessionId = getSessionProjectId(session);
            if (!sessionId.HasValue || sessionId.Value != projectId)
            {
                await getProjectFromId(projectId, session, user);
            }
            return projectId;
        }
    }
}
