using Microsoft.AspNetCore.Mvc;
using IMRepo.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Reflection;
using System.ComponentModel;
using IMRepo.Areas.AppReports.Models;
using JaosLib.Services.JaoTables;
using JaosLib.Models.JaoTables;
using Microsoft.AspNetCore.Authorization;

namespace IMRepo.Areas.AppReports.Controllers
{
    [Area("AppReports")]
    [Authorize(Roles = ProjectGlobals.registeredRoles)]
    public class BasicInfoController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IJaoTableServices jts;
        private readonly IJaoTableExcelServices jtExcel;

        public BasicInfoController(IMRepoDbContext context
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
            const string dbView = "report_ProjectBasicInfo";
            List<ProjectGeneralReportModel> records = new List<ProjectGeneralReportModel>();
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                records = (await sqlConnection.QueryAsync<ProjectGeneralReportModel>($"select * from {dbView}")).ToList();
                if (records.Count > 0)
                {
                    records = records
                        //.Where(r => r.name_projectStage == "Implementation" || r.name_projectStage == "Close-Out")
                        //.OrderBy(r => r.name_office + r.name_project).ToList();
                        .OrderBy(r => r.name_project).ToList();
                }
            }
            catch
            {
                throw;
            }
            return prepareTable(records);
        }



        JaoTable prepareTable(List<ProjectGeneralReportModel> records)
        {
            jts.setExcelWidths(new float[] { 50, 300, 150, 150, 60 });
            jts.setTitle("Listado general de Proyectos");
            jts.setSubtitle("");

            jts.addHeaderCell("");
            jts.addHeaderCell(displayFieldTitle("code_project"));
            jts.addHeaderCell(displayFieldTitle("name_project"));
            jts.addHeaderCell(displayFieldTitle("name_office"), JaoTable.screenOnlyClass);
            jts.addHeaderCell(displayFieldTitle("name_sector"), JaoTable.screenOnlyClass);
            jts.addHeaderCell(displayFieldTitle("name_subsector"), JaoTable.screenOnlyClass);
            jts.addHeaderCell(displayFieldTitle("name_projectStage"));

            int number = 1;
            foreach (var record in records)
            {
                jts.addRow();
                jts.addCell(number++);
                jts.addCell(record.code_project);
                jts.addCell(record.name_project);
                jts.addCell(record.name_office, JaoTable.screenOnlyClass);
                jts.addCell(record.name_sector, JaoTable.screenOnlyClass);
                jts.addCell(record.name_subsector, JaoTable.screenOnlyClass);
                jts.addCell(record.name_projectStage);
            }
            return jts.getTable();
        }

        /// <summary>
        /// Using the attribute name, returns the DisplayName (title) for a specific attribute from the Model
        /// </summary>
        /// <param name="fieldName">The name of the attribute</param>
        /// <returns>The title for the attribute from the Model</returns>
        string displayFieldTitle(string fieldName)
        {
            MemberInfo? property = typeof(ProjectGeneralReportModel).GetProperty(fieldName);
            var attribute = property?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                    .Cast<DisplayNameAttribute>().Single();
            return attribute?.DisplayName ?? "";
        }

    }


}