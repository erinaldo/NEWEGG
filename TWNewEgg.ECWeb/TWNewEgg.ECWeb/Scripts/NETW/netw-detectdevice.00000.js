(function (twNewegg, jQuery) {

    var detectDevice = function () {

        var setting = {
            mobileUrl: twNewegg().twNeweggMoURL,
            cookieName: "goNEMWeb"
        };

        var chooseStoreCSS = function (storeID) {
            var cssName = "storeFashion";
            switch (storeID) {
                case "737":
                    cssName = "storeNeweggUSA";
                    break;
                case "1279":
                    cssName = "storeDesign";
                    break;
                case "734":
                    cssName = "storeFashion";
                    break;
                case "1929":
                    cssName = "storeBeauty";
                    break;
                case "1930":
                    cssName = "storeHealthcare";
                    break;
                case "736":
                    cssName = "storeFoods";
                    break;
                case "735":
                    cssName = "storeHealth";
                    break;
                case "7":
                    cssName = "storeOutdoor";
                    break;
                case "1928":
                    cssName = "storeLovely";
                    break;
                case "6":
                    cssName = "storeLiving";
                    break;
                case "3":
                    cssName = "storeappliance";
                    break;
                case "264":
                    cssName = "store3C";
                    break;
                case "1":
                    cssName = "storeComputer";
                    break;
                case "2505":
                    cssName = "storeCity";
                    break;
                default:
                    break;
            }
            return cssName;
        };

        var store2M = function () {
            var content = window.location.search;
            var storeid = twNewegg().parseUrlFormatByNameIC(content, "storeid");
            if (!storeid) {
                return "";
            }
            return "?categoryid=" + storeid + "&cssname=" + chooseStoreCSS(storeid);
        };

        var item2M = function () {
            var content = window.location.search;
            var itemid = twNewegg().parseUrlFormatByNameIC(content, "itemid");
            if (!itemid) {
                return "";
            }
            return "?itemid=" + itemid;
        };

        var search2M = function () {
            var content = window.location.search;
            var searchword = twNewegg().parseUrlFormatByNameIC(content, "searchword");
            if (!searchword) {
                return "";
            }
            return "?searchword=" + searchword;
        };

        var desk2MobileArray = [
            ["/", "/", null],
            ["/home/index", "/", null],
            ["/item", "/item", item2M],
            ["/store", "/category", store2M],
            ["/storeus", "/category", store2M],
            ["/search", "/search", search2M],
            ["/flash", "/flash", null],
            ["/service/faq", "/service/index", null],
            ["/service/aboutshopping", "/service/index", null],
            ["/service/aboutshipping", "/service/index", null],
            ["/service/aboutpayment", "/service/index", null],
            ["/service/aboutservice", "/service/index", null],
            ["/service/memberservice", "/service/index", null],
            ["/cart/wishcart", "/wishlist/index", null],
            ["/myaccount/coupon", "/myaccount/coupon", null],
            ["/myaccount/editpersoninfo", "/myaccount/editpersoninfo", null],
            ["/myaccount/editpassword", "/myaccount/editpersoninfo", null],
            ["/policies/privacy", "/policies/privacy", null],
            ["/policies/member", "/policies/member", null],
            ["/myaccount/login", "/myaccount/login", null],
            ["/myaccount/signup", "/myaccount/signup", null],
            ["/policies/privacy", "/policies/privacy", null]
        ];

        var setCookies = function (key, value, expire, path, domain) {
            jQuery.cookie(key, value, {
                expires: expire,
                path: path,
                domain: domain
            });
        };

        var isMobile = function () {
            var currentAgent = navigator.userAgent || navigator.vendor || window.opera;
            if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(currentAgent) ||
        /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(currentAgent.substr(0, 4))) {
                return true;
            }
            return false;
        };

        var isPad = function () {
            var currentAgent = navigator.userAgent || navigator.vendor || window.opera;
            var isiPad = currentAgent.match(/iPad/i) != null;
            var isAndroidPad = currentAgent.match(/android/i) != null;
            if(isiPad || isAndroidPad)
                return true;
            return false;
        };

        var confirmPopUp = function (displayMessage) {
            return confirm(displayMessage);
        };

        var set2NEMobile = function () {
            setCookies(setting["cookieName"], "goNEMobile", 1, "/", twNewegg().twNeweggDomain);
        };

        var set2NEDesktop = function () {
            setCookies(setting["cookieName"], "goNEDesktop", 1, "/", twNewegg().twNeweggDomain);
        };

        var ready2GO = function () {
            var go2Mobile = jQuery.cookie(setting["cookieName"]);
            if (!go2Mobile) {
                if (confirmPopUp("已偵測到您使用行動裝置來瀏覽網站，\n是否切換至行動版網站？")) {
                    set2NEMobile();
                    return true;
                }
            }
            switch (go2Mobile) {
                case "goNEMobile":
                    return true;
                case "goNEDesktop":
                    return false;
                default:
                    return false;
            }
        };

        var gotoMobile = function () {
            if (!isMobile()) {
                return;
            }
            var currentPath = window.location.pathname;
            for (var i = 0; i < desk2MobileArray.length; i++) {
                if (desk2MobileArray[i][0].toLowerCase() === currentPath.toLowerCase()) {
                    if (!ready2GO()) {
                        break;
                    }
                    if (typeof desk2MobileArray[i][2] === "function") {
                        window.location = setting["mobileUrl"] + desk2MobileArray[i][1] + desk2MobileArray[i][2].apply(this, []);
                    } else {
                        window.location = setting["mobileUrl"] + desk2MobileArray[i][1];
                    }
                    break;
                }
            }
        };

        this.detectMethod = function (action, settingPara) {
            jQuery.extend(setting, settingPara);
            switch (action) {
                case "isMobile":
                    return isMobile();
                    break;
                case "isPad":
                    return isPad();
                    break;
                case "redirectUrl":
                    gotoMobile();
                    break;
                case "set2NEDesktop":
                    set2NEDesktop();
                    break;
                case "set2NEMobile":
                    set2NEMobile();
                    break;
                default:
                    break;
            }
        };

    };
    twNewegg.prototype.mobileDevice = function () {
        return new detectDevice();
    }
})(twNewegg, jQuery);
//(function (twNewegg) {
    //twNewegg().mobileDevice().detectMethod("redirectUrl", null);
//})(twNewegg);