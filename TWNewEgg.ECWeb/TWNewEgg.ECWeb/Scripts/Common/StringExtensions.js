if (typeof String.prototype.paddleLeft==='undefined') {
    String.prototype.paddleLeft = function (resultLength, char) {       ///在字串左方以char補至length = resultLength, 如未指定char則補0 
        var toAppend = "";
        var test = resultLength - this.length;
        for (var i = 0; i < test; i++) {
            toAppend = toAppend.concat(char || "0");
        }
        return toAppend.concat(this);
    };
}

if (typeof String.prototype.trimLeftChar === 'undefined') {
    String.prototype.trimLeftChar = function (char) {
        var pattern = new RegExp('^' + char + '*', 'g');
        return this.replace(pattern, '');
    };
}

if (typeof String.prototype.toHHMMSS === 'undefined') {
    String.prototype.toHHMMSS = function () {
        var sec_num = parseInt(this, 10); // don't forget the second parm
        var hours = Math.floor(sec_num / 3600);
        var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
        var seconds = sec_num - (hours * 3600) - (minutes * 60);

        if (hours < 10) { hours = "0" + hours; }
        if (minutes < 10) { minutes = "0" + minutes; }
        if (seconds < 10) { seconds = "0" + seconds; }
        var time = hours + ':' + minutes + ':' + seconds;
        return time;
    }
}

if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/gm, '');
    };
}