// 當圖片有縮圖時，使用此class，若圖片找不到，會到上一層使用原圖，若原圖也找不到，則使用not-found-src屬性中的圖片連結

//jQuery版本
var imgError = function (element) {
    $(element).attr("src", function (i, val) {
        if (val.indexOf("Min/") > -1) {
            return val.replace("Min/", "");
        } else {
            var imgNotFoundSrc = $(this).attr("not-found-src");
            if (imgNotFoundSrc && val != imgNotFoundSrc) {
                return imgNotFoundSrc;
            } else {
                $(element).attr('onerror', '');
                $(element).hide();
            }
        }
    });
}

//Angularjs版本
angular.module('imgNotFoundSrc', [])
.directive('checkImgExist', function () {
    var d = {
        restrict: 'A',
        link: function (scope, element, attrs) {
            // 當圖片有縮圖時，使用此directive，若圖片找不到，會到上一層使用原圖，若原圖也找不到，則使用not-found-src屬性中的圖片連結
            element.bind('error', function () {
                if (element.attr('src').indexOf("Min/") > -1) {
                    element.attr('src', element.attr('src').replace("Min/", ""));
                } else {
                    var imgNotFoundSrc = attrs.imgNotFoundSrc;
                    if (imgNotFoundSrc && element.attr('src').src != imgNotFoundSrc) {
                        element.attr('src', imgNotFoundSrc);
                    }
                }
            });
        }
    };
    return d;
});
