using Microsoft.AspNetCore.Mvc.Rendering;
using IMRepo.Models.Domain;

namespace IMRepo.Services.Utilities
{
    public interface IPipToolsService
    {
        #region Dates and Fiscal Year
        //======================================================================
        //
        // Dates and Fiscal Year
        //
        //----------------------------------------------------------------------

        //==============
        // Dates

        public DateTime? minDate(DateTime? date1, DateTime? date2);

        public DateTime? maxDate(DateTime? date1, DateTime? date2);

        public string monthRangeName(DateTime startDate, DateTime endDate);

        public int numberOfMonths(DateTime startDate, DateTime endDate);


        //==============
        // Fiscal Year

        public int? fiscalYearFor(DateTime? date);

        public DateTime firstDateOfFiscalYear(int fiscalYear);

        public DateTime lastDateOfFiscalYear(int fiscalYear);

        public bool inFiscalYear(DateTime date, int fiscalYear);


        /// <summary>
        /// Indicates if a date is within a date range.
        /// </summary>
        /// <param name="date">Date to be validated</param>
        /// <param name="startDate">First date in range</param>
        /// <param name="endDate">Last date in range</param>
        /// <returns></returns>
        public bool inDateRange(DateTime date, DateTime startDate, DateTime endDate);


        //=========================
        // Fiscal Year -> Time periods
        public DateTime startOfQuarter(int fiscalYear, int quarter);
        public DateTime endOfQuarter(int fiscalYear, int quarter);
        public string quarterMonths(int fiscalYear, int quarter);

        #endregion
        #region Projects Dates
        //==============================================================================
        //
        //                          P R O J E C T S
        //
        //------------------------------------------------------------------------------

        //---------------------------- Project related ---------------------------------
        //======================
        // Fiscal Year

        int? startFiscalYear(List<Project> projects);
        int? endFiscalYear(List<Project> projects);
        List<int>? getFiscalYears(List<Project> projects);

        int? setFiscalYearsViewBag(List<Project> projects, dynamic ViewBag);

        DateTime earliestDateOfProjectInFiscalYear(List<Project> projects, int fiscalYear);
        DateTime latestDateOfProjectInFiscalYear(List<Project> projects, int fiscalYear);


        bool isProjectInFiscalYear(List<Project> projects, int fiscalYear);


        //======================
        // Start and Completion dates

        int? revisedDuration(Project project);
        DateTime? startDateOf(Project project);
        DateTime? originalCompletionDate(Project? project);
        DateTime? revisedCompletionDate(Project? project);

        DateTime? startDateOf(List<Project> projects);
        DateTime? originalCompletionDate(List<Project> projects);
        DateTime? revisedCompletionDate(List<Project> projects);


        #endregion

    }
}
