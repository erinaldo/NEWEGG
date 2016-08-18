var uploader = {
    subpath: '',
    filters: [],
    size: '',
    container: '',
    multi_selection: true,
    max_file_count: 30,
    success: function (up, files) { },
    init: function (setting) {
        //alert(setting.subpath)
        uploader.subpath = setting.subpath;
        uploader.size = setting.size;
        uploader.container = '#' + setting.container;
        uploader.success = setting.success;
        if (setting.multi_selection != null) uploader.multi_selection = setting.multi_selection;
        if (setting.max_file_count) uploader.max_file_count = setting.max_file_count;
        switch (setting.ext) {
            case 'img':
                uploader.filters.push({ title: "Image files", extensions: "jpg,gif,png,bmp" });
                break;
        }
    },
    show: function () {
        $(uploader.container).show();

        $(uploader.container).pluploadQueue({
            runtimes: 'html5,html4',
            url: '/FileMgmt/File/Upload',
            max_file_size: uploader.size,
            max_files: uploader.max_file_count,
            chunk_size: '1mb',
            unique_names: true,
            multiple_queues: false,
            filters: uploader.filters,
            multi_selection: uploader.multi_selection,
            multipart_params: { subpath: uploader.subpath },
            preinit: function (pluploader) {
                pluploader.bind('FileUploaded', function (up, file, data) {
                });
                pluploader.bind('UploadComplete', function (up, files) {
                    uploader.success(up, files);
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
