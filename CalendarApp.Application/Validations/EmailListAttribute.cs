using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CalendarApp.Application.Validations
{
    /// <summary>
    /// Validates a free-text list of email addresses separated by commas or
    /// semicolons (e.g. the InviteEmails field). Null/empty passes — combine
    /// with [Required] when the field is mandatory. Works server-side and
    /// client-side (unobtrusive adapter in custom-validators.js).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EmailListAttribute : ValidationAttribute, IClientModelValidator
    {
        private static readonly Regex EmailRegex =
            new("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.Compiled);

        public EmailListAttribute()
        {
            ErrorMessage = "One or more of the invited email addresses is invalid.";
        }

        public static string[] SplitEmails(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<string>();
            }

            return value
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToArray();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string text || string.IsNullOrWhiteSpace(text))
            {
                return ValidationResult.Success;
            }

            var emails = SplitEmails(text);
            foreach (var email in emails)
            {
                if (!EmailRegex.IsMatch(email))
                {
                    var displayName = validationContext.DisplayName;
                    return new ValidationResult(
                        FormatErrorMessage(displayName),
                        new[] { validationContext.MemberName ?? string.Empty });
                }
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val", "true");
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-emaillist", ErrorMessage ?? "One or more of the invited email addresses is invalid.");
        }
    }
}
