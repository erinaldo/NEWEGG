angular.module('EditObjectDirective', ['ngSanitize','DB'])
.directive('editTextObject', function (objEditService, tinymceConfig) {
    var resizeWidth = function(width) {
        $("#mce-editor_ifr").width(width + 'px');
    }
    return {
        scope: {
            ngComponent: '='
        },
        restrict: "AE",
        replace: true,
        templateUrl: "/Content/PageMgmt/template/EditObject/edit-Text.html",
        controller: function ($scope) {
            $scope.ContentMceConfig = new tinymceConfig();
            $scope.ContentMceConfig.setup = function (fn) {
                fn.on('init', function (args) {
                    resizeWidth($scope.ngComponent.Width);
                });
            }
        },
        link: function (scope, element, attrs) {
            scope.$watch('ngComponent', function () {
                if (tinymce.activeEditor) {
                    tinymce.activeEditor.setContent(scope.ngComponent.Text.Content);
                }
            });
            scope.$watch('ngComponent.Width', function () {
                if (tinymce.activeEditor) resizeWidth(scope.ngComponent.Width);
            });
        }
    };
})
.directive('editDynamicObject', function (ApiRegist) {
    return {
        scope: {
            ngComponent: '=',
            vModel: '='
        },
        restrict: "AE",
        replace: true,
        templateUrl: "/Content/PageMgmt/template/EditObject/edit-Dynamic.html",
        link: function (scope, element, attrs) {
            scope.params = {};
            scope.types = ApiRegist;
            
            scope.$watch('ngComponent', function () {
                for (i in scope.types) {
                    if (scope.types[i].name == scope.ngComponent.Dynamic.Type) {
                        scope.myType = scope.types[i];
                    }
                }
            });
            scope.$watch('myType', function () {
                scope.params = {};
                scope.myApi = {};
                refresh();
            });
            scope.search = function () {
                scope.ngComponent.Dynamic.Type = scope.myType.name;
                scope.ngComponent.Dynamic.Api = scope.myApi.name;
                scope.ngComponent.Dynamic.Parameters = "";
                for (i in scope.params) {
                    scope.ngComponent.Dynamic.Parameters += '/' + scope.params[i];
                }
            }
            var refresh = function () {
                scope.apis = scope.myType.apis;
                for (i in scope.apis) {
                    if (scope.apis[i].name == scope.ngComponent.Dynamic.Api) {
                        scope.myApi = scope.apis[i];
                        scope.paramNames = scope.myApi.params;
                        for (var j = 0; j < scope.paramNames.length; j++) {
                            if (scope.ngComponent.Dynamic.Api == scope.myApi.name) {
                                scope.params[scope.paramNames[j]] = scope.ngComponent.Dynamic.Parameters.split('/')[j + 1];
                            }
                            else {
                                scope.params[scope.paramNames[j]] = '';
                            }
                        }
                    }
                }
            }
        }
    };
})
.directive('editImageObject', function ($filter, $http, Resources, Slider, SliderService, Resource, objEditService, Model, PageData) {
    return {
        scope: {
            ngComponent: '='
        },
        restrict: "AE",
        replace: true,
        templateUrl: "/Content/PageMgmt/template/EditObject/edit-Image.html",
        controller: function ($scope) {
            $scope.resx = Resources.getPageMgmtResx();
            $scope.Targets = [{ Target: '_blank', Name: $scope.resx.TargetBlank }, { Target: '_self', Name: $scope.resx.TargetSelf }];
            $scope.selected = {};
        },
        link: function (scope, element, attrs) {
            scope.subpath = Resource['subpath'] + PageData.PageInfo.Path + "/";
            scope.fullpath = (Resource['imageServerPath'] + scope.subpath).toLowerCase();
            scope.$watch('ngComponent', function () {
                scope.images = scope.ngComponent.Image;
                scope.selected = scope.images[0];
            });
            scope.fitOriginalSize = function () {
                var imgobj = new Image();
                imgobj.name = scope.fullpath + scope.selected.FileName;
                imgobj.src = scope.fullpath + scope.selected.FileName;
                imgobj.onload = function () {
                    scope.ngComponent.Height = this.height;
                    scope.ngComponent.Width = this.width;
                    scope.$apply();
                }
            }
            scope.openUploader = function(img) {
                $("#upload-photo-dialog").dialog('open');
                var uploadedfiles = []
                uploader.init({
                    subpath: '~'+scope.subpath,
                    size: '5mb',
                    multi_selection: true,
                    container: 'upload-photo-dialog',
                    ext: 'img',
                    max_file_count: 1,
                    success: function (up, files) {
                        img.FileName = files[0].target_name;
                        img.Path = scope.fullpath + img.FileName;
                    }
                });
                uploader.show();
            }
        }
    };
});
