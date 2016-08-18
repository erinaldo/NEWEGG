angular.module('CusWidgets', [])

.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });
                event.preventDefault();
            }
        });
    };
})
.directive('outClick', function ($document) {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            $document.bind('click', function (e) {
                if (e.target != elem[0]) {
                    scope.$apply(attr.outClick);
                }
            })
        }
    }
})
.directive('ngConfirm', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            var clickAction = attr.ngConfirm;
            element.bind('click', function (event) {
                if (window.confirm(attr.msg)) {
                    scope.$eval(attr.ngConfirm);
                }
            });
        }
    };
})
.directive('filteredInput', function ($filter) {
    return {
        require: 'ngModel',
        transclude: true,
        link: function (s, e, a, c) {
            var flags = a.regexFlags || '';
            var regex = new RegExp(a.regex, flags);
            s.$watch(a.ngModel, function (v) {
                s.result = s.$eval(a.ngModel);
                if (typeof (s.result) == 'string') {
                    s.result = s.result.replace(regex, "");
                    s.$eval(a.ngModel + "='" + s.result + "'");
                }
            });
        }
    }
})
.directive('imgUpload', function () {
    var result = {
        scope: {
            setting:'='
        },
        restrict: 'A',
        link: function (scope, element, attrs) {
            $("#" + scope.setting.container).dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: "auto",
                height: "auto",
                open: function (e, ui) {
                    uploader.init({
                        //minsubpath:scope.setting.subpath+"/Min",
                        subpath: scope.setting.subpath,
                        size: scope.setting.size,
                        multi_selection: true,
                        container: scope.setting.container,
                        ext: scope.setting.ext,
                        max_file_count: scope.setting.max_file_count,
                        resizeSet: scope.setting.resizeSet,
                        resizeValue: scope.setting.resizeValue,
                        success: function (up, files) {
                        }
                    });
                    uploader.show();
                }
            });
        }
    };
    return result;
})
.directive('imgSelector', function ($http) {
    var result = {
        scope: { setting:'=' },
        restrict: 'A',
        templateUrl: "/Areas/filemgmt/templates/imgSelector.html",
        replace: true,
        link: function (scope, element, attrs) {
            $("#" + scope.setting.container).dialog({
                width: 500,
                autoOpen: false,
                modal: true,
                resizable: false,
                width: "auto",
                height: "auto",
                open: function (e, ui) {
                    scope.getImages();
                }
            });
            scope.useImg = function (img) {
                scope.setting.callback(img);
            }
            scope.DeleteImg = function (img) {
                //scope.setting.Deletecallback(img);
                //scope.getImages();
                $http.post('/filemgmt/file/DeleteFile', {
                    subpath: "~" + scope.setting.root, name: img, compression: "Y"
                }).success(function (data) {
                    scope.setting.Deletecallback(img);
                    scope.getImages();
               });
            }
            scope.getImages = function () {
                $http.post('/filemgmt/file/GetImgList', { root: "~" + scope.setting.root, searchPattern: scope.setting.searchPattern }).success(function (data) {
                    scope.images = data;
                    scope.$apply();
                });
            }
        }
    };
    return result;
})
.directive('noImage', function () {
    var setDefaultImage = function (e, errsrc) {
        e.attr('src', errsrc);
    };
    return {
        transclude: true,
        restrict: 'A',
        link: function (s, e, a, c) {
            var errsrc = a.errSrc;
            s.$watch(function () {
                return a.ngSrc;
            }, function () {
                s.show = false;
                var src = a.ngSrc;
                if (!a.ngSrc) {
                    setDefaultImage(e, errsrc);
                }
            });
            $(e).load(function () {
                s.$apply(function () { s.show = true; });
            })
            e.bind('error', function () {
                setDefaultImage(e, errsrc);
            });
        }
    }
})
.directive('bindUnsafeHtml', ['$compile', function ($compile) {
    return function (scope, element, attrs) {
        scope.$watch(
          function (scope) {
              // watch the 'bindUnsafeHtml' expression for changes
              return scope.$eval(attrs.bindUnsafeHtml);
          },
          function (value) {
              // when the 'bindUnsafeHtml' expression changes
              // assign it into the current DOM
              element.html(value);

              // compile the new DOM and link it to the current
              // scope.
              // NOTE: we only compile .childNodes so that
              // we don't get into infinite loop compiling ourselves
              $compile(element.contents())(scope);
          }
      );
    };
}])
.directive('repeatDone', function ($http) {
    return function (scope, element, attrs) {
        if (scope.$last) {
            scope.$eval(attrs.repeatDone);
        }
    };
})
;