using JaosLib.Models.JaoTables;
using JaosLib.Services.JaoTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using IMRepo.Areas.AppReports.Models;
using IMRepo.Data;
using IMRepo.Services.Utilities;

namespace IMRepo.Areas.AppReports.Controllers
{
    [Area("AppReports")]
    [Authorize(Roles = ProjectGlobals.registeredRoles)]

    public class ExtensionsController : Controller
    {

        private readonly IMRepoDbContext context;
        private readonly IJaoTableServices jts;
        private readonly IJaoTableExcelServices jtExcel;


        JaosLibUtils libUtils = new JaosLibUtils();

        public ExtensionsController(IMRepoDbContext context
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
            MemoryStream memoryStream = jtExcel.createExcelFile(jaoTable, ModelsTexts.Extension.titlePlural, "Report", Response);
            return File(memoryStream, JaoTableExcelServices.fileStyle, jtExcel.fileName);
        }



        //==============================================
        //----------------------------------------------



        async Task<JaoTable> fillTable()
        {
            const string dbView = "[report_Extensions]";
            List<ExtensionsReportModel> records;
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                records = (await sqlConnection.QueryAsync<ExtensionsReportModel>($"select * from {dbView}")).ToList();
                if (records.Count > 0)
                {
                    //records = records
                    //.Where(r => r.name_projectStage == "Implementation" || r.name_projectStage == "Close-Out")
                    //.OrderBy(r => r.name_office + r.name_project).ToList();
                    //.OrderBy(r => r.name_project).ToList();
                }
            }
            catch
            {
                throw;
            }
            return prepareTable(records);
        }



        JaoTable prepareTable(List<ExtensionsReportModel> records)
        {
            jts.setExcelWidths(new float[] { 50, 300
                , 50, 50, 50, 50, 50, 50
                , 50, 50, 100, 50
            });
            jts.setTitle("Extensiones de Plazo");
            jts.setSubtitle("");

            JaosLibUtils libUtils = new JaosLibUtils();

            fieldTitle(nameof(ExtensionsReportModel.projectCode)!);
            fieldTitle(nameof(ExtensionsReportModel.projectName)!, $"{JaoTable.screenOnlyClass}");

            fieldTitle(nameof(ExtensionsReportModel.extensionCode)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(ExtensionsReportModel.stageName)!);
            fieldTitle(nameof(ExtensionsReportModel.days)!, JaoTable.classNumber);
            fieldTitle(nameof(ExtensionsReportModel.DateDelivery)!, JaoTable.classDate);
            fieldTitle(nameof(ExtensionsReportModel.DateApproved)!,  $"{JaoTable.classDate} {JaoTable.screenOnlyClass}");

            fieldTitle(nameof(ExtensionsReportModel.attached)!, $"{JaoTable.screenOnlyClass}");
//            fieldTitle(nameof(ExtensionsReportModel.otherAttachments)!, $"numCel {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(ExtensionsReportModel.motivo)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(ExtensionsReportModel.extensionDelay)!, JaoTable.classNumber);

            foreach (var record in records)
            {
                jts.addRow();
                jts.addCell(record.projectCode);
                jts.addCell(record.projectName, JaoTable.screenOnlyClass);
                jts.addCell(record.extensionCode, JaoTable.screenOnlyClass);
                jts.addCell(record.stageName);
                jts.addCell(record.days);
                jts.addCell(record.DateDelivery);
                jts.addCell(record.DateApproved, JaoTable.screenOnlyClass);
                jts.addCell(record.attached, JaoTable.screenOnlyClass);
 //               jts.addCell(record.otherAttachments, JaoTable.screenOnlyClass);
                jts.addCell(record.motivo, JaoTable.screenOnlyClass);
                jts.addCell(record.extensionDelay);
            }
            return jts.getTable();
        }


        void fieldTitle(string fieldName, string cssClass = "")
        {
            jts.addHeaderCell(libUtils.titleOf<ExtensionsReportModel>(fieldName), cssClass);
        }




    }
}
