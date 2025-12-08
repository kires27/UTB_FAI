var originalStep = $.validator.methods.step;
var wrappedStep = function (value, element, param) {
    var fixedValue = parseFloat(value.toString().replace(",", "."));
    return originalStep.call($.validator.prototype, fixedValue, element, param);
};
$.validator.methods.step = wrappedStep;
