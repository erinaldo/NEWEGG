// turn key-pairs to array
KeypairToArray = function (pairs) {
    var array = [];
    var i = 0;
    for (var key in pairs) {
        array[i] = pairs[key];
        i++;
    }
    return array;
}

if (typeof Array.prototype.indexOf === 'undefined') {
    Array.prototype.indexOf = function (searchElement, fromIndex) {
        if (!this) {
            throw new TypeError();
        }

        fromIndex = +fromIndex;
        if (isNaN(fromIndex)) {
            fromIndex = 0;
        }

        var length = this.length;

        if (length == 0 || fromIndex >= length) {
            return -1;
        }

        if (fromIndex < 0) {
            fromIndex += length;
        }

        while (fromIndex < length) {
            if (this[fromIndex] === searchElement) {
                return fromIndex;
            }
            fromIndex = fromIndex + 1;
        }

        return -1;
    };
}
