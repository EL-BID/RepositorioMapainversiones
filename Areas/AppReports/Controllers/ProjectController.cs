using Microsoft.AspNetCore.Mvc;
using IMRepo.Data;
using Microsoft.EntityFrameworkCore;
using IMRepo.Models.Domain;
using JaosLib.Services.JaoTables;
using JaosLib.Models.JaoTables;
using IMRepo.Services.Utilities;
using IMRepo.Areas.AppReports.Models;

namespace IMRepo.Areas.AppReports.Controllers
{
    [Area("AppReports")]
    public class ProjectController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IJaoTableServices jts;
        private readonly IJaoTableExcelServices jtExcel;

        JaosLibUtils libUtils = new JaosLibUtils();

        public ProjectController(IMRepoDbContext context
            , IJaoTableServices jaoTableServices
            , IJaoTableExcelServices jaoTableExcelServices
            )
        {
            this.context = context;
            jts = jaoTableServices;
            jtExcel = jaoTableExcelServices;
        }

        // GET: Index
        public async Task<IActionResult> Index(string sortOrder)
        {
            JaoTable jaoTable = await fillTable();
            ViewBag.NoContainer = true;
            return View(jaoTable);
        }

        // POST: Export
        public async Task<IActionResult> Export()
        {
            JaoTable jaoTable = await fillTable();
            MemoryStream memoryStream = jtExcel.createExcelFile(jaoTable, ModelsTexts.Project.titlePlural, "Report", Response);
            return File(memoryStream, JaoTableExcelServices.fileStyle, jtExcel.fileName);
        }



        //==============================================
        //----------------------------------------------



        async Task<JaoTable> fillTable()
        {
            List<Project> records = new List<Project>();
            try
            {
                records = await context.Project
                                    .Include(t => t.ProjectFundings!).ThenInclude(t => t.Source_info)
                                    .Include(t => t.Sector_info)
                                    .Include(t => t.Subsector_info)
                                    .Include(t => t.Stage_info)
                                    .Include(t => t.Office_info)
                                    .ToListAsync();

                //if (records.Count > 0)
                //{
                //    //records = records
                //    //    //.Where(r => r.name_projectStage == "Implementation" || r.name_projectStage == "Close-Out")
                //    //    //.OrderBy(r => r.name_office + r.name_project).ToList();
                //    //    .OrderBy(r => r.name_project).ToList();
                //}
            }
            catch
            {
                throw;
            }
            return prepareTable(records);
        }



        JaoTable prepareTable(List<Project> records)
        {
            JaosLibUtils libUtils = new JaosLibUtils();
            jts.setExcelWidths(new float[] { 50, 300, 120, 120, 120, 90, 50, 50, 300, 300, 150 });
            jts.setTitle("Reporte Proyectos");
            jts.setSubtitle("");

            //            jts.addHeaderCell("");
            fieldTitle(nameof(Project.Code)!, JaoTable.classNumber);
            fieldTitle(nameof(Project.Name)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(Project.Office)!,"");
            fieldTitle(nameof(Project.Sector)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(Project.Subsector)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(Project.Stage)!, "");
            fieldTitle(nameof(Project.PlannedCost)!, JaoTable.classNumber);
            fieldTitle(nameof(Project.Description)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(Project.Objectives)!, $"{JaoTable.screenOnlyClass}");
            jts.addHeaderCell(libUtils.titleOf<ProjectFunding>(), $"{JaoTable.screenOnlyClass}");

            //         int number = 1;
            foreach (var record in records)
            {
                jts.addRow();
                //              jts.addCell(number++);
                jts.addCell(record.Code);
                jts.addCell(record.Name, $"{JaoTable.screenOnlyClass}");
                jts.addCell(record.Office_info?.Name);
                jts.addCell(record.Sector_info?.Name, $"{JaoTable.screenOnlyClass}");
                jts.addCell(record.Subsector_info?.Name, $"{JaoTable.screenOnlyClass}");
                jts.addCell(record.Stage_info?.Name);
                jts.addCell(record.PlannedCost);
                jts.addCell(record.Description, $"{JaoTable.screenOnlyClass}");
                jts.addCell(record.Objectives, $"{JaoTable.screenOnlyClass}");
                jts.addCell(record.ProjectFundings?.Count > 0 ? record.ProjectFundings.ToList()[0].Source_info?.Name : null);
            }
            return jts.getTable();
        }


        void fieldTitle(string fieldName, string cssClass)
        {
            jts.addHeaderCell(libUtils.titleOf<Project>(fieldName), cssClass);
        }

    }


}