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

    public class PaymentsController : Controller
    {

        private readonly IMRepoDbContext context;
        private readonly IJaoTableServices jts;
        private readonly IJaoTableExcelServices jtExcel;


        JaosLibUtils libUtils = new JaosLibUtils();

        public PaymentsController(IMRepoDbContext context
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
            MemoryStream memoryStream = jtExcel.createExcelFile(jaoTable, ModelsTexts.Payment.titlePlural, "Report", Response);
            return File(memoryStream, JaoTableExcelServices.fileStyle, jtExcel.fileName);
        }



        //==============================================
        //----------------------------------------------



        async Task<JaoTable> fillTable()
        {
            const string dbView = "[report_Payments]";
            List<PaymentsReportModel> records;
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                records = (await sqlConnection.QueryAsync<PaymentsReportModel>($"select * from {dbView}")).ToList();
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



        JaoTable prepareTable(List<PaymentsReportModel> records)
        {
            jts.setExcelWidths(new float[] { 50, 300
                , 50, 50, 50, 50, 50, 50
                , 50, 50, 50, 50, 50
                , 50, 50, 50, 50, 50
                , 50, 50, 50, 50, 50
            });
            jts.setTitle("Reporte de Pagos");
            jts.setSubtitle("");

            JaosLibUtils libUtils = new JaosLibUtils();

            fieldTitle(nameof(PaymentsReportModel.projectCode)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PaymentsReportModel.projectName)!);
            fieldTitle(nameof(PaymentsReportModel.paymentCode)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PaymentsReportModel.stageName)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PaymentsReportModel.reportedMonth)!, JaoTable.classDate);
            fieldTitle(nameof(PaymentsReportModel.paymentValue)!, JaoTable.classNumber);
            fieldTitle(nameof(PaymentsReportModel.PhysicalAdvance)!, JaoTable.classNumber);
            fieldTitle(nameof(PaymentsReportModel.DateDelivery)!, JaoTable.classDate);
            fieldTitle(nameof(PaymentsReportModel.DateApproved)!, $"{JaoTable.classDate} {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PaymentsReportModel.DatePayed)!, $"{JaoTable.classDate} {JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PaymentsReportModel.PaymentDelay)!, JaoTable.classNumber);
            fieldTitle(nameof(PaymentsReportModel.attachedMedicion)!, $"{JaoTable.screenOnlyClass}");
            fieldTitle(nameof(PaymentsReportModel.attachedOrden)!, $"{JaoTable.screenOnlyClass}");

            foreach (var record in records)
            {
                jts.addRow();

                jts.addCell(record.projectCode, JaoTable.screenOnlyClass);
                jts.addCell(record.projectName);
                jts.addCell(record.paymentCode, JaoTable.screenOnlyClass);
                jts.addCell(record.stageName, JaoTable.screenOnlyClass);
                jts.addCell(record.reportedMonth);
                jts.addCell(record.paymentValue);
                jts.addCell(record.PhysicalAdvance);
                jts.addCell(record.DateDelivery);
                jts.addCell(record.DateApproved, JaoTable.screenOnlyClass);
                jts.addCell(record.DatePayed, JaoTable.screenOnlyClass);
                jts.addCell(record.PaymentDelay);
                jts.addCell(record.attachedMedicion, JaoTable.screenOnlyClass);
                jts.addCell(record.attachedOrden, JaoTable.screenOnlyClass);
            }
            return jts.getTable();
        }


        void fieldTitle(string fieldName, string cssClass = "")
        {
            jts.addHeaderCell(libUtils.titleOf<PaymentsReportModel>(fieldName), cssClass);
        }




    }
}
