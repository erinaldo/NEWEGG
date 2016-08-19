angular.module('PageDirective', ['ngSanitize', 'DesignerServices', 'DB'])
    .directive('pageView', function ($http, DataService, PageData, Resource, Resources, msgService) {
        return {
            scope: { path: '=', groupId: '=' },
            restrict: "AE",
            replace: true,
            template: '<div ng-include="getContentUrl()"></div>',
            controller: function ($scope) {
                $scope.isEditMode = false;
                $scope.isAdmin = false;
                $scope.toogleEditButton = "編輯";
                $scope.PageData = PageData;
                $scope.DataService = DataService;
                $scope.pagemgmtResx = Resources.getPageMgmtResx();
                $scope.ComponentStatus = { SAVED: "S", EDIT: "E", NEW: "N", DELETE: "D" };
                $scope.getContentUrl = function () {
                    if ($scope.isEditMode) return "/Content/PageMgmt/template/Page_E.html";
                    else return "/Content/PageMgmt/template/Page.html"
                }
                $scope.savePage = function () {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['SaveConfirm'] + '(' + PageData.PageInfo.Path + ')?', function () {
                            DataService.savePage(msg);
                        });
                    });
                }
                $scope.auditPage = function () {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['WaitConfirm'] + '(' + PageData.PageInfo.Path + ')?', function () {
                            DataService.auditPage(msg);
                        });
                    });
                }
                $scope.cancelAudit = function () {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['RejectConfirm'] + '(' + PageData.PageInfo.Path + ')?', function () {
                            DataService.rejectPage(msg);
                        });
                    });
                }
                $scope.launchPage = function () {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['ActiveConfirm'] + '(' + PageData.PageInfo.Path + ')?', function () {
                            DataService.launchPage(msg);
                        });
                    });
                }
                $scope.toggleEdit = function () {
                    LoginProcess(function () {
                        if (!$scope.isEditMode) {
                            $scope.isEditMode = true;
                            $scope.toogleEditButton = "完成";
                            DataService.selectPage({ Path: $scope.path }, true, function () {
                                PageData.ctrl.isObjEditable = false;
                            });
                        } else {
                            $scope.isEditMode = false;
                            $scope.toogleEditButton = "編輯";
                            DataService.getActivePage($scope.path);
                        }
                    });
                }
                $scope.inEdit = function () {
                    var result = false;
                    if (PageData.PageInfo != null &&
                        (PageData.PageInfo.Status == Resource.PageStatus['Editing'] || PageData.PageInfo.Status == Resource.PageStatus['Reject'])) {
                        result = true;
                    }
                    return result;
                }
                $scope.inAudit = function () {
                    var result = false;
                    if (PageData.PageInfo != null && PageData.PageInfo.Status == Resource.PageStatus['Waiting']) {
                        result = true;
                    }
                    return result;
                }
                var msg = function (data) {
                    msgService.msg($scope.pagemgmtResx['Confirm'], data, function () {
                        DataService.selectPage(PageData.PageInfo, true);
                    });
                }
            },
            link: function (scope, element, attrs) {
                scope.$watch('path', function () {
                    if (scope.groupId == '1') {
                        scope.isAdmin = true;
                    }
                    DataService.getActivePage(scope.path);
                });
                scope.bkstring = function () {
                    var bk = '';
                    var color = '#ffffff';
                    var image = 'url(' + Resource.subpath + PageData.PageInfo.BackgroundImg + ')';
                    var repeat = 'repeat-y';
                    var attachment = 'right';
                    var position = 'top';
                    bk = color + ' ' + image + ' ' + repeat + ' ' + attachment + ' ' + position;
                    if (PageData.PageInfo.BackgroundImg == "" || PageData.PageInfo.BackgroundImg == null) {
                        bk = "";
                    }
                    return bk;
                }
            }
        };
    })
.directive('componentView', function () {
    return {
        scope: { component: '=' },
        restrict: "AE",
        replace: true,
        templateUrl: "/Content/PageMgmt/template/Component.html"
    };
});