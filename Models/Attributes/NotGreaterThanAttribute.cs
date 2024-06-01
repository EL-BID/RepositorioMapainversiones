using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace IMRepo.Models.Domain
{
    /// <summary>
    /// Validation that controls a property value not being greater than another property value
    /// sample usage:         [NotGreaterThan("PlannedEndDate")]
    /// </summary>
    public class NotGreaterThanAttribute : ValidationAttribute
    {
        private readonly string compareProperty;

        public NotGreaterThanAttribute(string compareProperty)
        {
            this.compareProperty = compareProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(compareProperty);
            if (otherPropertyInfo != null)
            {
                if (value is DateTime currentDate)
                {
                    var otherDate = (DateTime?)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
                    if (currentDate > otherDate)
                    {
                        var attributeUtils = new AttributeUtils();
                        return new ValidationResult(string.Format("{0} no puede ser posterior a {1}", validationContext.DisplayName, attributeUtils.GetDisplayName(validationContext.ObjectType, compareProperty)));
                    }
                }
                else if (value is decimal currentDecimal)
                {
                    var otherDecimal = (Decimal?)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
                    if (currentDecimal > otherDecimal)
                    {
                        var attributeUtils = new AttributeUtils();
                        return new ValidationResult(string.Format("{0} no puede ser mayor que {1}", validationContext.DisplayName, attributeUtils.GetDisplayName(validationContext.ObjectType, compareProperty)));
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
