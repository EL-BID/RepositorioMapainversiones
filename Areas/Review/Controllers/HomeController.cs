using JaosLib.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using IMRepo.Data;
using IMRepo.Models.Domain;
using IMRepo.Services.basic;
using IMRepo.Services.Utilities;
using IMRepo.Models.Reports;
using Microsoft.EntityFrameworkCore;
using Dapper;
using IMRepo.Areas.Review.Models;
using System.Globalization;

namespace IMRepo.Areas.Review.Controllers
{
    [Authorize(Roles = ProjectGlobals.registeredRoles)]
    [Area("Review")]
    public partial class HomeController : Controller
    {
        const string currencyName = "$ ";

        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IPipToolsService pipToolsService;
        private readonly INPOIWordService wordService;

        public HomeController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IPipToolsService pipToolsService
            , INPOIWordService wordService
            )
        {
            this.context = context;
            this.parentProjectService = parentProjectService;
            this.pipToolsService = pipToolsService;
            this.wordService = wordService;
        }

        JaosLibUtils jaosLibUtils = new JaosLibUtils();
        JaosDataTools dataTools = new JaosDataTools();

        //----------- Index

        // GET: Project
        public async Task<IActionResult> Index(int? id)
        {
            Project? project = await parentProjectService.getProjectFromIdOrSession(id, User, HttpContext.Session, ViewBag);
            if (project == null)
                return await Task.Run(() => RedirectToAction("Select", "Project", new { Area = "" }));

            project = await context.Project.
                Include(p => p.Sector_info).
                Include(p => p.Subsector_info).
                Include(p => p.Sdg_info).
                Include(p => p.Office_info).
                Include(p => p.ExecutingAgency_info).
                Include(p => p.Stage_info).
                Include(p => p.ProjectFundings!).ThenInclude(c => c.Source_info).
                Include(p => p.ProjectFundings!).ThenInclude(c => c.Type_info).
                Include(p => p.Extensions!).ThenInclude(c => c.Stage_info).
                Include(p => p.Extensions!).ThenInclude(c => c.ExtensionAttachments).
                Include(p => p.Products!).ThenInclude(c => c.Payments!).ThenInclude(y => y.Stage_info).
                Include(p => p.Products!).ThenInclude(c => c.Payments!).ThenInclude(y => y.PaymentAttachments).
                Include(p => p.Products!).ThenInclude(c => c.Additions!).ThenInclude(y => y.Stage_info).
                Include(p => p.Products!).ThenInclude(c => c.Additions!).ThenInclude(y => y.AdditionAttachments).
                AsNoTracking().
                FirstOrDefaultAsync(p => p.Id == project.Id);
            if (project == null)
                return await Task.Run(() => RedirectToAction("Select", "Product", new { Area = "" }));


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            setInfoViewBags(project);
            produceCharts(project);

            //                ViewBag.animationValue = "animate";
            ViewBag.navGeneral = "active";
            return View(project);
        }


        [HttpPost]
        public async Task<IActionResult> Back()
        {
            return await Task.Run(() => RedirectToAction("Index"));
        }

        //========================================================================
        //
        //     General Methods
        //
        //------------------------------------------------------------------------



        void setInfoViewBags(Project project)
        {
            if (project != null)
            {
                // finance
                ViewBag.planned = project.PlannedCost.HasValue ? currencyName + project.PlannedCost.Value.ToString("n0") : "";
                AdvanceBasic? info = getFinanceChartData(project);
                if (info != null)
                {
                    if (info.programmed.HasValue)
                        ViewBag.programmed = info.programmed.Value > 0 ? currencyName + info.programmed.Value.ToString("n0") : "";
                    if (info.actual.HasValue)
                        ViewBag.expended = info.actual.Value > 0 ? currencyName + info.actual.Value.ToString("n0") : "";
                    if (info.programmed.HasValue)
                    {
                        double? balance = dataTools.substract(info.programmed, info.actual);
                        ViewBag.balance = balance.HasValue && balance.Value > 0 ? currencyName + balance.Value.ToString("n0") : null;
                    }
                }

                // available for Totals
                //ProductSummaryModel? summary = getProductSummary(product);
                //ViewBag.summary = summary;

                // Dates and duration
                int? revisedDuration = pipToolsService.revisedDuration(project);
                ViewBag.revisedDuration = revisedDuration.HasValue ? revisedDuration.Value.ToString("n0") : "";
                DateTime? originalCompletionDate = pipToolsService.originalCompletionDate(project);
                ViewBag.originalCompletionDate = originalCompletionDate.HasValue ? originalCompletionDate.Value.ToString("dd/MM/yyyy") : null;
                DateTime? revisedCompletionDate = pipToolsService.revisedCompletionDate(project);
                ViewBag.revisedCompletionDate = revisedCompletionDate.HasValue ? revisedCompletionDate.Value.ToString("dd/MM/yyyy") : null;
                DateTime? startDate = pipToolsService.startDateOf(project);
                ViewBag.startDate = startDate.HasValue ? startDate.Value.ToString("dd/MM/yyyy") : null;

                // Project Information
                if (project.ProjectFundings != null && project.ProjectFundings.Any())
                    ViewBag.source = project.ProjectFundings.Where(i => i.Source_info != null).Select(i => i.Source_info!.Name).Aggregate((i, j) => i + ", " + j);

                if (project.Sector_info != null)
                    ViewBag.sector = project.Sector_info.Name;

                if (project.Office_info != null)
                    ViewBag.office = project.Office_info.Name;
            }

            // Totals x 4
            ViewBag.paymentTotals = getPaymentTotals(project.Id);
            ViewBag.additionTotals = getAdditionTotals(project.Id);
            ViewBag.extensionTotals = getExtensionTotals(project.Id);
        }




        //=========================================================================================================
        //
        //                               C  H  A  R  T  S
        //
        //---------------------------------------------------------------------------------------------------------

        void produceCharts(Project project)
        {
            ViewBag.dataChartFinance = produceFinanceChart(project);
            ViewBag.dataChartPhysical = produceIndicatorChart(project);
            ViewBag.dataChart3 = produceTimeChart(project);
        }

        //----------- Expenditure Chart

        string produceFinanceChart(Project project)
        {
            AdvanceBasic? info = getFinanceChartData(project);
            if (info != null)
            {
                if (info.programmed.HasValue)
                {
                    double ejecutado = (info.actual.HasValue ? info.actual.Value : 0) / ((info.programmed.HasValue && info.programmed.Value > 0) ? info.programmed.Value : 1) * 100;
                    if (ejecutado < 0) ejecutado = 0;
                    double pendiente = 100 - ejecutado;

                    return string.Format(@"[[""%"",""Pagado"",""Disponible""],[""%"",{0},{1}]]", chartValue(ejecutado), chartValue(pendiente));
                }
            }
            return "";
        }

        // Finance Data
        AdvanceBasic? getFinanceChartData(Project project)
        {
            AdvanceBasic? data = null;
            if (project?.Id > 0)
            {
                SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                try
                {
                    sqlConnection.Open();
                    List<AdvanceBasic> result = sqlConnection.Query<AdvanceBasic>(string.Format(@"
WITH productsCost AS (
    SELECT Product.[Project] AS projectId,
           SUM(Product.Cost) AS [value]
    FROM Product
    GROUP BY Product.Project
),
projectPayments AS (
    SELECT Product.[Project] AS projectId,
           SUM(payment.[PaymentAmount]) AS [Total]
    FROM Payment
    INNER JOIN Product ON Product.Id = Payment.Product
    WHERE Payment.Stage IN (2, 3, 4) -- 4: Devengado 5: pagado
    GROUP BY Product.[Project]
),
projectAdditions AS (
    SELECT Product.[Project] AS projectId,
           SUM(addition.[Value]) AS [value]
    FROM Addition
    INNER JOIN Product ON Product.Id = Addition.Product
    WHERE Addition.Stage IN (2, 3) -- 4: Devengado 5: pagado
    GROUP BY Product.[Project]
)

-- Main SQL
SELECT [Project].id AS [id],
       SUM(projectPayments.[Total]) AS actual,
       NULLIF(ISNULL(SUM([productsCost].[value]), 0) + ISNULL(SUM(projectAdditions.[value]), 0), 0) AS programmed,
       CASE
           WHEN 1 < SUM(projectPayments.[Total]) /
                  NULLIF(ISNULL(SUM([productsCost].[value]), 0) + ISNULL(SUM(projectAdditions.[value]), 0), 0)
               THEN 1
           ELSE
               SUM(projectPayments.[Total]) /
               NULLIF(ISNULL(SUM([productsCost].[value]), 0) + ISNULL(SUM(projectAdditions.[value]), 0), 0)
       END AS [percent]
FROM [Project]
LEFT JOIN productsCost ON [Project].id = productsCost.projectId
LEFT JOIN projectPayments ON [Project].id = projectPayments.projectId
LEFT JOIN projectAdditions ON [Project].id = projectAdditions.projectId
WHERE [Project].[id] = {0}
GROUP BY [Project].id;
", project.Id)).ToList();
                    if (result.Any())
                        data = result[0];
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return data;
        }


        //----------- Indicator Chart

        string produceIndicatorChart(Project project)
        {
            AdvanceBasic? info = getPhysicalAdvanceChart(project);
            if (info != null)
            {
                if (info.programmed.HasValue)
                {
                    double pendiente = (info.programmed.HasValue) ? info.programmed.Value * 100 : 0;
                    if (pendiente < 0) pendiente = 0;
                    double ejecutado = info.actual.HasValue ? info.actual.Value * 100 : 0;
                    if (ejecutado < 0) ejecutado = 0;
                    return string.Format(@"[[""%"",""Ejecutado"",""Pendiente""],[""%"",{0},{1}]]", chartValue(ejecutado), chartValue(pendiente));
                }
            }
            return "";
        }

        // Physical Advance Data
        AdvanceBasic? getPhysicalAdvanceChart(Project project)
        {
            AdvanceBasic? data = null;
            if (project?.Id > 0)
            {
                SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                try
                {
                    sqlConnection.Open();
                    List<AdvanceBasic> result = sqlConnection.Query<AdvanceBasic>(string.Format(@"
With 
productPayments as (
SELECT 
[Product].[Project] as projectId
, sum(payment.[PhysicalAdvance]) / 100 AS [performed]
from Payment
left join Product on Product.id = Payment.Product
where Payment.Stage in (2,3,4,5) --5: pagado
GROUP BY [Product].[Project]
)

-- main SQL
select 
[Project].id as [id]
,avg(productPayments.[performed]) as actual
,1 - isnull(avg(productPayments.[performed]),0) as [programmed]
,avg(productPayments.[performed]) as [percent]
from [Project]
left join productPayments on [Project].id = productPayments.projectId
where [Project].[id] = {0}
group by [Project].id
", project.Id)).ToList();
                    if (result.Any())
                    {
                        data = result[0];
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return data;
        }



        //----------- Time Chart

        string produceTimeChart(Project project)
        {

            //------------------------- Dibujo 3 TimeLine
            DateTime? startDate = pipToolsService.startDateOf(project);
            DateTime? completionDate = pipToolsService.revisedCompletionDate(project);
            double? avance = null;
            double? pendiente = null;
            if (startDate.HasValue && startDate != DateTime.MinValue && completionDate.HasValue && completionDate != DateTime.MinValue)
            {
                if (startDate.Value < DateTime.Now && completionDate.Value > DateTime.Now)
                {
                    avance = DateTime.Now.Subtract(startDate.Value).TotalDays;
                    pendiente = completionDate.Value.Subtract(DateTime.Now).TotalDays;
                }
                if (startDate.Value < DateTime.Now && completionDate.Value < DateTime.Now)
                    if (startDate.Value < completionDate.Value)
                    {
                        avance = completionDate.Value.Subtract(startDate.Value).TotalDays;
                        pendiente = null;
                    }
                if (startDate.Value > DateTime.Now && completionDate.Value > DateTime.Now)
                    if (startDate.Value < completionDate.Value)
                    {
                        avance = null;
                        pendiente = completionDate.Value.Subtract(startDate.Value).TotalDays;
                    }
            }
            if (avance.HasValue || pendiente.HasValue)
                return string.Format(@"[[""días"",""Transcurridos"",""Faltantes""],[""días"",{0},{1}]]", chartIntValue(avance), chartIntValue(pendiente));
            else
                return "";
        }


        //===============================================
        //           Chart Utilities
        //-----------------------------------------------

        string chartSet(string title, double? value1, double? value2)
        {
            return string.Format(@",[""{0}"",{1},{2}]", title, chartValue(value1), chartValue(value2));
        }
        string chartValue(double? number)
        {
            CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
            return number.HasValue ? number.Value.ToString("0.0", usCulture) : "null";
        }

        string chartIntValue(double? number)
        {
            return number.HasValue ? number.Value.ToString("f0") : "null";
        }

        string chartValue(DateTime? date)
        {
            if (date.HasValue)
                return date.Value.Year.ToString();
            else
                return ("null");
        }


        //string createChartData(List<FinanceBasic> data)
        //{
        //    string chartData = "";
        //    if (data?.Count > 0)
        //    {
        //        int startYear = data.FindAll(d => d.year > 0).Min(e => e.year);
        //        int endYear = data.FindAll(d => d.year > 0).Max(e => e.year);
        //        if (startYear > 0 && endYear > 0)
        //        {
        //            chartData = @"[[""Year"",""Actual"",""Programmed""]";
        //            for (int y = startYear; y <= endYear; y++)
        //            {
        //                FinanceBasic? yearData = data.Find(e => e.year == y);
        //                if (yearData != null)
        //                    chartData += chartSet(y.ToString(), yearData.actual, yearData.programmed);
        //                else
        //                    chartData += chartSet(y.ToString(), null, null);
        //                ;
        //            }
        //            chartData += "]";
        //        }
        //    }
        //    return chartData;
        //}



        //---------------------------------------------------------------------------------------------------------------




        // ====================================================================
        //
        //  Detailed information
        //---------------------------------------------------------------------

        ProductSummaryModel? getProductSummary(Product product)
        {
            ProductSummaryModel? data = null;
            if (product.Id > 0)
            {
                SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
                try
                {
                    sqlConnection.Open();
                    List<ProductSummaryModel> result = sqlConnection.Query<ProductSummaryModel>(string.Format(@"
With 
productPayments as (
SELECT 
Payment.[Product] as productId
, sum(payment.[Total]) AS [Total]
, sum(payment.[PhysicalAdvance]) AS [performed]
from Payment
where Payment.Stage in (4, 5) --4:devengado 5: pagado
GROUP BY Payment.[Product]
)
,productAdditions as (
SELECT 
Addition.[Product] as productId
, sum(addition.Value) as [value]
from Addition
where stage = 3 -- 3:Aprobada
group by Addition.[Product]
)

-- main SQL
select 
[product].id as ProductId
,[product].record as record
,[product].[name] as [name]
,[product].[OriginalValue] as [originalValue]
,
	isnull([product].OriginalValue,0) 
	+ isnull(productAdditions.[value],0) 
	as [programmedValue]
,productPayments.[Total] as actualValue
,productPayments.[performed] as [advanced]
from [Product]
left join productPayments on [Product].id = productPayments.productId
left join productAdditions on [Product].id = productAdditions.productId
where [Product].[id] = {0}
", product.Id)).ToList();

                    if (result.Any())
                    {
                        data = result[0];
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return data;
        }





    }
}
