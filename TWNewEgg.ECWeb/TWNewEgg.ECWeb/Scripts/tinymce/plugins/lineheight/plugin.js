tinymce.create('tinymce.plugins.lineheight', {
    createControl: function (n, cm) {
        switch (n) {
            case 'lineheight':
                var ed = tinymce.activeEditor;
                var c = cm.createListBox('lineheight', {
                    title: 'Line Height',
                    onselect: function (v) {
                        ed.formatter.apply('LHT' + String(v)); // apply the selected format (line height)
                        return false;
                    }
                });
                if (!ed.settings.formats) { // if no formats defined, create the object
                    ed.settings.formats = {};
                }
                for (var h = 50; h <= 200; h += 10) { // edit the 50 -> 200 range if you want
                    ed.settings.formats['LHT' + String(h)] = { // dynamically generate new formats
                        'block': 'p',
                        'styles': {
                            'lineHeight': String(h) + '%'
                        }
                    }
                    c.add(String(h) + '%', h); // ...and add them to the menu
                };
                return c;
        }
    }
});

// register our custom plugin
tinymce.PluginManager.add('lineheight', function (editor) {
    var items = [];
    for (var h = 50; h <= 200; h += 10) { // edit the 50 -> 200 range if you want
        editor.settings.formats['LHT' + String(h)] = { // dynamically generate new formats
            'block': 'p',
            'styles': {
                'lineHeight': String(h) + '%'
            }
        }
        items.push({
            text: h.toString(),
            value: h
        });
    };
    editor.addButton('lineheight', {
        type: 'listbox',
        text: 'lineheight',
        tooltip: 'lineheight',
        values: items,
        onselect: function (v) {
            editor.formatter.apply('LHT' + this._value); // apply the selected format (line height)
            return false;
        }
    });
});