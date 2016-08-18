(function (global) {
    if (typeof global.JSON === 'undefined') {
        global.JSON = {};
        global.JSON.parse = $.parseJSON;
    }
})(window);


$(window).ready(function () { $('#top_link').css('top', $(window).height() / 2); });
$(window).resize(function () { $('#top_link').css('top', $(window).height() / 2); });

var get_data = function (_url, _data) {
    $('<div id="mask" style="z-index:10000; width: 1920px; height: 2000px; opacity:0; position:absolute; top:0; left:0;"></div>').appendTo('body');
    var response_data = $.ajax({
        url: _url,
        data: _data,
        async: false,
        cache: false
    }).responseText;
    $('#mask').remove();
    return response_data;
};

var post_data = function (_url, _data) {
   
    $('<div id="mask" style="z-index:10000; width: 1920px; height: 2000px; opacity:0; position:absolute; top:0; left:0;"></div>').appendTo('body');
    var response_data = $.ajax({
        url: _url,
        data: _data,
        type: 'POST',
        async: false,
        cache: false
    }).responseText;
    $('#mask').remove();
    return response_data;
};

var get_data_with_valid = function (_url, _data, _success_func, _success_not_show_dialog) {
    var result = get_data(_url, _data);
    if (result[0] === '1') {
        if (!(_success_not_show_dialog === true)) {
            if (_success_func === undefined)
                show_dialog_ob(result.substr(1));
            else
                show_dialog_ob(result.substr(1), _success_func);
        } else _success_func();
        return true;
    } else {
        show_dialog_ob(result.substr(1));
        return false;
    }
}

var post_data_with_valid = function (_url, _data, _success_func, _success_not_show_dialog) {
    var result = post_data(_url, _data);
  
    if (result[0] === '1') {
        if (!(_success_not_show_dialog === true)) {
            if (_success_func === undefined)
                show_dialog_ob(result.substr(1));
            else
                show_dialog_ob(result.substr(1), _success_func);
        } else _success_func();
        return true;
    } else {
        show_dialog_ob(result.substr(1));
        return false;
    }
}

var post_data_with_valid_obj = function (obj) { //not use
    var result = post_data(obj.url, obj.data);
    if (result[0] === '1') {
        if (!(obj.success_not_show_dialog === true)) {
            if (obj.success_func === undefined)
                show_dialog_ob(result.substr(1));
            else
                show_dialog_ob(result.substr(1), obj.success_func);
        } else obj.success_func();
        return true;
    } else {
        show_dialog_ob(result.substr(1));

        return false;
    }
}

//res
var res_process = {};
var res = {};
res_process.req_data = { lt: [] };
res_process.load_res = function (res_file, names) {
    var i = 0;
    for (i = 0; i < names.length; i++) {
        res_process.req_data.lt.push({ 'resources_file': res_file, 'name': names[i] });
    }
}

res_process.get = function () {
    var rd = decodeURIComponent($.param(res_process.req_data)).replace(/(\]\[)/g, '].').replace(/(]=)/g, '=');
    var temp = get_data('/AccountMgmt/Common/Resources', rd);
    var rps = JSON.parse(temp);
    for (var k in rps) if (res[k] == undefined) res[k] = rps[k];
    res_process.req_data.lt = [];
};

//show dialog ob
var show_dialog_ob = function (message, funcCloseAfter) {
    res_process.load_res('Common', ['Define']);
    res_process.get();
    var obj_id = 'show_dialog_';
    var i;
    for (i = 0; $('#' + obj_id + i).length > 0; i++);
    obj_id = obj_id + i;
    $('<div id="' + obj_id + '" class="show_dialog_ob" style="z-index:' + (3999 + i * 2) + '">' +
        '<div></div>' +
        '<div><p>' + message + '</p></div>' +
        '<div><button id="' + obj_id + 'btnOk">' + res.Define + '</button></div></div>').appendTo(document.body).hide();
    $('<div id="' + obj_id + 'mask" class="dialog_mask" style="z-index:' + (3998 + i * 2) + '"></div>').css('width', document.body.clientWidth).css('height', document.body.clientHeight).appendTo('body');


    obj_id = '#' + obj_id;
    $(obj_id).draggable({ cancel: obj_id + '>div:nth-child(2), ' + obj_id + '>div:nth-child(3)' });
    $(obj_id).css('left', ($(window).width() - $(obj_id).width()) / 2 + 'px').css('top', ($(window).height() - $(obj_id).height()) / 2 + 'px').show();

    $(obj_id + 'btnOk').focus().click(function () { $(obj_id).remove(); $(obj_id + 'mask').remove(); if (funcCloseAfter !== undefined) funcCloseAfter(); });
};

//show dialog close
var show_dialog_close = function (dialog_params) {
    res_process.load_res('Common', ['Close']);
    res_process.get();
    var obj_id = 'show_dialog_';
    var i;
    for (i = 0; $('#' + obj_id + i).length > 0; i++);
    obj_id = obj_id + i;
    $('<div id="' + obj_id + '" class="show_dialog_close" style="z-index:' + (3999 + i * 2) + '">' +
        '<div>' + (dialog_params.title !== undefined ? dialog_params.title : '') + '</div>' +
        '<div>' + dialog_params.content + '</div>' +
        '<div><button id="' + obj_id + 'btnClose">' + res.Close + '</button>' +
        '</div></div>').appendTo(document.body).hide();
    $('<div id="' + obj_id + 'mask" class="dialog_mask" style="z-index:' + (3998 + i * 2) + '"></div>').css('width', document.body.clientWidth).css('height', document.body.clientHeight).appendTo('body');


    obj_id = '#' + obj_id;
    $(obj_id).draggable({ cancel: obj_id + '>div:nth-child(2), ' + obj_id + '>div:nth-child(3)' });
    $(obj_id).css('left', ($(window).width() - $(obj_id).width()) / 2 + 'px').css('top', ($(window).height() - $(obj_id).height()) / 2 + 'px').show();

    $(obj_id + 'btnClose').focus().click(function () { $(obj_id).remove(); $(obj_id + 'mask').remove(); });
};

//show dialog save and cancel
var show_dialog_save_and_cancel = function (dialog_params) {
    res_process.load_res('Common', ['Save', 'Cancel']);
    res_process.get();
    var obj_id = 'show_dialog_';
    var i;
    for (i = 0; $('#' + obj_id + i).length > 0; i++);
    obj_id = obj_id + i;
    $('<div id="' + obj_id + '" class="show_dialog_save_and_cancel" style="z-index:' + (3999 + i * 2) + '">' +
        '<div>' + (dialog_params.title !== undefined ? dialog_params.title : '') + '</div>' +
        '<div ' + ('content_box_width' in dialog_params ? 'style="width:' + dialog_params.content_box_width + 'px"' : '') + '>' + dialog_params.content + '</div>' +
        '<div><button id="' + obj_id + 'btnSave" style="margin-left:8px">' + (dialog_params.save_text === undefined ? res.Save : dialog_params.save_text) + '</button>' +
        '<button id="' + obj_id + 'btnCancel" style="margin-left:8px">' + res.Cancel + '</button>' +
        '</div></div>').appendTo(document.body).hide();
    $('<div id="' + obj_id + 'mask" class="dialog_mask" style="z-index:' + (3998 + i * 2) + '"></div>').css('width', document.body.clientWidth).css('height', document.body.clientHeight).appendTo('body');


    obj_id = '#' + obj_id;
    $(obj_id).draggable({ cancel: obj_id + '>div:nth-child(2), ' + obj_id + '>div:nth-child(3)' });
    $(obj_id).css('left', ($(window).width() - $(obj_id).width()) / 2 + 'px').css('top', ($(window).height() - $(obj_id).height()) / 2 + 'px').show();

    $(obj_id + 'btnSave').click(function () { if (dialog_params.save !== undefined) if (dialog_params.save()) { $(obj_id).remove(); $(obj_id + 'mask').remove(); } });
    $(obj_id + 'btnCancel').click(function () { if (dialog_params.cancel !== undefined) dialog_params.cancel(); $(obj_id).remove(); $(obj_id + 'mask').remove(); });
    return { cf: function () { $(obj_id).remove(); $(obj_id + 'mask').remove(); } };
};

var AddThemes = function (href) {
    $(document.head).append('<link href="' + href + '" rel="stylesheet"/>');
};

function g_multi_obj(obj) {
    var get_new_number = function () {
        var i;
        for (i = ('start_nb' in obj ? obj.start_nb : 0) ; $('[name="' + obj.remove_trigger_name.replace('{nb}', i) + '"]').length > 0; i++);
        return i;
    };

    this.add_func = function (add_obj) {
        var nb = get_new_number(), data, data_content = '', is_add = false;
        if ($.isFunction(add_obj)) {
            data = add_obj(nb);
            data_content = typeof data === 'object' ? data.content : data;
            is_add = true;
        }
        $('#' + obj.target_box_id).append(is_add ? data_content : obj.create_func(nb));
        if (!is_add && 'create_build' in obj) obj.create_build(nb);
        if (is_add && data.build !== undefined) data.build();

        $('[name="' + obj.remove_trigger_name.replace('{nb}', nb) + '"]').click(function () {
            var cur_nb = $(this).attr('nb');
            var temp_nb;
            for (temp_nb = 0; temp_nb < obj.remove_target_name_list.length; temp_nb++) {
                $('[name="' + obj.remove_target_name_list[temp_nb].replace('{nb}', cur_nb) + '"]').remove();
            }

            cur_nb++;
            for (; $('[name="' + obj.remove_trigger_name.replace('{nb}', cur_nb) + '"]').length > 0; cur_nb++) {
                $('[name="' + obj.remove_trigger_name.replace('{nb}', cur_nb) + '"]').attr('nb', cur_nb - 1);
                $('[name="' + obj.remove_trigger_name.replace('{nb}', cur_nb) + '"]').attr('name', obj.remove_trigger_name.replace('{nb}', cur_nb - 1));
                var i;
                for (i = 0; i < obj.remove_target_name_list.length; i++) {
                    $('[name="' + obj.remove_target_name_list[i].replace('{nb}', cur_nb) + '"]').attr('name', obj.remove_target_name_list[i].replace('{nb}', cur_nb - 1));
                }
                for (i = 0; i < obj.rename_name_list.length; i++) {
                    $('[name="' + obj.rename_name_list[i].replace('{nb}', cur_nb) + '"]').attr('name', obj.rename_name_list[i].replace('{nb}', cur_nb - 1));
                }
            }
        });
    };
    $('#' + obj.add_id).click(this.add_func);

    this.rebuild = function () {
        $('#' + obj.add_id).unbind(); 
        $('#' + obj.add_id).click(this.add_func);
    };
}

function upload_to_base64(obj) {
    show_dialog_save_and_cancel({
        content: '<div id="divUpload"><img id="imgView" style="max-height:200px;max-width:200px;display:none" />' +
            '<form id="formUpload" style="display:inline"><input type="file" id="fileUpload" name="fileUpload" /></form>' +
            '<div>' + obj.append_content + '</div>' +
            '</div>',
        save: function () {
            obj.after_func($('#imgView').attr('src'));
            return true;
        }
    });

    var src = obj.input_cur_img_func();
    if (src !== '') {
        $('#imgView').attr('src', src);
        $('#imgView').css('display', 'inline');
    }


    $('#formUpload').submit(function () {
        $('<div id="mask" style="z-index:10000; width: 1903px; height: 836px; opacity:0; position:absolute; top:0; left:0;"></div>').appendTo('body');
        var noumenon = $('<iframe id="ifResult" name="ifResult" src="about:blank" style="display:none"/>');
        $('body').append(noumenon);
        $(this).attr({
            action: '/Account/GetTempImage',
            method: 'POST',
            enctype: 'multipart/form-data',
            encoding: 'multpart/form-data',
            target: 'ifResult'
        });

        $('#ifResult').load(function () {
            ifObj = $(this);
            $('#imgView').attr('src', ifObj.contents().find('div').html());
            $('#imgView').css('display', 'inline');
            $('#ifResult').remove();
            $('#mask').remove();
        });
    });

    $('#fileUpload').change(function () { $('#formUpload').submit() });

    return false;
}

//login bar
$(document).ready(function () {
    res_process.load_res('AccountCommon', ['LoginSuccess', 'LogoutSuccess']);
    $('.loginBar').submit(function () {
        $.post("/Account/Login", $('.loginBar>form').serialize()).done(function (content) {
            location.reload();
        });
        return false;
    });

    $('#btnLogout').click(function () {
        $.post('/Account/Logout').done(function (content) {
            window.location = '/';
        });
        return false;
    });
});


function LoginProcess(success_func) {
    twNewegg().checkNGO(function (isLogin) {
        if (isLogin) {
            success_func();
        } else {
            window.location = '/MyAccount/Login';
        }
    }, []);
}

function logout() {
    get_data('/Account/SPLogout');
}

$('#top_link').click(function () {
    window.header.focus;
    return false;
});