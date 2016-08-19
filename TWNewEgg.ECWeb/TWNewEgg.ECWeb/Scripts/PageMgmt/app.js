(function () {
    var initInjector = angular.injector(['ng']);
    var $http = initInjector.get('$http');
    $http.post('/Designer/GetResx').success(function (data) {
        var pagemgmtResx = {};
        var propertyResx = {};
        var sitemapResx = {};
        var cusObjResx = {};
        if (data.pagemgmtResx) {
            for (var i in data.pagemgmtResx) {
                pagemgmtResx[data.pagemgmtResx[i].Key] = data.pagemgmtResx[i].Value;
            }
        }
        if (data.propertyResx) {
            for (var i in data.propertyResx) {
                propertyResx[data.propertyResx[i].Key] = data.propertyResx[i].Value;
            }
        }
        if (data.sitemapResx) {
            for (var i in data.sitemapResx) {
                sitemapResx[data.sitemapResx[i].Key] = data.sitemapResx[i].Value;
            }
        }
        if (data.cusObjResx) {
            for (var prop in data.cusObjResx) {
                cusObjResx[data.cusObjResx[prop].Key] = data.cusObjResx[prop].Value;
            }
        }
        angular.module('config', []).constant('StringResources', {
            pagemgmtResx: pagemgmtResx,
            propertyResx: propertyResx,
            sitemapResx: sitemapResx,
            cusObjResx: cusObjResx
        });
        angular.module('designer-app', ['PageDirective', 'ObjectDirective', 'EditPageDirective', 'DesignerServices', 'EditObjectModule', 'EditObjectDirective', 'CusWidgets', 'CusServices', 'ui.tinymce']);
        angular.element(document).ready(function () {
            angular.bootstrap(document.getElementById('page_designer'), ['designer-app']);
        });
    });
})();

(function ($, undefined) {
    $.fn.multiDraggable = function (opts) {
        var initLeftOffset = []
            , initTopOffset = [];
        return this.each(function () {
            $(this).live("mouseover", function () {
                if (!$(this).data("init")) {
                    $(this).data("init", true).draggable(opts, {
                        start: function (event, ui) {
                            var pos = $(this).position();
                            $.each(opts.group || {}, function (key, value) {
                                var elemPos = $(value).position();
                                initLeftOffset[key] = elemPos.left - pos.left;
                                initTopOffset[key] = elemPos.top - pos.top;
                            });
                            opts.startNative ? opts.startNative() : {};
                        },
                        drag: function (event, ui) {
                            var pos = $(this).offset();
                            $.each(opts.group || {}, function (key, value) {
                                $(value).offset({
                                    left: pos.left + initLeftOffset[key],
                                    top: pos.top + initTopOffset[key]
                                });
                            });
                            opts.dragNative ? opts.dragNative() : {};
                        },
                        stop: function (event, ui) {
                            var pos = $(this).offset();
                            $.each(opts.group || {}, function (key, value) {
                                $(value).offset({
                                    left: pos.left + initLeftOffset[key],
                                    top: pos.top + initTopOffset[key]
                                });
                            });
                            opts.stopNative ? opts.stopNative() : {};
                        }
                    });
                }
            });
        });
    };
}(jQuery));