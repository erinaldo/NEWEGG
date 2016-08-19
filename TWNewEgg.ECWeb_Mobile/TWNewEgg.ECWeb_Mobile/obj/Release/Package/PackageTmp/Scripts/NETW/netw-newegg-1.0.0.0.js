(function (window, jQuery, undefined) {
    var _twNewegg = function () {
        if (!(this instanceof _twNewegg)) {
            return new _twNewegg();
        }
    };

    _twNewegg.prototype = {
        twNeweggDomain: neRootDM,
        checkEmail: function (email) {
            //var reg = new 
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
                dateType: "json",
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
                    dateType: "json",
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
                    dateType: "json",
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
                    dateType: "json",
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
                    dateType: "json",
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
        successMessage: function (success) {
            console.log(success.toString());
        },
        errorMessage: function (error) {
            console.log(error.toString());
        }
    };

    window.twNewegg = _twNewegg;
})(window, jQuery);
