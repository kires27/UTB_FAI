// Client-side validation for FutureDateAttribute
$.validator.addMethod('futuredate', function (value, element, params) {
    if (!value) {
        return true; // Let required validator handle empty values
    }
    
    var inputDate = new Date(value);
    var now = new Date();
    now.setMinutes(now.getMinutes() - 1); // 1 minute buffer
    
    return inputDate >= now;
});

$.validator.unobtrusive.adapters.addBool('futuredate');

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