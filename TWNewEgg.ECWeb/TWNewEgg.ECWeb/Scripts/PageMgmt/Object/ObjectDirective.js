angular.module('ObjectDirective', ['ngSanitize', 'DesignerServices', 'DB'])
    .directive('textObject', function () {
        return {
            scope: { ngComponent: '=', editable: '=' },
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/Object/Text.html",
            controller: function ($scope) { },
            link: function (scope, element, attrs) {
                scope.ngComponent.Text.Content = scope.ngComponent.Text.Content;
            }
        };
    })
    .directive('imageObject', function (Resource, $filter, PageData, Slider, SliderService) {
        return {
            scope: { ngComponent: '=' },
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/Object/Image.html",
            link: function (scope, element, attrs) {
                var timer = null;
                var slider = {};
                scope.subpath = (Resource['imageServerPath'] + Resource['subpath']).toLowerCase();
                scope.selected = 0;
                scope.Effect = { None: 'N', Turn: 'T', Rotate: 'R' };
                scope.filterImg = { EffectGroupID: scope.ngComponent.ObjectID };
                scope.$watch('ngComponent', function () {
                    scope.images = scope.ngComponent.Image;
                });
                scope.setSlider = function () {
                    if (scope.images[0].Effect == scope.Effect['Rotate']) {
                        if (Slider[scope.ngComponent.ComponentID]) {
                            SliderService.reloadSlider(Slider[scope.ngComponent.ComponentID], scope.images[0]);
                        } else {
                            SliderService.setSlider(slider, scope.images[0], scope.ngComponent.ComponentID);
                        }
                    }
                }
                scope.$watch(function () { return scope.ngComponent.Height; }, function () {
                    if (Slider[scope.ngComponent.ComponentID]) {
                        SliderService.reloadSlider(Slider[scope.ngComponent.ComponentID], scope.images[0]);
                    }
                });
            }
        };
    })
    .directive('likeCnt', function () {
        return {
            scope: { item: '=' },
            restrict: "A",
            replace: true,
            template: "<div class='icon addLike' ng-click='like()' ng-class='{added: item.IsLike}'><span>{{item.LikeCnt}}</span></div>",
            controller: function ($scope, $http) {
                $scope.like = function () {
                    if (!$scope.item.IsLike) {
                        $http.post($scope.item.LikeApi).success(function () {
                            $scope.item.IsLike = true;
                            $scope.item.LikeCnt = $scope.item.LikeCnt + 1;
                        });
                    }
                }
            }
        };
    })
    .directive('dynamicObject', function (ApiRegist, $http) {
        var component = {
            scope: {
                ngComponent: '='
            },
            template: '<div ng-include="templateUrl"></div>',
            replace: true,
            restrict: 'AE',
            controller: function ($scope) {
                $scope.vModel = {};
                $scope.templateUrl = '';
                $scope.getTemplateUrl = function () {
                    return '/Content/PageMgmt/template/Object/' + $scope.ngComponent.Dynamic.Type + '.html';
                }
                var getApiUrl = function () {
                    var api = {};
                    for (i in ApiRegist) {
                        if (ApiRegist[i].name == $scope.ngComponent.Dynamic.Type) {
                            var type = ApiRegist[i];
                            $scope.apis = type.apis;
                            for (i in $scope.apis) {
                                if ($scope.apis[i].name == $scope.ngComponent.Dynamic.Api) {
                                    $scope.templateUrl = $scope.apis[i].templateUrl;
                                    api = $scope.apis[i].api;
                                }
                            }
                        }
                    }
                    return api + $scope.ngComponent.Dynamic.Parameters;
                }
                $scope.$watch('ngComponent.Dynamic.Parameters', function () {
                    $http.post(getApiUrl()).success(function (data) {
                        angular.copy(data, $scope.vModel);
                    });
                });
                $scope.like = function () {
                    $http.post($scope.vModel.LikeApi).success(function (data) {
                        $scope.vModel.LikeCnt = data;
                    });
                }
            },
            link: function (scope, element, attr) {
                
            }
        };
        return component;
    })
;

