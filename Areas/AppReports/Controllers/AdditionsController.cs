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

    public class AdditionsController : Controller
    {

        private readonly IMRepoDbContext context;
        private readonly IJaoTableServices jts;
        private readonly IJaoTableExcelServices jtExcel;


        JaosLibUtils libUtils = new JaosLibUtils();

        public AdditionsController(IMRepoDbContext context
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
            MemoryStream memoryStream = jtExcel.createExcelFile(jaoTable, ModelsTexts.Addition.titlePlural, "Report", Response);
            return File(memoryStream, JaoTableExcelServices.fileStyle, jtExcel.fileName);
        }



        //==============================================
        //----------------------------------------------



        async Task<JaoTable> fillTable()
        {
            const string dbView = "[report_Additions]";
            List<AdditionsReportModel> records;
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                records = (await sqlConnection.QueryAsync<AdditionsReportModel>($"select * from {dbView}")).ToList();
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



        JaoTable prepareTable(List<AdditionsReportModel> records)
        {
            jts.setExcelWidths(new float[] { 50, 300
                , 50, 50, 50, 50, 50, 50
                , 50, 50, 50, 50
            });
            jts.setTitle("Reporte Redeterminaciones");
            jts.setSubtitle("");

            JaosLibUtils libUtils = new JaosLibUtils();

            fieldTitle(nameof(AdditionsReportModel.projectCode)!, JaoTable.screenOnlyClass);
            fieldTitle(nameof(AdditionsReportModel.projectName)!);
            fieldTitle(nameof(AdditionsReportModel.additionCode)!);
            fieldTitle(nameof(AdditionsReportModel.stageName)!);
            fieldTitle(nameof(AdditionsReportModel.additionValue)!, JaoTable.classNumber);
            fieldTitle(nameof(AdditionsReportModel.DateDelivery)!, JaoTable.classNumber);
            fieldTitle(nameof(AdditionsReportModel.DateApproved)!, JaoTable.classNumber);
            fieldTitle(nameof(AdditionsReportModel.attached)!, JaoTable.screenOnlyClass);
            //fieldTitle(nameof(AdditionsReportModel.otherAttachments)!, $"{JaoTable.screenOnlyClass} {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(AdditionsReportModel.AdditionDelay)!, $"{JaoTable.classNumber}");

            foreach (var record in records)
            {
                jts.addRow();

                jts.addCell(record.projectCode,JaoTable.screenOnlyClass);
                jts.addCell(record.projectName);
                jts.addCell(record.additionCode);
                jts.addCell(record.stageName);
                jts.addCell(record.additionValue);
                jts.addCell(record.DateDelivery);
                jts.addCell(record.DateApproved);
                jts.addCell(record.attached, JaoTable.screenOnlyClass);
                //jts.addCell(record.otherAttachments, JaoTable.screenOnlyClass);
                jts.addCell(record.AdditionDelay);
            }
            return jts.getTable();
        }


        void fieldTitle(string fieldName, string cssClass = "")
        {
            jts.addHeaderCell(libUtils.titleOf<AdditionsReportModel>(fieldName), cssClass);
        }




    }
}
