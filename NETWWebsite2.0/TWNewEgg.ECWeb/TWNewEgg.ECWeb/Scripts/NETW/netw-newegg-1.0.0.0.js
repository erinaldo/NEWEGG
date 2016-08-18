(function (window, jQuery, undefined) {
    var _twNewegg = function () {
        if (!(this instanceof _twNewegg)) {
            return new _twNewegg();
        }
    };

    _twNewegg.prototype = {
        twNeweggDomain: neRootDM,
        twNeweggMoURL: neMWebURL,
        checkEmail: function (email) {
            //var reg = new 
        },
        parseUrlFormatByName: function (content, name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(content);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        },
        parseUrlFormatByNameIC: function (content, name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)", "gi"),
                results = regex.exec(content);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        },
        isSecure: function () {
            if (window.location.protocol != "https:") {
                return false;
            } else {
                return true;
            }
        },
        checkNGO: function (methodGo, arrayPara) {
            if (!jQuery.cookie("neui")) {
                if (jQuery.isArray(arrayPara)) {
                    arrayPara.push(false);
                    methodGo.apply(this, arrayPara);
                } else {
                    methodGo.apply(this, [false]);
                }
                return;
            }
            jQuery.ajax({
                method: "GET",
                url: "/api/auchk",
                dataType: "json",
                //async: false,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Authorization", "Basic " + jQuery.cookie("neui"));
                }
            })
            .done(function (data) {
                if (typeof methodGo === "function") {
                    if (jQuery.isArray(arrayPara)) {
                        arrayPara.push(data);
                        methodGo.apply(this, arrayPara);
                    } else {
                        methodGo.apply(this, [data]);
                    }
                }
                return;
            })
            .fail(function () {
                return;
            });
        },
        neWebApi: {
            getNe: function (path, query, successMethod, successArray, failMethod, failArray) {
                var fullPath = path + ((!query || 0 === query.length) ? "" : "?" + query);
                jQuery.ajax({
                    method: "GET",
                    url: fullPath,
                    dataType: "json",
                    //async: false,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Basic " + jQuery.cookie("neui"));
                    }
                })
                .done(function (data) {
                    if (typeof successMethod === "function") {
                        if (jQuery.isArray(successArray)) {
                            successArray.push(data);
                            successMethod.apply(this, successArray);
                        } else {
                            successMethod.apply(this, []);
                        }
                    }
                    return;
                })
                .fail(function () {
                    if (typeof failMethod === "function") {
                        if (jQuery.isArray(failArray)) {
                            failMethod.apply(this, failArray);
                        } else {
                            failMethod.apply(this, []);
                        }
                    }
                    return;
                });
            },
            postNe: function (path, postData, successMethod, successArray, failMethod, failArray) {
                jQuery.ajax({
                    method: "POST",
                    url: path,
                    dataType: "json",
                    //async: false,
                    data: postData,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Basic " + jQuery.cookie("neui"));
                    }
                })
                .done(function (data) {
                    if (typeof successMethod === "function") {
                        if (jQuery.isArray(successArray)) {
                            successArray.push(data);
                            successMethod.apply(this, successArray);
                        } else {
                            successMethod.apply(this, []);
                        }
                    }
                    return;
                })
                .fail(function () {
                    if (typeof failMethod === "function") {
                        if (jQuery.isArray(failArray)) {
                            failMethod.apply(this, failArray);
                        } else {
                            failMethod.apply(this, []);
                        }
                    }
                    return;
                });
            },
            putNe: function (path, postData, successMethod, successArray, failMethod, failArray) {
                jQuery.ajax({
                    method: "PUT",
                    url: path,
                    dataType: "json",
                    //async: false,
                    data: postData,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Basic " + jQuery.cookie("neui"));
                    }
                })
                .done(function (data) {
                    if (typeof successMethod === "function") {
                        if (jQuery.isArray(successArray)) {
                            successArray.push(data);
                            successMethod.apply(this, successArray);
                        } else {
                            successMethod.apply(this, []);
                        }
                    }
                    return;
                })
                .fail(function () {
                    if (typeof failMethod === "function") {
                        if (jQuery.isArray(failArray)) {
                            failMethod.apply(this, failArray);
                        } else {
                            failMethod.apply(this, []);
                        }
                    }
                    return;
                });
            },
            deleteNe: function (path, postData, successMethod, successArray, failMethod, failArray) {
                jQuery.ajax({
                    method: "DELETE",
                    url: path,
                    dataType: "json",
                    //async: false,
                    data: postData,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Basic " + jQuery.cookie("neui"));
                    }
                })
                .done(function (data) {
                    if (typeof successMethod === "function") {
                        if (jQuery.isArray(successArray)) {
                            successArray.push(data);
                            successMethod.apply(this, successArray);
                        } else {
                            successMethod.apply(this, []);
                        }
                    }
                    return;
                })
                .fail(function () {
                    if (typeof failMethod === "function") {
                        if (jQuery.isArray(failArray)) {
                            failMethod.apply(this, failArray);
                        } else {
                            failMethod.apply(this, []);
                        }
                    }
                    return;
                });
            }
        },
        checkState: function (state, msg) {
            var winW = $(window).width(),
                winH = $(window).height();
            $("body")
                .append("<div class='mask'></div><div class='stateShow'></div>") //插入遮罩與訊息
                .css("overflow", "hidden"); //鎖定當前頁面捲軸

            
            //關閉
            function closeTip() {
                $(".mask, .stateShow").fadeOut(function () { $(this).remove(); }); //移除訊息
                $("body").removeAttr("style"); //解除鎖定捲軸
            }

            //訊息計時後自動消失
            function lightBoxAutoClosed() {
                setTimeout(function () { closeTip(); }, 1800);
            }

            //訊息預設不關閉
            function lightBoxNoClosed() {
            }

            //訊息點擊黑底關閉
            function lightBox() {
                $(".mask").on("click", closeTip);
            }

            //訊息自動符合視窗大小並置中
            function autoWidth() {
                //給予符合視窗大小的尺寸
                var $this = $(".stateShow").addClass("autoWidth");
                $this.width(winW).height(winH - 20); 
                //以目前的寬高做置中
                var left = (winW - $this.width()) / 2,
                    top = (winH - $this.height()) / 2
                $this.css({ "left": left, "top": top });
            }

            switch (state) {
                //處理中圖示
                case 'loadingNoClose':
                    $(".stateShow").html("<img src='/Themes/img/System/ProcessWindow.gif'/>");
                    lightBoxNoClosed();
                    break;
                //加入最愛圖示
                case 'addToFav':
                    $(".stateShow").html("<img src='/Themes/img/System/tipsImage_AddWish.png'/>");
                    $(".mask").hide();
                    lightBoxAutoClosed();
                    break;
                //加入購物車圖示
                case 'addToCart':
                    $(".stateShow").html("<img src='/Themes/img/System/tipsImage_AddCart.png'/>");
                    $(".mask").hide();
                    lightBoxAutoClosed();
                    break;
                //加入任選館圖示
                case 'addToChooseAnyPage':
                    $(".stateShow").html("<img src='/Themes/img/System/tipsImage_AddChoList.png'/>");
                    lightBoxAutoClosed();
                    break;
                //提示文字訊息
                case 'tips':
                    $(".stateShow").html("<p class='tips'>" + msg + "</p>");
                    lightBoxAutoClosed();
                    break;
                //開啟彈跳頁面,點擊可關閉
                case 'loadWin': 
                    $(".stateShow").html("<img src='/Themes/img/System/ProcessWindow.gif'/>");
                    $(".stateShow").load(msg);
                    lightBox();
                    autoWidth();
                    break;
            }
            return;
        },
        loginMail: function () {
            var logInfo = jQuery.cookie("neui");
            var email = this.parseUrlFormatByName(logInfo, "mail");
            return email;
        }
        //successMessage: function (success) {
        //    console.log(success.toString());
        //},
        //errorMessage: function (error) {
        //    console.log(error.toString());
        //}
    };

    window.twNewegg = _twNewegg;
})(window, jQuery);
