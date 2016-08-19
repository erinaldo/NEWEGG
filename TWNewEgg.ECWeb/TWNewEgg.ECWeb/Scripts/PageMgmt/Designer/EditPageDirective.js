angular.module('EditPageDirective', ['ngSanitize', 'DesignerServices', 'EditObjectModule', 'Status', 'DB'])
    .directive('pageEdit', function ($http, $rootScope, $filter, Status, objEditService, Resources, msgService, controlService, Model, DataService, PageData, Resource) {
        return {
            scope: {
                path: '='
            },
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/EditPage.html",
            controller: function ($scope) {
                $scope.subpath = Resource.subpath;
                $scope.pagemgmtResx = Resources.getPageMgmtResx();
                $scope.componentStatus = Resource.CompStatus; 
                $scope.PageData = PageData;
                $scope.DataService= DataService;
                $scope.controlService = controlService;
                
                $scope.newPage = function () {
                    msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['CleanConfirm'] + '(' + $scope.PageData.PageInfo.Path + ')?', function () {
                        for (var i in PageData.PageInfo.ComponentInfo) {
                            if (PageData.PageInfo.ComponentInfo[i].Status == $scope.componentStatus.NEW) {
                                delete PageData.PageInfo.ComponentInfo[i];
                            } else {
                                PageData.PageInfo.ComponentInfo[i].Status = $scope.componentStatus.DELETE;
                            }
                        }
                        $scope.$apply();
                    });
                }
                
                $scope.savePage = function () {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['SaveConfirm'] + '(' + PageData.PageInfo.Path + ')?', function () {
                            DataService.savePage(msg);
                        });
                    });
                }
                $scope.cancelPage = function () {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['CancelConfirm'] + '(' + PageData.PageInfo.Path + ')?', function () {
                            setTimeout(DataService.selectPage(DataService.getOnlyPageInfo(PageData.PageInfo), false), 1000);
                        });
                    });
                }
                $scope.recoverPage = function () {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['RecoverConfirm'] + '(' + PageData.PageInfo.Path + ')?', function () {
                            DataService.recoverPage(msg);
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
                $scope.bkstring = function () {
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
                var msg = function (msg, isEditPage) {
                    msgService.msg($scope.pagemgmtResx['Confirm'], msg, function () {
                        DataService.selectPage(PageData.PageInfo, isEditPage, function () {
                            DataService.getSiteMap();
                        });
                    });
                }
            },
            link: function (scope, element, attrs) {
                
                scope.show = function () {
                    $('#toolbar-functions').toggle('blind', { }, 500);
                }
                $("#upload-photo-dialog").dialog({
                    autoOpen: false,
                    modal: true,
                    resizable: false,
                    width: "auto",
                    height: "auto",
                    open: function (e, ui) {
                    }
                });
                $("#msg-dialog").dialog({
                    autoOpen: false,
                    height: 150,
                    width: 200,
                    closeOnEscape: false,
                    dialogClass: 'MyDialog',
                    modal: true,
                    open: function (event, ui) { $(".ui-dialog-titlebar", this.parentNode).hide(); }
                });
                tinyMCE.init({
                    mode: "none",
                    theme: "simple"
                });
            }
        };
    })
    .directive('sitemap', function ($http, Status, Resources, msgService, PageData, DataService, Model) {
        return {
            scope: {
            },
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/sitemap.html",
            controller: function ($scope) {
                $scope.pagemgmtResx = Resources.getPageMgmtResx();
                $scope.Status = Status.PageStatus;
                $scope.PageData = PageData;
                $scope.editpage = function (page) {
                    LoginProcess(function () {
                        DataService.selectPage(page, true, function () {
                            setTimeout(DataService.getSiteMap(), 1000);
                        });
                    });
                }
                $scope.showpage = function (page) {
                    LoginProcess(function () {
                        DataService.selectPage(page, false);
                    });
                }
                $scope.deletepage = function (page) {
                    LoginProcess(function () {
                        msgService.confirm($scope.pagemgmtResx['Confirm'], $scope.pagemgmtResx['DeleteConfirm'] + '(' + page.Path + ')?', function () {
                            DataService.deletePage(page, function (data) {
                                msgService.msg($scope.pagemgmtResx['Confirm'], data, function () {
                                    DataService.getSiteMap();
                                    DataService.init();
                                });
                            });
                        });
                    });
                }
                $scope.addpage = function (path) {
                    LoginProcess(function () {
                        var page = new Model['p']({ Path: path, Name: path });
                        DataService.addPage(page, function (data) {
                            msgService.msg($scope.pagemgmtResx['Confirm'], data, function () {
                                DataService.getSiteMap();
                                $scope.newPagePath = '';
                                $scope.showadd = false;
                            });
                        });
                    });
                }
                $scope.showUrl = function (page) {
                    LoginProcess(function () {
                        var url = '/p/' + page.Path;
                        window.prompt("Copy to clipboard: Ctrl+C, Enter", url);
                    });
                }
            },
            link: function (scope, element, attrs) {
                scope.show = function () {
                    $('#cssmenu').toggle('slide', { direction: 'left' }, 500);
                }
                DataService.getSiteMap();
            }
        };
    })
    .directive('property', function ($http, controlService, PageData, $rootScope, Resources, COPY, Resource) {
        return {
            scope: {
            },
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/property.html",
            controller: function ($scope) {
                $scope.Resource = Resources.getPropertyResx();
                $scope.subpath = Resource['subpath'] + PageData.PageInfo.Path + "/";
                $scope.fullpath = (Resource['imageServerPath'] + $scope.subpath).toLowerCase();
                $scope.PageData = PageData;
                $scope.controlService = controlService;
                $scope.COPY = COPY;
                $scope.changeBkImg = function (img) {
                    $scope.backgroundImg = 'url(' + Resource.subpath + img.FileName + ')';
                }
                $scope.clearBk = function () {
                    $scope.PageData.PageInfo.BackgroundImg = "";
                    $scope.backgroundImg = "";
                    $scope.backgroundColor = "";
                    $scope.backgroundOther = "";
                }
                $scope.saveBk = function () {
                    $scope.PageData.PageInfo.BackgroundImg = ($scope.backgroundColor || "") + " " + ($scope.backgroundImg || "") + " " + ($scope.backgroundOther || "");
                }
            },
            link: function (scope, element, attrs) {
                scope.$watch('PageData.PageInfo', function () {
                    scope.isFixedRatio = false;
                    scope.isSnapToComp = false;
                    scope.isLockedPos = false;
                    scope.COPY.init();
                    scope.backgroundImg = scope.PageData.PageInfo.BackgroundImg;
                });
                scope.show = function () {
                    $('#property-detail').toggle('slide', { direction: 'right' }, 500);
                }
                scope.openPageBkChooser = function () {
                    $("#imgChooser").dialog('open');
                }
                scope.openUploader = function () {
                    $("#upload-photo-dialog").dialog('open');
                    var uploadedfiles = [];
                    uploader.init({
                        subpath: '~' + scope.subpath,
                        size: '5mb',
                        multi_selection: true,
                        container: 'upload-photo-dialog',
                        ext: 'img',
                        max_file_count: 1,
                        success: function (up, files) {
                            scope.backgroundImg = 'url(' + scope.fullpath + files[0].target_name + ')';
                            //直接改背景，不存檔
                        }
                    });
                    uploader.show();
                }
                scope.snapToComp = function () {
                    if (scope.isSnapToComp) {
                        $('.obj').draggable({ snap: true });
                    } else {
                        $('.obj').draggable({ snap: false });
                    }
                }
                scope.$watch('isFixedRatio', function () {
                    $rootScope.$broadcast('fixedRatio', { isFixedRatio: scope.isFixedRatio });
                });
                scope.fixedRatio = function () {
                    //無法使用跟draggable一樣的方法改
                    
                }
                scope.$watch('isLockedPos', function () {
                    if (scope.isLockedPos) {
                        $('.obj').draggable('disable');
                    } else {
                        $('.obj').draggable('enable');
                    }
                });
                $(function () {
                    $("#property-detail").accordion({
                        autoHeight: false
                    });
                    $("#imgChooser").dialog({
                        width: 500,
                        autoOpen: false
                    });
                });
            }
        };
    })
    .directive('imgChooser', function ($http, Resource) {
        return {
            scope: {
                callback: '='
            },
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/imgChooser.html",
            link: function (scope, element, attrs) {
                scope.subpath = Resource.subpath;
                scope.AlbumImages = [];
                scope.pages = [];
                scope.PageCnt = 0;
                scope.page = 1;
                
                scope.useImg = function (img) {
                    scope.callback(img);
                }
                scope.openUploader = function (img) {
                    $("#upload-photo-dialog").dialog('open');
                    var uploadedfiles = [];
                    uploader.init({
                        subpath: '~' + scope.subpath,
                        size: '5mb',
                        multi_selection: true,
                        container: 'upload-photo-dialog',
                        ext: 'img',
                        max_file_count: 1,
                        success: function (up, files) {
                            img.FileName = files[0].target_name;
                            $http.post('/pagemgmt/Image/uploadImg', { 'img': img }).success(function (data) {
                                scope.AlbumImages = data;
                            });
                        }
                    });
                    uploader.show();
                }
                scope.getAlbumImages = function (page) {
                    scope.page = page;
                    $http.post('/pagemgmt/Image/getAllData', { topage: scope.page }).success(function (data) {
                        scope.AlbumImages = data.photos;
                        if (data.Count % data.take != 0) {
                            scope.PageCnt = Math.floor(data.Count / data.take) + 1;
                        } else {
                            scope.PageCnt = data.Count / data.take;
                        }
                        for (var i = 0; i < scope.PageCnt; i++) {
                            scope.pages[i] = i + 1;
                        }
                    });
                }
                scope.getAlbumImages(1);
            }
        };
    })
    .directive('componentOption', function (Status, objEditService, PageData) {
        return {
            scope: {
                component: '=',
                show: '='
            },
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/ComponentOption.html",
            link: function (scope, element, attrs) {
                scope.lock = false;
                scope.ctrl = PageData.ctrl;
                scope.editComp = function () {
                    if (scope.component != scope.ctrl.onEditComp || scope.ctrl.isObjEdit == false) {
                        scope.ctrl.onEditComp = scope.component;
                        objEditService.edit();
                    }
                }
                scope.deleteComp = function () {
                    if (scope.component.Status == Status.ComponentStatus.NEW) { //新增
                        delete PageData.PageInfo.ComponentInfo[scope.component.ComponentID];
                    } else {
                        scope.component.Status = Status.ComponentStatus.DELETE;//刪除
                    }
                }
                scope.isSelected = function () {
                    if (scope.ctrl.onEditComp == scope.component && scope.ctrl.isObjEdit) { return true; }
                    else { return false; }
                }
            }
        };
    })
    .controller('ComponentController', function ($scope, $element, Status, controlService, $rootScope, COPY) {
        $scope.componentUrl = "/Content/PageMgmt/template/Component.html";
        $scope.show = false;
        $scope.init = function (comp) {
            $scope.Component = comp;
            if ($scope.PageData.PageInfo.ext.isEditPage) {
                $($element).draggable({
                    containment: "parent",
                    delay: 200,
                    grid: [5, 5],
                    stop: function (e, ui) {
                        $scope.$apply(function () {
                            var moveX = $($element).position().left - $scope.Component.XIndex;
                            var moveY = $($element).position().top - $scope.Component.YIndex;
                            $scope.Component.YIndex = parseInt($scope.Component.YIndex) + moveY;
                            $scope.Component.XIndex = parseInt($scope.Component.XIndex) + moveX;
                            if ($scope.PageData.PageInfo.ext.pickedComps.length > 0) {
                                controlService.multiDrag($scope.Component.ComponentID, moveX, moveY);
                            }
                        });
                    }
                });
                $($element).resizable({
                    containment: "parent",
                    grid: [5, 5],
                    stop: function (e, ui) {
                        $scope.$apply(function () {
                            $scope.Component.Height = $(ui.element).height();
                            $scope.Component.Width = $(ui.element).width();
                        });
                    }
                });
            }
        }
        $scope.tempZIndex = 500;
        
        $scope.$on('fixedRatio', function (e, arg) {
            if (arg.isFixedRatio) {
                $($element).resizable('destroy');
                $($element).resizable({
                    containment: "parent",
                    grid: [5, 5],
                    aspectRatio: true,
                    stop: function (e, ui) {
                        $scope.$apply(function () {
                            $scope.Component.Height = $(ui.element).height();
                            $scope.Component.Width = $(ui.element).width();
                        });
                    }
                });
            } else {
                $($element).resizable('destroy');
                $($element).resizable({
                    containment: "parent",
                    grid: [5, 5],
                    aspectRatio: false,
                    stop: function (e, ui) {
                        $scope.$apply(function () {
                            $scope.Component.Height = $(ui.element).height();
                            $scope.Component.Width = $(ui.element).width();
                        });
                    }
                });
            }
        });
        $scope.focus = function (e) {
            if ($scope.PageData.PageInfo.ext.isEditPage) {
                controlService.selectComponent($scope.Component, e);
            }
        }
    });
