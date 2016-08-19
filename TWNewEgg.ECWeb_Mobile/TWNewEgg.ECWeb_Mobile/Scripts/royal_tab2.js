/**
|------------------------------------------|
| MelonHTML5 - Royal Tab                   |
|------------------------------------------|
| @author:  Lee Le (lee@melonhtml5.com)    |
| @version: 1.00 (xx Feb 2013)             |
| @website: www.melonhtml5.com             |
|------------------------------------------|
*/



/**
  (https://developer.mozilla.org/en-US/docs/DOM/document.cookie)
  docCookies.setItem(name, value[, end[, path[, domain[, secure]]]])
  docCookies.getItem(name)
  docCookies.removeItem(name[, path])
  docCookies.hasItem(name)
*/
var docCookies = {
    getItem: function (sKey) {
        if (!sKey || !this.hasItem(sKey)) {
            return null;
        }

        return unescape(document.cookie.replace(new RegExp("(?:^|.*;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*((?:[^;](?!;))*[^;]?).*"), "$1"));
    },

    setItem: function (sKey, sValue, vEnd, sPath, sDomain, bSecure) {
        if (!sKey || /^(?:expires|max\-age|path|domain|secure)$/i.test(sKey)) {
            return;
        }

        var sExpires = "";

        if (vEnd) {
            switch (vEnd.constructor) {
                case Number:
                    sExpires = vEnd === Infinity ? "; expires=Tue, 19 Jan 2038 03:14:07 GMT" : "; max-age=" + vEnd;
                    break;
                case String:
                    sExpires = "; expires=" + vEnd;
                    break;
                case Date:
                    sExpires = "; expires=" + vEnd.toGMTString();
                    break;
            }
        }

        document.cookie = escape(sKey) + "=" + escape(sValue) + sExpires + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "") + (bSecure ? "; secure" : "");
    },

    removeItem: function (sKey, sPath) {
        if (!sKey || !this.hasItem(sKey)) {
            return;
        }

        document.cookie = escape(sKey) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sPath ? "; path=" + sPath : "");
    },

    hasItem: function (sKey) {
        return (new RegExp("(?:^|;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
    }
};

var Royal_Tab_Data = {
    current_element:        null,
    global_events_attached: false,
    objects:                []
};

/*
 * MAIN CONSTRUCTOR
 */
function Royal_Tab(element, undefined) {
    var SELF = this;
    var BODY = $(document.body);

    this._element             = element;

    this._api                 = new Royal_Tab_Api(SELF._element);      // object

    this._tabs                = SELF._element.children('div.tabs');    // DOM
    this._views               = SELF._element.children('div.views');   // DOM

    this._menu                = SELF._element.children('div.tabs').children('ul');   // DOM

    this._sliding_menu_left   = null;   // DOM
    this._sliding_menu_right  = null;   // DOM

    this._menu_scroller       = null;   // DOM
    this._menu_items          = null;   // DOM

    this._dropdown_menu       = null;   // DOM
    this._dropdown_menu_items = null;   // DOM

    this._view_scroller       = null;   // DOM
    this._view_items          = null;   // DOM

    this._menu_width          = 0;      // Number
    this._menu_scroller_wdith = 0;      // Number

    this._active_menu         = 0;      // INT

    this._sliding_speed       = 100;    // INT
    this._animation_speed     = 300;    // INT

    // default options
    this._options = {
        position:     'top',   // top, bottom
        alignment:    'left',  // left, right
        keyboard:     false,
        mouse:        false,
        animation:    null,    // slide, blind
        cookie:       null
    };

    // build required elements
    this._build = function() {
        SELF._element.data('constructor', SELF);

        // options
        SELF._options.keyboard     = SELF._element.data('keyboard')   ? true : false;
        SELF._options.mouse        = SELF._element.data('mouse')      ? true : false;
        SELF._options.position     = SELF._element.data('position')   ? SELF._element.data('position')  : SELF._options.position;
        SELF._options.alignment    = SELF._element.data('alignment')  ? SELF._element.data('alignment') : SELF._options.alignment;
        SELF._options.animation    = SELF._element.data('animation')  ? SELF._element.data('animation') : SELF._options.animation;
        SELF._options.cookie       = SELF._element.data('cookie')     ? SELF._element.data('cookie')    : SELF._options.cookie;

        // add options to class and data
        SELF._element.addClass(SELF._options.position + ' ' + SELF._options.alignment + ' ' + SELF._options.animation);
        SELF._element.data('options', SELF._options);

        var _menu_items = SELF._menu.children('li');
        var _view_items = SELF._views.children('div');

        // move menu to the bottom
        if (SELF._options.position === 'bottom') {
            SELF._tabs.insertAfter(SELF._views);
        }

        // sliding menu
        SELF._sliding_menu_left  = $('<a>').attr('href', '#').html('<span></span>').addClass('sliding_menu left').appendTo(SELF._tabs);
        SELF._sliding_menu_right = $('<a>').attr('href', '#').html('<span></span>').addClass('sliding_menu right').appendTo(SELF._tabs);

        // scroller
        SELF._menu_scroller = $('<div>').addClass('scroller').append(SELF._menu).appendTo(SELF._tabs);

        // build dropdown menu
        SELF._buildDropdownMenuItems(_menu_items, true);

        // view scroller
        SELF._view_scroller = $('<div>').addClass('scroller').append(_view_items).appendTo(SELF._views);

        // wrap each view items with another div so that new menu items could have 0 margin/padding
        _view_items.each(function(index) {
            $('<div>').append($(this)).appendTo(SELF._view_scroller);
        });

        // cache elements
        SELF._cacheItems();

        // default menu
        SELF._active_menu = SELF._menu_items.filter('.active:first').index();
        if (SELF._options.cookie) {
            if (docCookies.getItem(SELF._options.cookie)) {
                SELF._active_menu = parseInt(docCookies.getItem(SELF._options.cookie));
            }
        }

        // bind events
        SELF._tabs.click(SELF.Events._menuClick);

        SELF._dropdown_menu.click(SELF.Events._dropdownMenuClick);

        SELF._sliding_menu_left.click(SELF.Events._slidingMenuClick);
        SELF._sliding_menu_right.click(SELF.Events._slidingMenuClick);

        SELF._element.mouseenter(SELF.Events._mouseEnter).mouseleave(SELF.Events._mouseLeave);
        SELF._element.bind('mousewheel DOMMouseScroll', SELF.Events._mouseWheel);

        if (Royal_Tab_Data.global_events_attached !== true) {
            $(document)
                .click(SELF.Events._documentClick)
                .keydown(SELF.Events._documentKeyDown);

            $(window).resize(SELF.Events._windowResize);

            Royal_Tab_Data.global_events_attached = true;
        }
    };

    // build dropdown menu items
    this._buildDropdownMenuItems = function(menu_items, build_menu) {
        var html = '';
        menu_items.each(function(index) {
            var class_name = '';
            if ($(this).hasClass('disabled')) {
                class_name = ' class="disabled"';
            } else if ($(this).hasClass('active')) {
                class_name = ' class="active"';
            }

            html += '<li' + class_name + '>' + $(this).text() + '</li>';
        });

        if (build_menu) {
            SELF._dropdown_menu = $('<a>').attr('href', '#').addClass('dropdown_menu').html('<span></span><ul>' + html + '</ul>').appendTo(SELF._tabs);
        } else {
            SELF._dropdown_menu.html('<span></span><ul>' + html + '</ul>');
        }

        SELF._dropdown_menu_items = SELF._dropdown_menu.find('li');
    };

    // cache DOM element
    this._cacheItems = function() {
        SELF._view_items          = SELF._view_scroller.children('div');
        SELF._menu_items          = SELF._menu.children('li');
        SELF._dropdown_menu_items = SELF._dropdown_menu.find('li');
    };

    // toggle dropdown menu
    this._toggleDropdown = function(e) {
        SELF._dropdown_menu.children('ul').toggleClass('open');
        SELF._dropdown_menu.toggleClass('open');
    };

    // close dropdown menu
    this._closeDropdown = function(e) {
        SELF._dropdown_menu.children('ul').removeClass('open');
        SELF._dropdown_menu.removeClass('open');
    };

    // get menu index to open
    this._getMenuIndex = function(index) {
        if (index <= 0) {
            index = SELF._menu_items.not('.disabled').first().index();
        } else if (index > SELF._menu_items.length - 1) {
            index = SELF._menu_items.not('.disabled').last().index();
        }

        return index;
    },

    // open tab by index
    this._openMenu = function(index, no_animation) {
        if (SELF._element.data('animating') === true) {
            return;
        }

        index = SELF._getMenuIndex(index);

        var next_view_item = SELF._view_items.eq(index);

        SELF._menu_items.filter('.active').removeClass('active');
        SELF._menu_items.eq(index).addClass('active');

        var _final_switch = function() {
            SELF._view_items.filter('.active').removeClass('active');
            next_view_item.addClass('active');
        };

        // animatinos
        if (no_animation === true) {
            _final_switch();
        } else if (SELF._options.animation === 'blind') {
            SELF._element.data('animating', true);

            next_view_item.addClass('measuring');
            var height = next_view_item.outerHeight();
            next_view_item.removeClass('measuring');

            SELF._views.animate({height:height + 'px'}, SELF._animation_speed, function() {
                SELF._views.removeAttr('style');
                SELF._element.data('animating', false);
            });

            _final_switch();
        } else if (SELF._options.animation === 'slide') {
            SELF._element.data('animating', true);
            SELF._view_scroller.addClass('animating');

            var animate_to = '-100%';
            // go back
            if (SELF._active_menu > index) {
                SELF._view_scroller.css('margin-left', '-100%');
                animate_to = '0%';
            }

            next_view_item.show();

            SELF._view_scroller.animate({'margin-left':animate_to}, SELF._animation_speed, function() {
                SELF._view_scroller.removeAttr('style').removeClass('animating');
                next_view_item.removeAttr('style');
                _final_switch();

                SELF._element.data('animating', false);
            });
        } else {
            _final_switch();
        }

        SELF._active_menu = index;

        // set cookie
        if (SELF._options.cookie !== null) {
            docCookies.setItem(SELF._options.cookie, index);
        }

        // update dropdown
        SELF._dropdown_menu_items.filter('.active').removeClass('active');
        SELF._dropdown_menu_items.eq(index).addClass('active');

        SELF._closeDropdown();

        // slide menu to be fully visible
        if (SELF._element.hasClass('sliding')) {
            SELF._slideToMenu(index);
        }

        // AJAX
        var ajax_view = SELF._view_items.eq(index).children('div');
        if (ajax_view.data('url')) {
            if (ajax_view.data('content')) {
                ajax_view.html(ajax_view.data('content'));
            } else  {
                $.ajax({
                    url        : ajax_view.data('url'),
                    cache      : false,
                    type       : 'POST',
                    data       : {},
                    beforeSend : function(jqXHR, settings) {
                                     ajax_view.addClass('loading');
                                 },
                    complete   : function(jqXHR, textStatus) {
                                     ajax_view.removeClass('loading');
                                 },
                    error      : function(jqXHR, textStatus, errorThrown) {
                                 },
                    success    : function(data, textStatus, jqXHR) {
                                     ajax_view.html(data);
                                     ajax_view.data('content', data)
                                 }
                });
            }
        }
    };

    // slide to the selected tab menu
    this._slideToMenu = function(index) {
        var scroll           = false;
        var menu_item        = SELF._menu_items.eq(index);
        var menu_item_left   = menu_item.data('left');
        var menu_item_right  = menu_item.data('right');
        var menu_item_wdith  = menu_item.data('width');
        var menu_position    = SELF._getMenuPosition();

        if (!menu_item_right) {
            menu_item_right = SELF._menu_width - menu_item_left - menu_item_wdith;
            menu_item.data('right', menu_item_right);
        }

        if (menu_item_left < menu_position * -1) {
            scroll = menu_item_left * -1;
        } else if (menu_item_right < (SELF._menu_width - SELF._menu_scroller_width - menu_position * -1)) {
            var menu_item_right = SELF._menu_items.eq(index).data('right');
            scroll = (SELF._menu_width - SELF._menu_scroller_width - menu_item_right) * -1 + 1;
        }

        if (scroll !== false) {
            SELF._menu.animate({'margin-left':scroll + 'px'}, SELF._animation_speed, SELF._checkSlidingMenu);
        }
    };

    this._slideLeft = function(e) {
        var menu_position = SELF._getMenuPosition();
        menu_position -= SELF._sliding_speed;
        menu_position = Math.max(menu_position, SELF._getMaxScrolWidth() * -1);

        SELF._menu.animate({'margin-left':menu_position + 'px'}, SELF._animation_speed, SELF._checkSlidingMenu);
    };

    this._slideRight = function(e) {
        var menu_position = SELF._getMenuPosition();
        menu_position += SELF._sliding_speed;
        menu_position = Math.min(menu_position, 0);

        SELF._menu.animate({'margin-left':menu_position + 'px'}, SELF._animation_speed, SELF._checkSlidingMenu);
    };

    // set menu width (<UL>)
    this._setMenuWidth = function() {
        SELF._menu_width = 0;
        SELF._menu_items.each(function() {
            var menu_item_width = $(this).outerWidth();
            $(this).data('left',  SELF._menu_width);
            $(this).data('width', menu_item_width);
            SELF._menu_width += menu_item_width;
        });
    },

    // get margin left of <UL> menu
    this._getMenuPosition = function() {
        return parseInt(SELF._menu.css('margin-left'), 10);
    };

    this._getMaxScrolWidth = function() {
        return SELF._menu_width - SELF._menu_scroller_width - 1;
    };

    // check if sliding menu should be disabled or not
    this._checkSlidingMenu = function() {
        if (!SELF._element.hasClass('sliding')) {
            return;
        }

        var menu_position = SELF._getMenuPosition();

        if (menu_position >= 0) {
            SELF._sliding_menu_left.removeClass('disabled');
            SELF._sliding_menu_right.addClass('disabled');
        } else {
            SELF._sliding_menu_right.removeClass('disabled');

            var max_scroll_width = SELF._getMaxScrolWidth();
            if (menu_position * -1 < max_scroll_width) {
                SELF._sliding_menu_left.removeClass('disabled');
            } else {
                SELF._sliding_menu_left.addClass('disabled');
            }

        }
    };

    // on window resize
    // toggle sliding class
    // drag menu when needed
    this._setSize = function() {
        if (SELF._element.data('reset_menu_width') === true) {
            SELF._setMenuWidth();
        }

        if (SELF._menu_width > SELF._tabs.outerWidth() - SELF._dropdown_menu.width() - 2) {  // 2 = border
            SELF._element.addClass('sliding');
            SELF._sliding_menu_left.removeClass('disabled');
            SELF._sliding_menu_right.addClass('disabled');
            var reset_menu_position = true;
        } else {
            SELF._element.removeClass('sliding');
            SELF._menu.removeAttr('style');
            var reset_menu_position = false;
        }

        SELF._menu_scroller_width = SELF._menu_scroller.width();

        if (reset_menu_position) {
            var menu_position = SELF._getMenuPosition();
            var max_scroll_width = SELF._getMaxScrolWidth();
            if (menu_position * -1 > max_scroll_width) {
                SELF._menu.css('margin-left', max_scroll_width * -1);
            }
        }
    };

    // Event Handlers
    this.Events = {
        // resize
        _windowResize: function(e) {
            $('.royal_tab').each(function() {
                $(this).data('constructor')._setSize();
            });
        },

        // click
        _documentClick: function(e) {
            SELF._api._closeAll();
        },

        // keydown
        _documentKeyDown: function(e) {
            if (Royal_Tab_Data.current_element === null) {
                return;
            } else if (Royal_Tab_Data.current_element.data('options').keyboard !== true) {
                return;
            }

            switch (parseInt(e.which, 10)) {
                case 37:  // LEFT
                case 38:  // UP
                    e.preventDefault();
                    SELF._api.prev();
                    break;
                case 39:  // RIGHT
                case 40:  // dOWN
                    e.preventDefault();
                    SELF._api.next();
                    break;
            }
        },

        // scroll mouse wheel
        _mouseWheel: function(e) {
            if (Royal_Tab_Data.current_element === null) {
                return;
            } else if (Royal_Tab_Data.current_element.data('options').mouse !== true) {
                return;
            }

            e.preventDefault();

            // firefox e.detail; other browsers: e.wheelDelta
            var scroll_direction = e.originalEvent.wheelDelta <  0 || e.originalEvent.detail > 0 ? 'down' : 'up';

            if (scroll_direction === 'up') {
                SELF._api.prev();
            } else {
                SELF._api.next();
            }
        },

        _dropdownMenuClick: function(e) {
            SELF._toggleDropdown(e);
        },

        // sliding menu click
        _slidingMenuClick: function(e) {
            if (SELF._element.hasClass('sliding')) {
                if ($(this).hasClass('left')) {
                    SELF._slideLeft(e);
                } else {
                    SELF._slideRight(e);
                }
            };
        },

        // menu click
        _menuClick: function(e) {
            e.stopPropagation();
            e.preventDefault();

            var menu_item  = $(e.target);

            if (menu_item.hasClass('active') || menu_item.hasClass('disabled')) {
                return;
            } else if (menu_item.prop('tagName').toLowerCase() === 'li') {
                SELF._openMenu(menu_item.index());
            }
        },

        // set current tab element (used for keyboard control)
        _mouseEnter: function(e) {
            Royal_Tab_Data.current_element = $(this);
        },

        // remove current tab element
        _mouseLeave: function(e) {
            Royal_Tab_Data.current_element = null;
        }
    };

    Royal_Tab_Data.current_element = SELF._element;

    // Main
    SELF._build();
    SELF._openMenu(SELF._active_menu, true);
    SELF._setMenuWidth();
    SELF._setSize();
    SELF._checkSlidingMenu();

    SELF._element.addClass('loaded');
};




/*
 * API
 */
function Royal_Tab_Api(element, undefined) {
    var SELF = this;
    var CONSTRUCTOR = element.data('constructor');

    this._getMenuItem = function(index) {
        return CONSTRUCTOR._menu_items.eq(index);
    };

    this._getDropdownMenuItem = function(index) {
        return CONSTRUCTOR._dropdown_menu_items.eq(index);
    };

    this._getViewItem = function(index) {
        return CONSTRUCTOR._view_items.eq(index);
    };

    this._closeAll = function() {
        $('.royal_tab a.dropdown_menu ul').removeClass('open');
        $('.royal_tab a.dropdown_menu').removeClass('open');
    };

    this.prev = function() {
        if (typeof element === 'undefined') {
            element = Royal_Tab_Data.current_element;
        }

        var menu_items   = element.find('ul:first').children();
        var active_index = menu_items.filter('.active').index();

        while (active_index > 0) {
            active_index--;
            var menu_item = menu_items.eq(active_index);

            if (!menu_item.hasClass('disabled')) {
                menu_item.click();
                break;
            }
        }
    };

    this.next = function() {
        if (typeof element === 'undefined') {
            element = Royal_Tab_Data.current_element;
        }

        var menu_items   = element.find('ul:first').children();
        var active_index = menu_items.filter('.active').index();

        while (active_index < menu_items.length - 1) {
            active_index++;
            var menu_item = menu_items.eq(active_index);

            if (!menu_item.hasClass('disabled')) {
                menu_item.click();
                break;
            }
        }
    };

    this.open = function(index) {
        CONSTRUCTOR._openMenu(index, true);
    };

    this.add = function(index, selected, tab, content, isajax) {
        var new_menu_item = $('<li>').text(tab);

        if (isajax) {
            var new_view_item = $('<div>').html('<div data-url="' + content + '"></div>');
        } else {
            var new_view_item = $('<div>').html('<div>' + content +  '</div>');
        }

        if (index === 0) {
            new_menu_item.prependTo(element.children('.tabs').children('.scroller').children('ul'));
            new_view_item.prependTo(element.children('.views').children('.scroller'));
        } else {
            new_menu_item.insertBefore(SELF._getMenuItem(index));
            new_view_item.insertBefore(SELF._getViewItem(index));
        }

        // recache
        CONSTRUCTOR._cacheItems();

        // resize
        element.data('reset_menu_width', true);
        CONSTRUCTOR._setSize();
        element.data('reset_menu_width', false);

        if (selected === true) {
            SELF.open(index);
        }

        // rebuild dropdown menu items
        CONSTRUCTOR._buildDropdownMenuItems(CONSTRUCTOR._menu_items, false);
    };

    this.remove = function(index) {
        if (element.data('animating') === true) {
            return;
        } else if (CONSTRUCTOR._menu_items.length <= 1) {
            return;
        }

        var menu_item = SELF._getMenuItem(index);
        var view_item = SELF._getViewItem(index);

        if (menu_item.hasClass('active')) {
            var next_index = menu_item.siblings().not('.disabled').first().index();
            SELF.open(next_index);
        }

        menu_item.remove();
        view_item.remove();

        // recache
        CONSTRUCTOR._cacheItems();

        // rebuild dropdown menu items
        CONSTRUCTOR._buildDropdownMenuItems(CONSTRUCTOR._menu_items, false);

        // resize
        element.data('reset_menu_width', true);
        CONSTRUCTOR._setSize();
        element.data('reset_menu_width', false);
    };

    this.toggle = function(index) {
        var menu_item = SELF._getMenuItem(index);

        if (menu_item.hasClass('disabled')) {
            SELF.enable(index);
        } else {
            SELF.disable(index);
        }
    };

    this.enable = function(index) {
        SELF._getMenuItem(index).removeClass('disabled');
        SELF._getDropdownMenuItem(index).removeClass('disabled');
    };

    this.disable = function(index) {
        var menu_item          = SELF._getMenuItem(index);
        var dropdown_menu_item = SELF._getDropdownMenuItem(index);
        menu_item.addClass('disabled');
        dropdown_menu_item.addClass('disabled');

        if (menu_item.hasClass('active')) {
            var next_index = menu_item.siblings().not('.disabled').first().index();
            SELF.open(next_index);
        }
    };

    this.slideLeft = function() {
        if (!element.hasClass('sliding')) {
            return;
        }

        element.find('.sliding_menu.left').click();
    };

    this.slideRight = function() {
        if (!element.hasClass('sliding')) {
            return;
        }

        element.find('.sliding_menu.right').click();
    };
};

$(document).ready(function() {
    $('.royal_tab').each(function() {
        var royal_tab = new Royal_Tab($(this));
        Royal_Tab_Data.objects.push(royal_tab);
    });
});