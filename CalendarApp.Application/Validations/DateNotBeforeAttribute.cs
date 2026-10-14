using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CalendarApp.Application.Validations
{
    /// <summary>
    /// DateTime property must not be before another DateTime property on the same object
    /// (e.g. RecurrenceEnd must not be before StartTime). Works server-side and
    /// client-side (unobtrusive adapter in custom-validators.js).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateNotBeforeAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _otherProperty;

        public DateNotBeforeAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
            ErrorMessage ??= "Date cannot be before {0} reference.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var instance = validationContext.ObjectInstance;
            var property = instance.GetType().GetProperty(_otherProperty);
            if (property == null)
            {
                return ValidationResult.Success;
            }

            if (value is DateTime dateValue &&
                property.GetValue(instance) is DateTime otherDate &&
                dateValue < otherDate)
            {
                var displayName = validationContext.DisplayName;
                return new ValidationResult(
                    FormatErrorMessage(displayName),
                    new[] { validationContext.MemberName ?? string.Empty });
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val", "true");
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-datenotbefore", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-datenotbefore-otherproperty", _otherProperty);
        }
    }
}
