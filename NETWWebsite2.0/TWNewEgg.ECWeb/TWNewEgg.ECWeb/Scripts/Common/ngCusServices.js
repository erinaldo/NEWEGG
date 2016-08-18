angular.module('CusServices', [])
.service('msgService', function () {
    this.confirm = function (btnName, msg, callback,Canclecallback) {
        $("#msg-dialog").dialog({
            buttons: [
                { text: btnName, click: function () { callback(); closeMsg(); } },
                {
                    text: '取消', click: function () {
                        if (Canclecallback)
                        {
                            Canclecallback();
                        }
                        closeMsg();
                    }
                }
            ]
        });
        $("#msg-dialog").html(msg);
        $("#msg-dialog").dialog('resize');
        $("#msg-dialog").dialog('open');
    }
    this.msg = function (btnName, msg, callback) {
        $("#msg-dialog").dialog({
            buttons: [
                { text: btnName, click: function () { callback(); closeMsg(); } }
            ]
        });
        $("#msg-dialog").html(msg);
        $("#msg-dialog").dialog('resize');
        $("#msg-dialog").dialog('open');
    }
    var closeMsg = function () {
        $("#msg-dialog").dialog('close');
    }
});