function twoDimQtyCheck(checkObj) {

    var errflag = false;

    if (checkObj == "all" || checkObj == "InventorySafeQty") {
        if (CheckInventorySafeQty()) {
            errflag = true;
        }
    }

    if (checkObj == "all" || checkObj == "Qty") {
        if (ISQtyTableExpand()) {
            errflag = true;
        }

        if (errflag == false) {
            if (CheckAllQty()) {
                errflag = true;
            }
        }
    }

    return errflag;
}

function CheckInventorySafeQty() {
    var errflag = false;

    var InventorySafeQty = $("#InventorySafeQty").val();

    if (InventorySafeQty.length == 0) {
        $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
        ErrMsgShow("InventorySafeQty", "必填欄位");
        errflag = true;
    }
    else {
        if (InventorySafeQty.search(/^[0-9]+$/) < 0) {
            ErrMsgShow("InventorySafeQty", "僅供輸入數量");
            $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            errflag = true;
        } else {
            if (InventorySafeQty < 0) {
                ErrMsgShow("InventorySafeQty", "不得小於0");
                $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
            }
            else {
                var maxQty = 0;

                var propertyQty = $("input[propertyqty='Qty']").val();
                if (typeof (propertyQty) == "undefined") {
                    errflag = true;
                } else {
                    $("input[propertyqty='Qty']").each(function () {
                        if ($(this).val() > parseInt(maxQty)) {
                            maxQty = $(this).val();
                        }
                    });

                    if (parseInt(InventorySafeQty) - parseInt(maxQty) > 0) {
                        ErrMsgShow("InventorySafeQty", "安全庫存量不得大於可售數量");
                        $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                        errflag = true;
                    } else {
                        ErrMsgHide("InventorySafeQty", "必填欄位");
                        $("#InventorySafeQty").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                    }
                }
            }
        }
    }

    return errflag;
}

// 檢查是否已展開表格
function ISQtyTableExpand() {
    var errflag = false;

    if ($("#propertySeletionUI").text() != "") {

        if ($("#propertyValueSeletionUI").text() != "") {

            if ($("#expandTbody").children().length == 1) {

                errflag = true;

                if ($("#mainPropertySelection").val().length > 0 && parseInt($("#mainPropertySelection").val()) > 0) {
                    $("#mainCheckboxSelect").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
                }

                if ($("#secondPropertySelection").val().length > 0 && parseInt($("#secondPropertySelection").val()) > 0) {
                    $("#secondCheckboxSelect").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
                }
            }
        }
        else {
            errflag = true;

            if ($("#mainPropertySelection").val().length == 0) {
                $("#mainPropertySelection").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            }
        }
    }
    else {
        errflag = true;

        $("#propertySeletionUI").html(
            "<fieldset id=\"setTwoDimensionProduct\" class=\"noticeFieldset\" style=\"border-color:red;\">" +
                "<legend class=\"noticeLegend\">&nbsp;&nbsp;規格商品&nbsp;&nbsp;</legend>" +
                "<span class=\"k-widget k-tooltip k-tooltip-validation k-invalid-msg\" data-for=\"setTwoDimensionProduct\" role=\"alert\" id=\"errsetTwoDimensionProductMsg\" style=\"display:none;\"></span>" +
            "</fieldset>");
        $("#propertySeletionUI").show();

        ErrMsgShow("setTwoDimensionProduct", "必填欄位");
    }

    return errflag;
}

function CheckAllQty() {
    var errflag = false;

    var propertyQty = $("input[propertyqty='Qty']").val();
    if (typeof (propertyQty) == "undefined") {
        errflag = true;
    } else {
        $("input[propertyqty='Qty']").each(function () {
            if (CheckQty($(this).attr("id"), "allQty")) {
                errflag = true;
            }
        });
    }

    return errflag;
}

function CheckQty(qtyID) {

    var errflag = false;

    var qty = $("#" + qtyID).val();
    if (qty.length == 0) {
        $("#" + qtyID).parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
        ErrMsgShow(qtyID, "必填欄位");
        errflag = true;
    } else {
        if (qty.search(/^[0-9]+$/) < 0) {
            $("#" + qtyID).parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow(qtyID, "僅供輸入數量");
            errflag = true;
        }
    }

    if (errflag == false) {
        if ($("#" + qtyID).parents("span.k-widget.k-numerictextbox").hasClass("errMsgShow")) {
            $("#" + qtyID).parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
        }
        ErrMsgHide(qtyID, "");
    }

    return errflag;
}