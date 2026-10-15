using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CalendarApp.Application.Validations
{
    // Shared helper for emitting unobtrusive client-validation attributes.
    public static class ClientValidationHelper
    {
        public static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }
    }
}
