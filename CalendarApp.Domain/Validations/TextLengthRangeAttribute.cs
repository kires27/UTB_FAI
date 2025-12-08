using System.ComponentModel.DataAnnotations;

namespace CalendarApp.Domain.Validations
{
    public class TextLengthRangeAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public TextLengthRangeAttribute(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
            ErrorMessage = $"Text must be between {minLength} and {maxLength} characters";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle null values
            }

            if (value is string text)
            {
                if (text.Length < _minLength || text.Length > _maxLength)
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid text format");
        }
    }
}