/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    //---點擊 Enter採用 <br/>
    config.enterMode = CKEDITOR.ENTER_BR;
    config.shiftEnterMode = CKEDITOR.ENTER_BR;
    //---點擊 Enter採用 <br/>
    config.pasteFromWordRemoveFontStyles = false;
    config.pasteFromWordRemoveStyles = false;
    //-- 貼入自動清除格式(正式環境還未啟用)
    //config.forcePasteAsPlainText = true;
    config.toolbar = 'Full';

    config.toolbar_Full =
[
    { name: 'document', items: [/*'Source',*/ '-', 'NewPage'/* , 'Save', 'DocProps', 'Preview', 'Print', '-', 'Templates'*/] },
    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', /*'PasteFromWord'*/, '-', 'Undo', 'Redo'] },
    /*{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },*/
    /*{ name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton',
          'HiddenField']
    },*/    
    { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
    {
      name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', /*'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv',*/
        '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'/*, 'BidiLtr', 'BidiRtl'*/]
    },
    { name: 'links', items: ['Link'/*, 'Unlink', 'Anchor'*/] },
    { name: 'insert', items: ['Image', /*'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', */'Iframe'] },
    '/',
    { name: 'styles', items: [/*'Styles', 'Format', 'Font', */'FontSize'] },
    { name: 'colors', items: ['TextColor', 'BGColor'] }
    /*{ name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] }*/
];


};

//隱藏Ckediter 目標 進階 tab
CKEDITOR.on('dialogDefinition', function (ev) {
    var dialogName = ev.data.name;
    var dialogDefinition = ev.data.definition;
    ev.data.definition.resizable = CKEDITOR.DIALOG_RESIZE_NONE;

    if (dialogName == 'link') {
        dialogDefinition.removeContents('target');
        dialogDefinition.removeContents('advanced');
    }
})