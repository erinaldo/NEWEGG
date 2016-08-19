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
        if ($('html body').scrollTop() > 150) {
            $FloatingAccount.show(500);
        }
        else {
            $FloatingAccount.fadeOut();
        }
    });
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

/*顯示或隱藏密碼字元*/
function showPassword() {
    var $account_pwd = $("#account_pwd"); $pswRuleBox = $(".pswRuleBox");/* $pswRuleBox = $("#ShowPasswordRule");*/
    $account_pwd.focus(function () {
        $pswRuleBox.animate({
            opacity: 1,
            height: "110px"
        }, 500);
    }).focusout(function () {
        $pswRuleBox.animate({
            opacity: 0,
            height: 0
        }, 500);
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
    console.log("_height:" + _height + "；$outHeight:" + $outHeight);
    if (_height != 0) {
        function marg(a, b) {
            return (a - b) / 2;
        }
        //console.log("marg(a, b):" + marg($outHeight, _height));
        $img.css("margin-top", marg($outHeight, _height));
    }
    else { }
}
/*放大商品圖*/
function itemPicHover() {
    var $img = $(".box .pic img"),
        _width = $img.outerWidth();
    console.log("_width:" + _width);
    if (_width != 0) {
        $img.on("mouseover", function () {
            $(this).css("max-width:", "none").delay(300).animate({
                width: _width * 1.5
            }, 800);
        }).on("mouseout", function () {
            $(this).delay(500).animate({
                width: _width * 1
            }, 500);
        });
    }
    else { }
}

/*賣場頁原圖檢視*/
function itemOriginalImg() {
    var $productPic = $(".productPic .pic img"),
        $originalImg = $(".originalPic");
    $productPic.on("mouseover", function () {
        $originalImg.fadeIn();
    }).on("mouseout", function () {
        $originalImg.fadeOut();
    });
}

/*BAK放大商品圖*/
function BAKitemPicHover(){
    var $itemImg = $(".box .pic"),
        _picWidth = $itemImg.width(),
        _picHeight = $itemImg.height(),
        _imgWidth = $itemImg.find("img").width(),
        _imgHeight = $itemImg.find("img").height(),
        _top = (_picWidth - _imgWidth) / 2;
    _left = (_picHeight - _imgHeight) / 2;
    width2 = $itemImg.find("img").height() * 2,

    $itemImg.css("position", "relative");
    $itemImg.find("img").css("position", "absolute");

   /*function rePosition() {
        $(this).find("img").animate({
            top: _top,
        });
    }
    
    rePosition();*/
  
    function sub(a, b)
    {
        return (a - b) / 2;

    }
    $itemImg.on("mouseenter", function () {
        //rePosition();
        console.log("beforheight" + $itemImg.height());
        console.log("beforfind()" + $itemImg.find("img").height());
        $(this).find("img").animate({

            maxWidth: width2,
            maxHeight: "200px",
            width: width2,
            height: "200px",
            top: sub($itemImg.height(), width2),
            left: sub(200, $itemImg.height()),
            zIndex: 9,

        }, 500);
        console.log("height" + $itemImg.height());
        console.log("find()" + $itemImg.find("img").height());
    }).on("mouseleave", function () {
        $(this).find("img").delay(200).animate({
            maxWidth: "160px",
            maxHeight: "160px"
        }, 500);
    });

    //event.stopPropagation();
}