using IMRepo.Models.Domain;
using System.Security.Claims;

namespace IMRepo.Services.basic
{
    public partial interface IParentProjectService
    {
        /// <summary>
        /// Get project information from Session variable
        /// </summary>
        /// <param name="session">The HttpContext.Session where data is stored.</param>
        /// <param name="ViewBag">The ViewBag where information will be replicated.</param>
        /// <returns>A Project model with the information, the same information is included in ViewBag.ParentProject.</returns>
        Project? getSessionProject(ISession session, dynamic ViewBag);

        /// <summary>
        /// Gets the project id from the session variable.
        /// </summary>
        /// <param name="session">The HttpContext.Session where data is stored.</param>
        /// <returns>The project id</returns>
        int? getSessionProjectId(ISession session);

        /// <summary>
        /// If the received id has a value, returns the project with that id and updates the session variable. 
        /// If the id does not have a values, returns the project available in the Session variable.
        /// </summary>
        /// <param name="id">The Project id</param>
        /// <param name="user">The user performing the operation, will be used to control access to the requested information.</param>
        /// <param name="session">The HttpContext.Session where data is stored.</param>
        /// <param name="ViewBag">The ViewBag where information will be replicated.</param>
        /// <returns>A Project Model with the information.</returns>
        Task<Project?> getProjectFromIdOrSession(int? id, ClaimsPrincipal user, ISession session, dynamic ViewBag);

        /// <summary>
        /// Retrieves the information for the project with the received id.
        /// </summary>
        /// <param name="id">id for the project to be retrieved</param>
        /// <param name="user">The user performing the operation, will be used to control access to the requested information.</param>
        /// <param name="session">The HttpContext.Session where data is stored.</param>
        /// <returns>The Project with the received id.</returns>
        Task<Project?> getProjectFromId(int? id, ISession session, ClaimsPrincipal user);

        /// <summary>
        /// Sets the project session information. project: contains the active Project information and projectId: contains the id for the active project.
        /// </summary>
        /// <param name="project">The project to be set as active.</param>
        /// <param name="session">The HttpContext.Session where data is stored.</param>
        void setSessionProject(Project? project, ISession session);

        /// <summary>
        /// Validates if the project is the same active project.
        /// If it is different, updates the active project.
        /// </summary>
        /// <param name="projectId">The projectId to be set as active.</param>
        /// <param name="user">The user information to allow controlling access to information based on user profile.</param>
        Task<int?> checkSessionProject(int projectId, ISession session, ClaimsPrincipal user);

    }
}
