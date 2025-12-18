using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Domain.Validations
{
    public class DateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || validationContext.ObjectInstance == null)
                return ValidationResult.Success;

            var instance = validationContext.ObjectInstance;
            
            // Get the start time property
            var startTimeProperty = instance.GetType().GetProperty("StartTime");
            var endTimeProperty = instance.GetType().GetProperty("EndTime");
            
            if (startTimeProperty == null || endTimeProperty == null)
                return ValidationResult.Success;

            var startTime = (DateTime?)startTimeProperty.GetValue(instance);
            var endTime = (DateTime?)endTimeProperty.GetValue(instance);

            if (startTime.HasValue && endTime.HasValue)
            {
                if (startTime.Value > endTime.Value)
                {
                    return new ValidationResult("Start time cannot be later than end time.");
                }
            }

            return ValidationResult.Success;
        }
    }
}