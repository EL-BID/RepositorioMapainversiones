using Microsoft.AspNetCore.Mvc;
using IMRepo.Data;
using JaosLib.Models.JaoTables;
using JaosLib.Services.JaoTables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Reflection;
using System.ComponentModel;
using IMRepo.Areas.AppReports.Models;
using IMRepo.Models.Domain;
using NPOI.Util;
using IMRepo.Services.Utilities;

namespace IMRepo.Areas.AppReports.Controllers
{
    [Area("AppReports")]
    public class PerformanceController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IJaoTableServices jts;
        private readonly IJaoTableExcelServices jtExcel;

        JaosLibUtils libUtils = new JaosLibUtils();

        public PerformanceController(IMRepoDbContext context
            , IJaoTableServices jaoTableServices
            , IJaoTableExcelServices jaoTableExcelServices
            )
        {
            this.context = context;
            jts = jaoTableServices;
            this.jtExcel = jaoTableExcelServices;
        }

        // GET: HomeController1
        public async Task<IActionResult> Index()
        {
            JaoTable jaoTable = await fillTable();
            ViewBag.NoContainer = true;
            return View(jaoTable);
        }


        async Task<JaoTable> fillTable()
        {
            const string dbView = "report_ProjectPerformance";

            List<PerformanceReportModel> records = new List<PerformanceReportModel>();
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                records = (await sqlConnection.QueryAsync<PerformanceReportModel>($"select * from {dbView}")).ToList();
                if (records.Count > 0)
                {
                    //if (User.IsInRole("Office"))
                    //{
                    //    UserProfile? userProfile = context.UserProfile.Include(p => p.Office_info).Where(p => p.Email == User.Identity.Name).FirstOrDefault();
                    //    string officeName = userProfile?.Office_info?.Name ?? "No entity available";
                    //    records = records.Where(r => r.name_office == officeName).ToList();
                    //}
                    //if (records.Count > 0)
                    //    records = records.Where(r => r.name_projectStage == "Planning" || r.name_projectStage == "Identification").OrderBy(r => r.name_entity + r.name_project).ToList();
                }
            }
            catch
            {
                throw;
            }
            return prepareTable(records);
        }


        public async Task<IActionResult> Export()
        {
            JaoTable jaoTable = await fillTable();
            MemoryStream memoryStream = jtExcel.createExcelFile(jaoTable, "Performance", "Report", Response);
            return File(memoryStream, JaoTableExcelServices.fileStyle, jtExcel.fileName);
        }


        JaoTable prepareTable(List<PerformanceReportModel> records)
        {
            jts.setExcelWidths(new float[] {
                300, 60,
                60, 
                //150, 150, 
                60,
                60,
                70, 70, 70,
                60
            });
            jts.setTitle("Reporte de avance");
            jts.setSubtitle("");


            fieldTitle(nameof(PerformanceReportModel.projectCode)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PerformanceReportModel.projectName)!);
            fieldTitle(nameof(PerformanceReportModel.plannedCost)!, $"{JaoTable.classNumber} {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PerformanceReportModel.programmedCost)!, JaoTable.classNumber);
            fieldTitle(nameof(PerformanceReportModel.paymentsTotal)!, JaoTable.classNumber);
            fieldTitle(nameof(PerformanceReportModel.financialRate)!, JaoTable.classNumber);
            fieldTitle(nameof(PerformanceReportModel.physicalRate)!, JaoTable.classNumber);
            fieldTitle(nameof(PerformanceReportModel.actualStartDate)!, $"{JaoTable.classDate} {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PerformanceReportModel.actualEndDate)!, $"{JaoTable.classDate} {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PerformanceReportModel.ProgrammedEndDate)!, $"{JaoTable.classDate} {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PerformanceReportModel.elapsedTime)!, JaoTable.classNumber);
            fieldTitle(nameof(PerformanceReportModel.timeRate)!, JaoTable.classNumber);


            foreach (var record in records)
            {
                jts.addRow();
                jts.addCell(record.projectCode, JaoTable.screenOnlyClass);
                jts.addCell(record.projectName);
                jts.addCell(record.plannedCost, JaoTable.screenOnlyClass);
                jts.addCell(record.programmedCost);
                jts.addCell(record.paymentsTotal);
                jts.addFloatCell(record.financialRate);
                jts.addFloatCell(record.physicalRate);
                jts.addCell(record.actualStartDate, JaoTable.screenOnlyClass);
                jts.addCell(record.actualEndDate, JaoTable.screenOnlyClass);
                jts.addCell(record.ProgrammedEndDate, JaoTable.screenOnlyClass);
                jts.addCell(record.elapsedTime);
                jts.addFloatCell(record.timeRate);
            }
            return jts.getTable();
        }

        string displayFieldTitle(string fieldName)
        {
            MemberInfo? property = typeof(PerformanceReportModel).GetProperty(fieldName);
            var attribute = property?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                    .Cast<DisplayNameAttribute>().Single();
            return attribute?.DisplayName ?? "";
        }


        void fieldTitle(string fieldName, string cssClass = "")
        {
            jts.addHeaderCell(libUtils.titleOf<PerformanceReportModel>(fieldName), cssClass);
        }


    }


}