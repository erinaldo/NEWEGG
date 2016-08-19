var uploader = {
    subpath: '',
    resizeSet: 0,
    resizeValue: 0,
    validateWidth: 0,
    validateHeight: 0,
    validateScale:0,
    filters: [],
    size: '',
    container: '',
    multi_selection: true,
    max_file_count: 30,
    success: function (up, files) { },
    init: function (setting) {
        uploader.subpath = setting.subpath;
        uploader.size = setting.size;
        uploader.container = '#' + setting.container;
        uploader.success = setting.success;
        if (setting.resizeSet != null) { uploader.resizeSet = setting.resizeSet } else { uploader.resizeSet = 0 };
        if (setting.resizeValue != null) { uploader.resizeValue = setting.resizeValue } else { uploader.resizeValue = 0 };
        if (setting.validateWidth != null) { uploader.validateWidth = setting.validateWidth } else { uploader.validateWidth = 0 };
        if (setting.validateHeight != null) { uploader.validateHeight = setting.validateHeight } else { uploader.validateHeight = 0 };
        if (setting.validateScale != null) { uploader.validateScale = setting.validateScale }else { uploader.validateScale = 0 };

        if (setting.multi_selection != null) uploader.multi_selection = setting.multi_selection;
        if (setting.max_file_count) uploader.max_file_count = setting.max_file_count;
        switch (setting.ext) {
            case 'img':
                uploader.filters.push({ title: "Image files", extensions: "jpg,gif,png,bmp,webp,jpeg" });
                break;
        }
    },
    show: function () {
        $(uploader.container).show();

        $(uploader.container).pluploadQueue({
            runtimes: 'html5,html4,flash',
            url: '/File/Upload',
            max_file_size: uploader.size,
            max_files: uploader.max_file_count,
            unique_names: true,
            multiple_queues: false,
            filters: uploader.filters,
            multi_selection: uploader.multi_selection,
            multipart_params: {
                subpath: uploader.subpath, resizeSet: uploader.resizeSet, resizeValue: uploader.resizeValue,
                validateWidth: uploader.validateWidth,validateHeight: uploader.validateHeight, validateScale: uploader.validateScale
            },
            preinit: function (pluploader) {
                pluploader.bind('FileUploaded', function (up, file, data) {
                    if (data.response != 'ok') {
                        alert("上傳時出現錯誤");
                        up.removeFile(file);
                    }
                 });
                pluploader.bind('UploadComplete', function (up, files) {
                    if (files.length != 0) {
                        uploader.success(up, files);
                        alert('上傳成功')
                    }
                    
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
                        alert("上傳時出現錯誤，請檢查後重新上傳!");
                    }
                  
                });
                //四捨五入取小數點後兩位
                function formatFloat(num, pos) {
                    var size = Math.pow(10, pos);
                    return Math.round(num * size) / size;
                }
            }
        });
        var up = $(uploader.container).pluploadQueue();
        up.refresh();
    },

    hide: function () {
        $(uploader.container).parent().hide();
        uploader.destroy();
    }

};


///for tinymce imageupload

var ImageSrcToAdd;

function openUploader(accountName, albumid) {
    var subpath = '~/albums/' + accountName + '/' + albumid
    $("#upload-photo-dialog").dialog('open');
    var uploadedfiles = []
    uploader.init({
        subpath: subpath,
        size: '5mb',
        container: 'upload-photo-dialog',
        ext: 'img',
        success: function (up, files) {
            for (var i = 0; i < files.length; i++) {
                if (files[i].status == plupload.DONE) {
                    var photo = {
                        AccountName: accountName, AlbumID: albumid, FileName: files[i].target_name, Title: files[i].name,
                        Type: 0
                    }
                    uploadedfiles.push(photo);
                }
            }
            savePhoto(uploadedfiles, subpath);
        }
    });
    uploader.show();
}

function myFileBrowser(field_name, url, type, win) {
    tinymce.activeEditor.windowManager.close();
    openUploader(AccountName, 0);
};


function savePhoto(files, subpath) {
    $.ajax({
        type: "POST",
        url: "/AlbumMgmt/Photo/Add",
        data: JSON.stringify(files),
        contentType: 'application/json',
        success: function (data) {
            $("#upload-photo-dialog").dialog('close');
            ImageSrcToAdd = subpath.substring(1) + "/" + files[0].FileName;
            tinymce.activeEditor.execCommand('mceImageWindow');
        }
    });
}
function deletePhoto(photoID) {
    $.ajax({
        type: "POST",
        url: "/AlbumMgmt/Photo/Delete",
        data: { photoID: photoID },
        success: function (data) {
            location.reload();
        }
    });
}


///for tinymce imageupload 
