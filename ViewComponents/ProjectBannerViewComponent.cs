using Microsoft.AspNetCore.Mvc;
using IMRepo.Data;
using IMRepo.Models.Domain;
using System.Text.Json;

namespace IMRepo.ViewComponents
{
    public class ProjectBannerViewComponent : ViewComponent
    {
        private readonly IMRepoDbContext context;

        Project? project = null;

        public ProjectBannerViewComponent(IMRepoDbContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (getSessionProject() != null)
                return await Task.Run(() => View(project));
            else
                return await Task.Run(() => View());
        }


        //----------- Controller Methods

        Project? getSessionProject()
        {
            string? textProject = HttpContext.Session.GetString("project");
            project = (!string.IsNullOrEmpty(textProject)) 
                ? JsonSerializer.Deserialize<Project>(textProject) 
                : null;
            if ((project == null) || !(project.Id > 0))
                project = null;
            return project;
        }


    }
}
