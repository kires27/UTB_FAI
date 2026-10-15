using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CalendarApp.Application.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _dependentProperty;
        private readonly object _targetValue;

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
            ErrorMessage ??= "This field is required.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var property = instance.GetType().GetProperty(_dependentProperty);
            if (property == null)
            {
                return ValidationResult.Success;
            }

            var dependentValue = property.GetValue(instance);
            var conditionMet = string.Equals(
                Convert.ToString(dependentValue),
                Convert.ToString(_targetValue),
                StringComparison.OrdinalIgnoreCase);

            if (!conditionMet)
            {
                return ValidationResult.Success;
            }

            var isEmpty = value == null ||
                          (value is string text && string.IsNullOrWhiteSpace(text));
            if (isEmpty)
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
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-requiredif", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-requiredif-dependentproperty", _dependentProperty);
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-requiredif-targetvalue", Convert.ToString(_targetValue)?.ToLowerInvariant() ?? "true");
        }
    }
}
