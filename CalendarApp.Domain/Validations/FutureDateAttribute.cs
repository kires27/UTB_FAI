using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Domain.Validations
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
        {
            ErrorMessage = "Date must be in the future";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle null values
            }

            if (value is DateTime dateTime)
            {
                if (dateTime < DateTime.Now.AddMinutes(-1)) // Allow 1 minute buffer for server time differences
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid date format");
        }
    }
}