namespace IMRepo.Areas.Review.Models
{
	public class DashboardInfo
	{
		public int? Office { get; set; }
		public int? Sector { get; set; }
		public int? Subsector { get; set; }
		public int? NotStartedProjectsCount { get; set; }
		public double? NotStartedProjectsValue { get; set; }
		public int? OngoingProjectsCount { get; set; }
		public double? OngoingProjectsCost { get; set; }
		public int? FinishedProjectsCount { get; set; }
		public double? FinishedProjectsCost { get; set; }
		public int? ToStartProjectsCount { get; set; }
		public double? TotalCostOfAllProducts { get; set; }
		public double? TotalPayedValue { get; set; }
		public double? TotalRemainingValue { get; set; }

        public int? PendingPaymentsCount { get; set; }
        public double? PendingPaymentsValue { get; set; }
        public int? PendingAdditionsCount { get; set; }
        public double? PendingAdditionsValue { get; set; }
        public int? PendingExtensionsCount { get; set; }


        public int? ProjectsEndedCurrentYear { get; set; }
        public int? ProjectsEndedLastYear { get; set; }
        public int? ProjectsEndedTwoYearsAgo { get; set; }
        public int? ProjectsEndedThreeYearsAgo { get; set; }


        public int? ProjectsNoAdvance { get; set; }
		public int? ProjectsAdvance0to25 { get; set; }
		public int? ProjectsAdvance25to50 { get; set; }
		public int? ProjectsAdvance50to75 { get; set; }
		public int? ProjectsAdvance75to100 { get; set; }

        public int? ProjectsStageNotStarted { get; set; }
        public int? ProjectsStageOngoing { get; set; }
        public int? ProjectsStageEnded { get; set; }

		public string? LocationInfo { get; set; }
    }
}
