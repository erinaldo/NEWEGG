angular.module('EditObjectModule', [])
    .service('objEditService', function ($rootScope, PageData) {
        this.onClose = function () { } // 由各object editor指派
        this.edit = function (comp, obj) {
            var comp = PageData.ctrl.onEditComp;
            comp.editing = true;
            PageData.ctrl.isObjEdit = true;
            $(document).scrollTop(comp.YIndex);
            $("#obj-editor").dialog({
                width: $('#easel').width(),
                position: [$('#toolbar').position().left,
                    comp.YIndex - $(document).scrollTop() + comp.Height + $('#toolbar').height()]
            });
            $("#obj-editor").dialog('open');
            $("#obj-editor").dialog({ height: 'auto' });
        }

        this.close = function () {
            PageData.ctrl.isObjEdit = false;
            if (!!PageData.ctrl.onEditComp) {
                PageData.ctrl.onEditComp.editing = false;
            }
            $("#obj-editor").dialog('close');
            $("#obj-editor").dialog({ height: 500 });
            this.onClose();
        }
    })
    .directive('objEditor', function ($http, objEditService, Resources, PageData) {
        return {
            restrict: "AE",
            replace: true,
            templateUrl: "/Content/PageMgmt/template/obj-editor.html",
            controller: function ($scope) {
                $scope.PageData = PageData;
            },
            link: function (scope, element, attrs) {
                scope.resx = Resources.getPageMgmtResx();
                var closeEdit = function () {
                    objEditService.close();
                    scope.$apply();
                }
                $(function () {
                    $("#obj-editor").dialog({
                        title: scope.resx.EditObject,
                        autoOpen: false,
                        height: 500,
                        closeOnEscape: false,
                        dialogClass: 'MyDialog',
                        buttons: [{ text: "完成", click: function () { closeEdit() } }],
                        open: function (event, ui) { $(".ui-dialog-titlebar-close", this.parentNode).hide(); }
                    });
                });
            }
        };
    });
