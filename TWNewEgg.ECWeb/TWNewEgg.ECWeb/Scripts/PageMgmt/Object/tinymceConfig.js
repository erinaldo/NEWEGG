var editorContent;
var EditorID;
var tinymceConfig = {
    menubar: false,
    selector: "",
    theme: "modern",
    language: 'zh_CN',
    plugins: [
         "advlist autolink link lists charmap print preview hr anchor pagebreak spellchecker",
         "searchreplace visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
         "save table contextmenu directionality emoticons template paste textcolor"
    ],
    toolbar: "undo redo | styleselect formatselect fontselect fontsizeselect | forecolor backcolor | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link",
    toolbar_items_size: 'small',
    //content_css: "/Themes/2013/stylesheets/PageMgmt/ds-text.css",
    content_css: "/Themes/2013/stylesheets/base.css,/Themes/2013/stylesheets/allModel.css,/Themes/2013/stylesheets/TextStyle.css",
    //content_css: "/Themes/2013/stylesheets/allModel.css" + "/Themes/2013/stylesheets/base.css" + "/Themes/2013/stylesheets/TextStyle.css",
    formats: {
        bold: { inline: 'span', classes: 'fontStyle_b' },
        italic: { inline: 'span', classes: 'fontStyle_i' },
        underline: { inline: 'span', classes: 'fontStyle_u' },
        strikethrough: { inline: 'span', classes: 'fontStyle_d' },
        forecolor: { inline: 'span', styles: { color: '%value' } }
    },
    theme_advanced_font_sizes: "10px,12px,13px,14px,16px,18px,20px",
    theme_modern_fonts: "Andale Mono=andale mono,times;" +
                "Arial=arial,helvetica,sans-serif;" +
                "Arial Black=arial black,avant garde;" +
                "Book Antiqua=book_antiquaregular,palatino;" +
                "Corda Light=CordaLight,sans-serif;" +
                "Courier New=courier_newregular,courier;" +
                "Flexo Caps=FlexoCapsDEMORegular;" +
                "Lucida Console=lucida_consoleregular,courier;" +
                "Georgia=georgia,palatino;" +
                "Helvetica=helvetica;" +
                "Impact=impactregular,chicago;" +
                "Museo Slab=MuseoSlab500Regular,sans-serif;" +
                "Museo Sans=MuseoSans500Regular,sans-serif;" +
                "Oblik Bold=OblikBoldRegular;" +
                "Sofia Pro Light=SofiaProLightRegular;" +
                "Symbol=webfontregular;" +
                "Tahoma=tahoma,arial,helvetica,sans-serif;" +
                "Terminal=terminal,monaco;" +
                "Tikal Sans Medium=TikalSansMediumMedium;" +
                "Times New Roman=times new roman,times;" +
                "Trebuchet MS=trebuchet ms,geneva;" +
                "Verdana=verdana,geneva;" +
                "Webdings=webdings;" +
                "Wingdings=wingdings,zapf dingbats" +
                "Aclonica=Aclonica, sans-serif;" +
                "Michroma=Michroma;" +
                "Paytone One=Paytone One, sans-serif;" +
                "Andalus=andalusregular, sans-serif;" +
                "Arabic Style=b_arabic_styleregular, sans-serif;" +
                "Andalus=andalusregular, sans-serif;" +
                "KACST_1=kacstoneregular, sans-serif;" +
                "Mothanna=mothannaregular, sans-serif;" +
                "Nastaliq=irannastaliqregular, sans-serif;" +
                "Samman=sammanregular",
    font_size_style_values: "12px,13px,14px,16px,18px,20px",
    style_formats: [
         { title: 'Bold text', inline: 'span', styles: { 'font-weight': 'bold' } },
         { title: 'Bold class', inline: 'span', classes: 'fontStyle_b', styles: { 'display': 'block' } }
    ],
    height: '300',
    entity_encoding: "raw",
    id: "",
    setup: function (ed) {}
};
//var initTinymce = function (editorID, content, width) {
//    editorContent = content;
//    EditorID = editorID;
//    tinymceConfig.selector = "textarea#" + editorID;
//    tinymceConfig.id = editorID;
//    tinymceConfig.setup = function (ed) {
//        ed.on('init', function (args) {
//            $("#mce-editor_ifr").width(width + 'px');
//        });
//    }
//    tinymce.settings = tinymceConfig;
//    tinymce.init(tinymceConfig);
//};

//var myCustomInitInstance = function () {
//    if (!tinymce.get(EditorID).getContent()) {
//        tinymce.get(EditorID).setContent($("<div/>").html(editorContent).text(), { format: 'raw' });
//    }
//};