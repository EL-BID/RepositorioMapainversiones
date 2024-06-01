using System.Collections.Generic;

namespace IMRepo.Settings
{
    public class MenuItem
    {
        public string? Title { get; set; }
        public List<MenuItem>? SubItems { get; set; }
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string[]? AllowedRoles { get; set; }
        public Dictionary<string, string>? RouteValues { get; set; }
        public bool endWithSeparator { get; set; }
        public bool requiresProject { get; set; }

    }


    public class MainMenuOptions
    {
        public static readonly List<MenuItem> MenuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Title = "Proyecto",
                SubItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Ver Resumen",
                        Area = "Review",
                        Controller = "Home",
                        Action = "Index",
                        AllowedRoles = null
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Seleccionar",
                        Area = "",
                        Controller = "Project",
                        Action = "Select",
                        AllowedRoles = null,
                        endWithSeparator = true
                    },
                    new MenuItem
                    {
                        Title = "Productos",
                        Area = "",
                        Controller = "Product",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Pagos",
                        Area = "",
                        Controller = "Payment",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Adiciones",
                        Area = "",
                        Controller = "Addition",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Extensiones",
                        Area = "",
                        Controller = "Extension",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Media",
                        Area = "Review",
                        Controller = "Media",
                        Action = "Index",
                        AllowedRoles = null,
                        RouteValues = new Dictionary<string, string> { { "projectId", "@sessionProject.Id" } },
                        endWithSeparator = true
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Consultar",
                        Area = "",
                        Controller = "Project",
                        Action = "Display",
                        AllowedRoles = null,
                        RouteValues = new Dictionary<string, string> { { "Id", "@sessionProject?.Id" } }
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Editar projecto",
                        Area = "",
                        Controller = "Project",
                        Action = "Edit",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                        , requiresProject = true
                    },
                    new MenuItem
                    {
                        Title = "Crear proyecto",
                        Area = "",
                        Controller = "Project",
                        Action = "Create",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin, ProjectGlobals.RoleDireccion }
                    }
                }
            },
            new MenuItem
            {
                Title = "Pendientes",
                Area = "Review",
                Controller = "Approval",
                Action = "Payments",
                AllowedRoles = new string[] { ProjectGlobals.RoleAdmin, ProjectGlobals.RoleDireccion }
            },
            new MenuItem
            {
                Title = "Reportes",
                SubItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Avance proyecto",
                        Area = "AppReports",
                        Controller = "Performance",
                        Action = "Index",
                        AllowedRoles = null
                    },
                    new MenuItem
                    {
                        Title = "Proyectos (Resumen)",
                        Area = "AppReports",
                        Controller = "Project",
                        Action = "Index",
                        AllowedRoles = null
                    },
                    new MenuItem
                    {
                        Title = "Proyectos (básico)",
                        Area = "AppReports",
                        Controller = "BasicInfo",
                        Action = "Index",
                        AllowedRoles = null
                    },
                    new MenuItem
                    {
                        Title = "Pagos",
                        Area = "AppReports",
                        Controller = "Payments",
                        Action = "Index",
                        AllowedRoles = null
                    },
                    new MenuItem
                    {
                        Title = "Reporte Adiciones",
                        Area = "AppReports",
                        Controller = "Additions",
                        Action = "Index",
                        AllowedRoles = null
                    },
                    new MenuItem
                    {
                        Title = "Extensiones",
                        Area = "AppReports",
                        Controller = "Extensions",
                        Action = "Index",
                        AllowedRoles = null
                    },
                }
            },
            new MenuItem
            {
                Title = "Configuración",
                SubItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Cambiar contraseña",
                        Area = "UserAdmin",
                        Controller = "Home",
                        Action = "ChangePassword",
                        AllowedRoles = null
                    },
                    new MenuItem
                    {
                        Title = "Usuarios",
                        Area = "UserAdmin",
                        Controller = "Home",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin },
                        endWithSeparator = true
                    },
                    new MenuItem
                    {
                        Title = "Áreas responsab.",
                        Area = "",
                        Controller = "Office",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                    },
                    new MenuItem
                    {
                        Title = "Entidades Ejecut.",
                        Area = "",
                        Controller = "Agency",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                    },
                    new MenuItem
                    {
                        Title = "Fuentes financ.",
                        Area = "",
                        Controller = "FundingAgency",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin },
                        endWithSeparator = true
                    },
                    new MenuItem
                    {
                        Title = "Sectores",
                        Area = "",
                        Controller = "Sector",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                    },
                    new MenuItem
                    {
                        Title = "Subsectores",
                        Area = "",
                        Controller = "Subsector",
                        Action = "Index",
                        AllowedRoles = new string[] { ProjectGlobals.RoleAdmin }
                    }
                }
            }
        };
    }
}
