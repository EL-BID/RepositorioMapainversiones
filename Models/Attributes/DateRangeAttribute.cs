using System.ComponentModel.DataAnnotations;

namespace IMRepo.Models.Domain;
/// <summary>
/// An attribute that controls the range of a date
/// Controls that a date is within a standard dateRange defined for the system.
/// The date range is determined in ProjectGlobals (Min and MaxValidDateString)
/// Usage:        [DateRange]
/// </summary>
public class DateRangeAttribute : ValidationAttribute
{
    private readonly DateTime _minDate;
    private readonly DateTime _maxDate;

    public DateRangeAttribute()
    {
        if (!DateTime.TryParse(ProjectGlobals.MinValidDateString, out _minDate) || !DateTime.TryParse(ProjectGlobals.MaxValidDateString, out _maxDate))
        {
            throw new ArgumentException("Invalid date format");
        }
        if (_maxDate < _minDate)
        {
            throw new ArgumentException("Max date should be greater than min date");
        }
    }
    public DateRangeAttribute(string minDate, string maxDate)
    {
        if (!DateTime.TryParse(minDate, out _minDate) || !DateTime.TryParse(maxDate, out _maxDate))
        {
            throw new ArgumentException("Invalid date format");
        }

        if (_maxDate < _minDate)
        {
            throw new ArgumentException("Max date should be greater than min date");
        }
    }

    public DateRangeAttribute(DateTime minDate, DateTime maxDate)
    {
        _minDate = minDate;
        _maxDate = maxDate;
        if (_maxDate < _minDate)
        {
            throw new ArgumentException("Max date should be greater than min date");
        }
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if( date >= _minDate && date <= _maxDate)
                return ValidationResult.Success;
        }
        if (value == null)
            return ValidationResult.Success;

        return new ValidationResult(string.Format("{0}: fecha no permitida.", validationContext.DisplayName));
    }
}
