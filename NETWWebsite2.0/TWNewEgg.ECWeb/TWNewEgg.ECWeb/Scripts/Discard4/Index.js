/*動態加入廢四機同意提示*/
/*依據 BSATW-173 廢四機需求增加*/
/*廢四機回收四聯單-----------add by bruce 20160512*/
$(function () {

    /*
    $("body").append("<div class='LightBoxUnderLay' style='display:none;'></div>\
            <div class='LightBoxMsg Stlye-1 showOrder'  style='width:700px; height:80%; font-size:18px; z-index:101;'>\
                <div class='MsgContent' style='border:0px solid #888888; height: 80%'>\
                    <img src='/Themes/img/img_trans.gif' alt='close' class='closeWindowBtn' />\
                    <div id='msgNote'></div>\
                </div>\
                <div class='button_container' style='border:0px solid #888888; text-align: center;'>\
                    <input type='button' id='btn_agree' value='我同意' class='btn btn-submit OrgBgBtn' style='padding: 10px 18px; font-size: 18px;'>\
                </div>\
            </div>\
        ");
    */

    $("body").append("<div class='LightBoxMsg Stlye-1 showOrder'  style='width:700px; height:35%; font-size:18px; z-index:101;'>\
                <div class='MsgContent' style='border:0px solid #888888; height: 50%; '>\
                    <img src='/Themes/img/img_trans.gif' alt='close' class='closeWindowBtn' />\
                    <div id='msgNote' style='margin: 30px 0; line-height: 25px; font-size: 18px; text-align: left;'></div>\
                </div>\
                <div class='button_container' style='border:0px solid #888888; text-align: center;'>\
                    <input type='button' id='btn_agree' value='確認' class='btn btn-submit OrgBgBtn' style='padding: 10px 18px; font-size: 18px;'>\
                </div>\
            </div>\
        ");

});

/*廢四機同意------------add by bruce 20160512*/
function DisplayDiscard4() {

    this.note_msg = ''; /*廢四機同意文字*/

    /*廢四機同意文字-------------add by bruce 20160516*/
    var _note_msg = '<font color="red">廢四機(電視機、洗衣機、電冰箱或冷、暖氣機)回收服務</font>';
    var _note_msg2 = '<br>新購公告指定規格之電視機、洗衣機、電冰箱或冷、暖氣機時，販賣業者應提供同項目、數量、時間及同送達地址之廢四機回收(搬、載運)無償服務。但不包含搬運、拆缷而動用大型機具、工程或危險施工等。';
    var _note_msg3 = '<br><font style="font-weight: bold;">環保署資源回收專線: 0800-085717</font>';
    this.note_msg = this.note_msg.concat(_note_msg, _note_msg2, _note_msg3);

    this.is_save = false; /*是否已儲存*/
    this.is_discard4 = false; /*是否已同意*/
    this.agreeDiscard4 = ''; /*Y或N*/
    console.log('your is new DisplayDiscard4');

}

DisplayDiscard4.prototype.init_data = function () {
    var my_key = 'AgreedDiscard4';
    var my_value = null;
    /*Check browser support*/
    if (typeof (Storage) !== "undefined") {
        /*Store*/
        localStorage.removeItem(my_key);
        console.log('removeItem for localStorage');
    } else {
        /*set cookie*/
        createCookie(my_key, my_value, 1);
        console.log('my_value for createCookie');
    }
}

/*設定廢四機同意提示文字
DisplayDiscard4.prototype.set_note = function (note_msg) {
    if (typeof (note_msg) !== "undefined") this.note_msg = note_msg;
}
*/

/*設定廢四機提示+顯示廢四機提示*/
DisplayDiscard4.prototype.show_note = function (note_msg) {

    if (typeof (note_msg) !== "undefined") this.note_msg = note_msg;
    var note_content = this.note_msg;
    clickLightBoxCenter(".LightBoxMsg.showOrder"); /*調用Scripts\TWEC\WebEffect.js*/
    $(".LightBoxMsg.showOrder").css("top", "");
    $(".LightBoxUnderLay").slideDown(600).fadeIn(); /*背景罩*/
    var msgNote = $(".showOrder .MsgContent #msgNote"); /*取得要寫入的區塊*/
    msgNote.html(note_content.replace(/\n\r?/g, '<br />'));/*填入資料*/

    var dd4 = new DisplayDiscard4();

    /*註冊關閉事件*/
    $('.closeWindowBtn').on("click", function () {        
        dd4.set_discard4(true);
        $(".LightBoxUnderLay, .LightBoxMsg").not('.DelayWindow').fadeOut();
    }).css({
        'position': 'absolute',
        'top': '10px',
        'right': '10px',
        'width': '20px',
        'height': '20px',
        'background': 'url("/Themes/img/Icon/closeIcon.png") no-repeat',
        'cursor': 'pointer'
    });

    /*註冊我同意*/
    $('#btn_agree').on("click", function () {            
        dd4.set_discard4(true);
        $(".LightBoxUnderLay, .LightBoxMsg").not('.DelayWindow').fadeOut();
    });

}

/*設定是否同意, 傳入true or false*/
DisplayDiscard4.prototype.set_discard4 = function (is_agree) {
    this.is_discard4 = is_agree;
    this.is_save = this.save_data();
    this.get_data();
}

/*取得資料,傳回Y或N*/
DisplayDiscard4.prototype.get_data = function () {    
    /*取得同意值, true or false*/
    var my_key = 'AgreedDiscard4';
    var my_value = '';
    /*Check browser support*/
    if (typeof (Storage) !== "undefined") {
        /*Store*/
        my_value = localStorage.getItem(my_key);
        console.log('my_value for localStorage');
    } else {
        my_value = getCookie(my_key);
        console.log('my_value for getCookie');
    }
    (my_value == "Y") ? this.is_discard4 = true : this.is_discard4 = false;
    this.agreeDiscard4 = my_value;
    console.log('is_discard4:' + this.is_discard4);
    console.log('agreeDiscard4:' + this.agreeDiscard4);    
    return my_value;
}

/*儲存廢四機同意*/
DisplayDiscard4.prototype.save_data = function () {
    this.is_save = false;
    var my_key = 'AgreedDiscard4';
    var my_value = 'N';
    (this.is_discard4) ? my_value = 'Y' : my_value = 'N';
    /*Check browser support*/
    if (typeof (Storage) !== "undefined") {
        /*Store*/
        localStorage.setItem(my_key, my_value);
        console.log('my_value for localStorage');
    } else {
        /*set cookie*/
        createCookie(my_key, my_value, 1);
        console.log('my_value for createCookie');
    }
    this.is_save = true;
    this.agreeDiscard4 = my_value;
    console.log('is_save:' + this.is_save);
    console.log('my_key:' + my_key);
    console.log('my_value:' + my_value);
    /*return my_value;*/
}

function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}

function createCookie(name, value, days) {
    var date, expires;
    if (days) {
        date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}


