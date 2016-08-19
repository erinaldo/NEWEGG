
var initGreCaptcha = function (idName, callBackFunction, theme, type) {
    grecaptcha.render(idName, {
        'sitekey': '6LdBUgATAAAAAMLJw020y8KArh95QbmWer4P_q7k',
        'callback': verifyCallback,
        'theme': theme,
        'type': type
    });
};