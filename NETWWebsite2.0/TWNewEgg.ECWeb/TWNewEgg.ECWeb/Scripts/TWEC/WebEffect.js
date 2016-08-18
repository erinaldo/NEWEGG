/*給子選單滑出滑入用的jquery*/
function TabSelectArea() {
    var $Tab = $(".TabArea .Tab"); $sub = $(".SubMenu");
    $Tab.click(function () {
        $(this).toggleClass("TabSelected").find($sub).slideToggle(200);
    });
}

/*說明顯示開關*/
function tipsSwitch() {
    var $tipsTips = $(".couponTipsTit"); $couponTxt = $(".couponTxt");
    $tipsTips.click(function () {
        $couponTxt.toggle(200);
    });
}

/*彈跳視窗自動偵測位置*/
function LightBoxCenter(aa) {
    var h = $(window).height();
    var w = $('body').width();
    var $LightBoxMsg = $('.LightBoxMsg');
    if (aa != null && aa != "") {
        $LightBoxMsg = $(aa);
    }
    $underLay = $('.LightBoxUnderLay');
    var MsgTop = h / 2 - $LightBoxMsg.outerHeight() / 2;
    var MsgLeft = w / 2 - $LightBoxMsg.outerWidth() / 2;
    $LightBoxMsg.css({ top: MsgTop, left: MsgLeft });
}

/*彈跳視窗自動偵測位置*/
function clickLightBoxCenter(msgDiv) {
    var h = $(window).height(),
        w = $('body').width(),
        $underLay = $('.LightBoxUnderLay'),
        $LightBoxMsg = $('.LightBoxMsg');
    if (msgDiv != null && msgDiv != "") {
        $LightBoxMsg = $(msgDiv);
    }
    var MsgTop = h / 2 - $LightBoxMsg.outerHeight() / 2;
    var MsgLeft = w / 2 - $LightBoxMsg.outerWidth() / 2;
    $LightBoxMsg.css({ top: MsgTop, left: MsgLeft });
    $LightBoxMsg.fadeIn(msgDiv);
    $LightBoxMsg.parent($underLay).fadeIn();
}

/*賣場頁商品圖垂直置中*/
function productImg() {
    var ImgH = $('.productPic').children('img').height() / 2;
    var PicH = $('.productPic').height() / 2;
    var PadH = PicH - ImgH;
    $('.productPic').children('img').css('margin-top', PadH);
}

/*TEMP編輯個人資料顯示*/
function showLightbox() {
    $underLay.show();
    $underLay.find("input").on("click", function () {
        $(this).parent().parent().parent().hide();
    });
};

/*我的帳戶中我的訂單Tab小箭頭樣式替換*/
function TabOpenClose() {
    $('#Order>a').children().eq(1).toggleClass('Down');
}