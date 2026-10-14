using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Domain.Validations
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public const string StartTimePropertyName = "StartTime";
        public const string EndTimePropertyName = "EndTime";

        public DateRangeAttribute()
        {
            ErrorMessage = "Start time cannot be later than end time.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || validationContext.ObjectInstance == null)
                return ValidationResult.Success;

            var instance = validationContext.ObjectInstance;
            
            var startTimeProperty = instance.GetType().GetProperty(StartTimePropertyName);
            var endTimeProperty = instance.GetType().GetProperty(EndTimePropertyName);
            
            if (startTimeProperty == null || endTimeProperty == null)
                return ValidationResult.Success;

            var startTime = (DateTime?)startTimeProperty.GetValue(instance);
            var endTime = (DateTime?)endTimeProperty.GetValue(instance);

            if (startTime.HasValue && endTime.HasValue)
            {
                if (startTime.Value > endTime.Value)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}