/*
    Requires Jan Odvárko's excellent JSColor routine, available to download 
        at http://www.jscolor.com/
    Include that script first, then this one.
    In your tinymce.init section, include "trueColorPicker" in the list of 
      plugins, and "trueFColor" and "trueBColor" in your toolbar buttons.
    The global variable 'edId' needs to be the id of the textarea you're 
      working with. (There's almost certainly a better way to do that)
    Hopefully, someone will find this useful; it kept me distracted from 
      doing any real work for a few nights!
        
            email: mail@coyotewebdesign.co.uk
*/
(function () {
	$(document).ready(
		function () {
			showColorPanel();
		});
	tinymce.PluginManager.add('trueColorPicker', function (editor) {
		function showDialog() {
			var tooltip = this.settings.tooltip;
			var selectcmd = this.settings.selectcmd;
			tinyMCEtextColorGet(selectcmd, tooltip);
		}
		editor.addButton('trueFColor', {
			icon: 'forecolor',
			selectcmd: 'ForeColor',
			tooltip: 'Text color',
			onclick: showDialog
		});
		editor.addButton('trueBColor', {
			icon: 'backcolor',
			selectcmd: 'HiliteColor',
			tooltip: 'Background color',
			onclick: showDialog
		});
	});
	function findBtnDiv(wrap, aria) {
		var els = wrap.getElementsByTagName('div');
		for (var i = 0; i < els.length; i++) {
			var id = els[i].id;
			if (id && id.search('mce_') == 0) {
				var el = document.getElementById(id);
				if (el.getAttribute('aria-label') == aria) { break; }
			}
		}
		return id;
	}
	function tinyMCEtextColorGet(mode, text) {
		var div = document.getElementById(edId).parentNode;
		var btnId = findBtnDiv(div, text);
		var rect = document.getElementById(btnId).getBoundingClientRect();
		var x = rect.left - 110; var y = rect.bottom + 2;
		document.getElementById('tinyMCEcolorPicker').style.left = x + 'px';
		document.getElementById('tinyMCEcolorPicker').style.top = y + $(document).scrollTop() + 'px';
		document.getElementById('tinyMCEcolorMode').value = mode;
		document.getElementById('outside').style.display = 'block';
		document.getElementById('tinyMCEcolorPicker').style.display = 'block';
		document.getElementById('tinyMCEcolorInput').color.showPicker()
	}
	function tinyMCEtextColorSet() {
		var ed = tinymce.activeEditor;
		var colorCmd = document.getElementById('tinyMCEcolorMode').value;
		var trueColor = '#' + document.getElementById('tinyMCEcolorInput').value;
		ed.focus();
		ed.execCommand(colorCmd, false, trueColor);
		closeColorPanel();
	}
	function closeColorPanel() {
		document.getElementById('tinyMCEcolorInput').color.hidePicker()
		document.getElementById('tinyMCEcolorPicker').style.display = 'none';
		document.getElementById('outside').style.display = 'none';
	}
	function showColorPanel() {
		var div = document.createElement('div');
		div.id = 'outside';
		div.style.cssText = 'position:fixed; top:0; left:0; width:100%; height:100%; background:white; opacity:.01; filter:alpha(opacity=1); z-index:98; display:none;';
		document.body.appendChild(div);
		document.getElementById('outside').onclick = closeColorPanel;
		div = document.createElement('div');
		div.id = 'tinyMCEcolorPicker';
		div.style.cssText = 'position:absolute; top:0; left:0; width:246px; height:158px; padding:10px 0 0 11px; z-index:99; display:none;';
		var html = '<input type="hidden" id="tinyMCEcolorMode" value="">';
		html += '<input id="tinyMCEcolorInput" class="color" value="ffffff" style="width:150px; margin-right:10px; float:left;">';
		html += '<button id="colorSetOK" type="button" style="width:66px; float:left;">OK</button>';
		div.innerHTML = html;
		document.body.appendChild(div);
		document.getElementById('tinyMCEcolorInput').color = new jscolor.color(document.getElementById('tinyMCEcolorInput'));
		$('#colorSetOK').click(tinyMCEtextColorSet);
	}
})();