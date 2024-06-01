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
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using NPOI.OpenXmlFormats.Dml;

namespace IMRepo.Areas.Review.Controllers
{
    [Authorize(Roles = ProjectGlobals.registeredRoles)]
    [Area("Review")]
    public partial class DashboardController : Controller
    {
        const string currencyName = "$ ";

        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentContractService;
        private readonly IPipToolsService pipToolsService;
        private readonly INPOIWordService wordService;

        public DashboardController(IMRepoDbContext context
            , IParentProjectService parentContractService
            , IPipToolsService pipToolsService
            , INPOIWordService wordService
            )
        {
            this.context = context;
            this.parentContractService = parentContractService;
            this.pipToolsService = pipToolsService;
            this.wordService = wordService;
        }

        JaosLibUtils jaosLibUtils = new JaosLibUtils();
        JaosDataTools dataTools = new JaosDataTools();

        //----------- Index

        // GET: Project

        [HttpGet]
        public IActionResult Index()
        {
            var contractsInfo = context.DbDashboardInfo.ToList();
            var sectorsData = context.SectorsQty.ToList();

            var dashboardData = new DashboardViewModel
            {
                DashboardProjectsInfo = contractsInfo,
                SectorsQty = sectorsData
            };
            setViewBagsForLists();
            dashboardData.totals(null);
            produceCharts(dashboardData);
            return View(dashboardData);
        }

        [HttpPost]
        public IActionResult Index(DashboardViewModel? model)
        {
            var contractsInfo = context.DbDashboardInfo.ToList();
            var sectorsData = context.SectorsQty.ToList();

            var dashboardData = new DashboardViewModel
            {
                DashboardProjectsInfo = contractsInfo,
                SectorsQty = sectorsData
            };
            setViewBagsForLists();
            dashboardData.totals(model);
            produceCharts(dashboardData);
            return View(dashboardData);
        }


        //=========================================================================================================
        //
        //                               Supporting Methods
        //
        //---------------------------------------------------------------------------------------------------------

        void setViewBagsForLists()
        {

            // set options for Project
            var listProject = new SelectList(context.Project.OrderBy(c => c.Name).Select(r => new SelectListItem(r.Code + " - " + r.Name , r.Id.ToString())), "Value", "Text").ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = "Seleccionar proyecto" });
            ViewBag.listProject = listProject;

            // set options for Sector
            var listSector = new SelectList(context.Sector.OrderBy(c => c.Name).Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text").ToList();
            listSector.Insert(0, new SelectListItem { Value = "0", Text = "Seleccionar una opción" });
            ViewBag.listSector = listSector;

            // set options for Subsector
            var listSubsector = new SelectList(context.Subsector.OrderBy(c => c.Sector).OrderBy(c => c.Name).Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text").ToList();
            listSubsector.Insert(0, new SelectListItem { Value = "0", Text = "Seleccionar una opción" });
            ViewBag.listSubsector = listSubsector;
            ViewBag.listSubsectorParent = context.Subsector.Select(r => new { parentId = r.Sector.ToString(), id = r.Id.ToString() }).ToList().ToJson();

            // set options for Stage
            var listStage = new SelectList(context.ProjectStage.OrderBy(c => c.Name).Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text").ToList();
            listStage.Insert(0, new SelectListItem { Value = "0", Text = "Seleccionar una opción" });
            ViewBag.listStage = listStage;

            // set options for Office
            var listOffice = new SelectList(context.Office.OrderBy(c => c.Name).Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text").ToList();
            listOffice.Insert(0, new SelectListItem { Value = "0", Text = "Seleccionar una opción" });
            ViewBag.listOffice = listOffice;
        }





        //=========================================================================================================
        //
        //                               C  H  A  R  T  S
        //
        //---------------------------------------------------------------------------------------------------------

        void produceCharts(DashboardViewModel dashboardData)
        {
            ViewBag.contractsPerStageChart = contractsPerStageChart(dashboardData.Total);
            ViewBag.contractsPerYearChart = contractsPerYearChart(dashboardData.Total);
            ViewBag.physicalAdvanceChart = physicalAdvanceChart(dashboardData.Total);
            ViewBag.contractsPerSectorChart = contractsPerSectorChart(dashboardData.SectorsQty);
        }

        //----------- Contracts per year Chart


        string contractsPerStageChart(DashboardInfo info)
        {
            int year = DateTime.Now.Year;
            string chartData = @"[[""Estado"",""Proyectos""]";
            chartData += chartSet("A iniciar", info.ProjectsStageNotStarted);
            chartData += chartSet("En ejecución", info.ProjectsStageOngoing);
            chartData += chartSet("Finalizada", info.ProjectsStageEnded);
            chartData += "]";
            return chartData;
        }

        string contractsPerYearChart(DashboardInfo info)
        {// #9cc8e3 #69abd4 #398fc7 #0a578a #043a5d #0c1a5b
            int year = DateTime.Now.Year;
            string chartData = @"[[""Año"",""Proyectos"", { role: ""style"" }]";
            chartData += chartSet((year - 3).ToString(), info.ProjectsEndedThreeYearsAgo, @"""color: #9cc8e3""");
            chartData += chartSet((year - 2).ToString(), info.ProjectsEndedTwoYearsAgo, @"""color: #69abd4""");
            chartData += chartSet((year - 1).ToString(), info.ProjectsEndedLastYear, @"""color: #398fc7""");
            chartData += chartSet((year).ToString(), info.ProjectsEndedCurrentYear, @"""color: #0a578a""");
            //            chartData += chartSet(string.Format("{0} y posterior", year + 1), info.ProjectsNotFinishedPlannedToEndInFuture, @"""color: #043a5d""");
            //            chartData += chartSet("Sin planificar", info.ProjectsNoEndDateOrPlannedEndDate, @"""color: #0c1a5b""");
            chartData += "]";
            chartData = chartData.Replace("null", "0");
            return chartData;
        }


        string physicalAdvanceChart(DashboardInfo info)
        {
            string chartData = @"[[""%"",""Proyectos"", { role: ""style"" }]";
            chartData += chartSet("Sin Avance", info.ProjectsNoAdvance, @"""color: #9cc8e3""");
            chartData += chartSet("0-25%", info.ProjectsAdvance0to25, @"""color: #69abd4""");
            chartData += chartSet("+25%", info.ProjectsAdvance25to50, @"""color: #398fc7""");
            chartData += chartSet("+50%", info.ProjectsAdvance50to75, @"""color: #0a578a""");
            chartData += chartSet("+75%", info.ProjectsAdvance75to100, @"""color: #043a5d""");
            chartData += "]";

            return chartData;
        }

        string contractsPerSectorChart(List<ItemQty> info)
        {
            string chartData = @"[[""Estado"",""Proyectos""]";
            foreach (var item in info)
                chartData += chartSet(item.itemName, item.itemCount);
            chartData += "]";
            return chartData;
        }


        //string contractsPerYearChart(DashboardContractInfo info)
        //{
        //    if (contractInfo != null)
        //    {
        //            double ejecutado = (info.actual.HasValue ? info.actual.Value : 0) / ((info.programmed.HasValue && info.programmed.Value > 0) ? info.programmed.Value : 1) * 100;
        //            if (ejecutado < 0) ejecutado = 0;
        //            double pendiente = 100 - ejecutado;

        //            return string.Format(@"[[""%"",""Pagado"",""Disponible""],[""%"",{0},{1}]]", chartValue(ejecutado), chartValue(pendiente));
        //    }
        //    return "";
        //}



        //===============================================
        //           Chart Utilities
        //-----------------------------------------------
        string chartSet(string title, double? value)
        {
            return string.Format(@",[""{0}"",{1}]", title, chartValue(value));
        }

        string chartSet(string title, double? value1, double? value2)
        {
            return string.Format(@",[""{0}"",{1},{2}]", title, chartValue(value1), chartValue(value2));
        }

        string chartSet(string title, double? value1, string color)
        {
            return string.Format(@",[""{0}"",{1},{2}]", title, chartValue(value1), color);
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




        //---------------------------------------------------------------------------------------------------------------






    }
}
