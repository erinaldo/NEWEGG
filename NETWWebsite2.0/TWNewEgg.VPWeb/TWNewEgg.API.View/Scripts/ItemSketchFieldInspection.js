function getItemVaule(itemSketIDValue) {
    var ActionType = $("#ActionType").val();
    var ID = 0;
    if (itemSketIDValue != null) {
        ID = itemSketIDValue;
    }
    //var ShowOrder = $("#Purchase").prop('checked') ? -3 : $("#OrignShowOrder").val();
    var Discard4 = $("#Discard4").prop('checked') ? "Y" :"N";
    var ShowOrder = $("#Purchase").prop('checked') ? -3 : 0;
    var ItemTempID = $("#ItemTempID").val();
    var ItemID = $("#ItemID").val();
    var ProductTempID = $("#ProductTempID").val();
    var ProductID = $("#ProductID").val();
    var IsNew = $("#IsNew").val();
    var MainCategoryID_Layer2 = $("#MainCategoryID_Layer2").val();
    var SubCategoryID_1_Layer2 = $("#SubCategoryID_1_Layer2").val();
    var SubCategoryID_2_Layer2 = $("#SubCategoryID_2_Layer2").val();
    var ManufactureID = $("#ManufactureID").val();
    var SellerProductID = $("#SellerProductID").val();
    var Model = $("#Model").val();
    var Warranty = $("#Warranty").val();
    var BarCode = $("#BarCode").val();
    var Length = $("#Length").val();
    var Width = $("#Width").val();
    var Height = $("#Height").val();
    var Weight = $("#Weight").val();
    var UPC = $("#UPC").val();
    var DelvDate = $("#DelvDate").val();
    var ItemPackage = $("#ItemPackage").val(); // Retail、OEM
    var IsChokingDanger = $("#IsChokingDanger").val(); // ChokingDanger、n_ChokingDanger
    var Is18 = $("#Is18").val(); // UnderAge、n_UnderAge
    var IsShipDanger = $("#IsShipDanger").val(); // Danger、n_Danger
    var ShipType = $("#ShipType").val(); // ShipWay_Vendor、ShipWay_Newegg
    var MarketPrice = $("#MarketPrice").val();
    var PriceCash = $("#PriceCash").val();
    var Cost = $("#Cost").val();
    var CanSaleLimitQty = $("#CanSaleLimitQty").val();
    var AesItemQtyReg = $("#AseItemQtyReg").val();
    var InventorySafeQty = $("#InventorySafeQty").val();
    var QtyLimit = $("#QtyLimit").val();
    var CanSaleQty = $("#CanSaleQty").val();
    var AesInventoryQtyReg = $("#AseInventoryQtyReg").val();
    var DateStart = $("#DateStart").val();
    var Name = $("#Name").val();
    var Spechead = $("#Spechead").val();
    var sdesc1 = $("#Sdesc1").val().replace(/<li>/g, "").replace(/<\/li>/g, "");
    var sdesc2 = $("#Sdesc2").val().replace(/<li>/g, "").replace(/<\/li>/g, "");
    var sdesc3 = $("#Sdesc3").val().replace(/<li>/g, "").replace(/<\/li>/g, "");
    if (sdesc1.length > 0) {
        sdesc1 = "<li>" + sdesc1 + "</li>";
    }

    if (sdesc2.length > 0) {
        sdesc2 = "<li>" + sdesc2 + "</li>";
    }

    if (sdesc3.length > 0) {
        sdesc3 = "<li>" + sdesc3 + "</li>";
    }

    $("#Sdesc").val(sdesc1 + sdesc2 + sdesc3);
    var Sdesc = $("#Sdesc").val();
    var Description = CKEDITOR.instances["Description"].getData();
    var Note = $("#Note").val();
    var PicPatch_Edit = getImgSrc();
    var SaveProductPropertyList = getPropertyDetail();

    return JSON.stringify({
        "ID": ID,
        "Item": {
            "ID": ItemTempID,
            "ItemID": ItemID,
            "IsNew": IsNew,
            "DelvDate": DelvDate,
            "ItemPackage": ItemPackage,
            "ShipType": ShipType,
            "MarketPrice": MarketPrice,
            "PriceCash": PriceCash,
            "AesItemQtyReg": AesItemQtyReg,
            "QtyLimit": QtyLimit,
            "DateStart": DateStart,
            "Spechead": Spechead,
            "Sdesc": Sdesc,
            "Note": Note,
            "CanSaleLimitQty": CanSaleLimitQty,
            "ShowOrder": ShowOrder,
            "Discard4": Discard4,
        },
        "Product": {
            "ID": ProductTempID,
            "ProductID": ProductID,
            "ManufactureID": ManufactureID,
            "SellerProductID": SellerProductID,
            "Model": Model,
            "Warranty": Warranty,
            "BarCode": BarCode,
            "Length": Length,
            "Width": Width,
            "Height": Height,
            "Weight": Weight,
            "UPC": UPC,
            "IsChokingDanger": IsChokingDanger,
            "Is18": Is18,
            "IsShipDanger": IsShipDanger,
            "Cost": Cost,
            "Name": Name,
            "Description": Description,
            "PicPatch_Edit": PicPatch_Edit,
        },
        "ItemStock": {
            "CanSaleQty": CanSaleQty,
            "AesInventoryQtyReg": AesInventoryQtyReg,
            "InventorySafeQty": InventorySafeQty,
        },
        "ItemCategory": {
            "MainCategoryID_Layer2": MainCategoryID_Layer2,
            "SubCategoryID_1_Layer2": SubCategoryID_1_Layer2,
            "SubCategoryID_2_Layer2": SubCategoryID_2_Layer2,
        },
        "SaveProductPropertyList": SaveProductPropertyList,
        "ActionType": ActionType,
        "AesItemQtyReg": AesItemQtyReg,
        "AesInventoryQtyReg": AesInventoryQtyReg
    });
}

function TextChange(changeID, isOnfocus) {
    if (isOnfocus) {
        $("#" + changeID + "Hide").attr("style", "display:none;");
        $("#" + changeID).parents("span.k-widget.k-numerictextbox").attr("style", "width:110px;");
    }
    else {
        $("#" + changeID + "Hide").attr("style", "width:110px;");
        $("#" + changeID).parents("span.k-widget.k-numerictextbox").attr("style", "display:none;");
    }
}

function TextCopy(sourceID) {
    var sourceValue = $("#" + sourceID).val();
    $("#" + sourceID + "Hide").val(sourceValue);
}

// 取得所有屬性值
function getPropertyDetail() {
    var getPropertyDetailList = [];
    $("#ProductProperty input[name=InputValue]").each(function () {
        var getPropertyID = $(this).attr("getPropertyID");
        var getValueID = $("#" + getPropertyID).val();
        var getGroupID = $(this).attr("getGroupID");
        var getInputValue = $(this).val();
        if (getInputValue.length > 0) {
            getInputValue = getInputValue.trim();
        }

        if ((getValueID != null && getValueID != 0) || getInputValue.length > 0) {
            getPropertyDetailList.push({ "GroupID": getGroupID, "PropertyID": getPropertyID, "ValueID": getValueID, "InputValue": getInputValue });
        }
    });

    return getPropertyDetailList;
}

// 取得Img的url
function getImgSrc() {
    getImgSrcList = [];
    $("#ItemImgTable img").each(function () {
        getSrc = $(this).attr("src");
        if (getSrc.length > 0) {
            getSrc = getSrc.split('?')[0];
            getImgSrcList.push(getSrc);
        }
    });

    return getImgSrcList;
}

// 毛利率計算
function GrossProfitCalculate() {
    var priceCashValue = $("#PriceCash").val();
    var CostValue = $("#Cost").val();
    var CurrencyAverageExchange = $("#CurrencyAverageExchange").val();
    var denominator = priceCashValue;
    if (priceCashValue == 0) {
        denominator = 1;
    }
    var GrossProfitValue = 0;
    if (priceCashValue.length > 0
        && priceCashValue.search(/^[0-9]{0,8}[.]?[0-9]{0,2}$/) >= 0
        && CostValue.length > 0
        && CostValue.search(/^[0-9]{0,8}[.]?[0-9]{0,2}$/) >= 0) {
        GrossProfitValue = (((priceCashValue - (CostValue * CurrencyAverageExchange)) / denominator) * 100).toFixed(2);
        $("#GrossProfit").val(GrossProfitValue + " %");
    }
    else {
        $("#GrossProfit").val("0 %");
    }

    if (parseInt(GrossProfitValue) < 0) {
        $("#GrossProfit").addClass("errMsgShow");
        ErrMsgShow("GrossProfit", "毛利率不可為負數");
        return true;
    }
    else {
        $("#GrossProfit").removeClass("errMsgShow");
        ErrMsgHide("GrossProfit", "毛利率不可為負數");
        return false;
    }
}

function ErrMsgHide(id, msg) {
    $("#" + id).attr("data-valid" + id + "-msg", msg);
    $("#err" + id + "Msg").attr("style", "display:none;");
    $("#err" + id + "Msg").html("<span class='k-icon k-warning'> </span> " + msg);
}

function ErrMsgShow(id, msg) {
    $("#" + id).attr("data-valid" + id + "-msg", msg);
    $("#err" + id + "Msg").html("<span class='k-icon k-warning'> </span> " + msg);
    $("#err" + id + "Msg").show();
}

function DataCheck(checkObj) {
    var errMsg = "";
    var errflag = false;
    if (checkObj == "all" || checkObj == "IsNew") {
        var IsNew = $("#IsNew").val();
        if (IsNew != "Y" && IsNew != "N") {
            $("#IsNew").addClass("errMsgShow");
            errflag = true;
            errMsg += "[IsNew]";
        }
        else {
            $("#IsNew").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Category") {
        var MainCategoryID_Layer2 = $("#MainCategoryID_Layer2").val();
        var SubCategoryID_1_Layer2 = $("#SubCategoryID_1_Layer2").val();
        var SubCategoryID_2_Layer2 = $("#SubCategoryID_2_Layer2").val();
        if ($("#MainCategoryID_Layer0").val().length <= 0 || $("#MainCategoryID_Layer0").val() == 0) {
            $("#MainCategoryID_Layer0").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            errflag = true;
            errMsg += "[MainCategoryID_Layer0]";
        }
        else {
            $("#MainCategoryID_Layer0").parents("span.k-widget.k-dropdown.k-header").removeClass("errMsgShow");
        }
        if ($("#MainCategoryID_Layer1").val().length <= 0 || $("#MainCategoryID_Layer1").val() == 0) {
            $("#MainCategoryID_Layer1").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            errflag = true;
            errMsg += "[MainCategoryID_Layer1]";
        }
        else {
            $("#MainCategoryID_Layer1").parents("span.k-widget.k-dropdown.k-header").removeClass("errMsgShow");
        }
        if (MainCategoryID_Layer2.length <= 0 || MainCategoryID_Layer2 == 0) {
            // 主分類不得為空或為0
            $("#MainCategoryID_Layer2").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            errflag = true;
            errMsg += "[MainCategoryID_Layer2]";
        }
        else {
            $("#MainCategoryID_Layer2").parents("span.k-widget.k-dropdown.k-header").removeClass("errMsgShow");
        }

        if (SubCategoryID_1_Layer2 != 0 && MainCategoryID_Layer2 == SubCategoryID_1_Layer2 && SubCategoryID_1_Layer2.length > 0) {
            // 跨分類子項不得與主類別相同
            $("#SubCategoryID_1_Layer2").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            $("#emptySubCategoryID_1_Layer2Msg").attr("style", "display:none");
            $("#errSubCategoryID_1_Layer2Msg").removeAttr("style");
            errflag = true;
            errMsg += "[SubCategoryID_1_Layer2]";
        }
        else {
            $("#SubCategoryID_1_Layer2").parents("span.k-widget.k-dropdown.k-header").removeClass("errMsgShow");
            $("#errSubCategoryID_1_Layer2Msg").attr("style", "display:none");
            $("#emptySubCategoryID_1_Layer2Msg").attr("style", "display:none");
        }

        if (SubCategoryID_2_Layer2 != 0 && MainCategoryID_Layer2 == SubCategoryID_2_Layer2 && SubCategoryID_2_Layer2.length > 0) {
            // 跨分類子項不得與主類別相同
            $("#SubCategoryID_2_Layer2").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            $("#emptySubCategoryID_2_Layer2Msg").attr("style", "display:none");
            $("#errSubCategoryID_2_Layer2Msg").removeAttr("style");
            errflag = true;
            errMsg += "[SubCategoryID_2_Layer2]";
        }
        else {
            $("#SubCategoryID_2_Layer2").parents("span.k-widget.k-dropdown.k-header").removeClass("errMsgShow");
            $("#errSubCategoryID_2_Layer2Msg").attr("style", "display:none");
            $("#emptySubCategoryID_2_Layer2Msg").attr("style", "display:none");
        }

        if (SubCategoryID_2_Layer2 != 0 && SubCategoryID_1_Layer2.length > 0 && SubCategoryID_2_Layer2.length > 0 && SubCategoryID_1_Layer2 == SubCategoryID_2_Layer2) {
            // 跨分類子項不得與前一跨分類子項相同
            $("#SubCategoryID_2_Layer2").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            errflag = true;
            $("#emptySubCategoryID_2_Layer2Msg").attr("style", "display:none");
            $("#errSubCategoryID_2_Layer2Msg").removeAttr("style");
            errMsg += "[SubCategoryID_1_Layer2 == SubCategoryID_2_Layer2]";
        }
        else {
            if (SubCategoryID_2_Layer2 != MainCategoryID_Layer2) {
                $("#SubCategoryID_2_Layer2").parents("span.k-widget.k-dropdown.k-header").removeClass("errMsgShow");
                $("#errSubCategoryID_2_Layer2Msg").attr("style", "display:none");
                $("#emptySubCategoryID_2_Layer2Msg").attr("style", "display:none");
            }
        }
    }

    if (checkObj == "all" || checkObj == "ManufactureID") {
        var ManufactureID = $("#ManufactureID").val();
        if (ManufactureID.length == 0 || ManufactureID <= 0) {
            $("#ManufactureID").parents("span.k-widget.k-dropdown.k-header").addClass("errMsgShow");
            errflag = true;
            errMsg += "[ManufactureID]";
        }
        else {
            $("#ManufactureID").parents("span.k-widget.k-dropdown.k-header").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "SellerProductID") {
        var SellerProductID = $("#SellerProductID").val();
        if (SellerProductID.length > 0 && SellerProductID.search(/^[^\u4e00-\u9fa5]+$/) < 0) {
            $("#SellerProductID").addClass("errMsgShow");
            ErrMsgShow("SellerProductID", "不可輸入中文字");
            errflag = true;
            errMsg += "[SellerProductID]";
        }
        else {
            ErrMsgHide("SellerProductID", "不可輸入中文字");
            $("#SellerProductID").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Model") {
        var Model = $("#Model").val();
        if (Model.length > 0 && Model.search(/^[^\u4e00-\u9fa5]+$/) < 0) {
            $("#Model").addClass("errMsgShow");
            ErrMsgShow("Model", "不可輸入中文字");
            errflag = true;
            errMsg += "[Model]";
        }
        else {
            ErrMsgHide("Model", "不可輸入中文字");
            $("#Model").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Warranty") {
        var Warranty = $("#Warranty").val();
        if (Warranty.length > 0 && Warranty.search(/^[0-9]+$/) < 0) {
            $("#Warranty").addClass("errMsgShow");
            ErrMsgShow("Warranty", "僅供輸入數字");
            errflag = true;
            errMsg += "[Warranty]";
        }
        else {
            ErrMsgHide("Warranty", "僅供輸入數字");
            $("#Warranty").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "BarCode") {
        var BarCode = $("#BarCode").val();
        if (BarCode.length > 0 && BarCode.search(/^[0-9]+$/) < 0) {
            $("#BarCode").addClass("errMsgShow");
            ErrMsgShow("BarCode", "僅供輸入數字");
            errflag = true;
            errMsg += "[BarCode]";
        }
        else {
            ErrMsgHide("BarCode", "僅供輸入數字");
            $("#BarCode").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Length") {
        var Length = $("#Length").val();
        if (Length.length == 0) {
            $("#Length").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("Length", "必填欄位");
            errflag = true;
            errMsg += "[Length]";
        }
        else {
            if (Length.search(/^[0-9]{0,8}[.]?[0-9]{0,2}$/) < 0) {
                ErrMsgShow("Length", "僅供輸入長度");
                $("#Length").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[Length]";
            } else {
                if (Length <= 0) {
                    ErrMsgShow("Length", "不得小於等於0");
                    $("#Length").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[Length]";
                }
                else {
                    ErrMsgHide("Length", "必填欄位");
                    $("#Length").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
    }

    if (checkObj == "all" || checkObj == "Width") {
        var Width = $("#Width").val();
        if (Width.length == 0) {
            $("#Width").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("Width", "必填欄位");
            errflag = true;
            errMsg += "[Width]";
        }
        else {
            if (Width.search(/^[0-9]{0,8}[.]?[0-9]{0,2}$/) < 0) {
                ErrMsgShow("Width", "僅供輸入寬");
                $("#Width").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[Width]";
            } else {
                if (Width <= 0) {
                    ErrMsgShow("Width", "不得小於等於0");
                    $("#Width").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[Width]";
                }
                else {
                    ErrMsgHide("Width", "必填欄位");
                    $("#Width").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
    }

    if (checkObj == "all" || checkObj == "Height") {
        var Height = $("#Height").val();
        if (Height.length == 0) {
            $("#Height").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("Height", "必填欄位");
            errflag = true;
            errMsg += "[Height]";
        }
        else {
            if (Height.search(/^[0-9]{0,8}[.]?[0-9]{0,2}$/) < 0) {
                ErrMsgShow("Height", "僅供輸入高度");
                $("#Height").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[Height]";
            } else {
                if (Height <= 0) {
                    ErrMsgShow("Height", "不得小於等於0");
                    $("#Height").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[Height]";
                }
                else {
                    ErrMsgHide("Height", "必填欄位");
                    $("#Height").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
    }

    if (checkObj == "all" || checkObj == "Weight") {
        var Weight = $("#Weight").val();
        if (Weight.length == 0) {
            $("#Weight").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("Weight", "必填欄位");
            errflag = true;
            errMsg += "[Weight]";
        }
        else {
            if (Weight.search(/^[0-9]{0,8}[.]?[0-9]{0,4}$/) < 0) {
                ErrMsgShow("Weight", "僅供輸入重量");
                $("#Weight").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[Weight]";
            } else {
                if (Weight <= 0) {
                    ErrMsgShow("Weight", "不得小於等於0");
                    $("#Weight").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[Weight]";
                }
                else {
                    ErrMsgHide("Weight", "必填欄位");
                    $("#Weight").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
    }

    if (checkObj == "all" || checkObj == "UPC") {
        var UPC = $("#UPC").val();
        if (UPC.length > 0 && UPC.search(/^[A-Za-z0-9 _-]+$/) < 0) {
            $("#UPC").addClass("errMsgShow");
            ErrMsgShow("UPC", "僅允許填入數字、英文，特殊符號 ( -­ ， _ ，空格)");
            errflag = true;
            errMsg += "[UPC]";
        }
        else {
            ErrMsgHide("UPC", "僅允許填入數字、英文，特殊符號 ( -­ ， _ ，空格)");
            $("#UPC").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "DelvDate") {
        var DelvDate = $("#DelvDate").val();
        if (DelvDate.length > 0 && DelvDate.search(/^[0-9 ~-]+$/) < 0) {
            $("#DelvDate").addClass("errMsgShow");
            ErrMsgShow("DelvDate", "僅允許填入數字，特殊符號 ( -­ ~ )");
            errflag = true;
            errMsg += "[DelvDate]";
        }
        else {
            ErrMsgHide("DelvDate", "僅允許填入數字，特殊符號 ( -­ ~ )");
            $("#DelvDate").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "ItemPackage") {
        var ItemPackage = $("#ItemPackage").val(); // Retail、OEM
        if (ItemPackage.length == 0 || (ItemPackage != "0" && ItemPackage != "1")) {
            $("#ItemPackage").addClass("errMsgShow");
            errflag = true;
            errMsg += "[ItemPackage]";
        }
        else {
            $("#ItemPackage").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "IsChokingDanger") {
        var IsChokingDanger = $("#IsChokingDanger").val(); // ChokingDanger、n_ChokingDanger
        if (IsChokingDanger.length == 0 || (IsChokingDanger != "Y" && IsChokingDanger != "N")) {
            $("#IsChokingDanger").addClass("errMsgShow");
            errflag = true;
            errMsg += "[IsChokingDanger]";
        }
        else {
            $("#IsChokingDanger").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Is18") {
        var Is18 = $("#Is18").val(); // UnderAge、n_UnderAge
        if (Is18.length == 0 || (Is18 != "Y" && Is18 != "N")) {
            $("#Is18").addClass("errMsgShow");
            errflag = true;
            errMsg += "[Is18]";
        }
        else {
            $("#Is18").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "IsShipDanger") {
        var IsShipDanger = $("#IsShipDanger").val(); // Danger、n_Danger
        if (IsShipDanger.length == 0 || (IsShipDanger != "Y" && IsShipDanger != "N")) {
            $("#IsShipDanger").addClass("errMsgShow");
            errflag = true;
            errMsg += "[IsShipDanger]";
        }
        else {
            $("#IsShipDanger").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "ShipType") {
        var ShipType = $("#ShipType").val(); // ShipWay_Vendor、ShipWay_Newegg
        if (ShipType.length == 0 || (ShipType != "S" && ShipType != "V" && ShipType != "N")) {
            $("#ShipType").addClass("errMsgShow");
            errflag = true;
            errMsg += "[ShipType]";
        }
        else {
            $("#ShipType").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "MarketPrice") {
        var MarketPrice = $("#MarketPrice").val();
        if (MarketPrice.length > 0 && MarketPrice.search(/^[0-9]{0,10}$/) < 0) {
            $("#MarketPrice").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("MarketPrice", "請填入整數金額");
            errflag = true;
            errMsg += "[MarketPrice]";
        }
        else {
            ErrMsgHide("MarketPrice", "請填入整數金額");
            $("#MarketPrice").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
        }
    }
    if (checkObj == "ConfirmPriceCashEqual") {
        var PriceCash = $("#PriceCash").val();
        var ConfirmPrice = $("#ConfirmPrice").val();
        $("#PriceCash").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
        $("#PriceCashHide").removeClass("errMsgShow");
        if (PriceCash != ConfirmPrice) {
            $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("ConfirmPrice", "需等同於售價");
            errflag = true;
            errMsg += "[ConfirmPrice]";
        }
    }
    if (checkObj == "all" || checkObj == "PriceCash") {
        var PriceCash = $("#PriceCash").val();
        var ConfirmPrice = $("#ConfirmPrice").val();
        if (PriceCash.length == 0) {
            $("#PriceCash").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            $("#PriceCashHide").addClass("errMsgShow");
            ErrMsgShow("PriceCash", "必填欄位");
            errflag = true;
            errMsg += "[PriceCash]";
        }
        else {
            if (PriceCash.search(/^[0-9]{0,10}$/) < 0) {
                ErrMsgShow("PriceCash", "僅可填入整數金額");
                $("#PriceCash").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                $("#PriceCashHide").addClass("errMsgShow");
                errflag = true;
                errMsg += "[PriceCash]";
            }
            else {
                ErrMsgHide("PriceCash", "必填欄位");
                $("#PriceCash").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                $("#PriceCashHide").removeClass("errMsgShow");
                if (PriceCash != ConfirmPrice) {
                    $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    ErrMsgShow("ConfirmPrice", "需等同於售價");
                    errflag = true;
                    errMsg += "[ConfirmPrice]";
                }
                else {
                    ErrMsgHide("ConfirmPrice", "必填欄位");
                    $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
            if (PriceCash == "0") {
                ErrMsgShow("PriceCash", "金額不可為0");
                $("#PriceCash").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                $("#PriceCashHide").addClass("errMsgShow");
                errflag = true;
                errMsg += "[PriceCash]";
            }
        }
    }

    if (checkObj == "all" || checkObj == "ConfirmPrice") {
        var PriceCash = $("#PriceCash").val();
        var ConfirmPrice = $("#ConfirmPrice").val();
        if (ConfirmPrice.length == 0) {
            $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("ConfirmPrice", "必填欄位");
            errflag = true;
            errMsg += "[ConfirmPrice]";
        }
        else {
            if (ConfirmPrice.search(/^[0-9]{0,10}$/) < 0) {
                ErrMsgShow("ConfirmPrice", "僅可填入整數金額");
                $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[ConfirmPrice]";
            }
            else {
                ErrMsgHide("ConfirmPrice", "必填欄位");
                $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                if (PriceCash != ConfirmPrice) {
                    $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    ErrMsgShow("ConfirmPrice", "需等同於售價");
                    errflag = true;
                    errMsg += "[ConfirmPrice]";
                }
                else {
                    ErrMsgHide("ConfirmPrice", "必填欄位");
                    $("#ConfirmPrice").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
    }

    if (checkObj == "all" || checkObj == "Cost") {
        var Cost = $("#Cost").val();
        var ConfirmCost = $("#ConfirmCost").val();
        if (Cost.length == 0) {
            $("#Cost").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            $("#CostHide").addClass("errMsgShow");
            ErrMsgShow("Cost", "必填欄位");
            errflag = true;
            errMsg += "[Cost]";
        }
        else {
            ErrMsgHide("Cost", "必填欄位");
            $("#Cost").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
            $("#CostHide").removeClass("errMsgShow");
            if (Cost != ConfirmCost) {
                $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                ErrMsgShow("ConfirmCost", "需等同於成本");
                errflag = true;
                errMsg += "[ConfirmCost]";
            }
            else {
                ErrMsgHide("ConfirmCost", "必填欄位");
                $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
            }
            if (Cost.toString() == "0") {
                ErrMsgShow("Cost", "金額不可為0");
                $("#Cost").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                $("#CostHide").addClass("errMsgShow");
                errflag = true;
                errMsg += "[Cost]";
            }
        }
    }
    if (checkObj == "ConfirmCostCashEqual") {
        var Cost = $("#Cost").val();
        var ConfirmCost = $("#ConfirmCost").val();
        $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
        if (Cost != ConfirmCost) {
            $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("ConfirmCost", "需等同於成本");
            errflag = true;
            errMsg += "[ConfirmCost]";
        }
    }
    if (checkObj == "all" || checkObj == "ConfirmCost") {
        var Cost = $("#Cost").val();
        var ConfirmCost = $("#ConfirmCost").val();
        if (ConfirmCost.length == 0) {
            $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
            ErrMsgShow("ConfirmCost", "必填欄位");
            errflag = true;
            errMsg += "[ConfirmCost]";
        }
        else {
            ErrMsgHide("ConfirmCost", "必填欄位");
            $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
            if (Cost != ConfirmCost) {
                $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                ErrMsgShow("ConfirmCost", "需等同於成本");
                errflag = true;
                errMsg += "[ConfirmCost]";
            }
            else {
                ErrMsgHide("ConfirmCost", "必填欄位");
                $("#ConfirmCost").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
            }
        }
    }

    if (checkObj == "all" || checkObj == "QtyLimit") {
        var QtyLimit = $("#QtyLimit").val();
        if (QtyLimit.length > 0) {
            if (QtyLimit.search(/^[0-9]+$/) < 0) {
                ErrMsgShow("QtyLimit", "僅供輸入數量");
                $("#QtyLimit").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[QtyLimit]";
            } else {
                if (QtyLimit < 0) {
                    ErrMsgShow("QtyLimit", "不得小於0");
                    $("#QtyLimit").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[QtyLimit]";
                }
                else {
                    ErrMsgHide("QtyLimit", "僅供輸入數量");
                    $("#QtyLimit").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
        else {
            ErrMsgHide("QtyLimit", "僅供輸入數量");
            $("#QtyLimit").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "CanSaleLimitQty") {
        var CanSaleLimitQty = $("#CanSaleLimitQty").val();
        if (CanSaleLimitQty.length > 0) {
            if (CanSaleLimitQty.search(/^[0-9]+$/) < 0) {
                ErrMsgShow("CanSaleLimitQty", "僅供輸入數量");
                $("#CanSaleLimitQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                errflag = true;
                errMsg += "[CanSaleLimitQty]";
            } else {
                if (CanSaleLimitQty < 0) {
                    ErrMsgShow("CanSaleLimitQty", "不得小於0");
                    $("#CanSaleLimitQty").parents("span.k-widget.k-numerictextbox").addClass("errMsgShow");
                    errflag = true;
                    errMsg += "[CanSaleLimitQty]";
                }
                else {
                    ErrMsgHide("CanSaleLimitQty", "僅供輸入數量");
                    $("#CanSaleLimitQty").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
                }
            }
        }
        else {
            ErrMsgHide("CanSaleLimitQty", "僅供輸入數量");
            $("#CanSaleLimitQty").parents("span.k-widget.k-numerictextbox").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "DateStart") {
        var DateStart = $("#DateStart").val();
        if (DateStart.length == 0) {
            ErrMsgShow("DateStart", "不得為空值");
            $("#DateStart").parents("span.k-widget.k-datepicker.k-header").addClass("errMsgShow");
            errflag = true;
            errMsg += "[DateStart]";
        }
        else {
            ErrMsgHide("DateStart", "");
            $("#DateStart").parents("span.k-widget.k-datepicker.k-header").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Name") {
        var Name = $("#Name").val();
        if (Name.length == 0) {
            ErrMsgShow("Name", "必填欄位");
            $("#Name").addClass("errMsgShow");
            errflag = true;
            errMsg += "[Name]";
        }
        else {
            ErrMsgHide("Name", "");
            $("#Name").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Spechead") {
        var Spechead = $("#Spechead").val();
        if (Spechead.length == 0) {
            ErrMsgShow("Spechead", "必填欄位");
            $("#Spechead").addClass("errMsgShow");
            errflag = true;
            errMsg += "[Spechead]";
        }
        else {
            ErrMsgHide("Spechead", "");
            $("#Spechead").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Sdesc") {
        var sdesc1 = $("#Sdesc1").val().replace(/<li>/g, "").replace(/<\/li>/g, "");
        var sdesc2 = $("#Sdesc2").val().replace(/<li>/g, "").replace(/<\/li>/g, "");
        var sdesc3 = $("#Sdesc3").val().replace(/<li>/g, "").replace(/<\/li>/g, "");
        if (sdesc1.length > 0) {
            sdesc1 = "<li>" + sdesc1 + "</li>";
        }

        if (sdesc2.length > 0) {
            sdesc2 = "<li>" + sdesc2 + "</li>";
        }

        if (sdesc3.length > 0) {
            sdesc3 = "<li>" + sdesc3 + "</li>";
        }

        $("#Sdesc").val(sdesc1 + sdesc2 + sdesc3);
        var Sdesc = $("#Sdesc").val();
        if (Sdesc.length == 0) {
            ErrMsgShow("Sdesc", "必填欄位");
            var Sdesc1 = $("#Sdesc1").val();
            if (Sdesc1.length == 0) {
                $("#Sdesc1").addClass("errMsgShow");
                errflag = true;
                errMsg += "[Sdesc]";
            }
            else {
                ErrMsgHide("Sdesc", "");
                $("#Sdesc1").removeClass("errMsgShow");
            }
        }
        else {
            ErrMsgHide("Sdesc", "");
            $("#Sdesc1").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "Description") {
        var Description = CKEDITOR.instances["Description"].getData();
        if (Description.length == 0) {
            ErrMsgShow("Description", "必填欄位");
            $("#Description").parents("div.editor").children("span.cke_skin_kama.cke_1.cke_editor_Description").children("span.cke_browser_webkit").addClass("errMsgShow");
            errflag = true;
            errMsg += "[Description]";
        }
        else {
            ErrMsgHide("Description", "");
            $("#Description").parents("div.editor").children("span.cke_skin_kama.cke_1.cke_editor_Description").children("span.cke_browser_webkit").removeClass("errMsgShow");
        }
    }

    if (checkObj == "all" || checkObj == "DescriptionScriptCheck") {
        var Description = CKEDITOR.instances["Description"].getData();
        if (Description.indexOf("<script") >= 0 || Description.indexOf("</script>") >= 0) {
            errflag = true;
            ErrMsgShow("Description", "商品中文說明不可使用 Script 語法");
            $("#Description").parents("div.editor").children("span.cke_skin_kama.cke_1.cke_editor_Description").children("span.cke_browser_webkit").addClass("errMsgShow");
            errflag = true;
            errMsg += "[Description] Get Script";
        } else {
            ErrMsgHide("Description", "");
            $("#Description").parents("div.editor").children("span.cke_skin_kama.cke_1.cke_editor_Description").children("span.cke_browser_webkit").removeClass("errMsgShow");
        }
    }

    return errflag;
}

// 字數計算
function wordCount(id, length) {
    var numChar = 0;
    var temp_str = "";
    var err = 0;
    var str = $("#" + id).val();
    if (err) $("#Name").val(temp_str);
    iCount = length - str.length;
    if (iCount < 0) iCount = 0;
    $("#Count" + id).html('<b>' + iCount + ' (字)剩餘.</b>');
}