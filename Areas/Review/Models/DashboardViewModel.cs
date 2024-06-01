using Microsoft.EntityFrameworkCore;
using IMRepo.Models.Reports;

namespace IMRepo.Areas.Review.Models
{
    [Keyless]
    public class DashboardViewModel
    {
        public int Office { get; set; }
        public int Sector { get; set; }
        public int Subsector { get; set; }

        public List<DashboardInfo> DashboardProjectsInfo { get; set; } = new List<DashboardInfo>();
        public List<ItemQty> SectorsQty { get; set; } = new List<ItemQty>();

        public DashboardInfo Total { get; set; }

        public DashboardInfo totals(DashboardViewModel? model)
        {
            var temList = DashboardProjectsInfo;
            if (model?.Office > 0)
                temList = temList.Where(x => x.Office == model.Office).ToList();
            if (model?.Sector > 0)
                temList = temList.Where(x => x.Sector == model.Sector).ToList();
            if (model?.Subsector > 0)
                temList = temList.Where(x => x.Subsector == model.Subsector).ToList();
            this.Total = new DashboardInfo
            {
                Office = temList.Sum(x => x.Office),
                Sector = temList.Sum(x => x.Sector),
                Subsector = temList.Sum(x => x.Subsector),
                NotStartedProjectsCount = temList.Sum(x => x.NotStartedProjectsCount),
                NotStartedProjectsValue = temList.Sum(x => x.NotStartedProjectsValue),

                OngoingProjectsCount = temList.Sum(x => x.OngoingProjectsCount),
                OngoingProjectsCost = temList.Sum(x => x.OngoingProjectsCost),
                FinishedProjectsCount = temList.Sum(x => x.FinishedProjectsCount),
                FinishedProjectsCost = temList.Sum(x => x.FinishedProjectsCost),
                ToStartProjectsCount = temList.Sum(x => x.ToStartProjectsCount),
                TotalCostOfAllProducts = temList.Sum(x => x.TotalCostOfAllProducts),
                TotalPayedValue = temList.Sum(x => x.TotalPayedValue),
                TotalRemainingValue = temList.Sum(x => x.TotalRemainingValue),

                PendingPaymentsCount = temList.Sum(x => x.PendingPaymentsCount),
                PendingPaymentsValue = temList.Sum(x => x.PendingPaymentsValue),
                PendingAdditionsCount = temList.Sum(x => x.PendingAdditionsCount),
                PendingAdditionsValue = temList.Sum(x => x.PendingAdditionsValue),
                PendingExtensionsCount = temList.Sum(x => x.PendingExtensionsCount),

                ProjectsEndedCurrentYear = temList.Sum(x => x.ProjectsEndedCurrentYear),
                ProjectsEndedLastYear = temList.Sum(x => x.ProjectsEndedLastYear),
                ProjectsEndedTwoYearsAgo = temList.Sum(x => x.ProjectsEndedTwoYearsAgo),
                ProjectsEndedThreeYearsAgo = temList.Sum(x => x.ProjectsEndedThreeYearsAgo),


                ProjectsNoAdvance = temList.Sum(x => x.ProjectsNoAdvance),
                ProjectsAdvance0to25 = temList.Sum(x => x.ProjectsAdvance0to25),
                ProjectsAdvance25to50 = temList.Sum(x => x.ProjectsAdvance25to50),
                ProjectsAdvance50to75 = temList.Sum(x => x.ProjectsAdvance50to75),
                ProjectsAdvance75to100 = temList.Sum(x => x.ProjectsAdvance75to100),

                ProjectsStageNotStarted = temList.Sum(x => x.ProjectsStageNotStarted),
                ProjectsStageOngoing = temList.Sum(x => x.ProjectsStageOngoing),
                ProjectsStageEnded = temList.Sum(x => x.ProjectsStageEnded),
                LocationInfo = temList.FirstOrDefault()?.LocationInfo
            };
            return Total;
        }
    }


}
