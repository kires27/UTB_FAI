using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CalendarApp.Application.Validations
{
    // emits the unobtrusive data-* attributes consumed by custom-validators.js.
    public class DateRangeAttribute : Domain.Validations.DateRangeAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val", "true");
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-daterange", ErrorMessage ?? "Invalid date range.");
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-daterange-start", Domain.Validations.DateRangeAttribute.StartTimePropertyName);
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-daterange-end", Domain.Validations.DateRangeAttribute.EndTimePropertyName);
        }
    }
}
