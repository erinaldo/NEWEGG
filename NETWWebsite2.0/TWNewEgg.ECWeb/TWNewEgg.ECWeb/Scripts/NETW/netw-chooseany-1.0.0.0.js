
twNewegg.ChooseAny = {
    CookieName: 'twcl',
    ShoppintCarName: 'sc',
    Options: {
        'expires': 7,
        'path': '/',
        'domain': twNewegg().twNeweggDomain
    },
    ItemID: 'iid',
    ItemQty: 'qty',
    ItemStatus: 'stu',
    CategoryID: 'cid',
    CategoryType: 'cty',
    CouponID: 'cpd',
    AmountShortage: 0,
    LimitAmount: function () {
        return parseInt($('#hidLimitAmount').val());
    },
    ListIndex: function () {
        return parseInt($('#hidListIndex').val());
    },
    AddCommas: function addCommas(x) {
        var parts = x.toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    },
    HasCookie: function (cookieName) {
        return !($.cookie(cookieName) === undefined);
    },
    CreateCookie: function () {
        $.cookie(this.CookieName, JSON.stringify([]), this.Options);
    },
    GetIndex: function (arrayObj, key, value) {
        if (arrayObj != null && arrayObj.length > 0) {
            return arrayObj.map(function (data) { return data[key]; }).indexOf(value);
        }

        return -1;
    },
    ReadShoppingCartList: function () {
        twNewegg().cart().cartMethod("readFromCart", { categoryIds: [], categoryTypes: [], successMethods: [], successParas: [[]] });
        if (this.HasCookie(this.ShoppintCarName)) {
            return JSON.parse($.cookie(this.ShoppintCarName));
        }

        return [];
    },
    WriteShoppingCartList: function (cartList) {
        $.cookie(this.ShoppintCarName, JSON.stringify(cartList), this.Options);
    },
    ReadAllList: function () {
        if (this.HasCookie(this.CookieName)) {
            return JSON.parse($.cookie(this.CookieName));
        }
        else {
            this.CreateCookie();
        }

        return JSON.parse($.cookie(this.CookieName));
    },
    WriteAllList: function (allList) {
        $.cookie(this.CookieName, JSON.stringify(allList), this.Options);
    },
    HasItem: function (itemID) {
        var hasItem = false;
        var allList = this.ReadAllList();
        if (allList != undefined && allList != null && allList.length > 0) {
            hasItem = this.GetIndex(allList, this.ItemID, itemID) > -1 ? true : false;
        }

        return hasItem;
    },
    GetItem: function (itemID) {
        if (this.HasItem(itemID)) {
            var allList = this.ReadAllList();
            return allList[this.GetIndex(allList, this.ItemID, itemID)];
        }

        return null;
    },
    UpdateItem: function (itemID, qty) {
        var itemObj = this.GetItem(itemID);
        if (itemObj != undefined && itemObj != null) {
            var allList = this.ReadAllList();
            if (this.IsQtyOverLimit(itemID, qty)) {
                allList[this.GetIndex(allList, this.ItemID, itemID)].qty = parseInt(this.GetMaxQtyLimit(itemID));
            }
            else {
                allList[this.GetIndex(allList, this.ItemID, itemID)].qty = qty;
            }
            this.WriteAllList(allList);
        }
    },
    UpdateItemCell: function (itemID, qty) {
        var targetCell = $('div.chooseArea ul li[data-choose-item-id=' + itemID + ']');
        if (targetCell != undefined && targetCell != null && targetCell.length == 1) {
            targetCell.find('span.buyNum span.num').text(' ' + this.ReadAllList()[this.GetIndex(this.ReadAllList(), this.ItemID, itemID)].qty);
        }
    },
    IsQtyOverLimit: function (itemID, qty) {
        var isOverLimit = false;
        var itemObj = this.GetItem(itemID);
        var maxQty = $('input[data-choose-item-id=' + itemID + ']').parent().parent().find('div.numTxt option:last').val();
        if (maxQty != undefined && maxQty != null && maxQty > 0 && itemObj != undefined && itemObj != null && qty > maxQty) {
            isOverLimit = true;
        }

        return isOverLimit;
    },
    GetMaxQtyLimit: function (itemID) {
        var maxQty = $('input[data-choose-item-id=' + itemID + ']').parent().parent().find('div.numTxt option:last').val();
        if (maxQty != undefined && maxQty != null && maxQty > 0) {
            return maxQty;
        }

        return null;
    },
    LoadLightBoxUnderLay: function () {
        var h = $(window).height();
        var w = $(window).width();
        $LightBoxMsg = $("div.LightBoxMsg.DelayWindow");
        $underLay = $('.LightBoxUnderLay');
        var MsgTop = h / 2 - $LightBoxMsg.outerHeight() / 2;
        var MsgLeft = w / 2 - $LightBoxMsg.outerWidth() / 2;

        $LightBoxMsg.css({ top: MsgTop, left: MsgLeft });
        $("div.LightBoxUnderLay").slideDown(888).fadeIn();
        $("div.LightBoxMsg.DelayWindow").fadeIn();
    },
    AddItem: function (listIndex, itemID, qty, unitPrice) {
        if (this.HasItem(itemID)) {
            var itemObj = this.GetItem(itemID);
            this.UpdateItem(itemID, qty);
            this.UpdateItemCell(itemID);
            twNewegg().checkState("addToChooseAnyPage", "已放入任選館");
        }
        else {
            var allList = this.ReadAllList();
            if (allList == undefined || allList == null || !allList.length) {
                allList = [];
            }

            var currentItemCount = 0;
            $.grep(allList, function (item) {
                if (item.cid == listIndex) {
                    currentItemCount++;
                }
            });

            if (allList.length >= 40) {
                twNewegg().checkState("tips", "購物車已滿");
                return false;
            }

            if (currentItemCount < 10) {
                allList.push({ 'iid': itemID, 'qty': qty, 'stu': 0, 'cid': listIndex, 'cty': 2 });
                this.WriteAllList(allList);
                this.AddItemCell(itemID);
                twNewegg().checkState("addToChooseAnyPage", "已放入任選館");
            }
            else {
                twNewegg().checkState("tips", "此任選館已滿");
                return false;
            }
        }

        this.UpdateChooseArea();
    },
    AddItemFromItemIndex: function (listIndex, itemID, qty, unitPrice) {
        if (this.HasItem(itemID)) {
            var itemObj = this.GetItem(itemID);
            this.UpdateItem(itemID, qty);
            this.UpdateItemCell(itemID);
            twNewegg().checkState("addToChooseAnyPage", "");
            return true;
        }
        else {
            var allList = this.ReadAllList();
            if (allList == undefined || allList == null || !allList.length) {
                allList = [];
            }

            var currentItemCount = 0;
            $.grep(allList, function (item) {
                if (item.cid == listIndex) {
                    currentItemCount++;
                }
            });

            if (allList.length >= 40) {
                twNewegg().checkState("tips", "購物車已滿");
                return false;
            }

            if (currentItemCount < 10) {
                allList.push({ 'iid': itemID, 'qty': qty, 'stu': 0, 'cid': listIndex, 'cty': 2 });
                this.WriteAllList(allList);
                twNewegg().checkState("addToChooseAnyPage", "已放入任選館");
                return true;
            }
            else {
                twNewegg().checkState("tips", "此任選館已滿");
                return false;
            }
        }
    },
    AddItemCell: function (itemID) {
        var selectButton = $('div.productListArea input[data-choose-item-id=' + parseInt(itemID) + ']');
        selectButton.addClass('btn-selected');
        var itemSource = selectButton.parent()/*div.status*/.parent()/*div.chooseBar*/.parent()/*div.box.choose*/;
        var itemCount = 0;
        var allList = this.ReadAllList();
        if (itemSource != undefined && itemSource != null && itemSource.length == 1) {
            itemSource.addClass('choose');
            var unitPrice = parseInt(selectButton.attr('data-choose-unit-price'));
            var imageObj = itemSource.find('div.pic img');
            var imageUrl = imageObj.attr('src');
            var titleText = imageObj.attr('alt');
            var pickQty = itemSource.find('div.numTxt .select_sm').val();
        }

        var newCell = $('<li class="box" data-choose-item-id="' + itemID + '" data-choose-item-unit-price="' + unitPrice + '">' +
                        '<div class="pic">' +
                        '<a href="/item?itemID=' + itemID + '">' +
                        '<img src="' + imageUrl + '" alt=" ' + titleText + ' ">' +
                        '</a>' +
                        '</div>' +
                        '<div class="txt">' +
                        '<h3> ' + titleText + ' </h3>' +
                        '<span class="price">$ ' + this.AddCommas(unitPrice) + ' </span>' +
                        '<span class="buyNum">x<span class="num"> ' + pickQty + ' </span>件</span>' +
                        '<input type="button" class="icon icon-delete" onclick="twNewegg.ChooseAny.DeleteItem(this);">' +
                        '</div>' +
                        '</li>');
        $("#subPageContainer").trigger("insertItem", [newCell, 0, false, 0]);
        for (var i = 0; allList && i < allList.length; i++) {
            if (allList[i].cid == this.ListIndex) {
                itemCount++;
            }
        }
        if (itemCount == 1) {
            setTimeout(function () {
                var wrapper = $('div.caroufredsel_wrapper');
                wrapper.css('height', wrapper.parent().height());
                wrapper.css('width', wrapper.parent().width());
            }, 200);
        }
    },
    RemoveItem: function (itemID) {
        if (this.HasItem(itemID)) {
            var allList = this.ReadAllList();
            allList.splice(this.GetIndex(allList, this.ItemID, itemID), 1);
            this.WriteAllList(allList);
            this.RemoveItemCell(itemID);
            this.UpdateChooseArea();
            /*刪除一個Item, 要去檢查SC, Item在SC出現, 把所有Item刪除*/
            var cartList = this.ReadShoppingCartList();
            if (cartList && cartList.length > 0) {
                var item = cartList[this.GetIndex(cartList, this.ItemID, itemID)];

                //if (this.AmountShortage > 0 && item) {
                //    var removeList = [];
                //    $.grep(cartList, function (item) {
                //        if (item.cid == twNewegg.ChooseAny.ListIndex && item.stu == 0) {
                //            removeList.push(item.iid);
                //        }
                //    });

                //    twNewegg().cart().cartMethod("removeFromCart", { itemIds: removeList });
                //}
                //else {
                //    if (item && item.stu == 0) {
                //        twNewegg().cart().cartMethod("removeFromCart", { itemIds: [itemID] });
                //    }
                //}

                if (item && item.stu == 0) {
                    twNewegg().cart().cartMethod("removeFromCart", { itemIds: [itemID], successMethods: [this.RefreshAfterRemove] });
                }
            }
        }
    },
    RefreshAfterRemove: function () {
        twNewegg().cart().cartMethod("readFromCart", { categoryIds: [], categoryTypes: [], successMethods: [countCartNumber], successParas: [[]] });
    },
    RemoveItemCell: function (itemID) {
        var index = $('div.chooseBox li').index($('div.chooseBox li[data-choose-item-id=' + itemID + ']'));
        $("#subPageContainer").trigger("removeItem", [index, false, 0]);

        var selectBtn = $('div.status input[data-choose-item-id=' + itemID + ']');
        if (selectBtn) {
            var qtyObj = selectBtn.parent().parent().find('.numTxt select');
            $(qtyObj).val(1);
            selectBtn.parent().parent().parent().removeClass('choose');
            selectBtn.removeClass('btn-selected');
        }
    },
    RemoveItemFromCart: function(itemGroup){
        if (!itemGroup && !jQuery.isArray(itemGroup)) {
            twNewegg().checkState("tips", "無此商品");
        } else {
            var allList = twNewegg.ChooseAny.ReadAllList();
            for (i = 0; i < itemGroup.length; i++) {
                allList.splice(twNewegg.ChooseAny.GetIndex(allList, twNewegg.ChooseAny.ItemID, itemGroup[i]), 1);
            }
            twNewegg.ChooseAny.WriteAllList(allList);
        }
    },
    BindDeleteEvent: function () {
        $('#subPageContainer ul  li input.icon-delete').click(function () {
            var itemID = parseInt($(this).parent().parent().attr('data-choose-item-id'));
            twNewegg.ChooseAny.RemoveItem(itemID);
        });
    },
    Openwin:function(path,title){
        var iWidth = $(window).innerWidth() * .75;  //視窗的寬度;
        var iHeight = $(window).innerHeight() * .75; //視窗的高度;
        var iTop = (window.screen.availHeight - 30 - iHeight) / 2;  //視窗的垂直位置;
        var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;   //視窗的水平位置;
        var win = window.open(path, title,
            'height=' + iHeight +
            ',innerHeight=' + iHeight +
            ',width=' + iWidth +
            ',innerWidth=' + iWidth +
            ',top=' + iTop +
            ',left' + iLeft +
            ',location=yes,status=yes,menubar=no,toolbar=no,resizable=yes,scrollbars=yes');
        win.moveTo(iLeft, iTop);
        win.focus();
    },
    DeleteItem: function (obj) {
        var itemID = parseInt($(obj).parent().parent().attr('data-choose-item-id'));
        twNewegg.ChooseAny.RemoveItem(itemID);
    },
    ReloadChooseList: function () {
        $("#subPageContainer").carouFredSel({
            items: 5,
            auto: false,
            prev: "#prev",
            next: "#next",
            scroll: {
                items: 5,
                duration: 800
            }
        });
    },
    CountTotlaAmount: function () {
        var totalAmount = 0;
        var allList = this.ReadAllList();
        if (allList != undefined && allList != null && allList.length > 0) {
            var listIndex = this.ListIndex;
            $.grep(allList, function (item) {
                if (item.cid === listIndex) {
                    var pickQty = item.qty;
                    var unitPrice = $('div.chooseBox li[data-choose-item-id=' + item.iid + ']').attr('data-choose-item-unit-price');
                    totalAmount += pickQty * unitPrice;
                }
            });
        }
        else {
            this.AmountShortage = this.LimitAmount;
        }
        this.AmountShortage = this.LimitAmount < totalAmount ? 0 : this.LimitAmount - totalAmount;
        return this.AddCommas(parseInt(totalAmount));
    },
    CountTotlaQty: function () {
        var totalQty = 0;
        var allList = this.ReadAllList();
        if (allList != undefined && allList != null && allList.length > 0) {
            var listIndex = this.ListIndex;
            $.grep(allList, function (item) {
                if (item.cid === listIndex) {
                    var pickQty = item.qty;
                    totalQty += pickQty;
                }
            });
        }

        return parseInt(totalQty);
    },
    EnableAddToCart: function () {
        $('div.chooseArea > div.titBar > div.buyBtn > input.btn').removeClass('gray');
        $('div.chooseArea div.tit span.shortText').hide();
        $('div.chooseArea div.tit span.rightText').show();
    },
    DisableAddToCart: function () {
        $('div.chooseArea > div.titBar > div.buyBtn > input.btn').addClass('gray');
        $('div.chooseArea div.tit span.shortText').show();
        $('div.chooseArea div.tit span.rightText').hide();
    },
    UpdateChooseQty: function () {
        var totalQty = this.CountTotlaQty();
        $('div.chooseArea > div.titBar > div.tit > span.red.num').text(totalQty);
        if (totalQty == 0) {
            $('div.chooseArea div.chooseBox:eq(0)').hide();
            $('div.chooseArea div.chooseBox:eq(1)').show();
        }
        else {
            $('div.chooseArea div.chooseBox:eq(0)').show();
            $('div.chooseArea div.chooseBox:eq(1)').hide();
        }
    },
    UpdateTotalAmount: function () {
        $('div.tit span.red.totalPrice').text(this.CountTotlaAmount());
    },
    UpdateAmountShortage: function () {
        ////shortPrice
        $('div.tit span.shortPrice').text(this.AddCommas(this.AmountShortage));
        if (parseInt(this.AmountShortage) == 0) {
            this.EnableAddToCart();
        }
        else {
            this.DisableAddToCart();
        }
    },
    UpdateChooseArea: function () {
        this.UpdateTotalAmount();
        this.UpdateChooseQty();
        this.UpdateAmountShortage();
    },
    LoadChooseArea: function () {
        var subIndex = $('div.chooseBox ul:visible').find('li').length > 0 ? $('div.chooseBox ul:visible').attr('data-choose-sub-page-id') : 0;
        if (subIndex == undefined) {
            subIndex = 0;
        }
        $(".chooseArea").css('background-color', '#000000');
        $(".chooseArea").load('@(Html.Raw(Url.Action("GetChooseArea", new { CategoryID = ViewCategoryID, LimitAmount = Model.LimitAmount })))&SubPageIndex=' + subIndex, function (responseText, textStatus, req) {
            if (textStatus == "error") {
                console.log("load choose area error");
            }
            $(".chooseArea").css('background-color', '');
        });
    },
    LoadChooseList: function (listIndex, pageIndex, filters, sortValue) {
        $('div.productListArea').load('/category/getchooselistarea?CategoryID=' + parseInt(listIndex) + '&pageIndex=' + parseInt(pageIndex) + '&filters=' + filters + '&sortValue=' + sortValue, function () {
            $(".LightBoxUnderLay").slideDown(200).fadeOut();
            $(".LightBoxMsg").fadeOut();
            jQuery("html,body").animate({
                scrollTop: $('div.subCategory').offset().top - 10
            }, 250);
            sectionImgVerticalAlignCenter($(this));
            twNewegg.History.pushState({ "CategoryID": listIndex, "PVID": filters, "Page": pageIndex, "OrderBy": sortValue, "historyType": "ChooseAnyItemCategory" }, "");
        });
    },
    LoadChooseListHistory: function (listIndex, pageIndex, filters, sortValue) {
        $('div.productListArea').load('/category/getchooselistarea?CategoryID=' + parseInt(listIndex) + '&pageIndex=' + parseInt(pageIndex) + '&filters=' + filters + '&sortValue=' + sortValue, function () {
            $(".LightBoxUnderLay").slideDown(200).fadeOut();
            $(".LightBoxMsg").fadeOut();
            jQuery("html,body").animate({
                scrollTop: $('div.subCategory').offset().top - 10
            }, 250);
            sectionImgVerticalAlignCenter($(this));
        });
    },
    GetCheckecAttr: function () {
        var result = "";
        var checkedFilter = $('.aside2 div.property input:checked');
        if (checkedFilter != undefined && checkedFilter != null && checkedFilter.length > 0) {
            checkedFilter.each(function () {
                result += $(this).val() + ",";
            });
        }

        return result;
    },
    SortChooseList: function (obj) {
        var listIndex = twNewegg.ChooseAny.ListIndex;
        var pageIndex = 1;
        var filters = this.GetCheckecAttr();
        var sortValue = "";
        if ($(obj).attr('type')) {
            sortValue = $(obj).attr('type');
            $(obj).parent().find('div.orderList').removeClass('active');
            $(obj).addClass('active');
        }

        this.LoadChooseList(listIndex, pageIndex, filters, sortValue);
    },
    FilterChooseList: function () {
        var listIndex = twNewegg.ChooseAny.ListIndex;
        var pageIndex = 1;
        var filters = this.GetCheckecAttr();
        var sortValue = "";
        if ($('div.categoryOrder div.active').attr('type')) {
            sortValue = $('div.categoryOrder div.active').attr('type');
        }

        this.LoadChooseList(listIndex, pageIndex, filters, sortValue);
    },
    PageChooseList: function (obj) {
        var listIndex = twNewegg.ChooseAny.ListIndex;
        var pageIndex = 0;
        if (obj != undefined && obj != null) {
            pageIndex = $(obj).attr('data-choose-page-index');
        }
        var filters = this.GetCheckecAttr();
        var sortValue = "";
        if ($('div.categoryOrder div.active').attr('type')) {
            sortValue = $('div.categoryOrder div.active').attr('type');
        }
        this.LoadChooseList(listIndex, pageIndex, filters, sortValue);
    },
    BreadListChange: function (obj) {
        var isOptionStore = false;
        var categoryID = -1;
        if (obj != undefined && obj != null) {
            isOptionStore = $(obj).children('option:selected').attr('data-is-choose-store');
            categoryID = $(obj).children('option:selected').val();
        }

        if (categoryID > -1) {
            if (isOptionStore) {
                window.location.href = "/Category/ChooseAny?CategoryID=" + categoryID;
            }
            else {
                window.location.href = "/Category?CategoryID=" + categoryID;
            }
        }
    },
    AddToCart: function () {
        this.CountTotlaAmount();
        if (this.AmountShortage > 0) {
            return;
        }
        twNewegg().cart().cartMethod("readFromCart", { categoryIds: [], categoryTypes: [], successMethods: [], successParas: [[]] });
        var scCookie = [];
        if ($.cookie('sc') != undefined && $.cookie('sc') != null) {
            scCookie = JSON.parse($.cookie('sc'));
        }

        var chCookie = this.ReadAllList();
        var existList = [];
        var qtyList = [];
        var cidList = [];
        var ctyList = [];

        if (scCookie.length > 0) {
            /*找出已存在的清單, 並更新*/
            existList = [];
            $.grep(chCookie, function (item) {
                var scIndex = twNewegg.ChooseAny.GetIndex(scCookie, 'iid', item.iid);
                if (scIndex > -1 && scCookie[scIndex].cid == twNewegg.ChooseAny.ListIndex) {
                    existList.push(item.iid);
                    qtyList.push(item.qty >= 10 ? 10 : item.qty);
                    cidList.push(item.cid);
                }
                else if (item != undefined && item != null && item.cid == twNewegg.ChooseAny.ListIndex && item.stu == 0) {
                    existList.push(item.iid);
                    qtyList.push(item.qty >= 10 ? 10 : item.qty);
                    cidList.push(twNewegg.ChooseAny.ListIndex);
                    ctyList.push(2);
                }
            });
            $.grep(existList, function (item) {
                if (item != undefined && item != null) {
                    chCookie.splice(twNewegg.ChooseAny.GetIndex(chCookie, 'iid', item), 1);
                }
            });
            twNewegg().cart().cartMethod("addToCart", { itemIds: existList, qtys: qtyList, categoryIds: cidList, categoryTypes: ctyList, successMethods: [this.JumpToShoppingCart] });
        }
        else {
            /*找出不存在的清單, 並新增*/
            existList = [];
            qtyList = [];
            cidList = [];
            ctyList = [];
            $.grep(chCookie, function (item) {
                if (item != undefined && item != null && item.cid == twNewegg.ChooseAny.ListIndex && item.stu == 0) {
                    existList.push(item.iid);
                    qtyList.push(item.qty >= 10 ? 10 : item.qty);
                    cidList.push(twNewegg.ChooseAny.ListIndex);
                    ctyList.push(2);
                }
            });
            $.grep(existList, function (item) {
                if (item != undefined && item != null) {
                    chCookie.splice(twNewegg.ChooseAny.GetIndex(chCookie, 'iid', item.iid), 1);
                }
            });
            this.WriteAllList(chCookie);
            twNewegg().cart().cartMethod("addToCart", { itemIds: existList, qtys: qtyList, categoryIds: cidList, categoryTypes: ctyList, successMethods: [this.JumpToShoppingCart] });
        }
    },
    JumpToShoppingCart: function () {
        twNewegg.ChooseAny.LoadLightBoxUnderLay();
        window.location.href = "/Cart?typeid=3";
    }
};

$(document).ready(function () {
    /*面包屑下拉選單事件註冊*/
    $('div.pathLink select').change(function () {
        twNewegg.ChooseAny.BreadListChange(this);
    });
    /*商品清單排序事件註冊*/
    $('div.subCategory div.filter a').click(function () {
        $(this).parent().parent().find('a').css('color', '');
        $(this).css('color', 'orange');
        twNewegg.ChooseAny.LoadLightBoxUnderLay();
        twNewegg.ChooseAny.SortChooseList(this);
    });
    /*商品屬性選取事件註冊*/
    $('.aside2 input').click(function () {
        twNewegg.ChooseAny.LoadLightBoxUnderLay();
        twNewegg.ChooseAny.FilterChooseList();
    });
});

/*屬性選單資料載入*/
function DisplayNoneOrNot(IDName) {
    if (IDName == "property_head_All") {
        if ($("#property_head_All").hasClass("close")) {
            $("#property_head_All").removeClass("close").addClass("open");
            $("#property_head_All").html("-");
            $(".filterList .list").each(function (e) {
                $(this).show();
            }
            );
            $(".filterList .showArrow").each(function (e) {
                $(this).removeClass("down").addClass("top");
            }
        );
        }
        else {
            $("#property_head_All").removeClass("open").addClass("close");
            $("#property_head_All").html("+");
            $(".filterList .list").each(function (e) {
                $(this).hide();
            });
            $(".filterList .showArrow").each(function (e) {
                $(this).removeClass("top").addClass("down");
            });
        }
    }
    else {
        $("#property_head_All").removeClass("open").addClass("close");
        $("#property_head_All").html("+");
        //$(".filterList .list").each(function (e) {
        //    if ($(this).attr('id') != IDName) {
        //        $(this).hide();
        //    }
        //});

        if ($("#" + IDName + "_li").css("display") == "none") {
            $("#" + IDName + "_top").removeClass("down").addClass("top");
            $("#" + IDName + "_li").show();
        }
        else {
            $("#" + IDName + "_top").removeClass("top").addClass("down");
            $("#" + IDName + "_li").hide();
        }
    }
}

function InitializationScript() {
    if ($(".filterList .showArrow")[0] != "undefined") {
        var $elem = $(".filterList .showArrow")[0];
        $($elem).removeClass("down").addClass("top");
        $($(".list.PropertyGroupList")[0]).show();
    }
}

function ChooseAnyHistory() {
    var preState = twNewegg.History.getState();
    if (preState != null && preState.historyType == "ChooseAnyItemCategory") {
        ChooseAnyViewState(preState);
        twNewegg.ChooseAny.LoadChooseList(preState.CategoryID, preState.Page, preState.PVID, preState.OrderBy);
    } else {
    }
}

function ChooseAnyViewState(preState) {
    if (preState.OrderBy) {
        var SortType = "div[type='" + preState.OrderBy + "']";
        $(SortType).parent().find('div.orderList').removeClass('active');
        $(SortType).addClass('active');
    }
}
var objHtml5History = function (event) {
    var preState = twNewegg.History.getState();
    if (preState != null && preState.historyType == "ChooseAnyItemCategory") {
        ChooseAnyViewState(preState);
        twNewegg.ChooseAny.LoadChooseListHistory(preState.CategoryID, preState.Page, preState.PVID, preState.OrderBy);
    } else {
        //mobile device will reload every time.
        //window.location.reload();
    }
};
var objNonHtml5History = function (event) {  
    // 利用 Hash 判斷是否上下頁
    if (window.location.hash == ("#" + twNewegg.History.getState().hash)) {
        var preState = twNewegg.History.getState().data;
        if (preState != null && preState.historyType == "ChooseAnyItemCategory") {
            ChooseAnyViewState(preState);
            twNewegg.ChooseAny.LoadChooseListHistory(preState.CategoryID, preState.Page, preState.PVID, preState.OrderBy);
        } 
    }
    if (window.location.href == twNewegg.History.getState().url) {
        //mobile device will reload every time.
        //window.location.reload();
    }
};

twNewegg.History.onPopState(objHtml5History, objNonHtml5History);
