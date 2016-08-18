var JcropUploader = {
    subpath: '',
    filters: [],
    size: '',
    container: '',
    showJcropWidth: 240, //目前裁圖畫面需要顯示的寬
    showJcropHeight: 240,//目前裁圖畫面需要顯示的長
    minSizeWidth: 30,//設定裁圖最小寬
    minSizeHeight: 30,//設定裁圖最小長
    maxSizeWidth: 240,//設定裁圖最大寬
    maxSizeHeight: 240,//設定裁圖最大長
    setSelectWidth: 240, //設定裁圖初始寬
    setSelectHeight: 240,  //設定裁圖初始長
    aspectRatioWidth: 1,//裁圖的比例寬
    aspectRatioHeight: 1,//裁圖的比例長
    multi_selection: true,
    max_file_count: 30,
    success: function (up, files) { },
    init: function (setting) {
        JcropUploader.subpath = setting.subpath;
        JcropUploader.size = setting.size;
        JcropUploader.container = '#' + setting.container;
        JcropUploader.success = setting.success;
        if (setting.showJcropWidth != undefined) JcropUploader.showJcropWidth = setting.showJcropWidth;
        if (setting.showJcropHeight != undefined) JcropUploader.showJcropHeight = setting.showJcropHeight;
        if (setting.minSizeWidth != undefined) JcropUploader.minSizeWidth = setting.minSizeWidth;
        if (setting.minSizeHeight != undefined) JcropUploader.minSizeHeight = setting.minSizeHeight;
        if (setting.maxSizeWidth != undefined) JcropUploader.maxSizeWidth = setting.maxSizeWidth;
        if (setting.maxSizeHeight != undefined) JcropUploader.maxSizeHeight = setting.maxSizeHeight;
        if (setting.setSelectWidth != undefined) JcropUploader.setSelectWidth = setting.setSelectWidth;
        if (setting.setSelectHeight != undefined) JcropUploader.setSelectHeight = setting.setSelectHeight;
        if (setting.aspectRatioWidth != undefined) JcropUploader.aspectRatioWidth = setting.aspectRatioWidth;
        if (setting.aspectRatioHeight != undefined) JcropUploader.aspectRatioHeight = setting.aspectRatioHeight;
        if (setting.multi_selection != null) JcropUploader.multi_selection = setting.multi_selection;
        if (setting.max_file_count) JcropUploader.max_file_count = setting.max_file_count;
        switch (setting.ext) {
            case 'img':
                JcropUploader.filters.push({ title: "Image files", extensions: "jpg,gif,png,bmp,webp,jpeg" });
                break;
        }
    },
    show: function () {
        $(JcropUploader.container).show();

        $(JcropUploader.container).pluploadQueue({
            runtimes: 'html5,html4,flash',
            url: '/FileMgmt/File/Upload',
            max_file_size: JcropUploader.size,
            max_files: JcropUploader.max_file_count,
            unique_names: true,
            multiple_queues: false,
            filters: JcropUploader.filters,
            multi_selection: JcropUploader.multi_selection,
            multipart_params: {
                subpath: JcropUploader.subpath
            },
            preinit: function (pluploader) {
                pluploader.bind('FileUploaded', function (up, file, data) {
                    Jcropfunc(file);
                });
               
                pluploader.bind('FilesAdded', function (up, files) {
                    if (up.files.length + files.length >= up.settings.max_files) {
                        files.splice(up.settings.max_files);
                        $(up.settings.browse_button).hide();
                    }
                });

                pluploader.bind('FilesRemoved', function (up, files) {
                    if (up.files.length < up.settings.max_files) {
                        $(up.settings.browse_button).show();
                    }
                });
                pluploader.bind('Error', function (up, err) {
                    //当服务器端返回错误信息时error方法显示错误提示，  
                    //服务器端返回错误信息会被plupload转换为-200 http错误,  
                    //所以只能做==-200比较。更好的提示，需要修改插件源代码。  
                    if (err.code == -200) {
                        alert("上传错误，请检查后重新上传!");
                    }

                });
            }
        });
        var up = $(JcropUploader.container).pluploadQueue();
        up.refresh();
    },

    hide: function () {
        $(JcropUploader.container).parent().hide();
        JcropUploader.destroy();
    }

};


///for jcrop imageupload

function Jcropfunc(file) {
   
    $("#jcrop_target").attr("src", JcropUploader.subpath + '/' + file.target_name);
    $("#preview").attr("src", JcropUploader.subpath + '/' + file.target_name);
    $("#newpicname").val(JcropUploader.subpath + '/' + file.target_name);
    alert(JcropUploader.showJcropWidth)
    $(function ($) {
        var jcrop_api,
            boundx,
            boundy,

            // Grab some information about the preview pane
            $preview = $('#preview-pane'),
            $pimg = $('#preview');

        $('#jcrop_target').Jcrop({
            bgColor: 'white',
            minSize: [JcropUploader.minSizeWidth, JcropUploader.minSizeHeight],
            maxSize: [JcropUploader.maxSizeWidth, JcropUploader.maxSizeHeight],
            setSelect: [0, 0, JcropUploader.setSelectWidth, JcropUploader.setSelectHeight],//設定初始值
            onChange: moveImage,
            onSelect: moveImage,
            aspectRatio: JcropUploader.aspectRatioWidth / JcropUploader.aspectRatioHeight,
          }, function () {
            // 取得原圖大小
            var bounds = this.getBounds();
          
            boundx = bounds[0];
            boundy = bounds[1];
            //console.log(boundx);
            //console.log(boundy);
           
            // Store the API in the jcrop_api variable
            jcrop_api = this;
            // Move the preview into the jcrop container for css positioning
            $preview.appendTo(jcrop_api.ui.holder);
        });

        function moveImage(coords) {
            updatePreview(coords);
            showCoords(coords)
        }
        //當圖片移動時右邊的小圖也跟著移動
        //coords:選擇圖片的大小
        function updatePreview(coords) {
            
            var rx = JcropUploader.showJcropWidth / coords.w;
            var ry = JcropUploader.showJcropHeight / coords.h;
           
            $("#preview").css({
                width: Math.round(rx * boundx) + 'px',
                height: Math.round(ry * boundy) + 'px',
                marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                marginTop: '-' + Math.round(ry * coords.y) + 'px'
            });
        };
        //儲存座標相關資料
        function showCoords(c) {
                                 //c 就是座標相關資料
            $('#x1').val(c.x);   //c.x  --> 左上角的 x  
            $('#y1').val(c.y);   //c.y  --> 左上角的 y  
            $('#x2').val(c.x2);  //c.x2 --> 右下角的 x  
            $('#y2').val(c.y2);  //c.y2 --> 右下角的 y  
            $('#w').val(c.w);    //c.w  --> 選取範圍的寬度  
            $('#h').val(c.h);    //c.h  --> 選取範圍的高度  
        };
    });

    $("#Crop_Dialog").dialog({
        width: 'auto',
        closeOnEscape: false,
        autoOpen: false,
        modal: true,
        open: function(event, ui) {
            $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
        },
        buttons: {
            "Crop": function () {
                $.ajax({
                    url: '/FileMgmt/File/JcropUpload',
                    type: "POST",
                    data: {
                        path: JcropUploader.subpath, pictureName: file.target_name, x1: $('#x1').val(), y1: $('#y1').val(), x2: $('#x2').val(), y2: $('#y2').val(), w: $('#w').val(), h: $('#h').val()
                        , showJcropWidth: JcropUploader.showJcropWidth, showJcropHeight: JcropUploader.showJcropHeight
                    },
                    success: function (data) {

                        var img_prev = $("#previewPane").html();
                        var oldimagetag = $("#preview").parent().parent().html();
                        
                        $('#img_prev').attr("src", $("#newpicname").val() + "?refresh=" + Math.random());//因有時間差需refresh圖片才會顯示切割圖
                        $("#Crop_Dialog").html(htmlstring());//清空小圖
                        alert('上传成功')
                    },
                    error: function (xhr, status, error) {
                        $("#Crop_Dialog").html(htmlstring());
                    }
                });

                $(this).dialog("close");

            }
        }
    });
    $("#Crop_Dialog").dialog("open");

}
function htmlstring() {
    return '<table cellspacing="0" cellpadding="0" border="0">' +
                                '<tbody>' +
                                    '<tr>' +
                                        '<td>' +
                                            '<img id="jcrop_target" src="" style="display: none; visibility: hidden;">' +
                                        '</td>' +
                                        '<td>' +
                                            '<div style="width:'+JcropUploader.showJcropWidth+'px;height:'+JcropUploader.showJcropHeight+'px;overflow:hidden;margin-left:5px;">' +
                                                '<img id="preview" src="">' +
                                            '</div>' +
                                        '</td>' +
                                    '</tr>' +
                                '</tbody>' +
                            '</table>' +
                            '<input type="hidden" size="4" id="x1" name="x1" />' +
                            '<input type="hidden" size="4" id="y1" name="y1" />' +
                            '<input type="hidden" size="4" id="x2" name="x2" />' +
                            '<input type="hidden" size="4" id="y2" name="y2" />' +
                            '<input type="hidden" size="4" id="w" name="w" />' +
                            '<input type="hidden" size="4" id="h" name="h" />' +
                            '<input id="newpicname" type="hidden" value=""/>';

}

///for jcrop imageupload