const originalRange = $.validator.methods.range;
var wrappedRange = function (value, element, param) {
    var fixedValue2 = parseFloat(value.toString().replace(",", "."));
    return originalRange.call($.validator.prototype, fixedValue2, element, param);
};
$.validator.methods.range = wrappedRange;
