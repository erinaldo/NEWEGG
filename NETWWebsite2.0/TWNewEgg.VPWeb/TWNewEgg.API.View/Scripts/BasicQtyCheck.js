function QtyCheck(checkObj) {
    var errMsg = "";
    var errflag = false;
    if (checkObj == "all" || checkObj == "CanSaleQty") {
        var CanSaleQty = $("#CanSaleQty").val();
        var InventorySafeQty = $("#InventorySafeQty").val();
        if (CanSaleQty.length == 0) {
            $("#CanSaleQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("CanSaleQty", "必填欄位");
            errflag = true;
            errMsg += "[CanSaleQty]";
        }
        else {
            if (CanSaleQty.search(/^[0-9]+$/) < 0) {
                ErrMsgShow("CanSaleQty", "僅供輸入數量");
                $("#CanSaleQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[CanSaleQty]";
            } else {
                if (CanSaleQty < 0) {
                    ErrMsgShow("CanSaleQty", "不得小於0");
                    $("#CanSaleQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[CanSaleQty]";
                }
                else {
                    if (parseInt(CanSaleQty) < parseInt(InventorySafeQty)) {
                        ErrMsgShow("InventorySafeQty", "安全庫存量不得大於可售數量");
                        $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                        errflag = true;
                        errMsg += "[可售數量檢查]" + "安全: " + InventorySafeQty + ", 可售: " + CanSaleQty;
                    } else {
                        ErrMsgHide("InventorySafeQty", "必填欄位");
                        $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                    }

                    ErrMsgHide("CanSaleQty", "必填欄位");
                    $("#CanSaleQty").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
    }

    if (checkObj == "all" || checkObj == "InventorySafeQty") {
        var CanSaleQty = $("#CanSaleQty").val();
        var InventorySafeQty = $("#InventorySafeQty").val();
        if (CanSaleQty.length == 0) {
            $("#CanSaleQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("CanSaleQty", "必填欄位");
            errflag = true;
            errMsg += "[CanSaleQty]";
        }
        if (InventorySafeQty.length == 0) {
            $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("InventorySafeQty", "必填欄位");
            errflag = true;
            errMsg += "[InventorySafeQty]";
        }
        else {
            if (InventorySafeQty.search(/^[0-9]+$/) < 0) {
                ErrMsgShow("InventorySafeQty", "僅供輸入數量");
                $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[InventorySafeQty]";
            } else {
                if (InventorySafeQty < 0) {
                    ErrMsgShow("InventorySafeQty", "不得小於0");
                    $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[InventorySafeQty]";
                }
                else {
                    if (parseInt(InventorySafeQty) - parseInt(CanSaleQty) > 0) {
                        ErrMsgShow("InventorySafeQty", "安全庫存量不得大於可售數量");
                        $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                        errflag = true;
                        errMsg += "[可售數量檢查]" + "安全: " + InventorySafeQty + ", 可售: " + CanSaleQty;
                    }
                    else {
                        ErrMsgHide("InventorySafeQty", "必填欄位");
                        $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                    }
                }
            }
        }
    }

    return errflag;
}