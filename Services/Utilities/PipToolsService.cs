using Microsoft.AspNetCore.Mvc.Rendering;
using IMRepo.Models.Domain;

namespace IMRepo.Services.Utilities
{
    public class PipToolsService : IPipToolsService
    {
        const int FiscalYearMonth = 4;
        const int minValidFiscalYear = 1900;

        JaosDataTools dataTools = new JaosDataTools();

        #region Dates and Fiscal Year
        //======================================================================
        //
        // Dates and Fiscal Year
        //
        //----------------------------------------------------------------------

        //==============
        // Dates

        public DateTime? minDate(DateTime? date1, DateTime? date2)
        {
            if (date2.HasValue && date2.Value != DateTime.MinValue)
            {
                if (date1.HasValue && date1.Value != DateTime.MinValue)
                    return date1 < date2 ? date1 : date2;
                else
                    return date2;
            }
            else
                return date1;
        }

        public DateTime? maxDate(DateTime? date1, DateTime? date2)
        {
            if (date2.HasValue && date2.Value != DateTime.MinValue)
            {
                if (date1.HasValue && date1.Value != DateTime.MinValue)
                    return date1 > date2 ? date1 : date2;
                else
                    return date2;
            }
            else
                return date1;
        }

        public string monthRangeName(DateTime startDate, DateTime endDate)
        {
            if (startDate.Year > 0 && endDate.Year > 0)
            {
                if (startDate.Year == endDate.Year)
                    return $"{startDate.ToString("MMM")}-{endDate.ToString("MMM")} {startDate.ToString("yyyy")}";
                else if (startDate.Year > 0 && endDate.Year > 0)
                    return $"{startDate.ToString("MMM")}/{startDate.ToString("yyyy")}-{endDate.ToString("MMM")}/{endDate.ToString("yyyy")}";
                else if (startDate.Year > 0)
                    return $"{startDate.ToString("MMM")}/{startDate.ToString("yyyy")}-";
                else
                    return $"-{endDate.ToString("MMM")}/{endDate.ToString("yyyy")}";
            }
            return "";
        }

        public int numberOfMonths(DateTime startDate, DateTime endDate)
        {
            if (startDate != default && endDate != default)
                return (endDate.Year * 12 + endDate.Month) - (startDate.Year * 12 + startDate.Month) + 1;
            else
                return 0;
        }


        //==============
        // Fiscal Year

        public int? fiscalYearFor(DateTime? date)
        {
            if (date.HasValue && date.Value != DateTime.MinValue)
            {
                if (date.Value.Month >= FiscalYearMonth)
                    return date.Value.Year;
                else
                    return date.Value.Year - 1;
            }
            else
                return null;
        }

        public DateTime firstDateOfFiscalYear(int fiscalYear)
        {
            if (fiscalYear > minValidFiscalYear)
                return new DateTime(fiscalYear, FiscalYearMonth, 1);
            else
                return DateTime.MinValue;
        }

        public DateTime lastDateOfFiscalYear(int fiscalYear)
        {
            if (fiscalYear > minValidFiscalYear)
                return firstDateOfFiscalYear(fiscalYear).AddYears(1).AddDays(-1);
            else
                return DateTime.MinValue;
        }

        public bool inFiscalYear(DateTime date, int fiscalYear)
        {
            if (fiscalYear > minValidFiscalYear && date != DateTime.MinValue)
            {
                return date >= firstDateOfFiscalYear(fiscalYear) && date < firstDateOfFiscalYear(fiscalYear).AddYears(1); // to avoid time problems on last date
            }
            return false;
        }


        /// <summary>
        /// Indicates if a date is within a date range.
        /// </summary>
        /// <param name="date">Date to be validated</param>
        /// <param name="startDate">First date in range</param>
        /// <param name="endDate">Last date in range</param>
        /// <returns></returns>
        public bool inDateRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            if (date != DateTime.MinValue && startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                DateTime start = new DateTime(startDate.Year, startDate.Month, 1);
                DateTime end = new DateTime(endDate.Year, startDate.Month, 1).AddMonths(1);
                return (date >= start) && (date < end); // to avoid day and time problems on last date
            }
            return false;
        }


        //=========================
        // Fiscal Year -> Time periods
        public DateTime startOfQuarter(int fiscalYear, int quarter)
        {
            if (fiscalYear < 1900 || fiscalYear > 2100 || quarter < 0 || quarter > 3)
                return DateTime.MinValue;
            if (quarter < 3)
                return new DateTime(fiscalYear, 4 + 3 * quarter, 1);
            else
                return new DateTime(fiscalYear + 1, 1, 1);
        }
        public DateTime endOfQuarter(int fiscalYear, int quarter)
        {
            if (fiscalYear < 1900 || fiscalYear > 2100 || quarter < 0 || quarter > 3)
                return DateTime.MinValue;
            if (quarter < 3)
                return startOfQuarter(fiscalYear, quarter + 1).AddDays(-1);
            else
                return startOfQuarter(fiscalYear + 1, 0).AddDays(-1);
        }
        public string quarterMonths(int fiscalYear, int quarter)
        {
            DateTime start = startOfQuarter(fiscalYear, quarter);
            DateTime end = endOfQuarter(fiscalYear, quarter);
            return $"{start.ToString("MMM")}-{end.ToString("MMM")} {start.ToString("yyyy")}";
        }

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

        public int? startFiscalYear(List<Project> projects)
        {
            return fiscalYearFor(startDateOf(projects));
        }
        public int? endFiscalYear(List<Project> projects)
        {
            return fiscalYearFor(originalCompletionDate(projects));
        }

        public List<int>? getFiscalYears(List<Project> projects)
        {
            if (projects?.Count > 0)
            {
                int? startYear = startFiscalYear(projects);
                int? endYear = endFiscalYear(projects);
                if (startYear.HasValue && endYear.HasValue && startYear.Value <= endYear.Value)
                {
                    List<int> years = new List<int>();
                    for (int year = startYear.Value; year <= endYear.Value; year++)
                        years.Add(year);
                    return years;
                }
                else
                    return null;
            }
            else
                return null;
        }

        public int? setFiscalYearsViewBag(List<Project> projects, dynamic ViewBag)
        {
            List<int>? years = getFiscalYears(projects);
            if (years != null && years.Count > 0)
            {
                List<SelectListItem> yearsList = new List<SelectListItem>();
                foreach (int year in years)
                    yearsList.Add(new SelectListItem { Text = $"{year}-{year + 1}", Value = $"{year}" });
                ViewBag.yearList = yearsList;
                return years[0];
            }
            else
            {
                ViewBag.yearList = null;
                return null;
            }
        }

        public DateTime earliestDateOfProjectInFiscalYear(List<Project> projects, int fiscalYear)
        {
            if (fiscalYear > minValidFiscalYear)
            {
                DateTime firstDate = startDateOf(projects).GetValueOrDefault();
                DateTime lastDate = originalCompletionDate(projects).GetValueOrDefault();
                if (firstDate != default && lastDate != default)
                {
                    if (inFiscalYear(firstDate, fiscalYear))
                        return firstDate;
                    else if (inFiscalYear(lastDate, fiscalYear))
                        return firstDateOfFiscalYear(fiscalYear);
                    else if (firstDate < firstDateOfFiscalYear(fiscalYear) && lastDate > lastDateOfFiscalYear(fiscalYear))
                        return firstDateOfFiscalYear(fiscalYear);
                }
            }
            return default;
        }
        public DateTime latestDateOfProjectInFiscalYear(List<Project> projects, int fiscalYear)
        {
            if (fiscalYear > minValidFiscalYear)
            {
                DateTime firstDate = startDateOf(projects).GetValueOrDefault();
                DateTime lastDate = originalCompletionDate(projects).GetValueOrDefault();
                if (firstDate != default && lastDate != default)
                {
                    if (inFiscalYear(lastDate, fiscalYear))
                        return lastDate;
                    else if (inFiscalYear(firstDate, fiscalYear))
                        return lastDateOfFiscalYear(fiscalYear);
                    else if (firstDate < firstDateOfFiscalYear(fiscalYear) && lastDate > lastDateOfFiscalYear(fiscalYear))
                        return lastDateOfFiscalYear(fiscalYear);
                }
            }
            return default;
        }


        public bool isProjectInFiscalYear(List<Project> projects, int fiscalYear)
        {
            return earliestDateOfProjectInFiscalYear(projects, fiscalYear) != default;
        }



        //======================
        // Start and Completion dates

        public int? revisedDuration(Project project)
        {
            int? duration = null;
            if (project != null)
            {
                duration = project.PlannedDuration;
                if (project.Extensions?.Count > 0)
                {
                    foreach (Extension extension in project.Extensions)
                        duration = dataTools.add(duration, extension.Days);
                }
            }
            return duration;
        }

        public DateTime? originalCompletionDate(Project? project)
        {
            if (project != null)
            {
                if (project.PlannedStartDate.HasValue && project.PlannedDuration.HasValue)
                    if (project.PlannedStartDate != DateTime.MinValue)
                        return project.PlannedStartDate.Value.AddDays(project.PlannedDuration.Value);
            }
            return null;
        }
        public DateTime? revisedCompletionDate(Project? project)
        {
            DateTime? completionDate = null;
            if (project != null)
            {
                completionDate = startDateOf(project);
                if (completionDate != null && completionDate != DateTime.MinValue)
                {
                    int? duration = revisedDuration(project);
                    if (duration.HasValue)
                        completionDate = completionDate.Value.AddDays(duration.Value);
                }
            }
            return completionDate;
        }

        public DateTime? startDateOf(Project project)
        {
            if (project != null)
            {
                if (project.ActualStartDate != null && project.ActualStartDate != DateTime.MinValue)
                    return project.ActualStartDate;
                else if (project.PlannedStartDate != null && project.PlannedStartDate != DateTime.MinValue)
                    return project.PlannedStartDate;
            }
            return null;
        }


        public DateTime? startDateOf(List<Project> projects)
        {
            if (projects != null)
            {
                DateTime? date = null;
                foreach (var project in projects)
                {
                    date = minDate(date, startDateOf(project));
                }
                return date;
            }
            else
                return null;
        }

        public DateTime? originalCompletionDate(List<Project> projects)
        {
            if (projects != null)
            {
                DateTime? date = null;
                foreach (var project in projects)
                {
                    date = maxDate(date, originalCompletionDate(project));
                }
                return date;
            }
            else
                return null;
        }

        public DateTime? revisedCompletionDate(List<Project> projects)
        {
            if (projects != null)
            {
                DateTime? date = null;
                foreach (var project in projects)
                {
                    date = maxDate(date, revisedCompletionDate(project));
                }
                return date;
            }
            else
                return null;
        }


        #endregion



    }
}
