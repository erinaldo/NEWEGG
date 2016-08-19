//賣場頁用的圖片置中jQuery
//IE8跳出原因:[ 'Audio' is undefined]，只要想辦法跳過AudioElement就可執行。
function imgVerticalAlignCenter() {
    var strUrl = window.location.href,
        str = strUrl.toUpperCase().indexOf("ACTIVITY"); 
        
    if (str > 0) {
        //return false;
        isImgLoad(); //for 活動頁使用
    } else {
        verticalDetect($(".itemImgCen"));
    }

}

function sectionImgVerticalAlignCenter(section) {
    verticalDetect(section.find(".itemImgCen"));

}

function resetImgVerticalAlignCenter(section) {
    verticalDetect(section.find(".itemImgCen"));
}

function isImgLoad() {
    var t_img; //計時器設定
    var isLoad = true; //狀態初始設定

    $(".itemImgCen").each(function () {
        var activiteImgSrc = $(this).attr("src");

        if (!!activiteImgSrc) {
            if (activiteImgSrc.indexOf("itemImgUrl") == -1) {
                verticalDetect($(this));
            } else if (activiteImgSrc.indexOf("itemImgUrl") > -1) {
                isLoad = false;
            }
        }

    });

    if (isLoad === false) {
        t_img = setTimeout(function () {
            isImgLoad();
        }, 1000);
    }

}



function verticalDetect(obj) {
    obj.each(function () {
        //$(this).attr("src", $(this).attr("src") + "?" + new Date().getTime());
        var loadSize = $(this).parents(".pic").find(".load").length;
        var $src = $(this).attr("src");


        if (loadSize == 0) {
            $(this).parents(".pic").css('position', 'relative');
            $(this).parents(".pic").append("<span class='load' style='position:absolute; left: 50%; top: 50%; display:inline-block; width: 100px; height: 20px; margin: -10px 0 0 -50px; text-align:center; color: #000; line-height: 20px;'>loading...</span>");
            $(this).hide();
        }

        $(this).attr("title", $src);
        $(this).attr("src", " ");
        var dataSrc = $(this).attr("title");


        //load detect
        if (!!$src) {
            $(this).on("load", function (index) {

                if (this.complete === true || this.readyState === "complete") {
                    $(this).parents(".pic").find("span.load").remove();
                    $(this).show();

                    verticalPic($(this));
                }

            }).attr("src", dataSrc).on("error", function () {
                $(this).parents(".pic").find(".load").text("圖片準備中，請稍候");
            });
        }

    });

}

//垂直橫向置中函式
function verticalPic(e) {
    var ua = navigator.userAgent;

    if (ua.indexOf('MSIE') > -1) {
        var image = new Image();
        image.src = e.attr("src");


        var imgHeight = image.height; //獲取圖片原始尺寸高
        var imgWidth = image.width; //獲取圖片原始尺寸寬

    } else {

        var imgHeight = e.get(0).naturalHeight; //獲取圖片原始尺寸高
        var imgWidth = e.get(0).naturalWidth; //獲取圖片原始尺寸寬

    }

    var imgSize = imgWidth / imgHeight; //計算圖片尺寸比例

    var maxHeight = parseInt(e.parents(".pic").height()); //獲取父層框高度
    var maxWidth = parseInt(e.parents(".pic").width()); //獲取父層框寬度
    var picSize = maxWidth / maxHeight;


    e.parent("a").css({
        "position": "relative",
        "display": "block",
        "width": maxWidth,
        "height": maxHeight,
        "overflow": "hidden"
    });

    e.parents(".pic").css({
        "position": "relative",
        "display": "block",
        "width": maxWidth,
        "height": maxHeight,
        "overflow": "hidden"
    });

    e.css({
        "position": "absolute",
        "left": "50%",
        "top": "50%"
    });


    if (picSize == 1) {
        e.css({
            "max-width": "none",
            "max-height": "none"
        });

        if (imgSize == 1) {
            e.css({
                "width": maxHeight,
                "height": maxHeight,
                "margin-left": (maxHeight / 2) * -1,
                "margin-top": (maxHeight / 2) * -1
            });
        } else if (imgSize < 1) {
            e.css({
                "height": maxHeight,
                "width": maxHeight * imgSize,
                "margin-left": ((maxHeight * imgSize) / 2) * -1,
                "margin-top": (maxHeight / 2) * -1
            });
        } else if (imgSize > 1) {
            e.css({
                "width": maxWidth,
                "height": maxWidth / imgSize,
                "margin-left": (maxWidth / 2) * -1,
                "margin-top": ((maxWidth / imgSize) / 2) * -1
            });
        }

    } else if (picSize < 1) {
        e.css({
            "max-width": "none",
            "max-height": "none"
        });

        if (imgSize == 1) {
            e.css({
                "width": maxWidth,
                "height": maxWidth,
                "margin-top": (maxWidth / 2) * -1,
                "margin-left": (maxWidth / 2) * -1
            });
        } else if (imgSize < 1) {
            if (imgSize > 0.6) {
                e.css({
                    "height": maxWidth / imgSize,
                    "width": maxWidth,
                    "margin-top": ((maxWidth / imgSize) / 2) * -1,
                    "margin-left": (maxWidth / 2) * -1
                });

            } else {
                e.css({
                    "height": maxHeight,
                    "width": maxHeight * imgSize,
                    "margin-top": (maxHeight / 2) * -1,
                    "margin-left": ((maxHeight * imgSize) / 2) * -1
                });
            }
        } else if (imgSize > 1) {
            e.css({
                "width": maxWidth,
                "height": maxWidth / imgSize,
                "margin-top": ((maxWidth / imgSize) / 2) * -1,
                "margin-left": (maxWidth / 2) * -1
            });
        }

    } else if (picSize > 1) {
        e.css({
            "max-width": "none",
            "max-height": "none",
        });

        if (imgSize == 1) {
            e.css({
                "width": maxHeight,
                "height": maxHeight,
                "margin-left": (maxHeight / 2) * -1,
                "margin-top": (maxHeight / 2) * -1
            });
        } else if (imgSize < 1) {
            e.css({
                "width": maxHeight * imgSize,
                "height": maxHeight,
                "margin-left": ((maxHeight * imgSize) / 2) * -1,
                "margin-top": (maxHeight / 2) * -1
            });
        } else if (imgSize > 1) {
            e.css({
                "width": maxHeight * imgSize,
                "height": maxHeight,
                "margin-left": ((maxHeight * imgSize) / 2) * -1,
                "margin-top": (maxHeight / 2) * -1
            });
        }
    }

    MagnifyImg(e, 110);
}


//圖片放大函式
//size以100%為基準
function MagnifyImg(argImg,size) {
    //產品頁不使用
    var strUrl = window.location.href;
    if (strUrl.toUpperCase().indexOf("/ITEM?ITEMID=") >= 0) {
        return;
    }

    var numMaxHeight = 0;
    var numMaxWidth = 0;
    //取得圖片的css設定
    numMaxHeight = parseInt($(argImg).css('height'));
    numMaxWidth = parseInt($(argImg).css('width'));

    if (size == undefined) {
        var widthResize = numMaxWidth;
        var heightResize = numMaxHeight;
    } else {
        var widthResize = (size / 100) * numMaxWidth;
        var heightResize = (size / 100) * numMaxHeight;
    }


    var orgWidth = $(argImg).width(),
        orgHeight = $(argImg).height(),
        orgMarginTop = $(argImg).css("margin-top"),
        orgMarginLeft = $(argImg).css("margin-left");


    $(argImg).hover(
    function () {
        $(argImg).css({
            "left": "50%",
            "top": "50%",
            "width": widthResize,
            "height": heightResize,
            "margin-left": (widthResize / 2) * -1,
            "margin-top": (heightResize / 2) * -1,
            "transition-duration": "0.3s"
        });
    }, function () {
        $(argImg).css({
            "left": "50%",
            "top": "50%",
            "width": orgWidth,
            "height": orgHeight,
            "margin-left": orgMarginLeft,
            "margin-top": orgMarginTop
        });
    });

}