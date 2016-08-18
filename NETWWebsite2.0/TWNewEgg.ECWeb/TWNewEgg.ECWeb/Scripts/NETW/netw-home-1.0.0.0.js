/*浮動小車*/
function myFloatingAccount() {
    var $FloatingAccount = $("#FloatingAccount");
    var $FloatingTit = $("#FloatingSwitch");
    var $FloatingCon = $(".FloatingCon");

    $FloatingTit.on("click", function () {
        $FloatingCon.toggle(
            function () {
                $(this).animate({
                    opacity: 0,
                    width: 0
                });
            }, function () {
                $(this).animate({
                    opacity: 1,
                    width: "90px"
                });
            }
        );
    });
    $(window).scroll(function () {
        if ($(document).scrollTop() > 150) {
            $FloatingAccount.animate({ width: "show" });
        }
        else {
            $FloatingAccount.fadeOut();
            $FloatingCon.hide();
        }
    });
}

/*電梯資料組成*/
function elevatorData() {
    var $elevator = $("#elevatorWindowArea");
    var $group = $("#elevatorWinGroup");
    var totalTitle = 0;
    var _floor = "";

    t2t = $(".section-title").each(function () {
        var $this = $(this);
        _floor += "<div class='text'><a href='javascript:void(0)'>" + $this.text() + "</a></div>";
        totalTitle++;
    });
    $group.append(_floor);

    $(function () {
        /*電梯顯示*/
        elevatorShow();
    });

    /*首頁-電梯動作*/
    var $text = $("#elevatorWinGroup .text");

    /* 點擊滑動 */
    elevatorActive();

    function elevatorActive() {
        $text.each(function (i) {
            $(this).on("click", function () {
                $("html, body").animate({ scrollTop: $("#winShop" + i).offset().top });
            });
        });
    }

    /* 偵測所在位置 */
    $(window).scroll(function () {
        elevatorScroll();
    });
    function elevatorScroll() {
        for (var count = 0 ; count < totalTitle; count++) {
            if ($("#winShop" + count).offset().top <= $(document).scrollTop() + 300) {
                if (count == totalTitle - 1) {                   
                    $text.removeClass("active");
                    $text.eq(count).addClass("active");
                    break;
                } else {
                    if ($(document).scrollTop() <= $("#winShop" + (count)).offset().top) {
                        $text.removeClass("active");
                        $text.eq(count).addClass("active");
                        break;
                    }
                }
            }
            else {
                $text.removeClass("active");
            }
        }
    }


}

/*分類頁電梯資料組成*/
function elevatorStore() {
    var $elevator = $("#elevatorWindowArea");
    var $group = $("#elevatorWinGroup");
    var totalTitle = 0;
    var _floor = "";

    t2t = $(".section3").each(function () {
        var $this = $(this);
        totalTitle++;
    });

    $(function () {
        /*電梯顯示*/
        elevatorShow();
    });

    /*首頁-電梯動作*/
    var $text = $("#elevatorWinGroup .text");

    /* 點擊滑動 */
    elevatorActive();

    function elevatorActive() {
        $text.each(function (i) {
            var $win = $("#shop" + i);
            /*var midWinFloor = ($(window).height() - $win.height()) / 2;*/
            var winFloor = $win.offset().top + 60;//head banner高度
            //var winFloor = $win.offset().top - midWinFloor + 60;//櫥窗置中
            $(this).on("click", function () {
                $("html, body").animate({ scrollTop: winFloor });
            });
        });
    }

    /* 偵測所在位置 */
    $(window).scroll(function () {
        elevatorScroll();
    });
    function elevatorScroll() {
        for (var count = 0 ; count < totalTitle; count++) {
            if ($("#shop" + count).offset().top <= $(document).scrollTop() + 300) {
                if (count == totalTitle - 1) {
                    $text.removeClass("active");
                    $text.eq(count).addClass("active");
                    //alert("active");
                    break;
                } else {
                    if ($(document).scrollTop() < $("#shop" + (count + 1)).offset().top) {
                        $text.removeClass("active");
                        $text.eq(count).addClass("active");
                        break;
                    }
                }
            }
            else {
                $text.removeClass("active");
            }
        }
    }
}

/*電梯顯示*/
function elevatorShow() {
    var $elevator = $("#elevatorWindowArea");
    var $text = $("#elevatorWinGroup .text");
    var num = $("section[windowid]").length;
    var marTop = ($(window).height() - $elevator.height()) / 2;

    $elevator.css({ top: marTop });

    $(window).scroll(function () {
        //判斷瀏覽器寬度>=1320px
        if (num = 0) {
            $elevator.hide();
        }
        else if ($(document).innerWidth() >= 1303 && $(document).scrollTop() >= 500) {
            $elevator.animate({ width: 'show' });
        }
        else {
            $elevator.fadeOut();
        }
    });

}

/*電梯動作*/
function elevatorAction(action, floor) {
    switch (action)
    {
        case 1:
            $('html, body').animate({ scrollTop: 0 });
            break;
        case 2:
            $('html, body').animate({ scrollTop: $(document).height() });
            break;
        case 3:
            var $shop = $("#shop" + floor),
                shopFloor = $shop.offset().top;
            $("html, body").animate({ scrollTop: shopFloor });
            break;
    }
}

/*我的帳戶下拉*/
function myDropAccount() {
    var $dropAccount = $("#dropAccount");
    var $dropCon = $(".dropCon");

    $dropAccount.on("click", function () {
        $(this).toggleClass("active");
        $dropCon.fadeToggle(500);
    });
}

/*密碼設定條件*/
function showPassword() {
    var $account_pwd = $(".account_pwd"); $pswRuleBox = $(".registerArea .pswRuleBox");
    $account_pwd.focus(function () {
        $pswRuleBox.animate({
            opacity: 1,
            marginTop: "10px",
            height:"100px",
            position:"relative",
            zIndex:9
        }, 500).fadeIn();
    });
}

/*側選單下拉*/
function dropDownSwitch() {
    var $menuSwitch = $(".menudropDown"),
        $mainMenu = $("#mainMenu");
    $menuSwitch.on("click", function () {
        $mainMenu.toggle(
            function () {
                $(this).animate({
                    opacity: 0,
                    width:"100%",
                    height:0
                }, 500);
            }, function () {
                $(this).animate({
                    opacity: 1,
                    width: "100%",
                    height: "480px"
                }, 500);
            }
        );
    });
}

/*商品圖垂直置中*/
function verticalImg(obj) {
    var $img = $(".box .pic img");
    var _height = $img.outerHeight();
    if (obj!=null) {
        _height = $(obj).outerHeight();
    }
    var $outHeight = $(".box .pic").outerHeight();
    //console.log("_height:" + _height + "；$outHeight:" + $outHeight);
    if (_height != 0) {
        function marg(a, b) {
            return (a - b) / 2;
        }
        //console.log("marg(a, b):" + marg($outHeight, _height));
        $img.css("margin-top", marg($outHeight, _height));
    }
    else { }
}
