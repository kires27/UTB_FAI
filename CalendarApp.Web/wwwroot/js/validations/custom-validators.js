
// Client-side validation for TextLengthRangeAttribute
$.validator.addMethod('textlengthrange', function (value, element, params) {
    if (!value) {
        return true; // Let required validator handle empty values
    }
    
    var minLength = parseInt(params.min);
    var maxLength = parseInt(params.max);
    
    return value.length >= minLength && value.length <= maxLength;
});

$.validator.unobtrusive.adapters.add('textlengthrange', ['min', 'max'], function (options) {
    var params = {
        min: options.params.min,
        max: options.params.max
    };
    options.rules['textlengthrange'] = params;
    options.messages['textlengthrange'] = options.message;
});

// Client-side validation for DateRangeAttribute
$.validator.addMethod('daterange', function (value, element, params) {
    var form = $(element).closest('form');
    var startTimeField = form.find('[name="StartTime"]');
    
    if (!value || !startTimeField.val()) {
        return true; // Let required validator handle empty values
    }
    
    var startTime = new Date(startTimeField.val());
    var endTime = new Date(value);
    
    // Check if start time is later than end time
    if (startTime > endTime) {
        return false;
    }
    
    return true;
});

$.validator.unobtrusive.adapters.add('daterange', [], function (options) {
    options.rules['daterange'] = {};
    options.messages['daterange'] = options.message;
});