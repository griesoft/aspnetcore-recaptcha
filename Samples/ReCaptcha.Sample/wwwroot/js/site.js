// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var onloadCallback = function () {
    alert("grecaptcha is ready!");

    //@* grecaptcha.render('test-button', {
    //    'sitekey': '@options.CurrentValue.SiteKey',
    //    'callback': successCallback
    //});*@

        };
var successCallback = function(token) {
    alert("grecaptcha was successfull! The response is: " + token);

    //$("#g-recaptcha-response").val(token);

    $("#demo-form").submit();
};
var recaptchaValidationDefaultCallback = function () {
    alert("grecaptcha was successfull! The response is: ");
};