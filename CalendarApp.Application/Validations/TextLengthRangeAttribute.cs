using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CalendarApp.Application.Validations
{
    /// emits the data-* attributes consumed by custom-validators.js.
    public class TextLengthRangeAttribute : Domain.Validations.TextLengthRangeAttribute, IClientModelValidator
    {
        public TextLengthRangeAttribute(int minLength, int maxLength) : base(minLength, maxLength)
        {
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val", "true");
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-textlengthrange", ErrorMessage ?? "Invalid text length.");
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-textlengthrange-min", MinLength.ToString());
            ClientValidationHelper.MergeAttribute(context.Attributes, "data-val-textlengthrange-max", MaxLength.ToString());
        }
    }
}
