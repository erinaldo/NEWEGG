(function (twNewegg, jQuery) {
    var trackApi = "/api/track";
    var scName = "sc";
    
	var shoppingCart = function () {

	    var setting = {
	        itemIds: [0],
	        qtys: [0],
	        status: 0,
	        categoryIds: [null],
	        categorytypes: [null],
            couponIds: [null],
	        successMethods: [null],
            successParas: [null]
	    };

	    var setCookies = function (key, value, expire, path, domain) {
	        jQuery.cookie(key, value, {
	            expires: expire,
	            path: path, 
	            domain: domain
	        });
	    };

	    var successAction = function (action, itemDetails, successMethods, successParas, data) {
	        if (jQuery.isArray(itemDetails) && jQuery.isArray(data)) {
	            for (i = 0; i < data.length; i++) {
	                if (data[i] && data[i].lastIndexOf("成功") < 0) {
	                    continue;
	                }
	                switch (action) {
	                    case "add":
	                        addToCookies(itemDetails[i]);
	                        break;
	                    case "update":
	                        updateToCookies(itemDetails[i]);
	                        break;
	                    case "delete":
	                        deleteFromCookies(itemDetails[i]);
	                        break;
	                    default:
	                        break;
	                }
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successParas.push(data);
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, [data]);
	                    }
	                }
	            }
	        }
	    };

	    var successQuery = function (action, categoryId, categoryType, successMethods, successParas, data) {
	        var cid = 0, ctp = 0;
	        if (typeof categoryId === 'number') {
	            cid = categoryId;
	        }
	        if (typeof categoryType === 'number') {
	            ctp = categoryType;
	        }
	        if (typeof data === "object") {
	            var queryData = queryModelToCookies(data);
	            if (cid === 0 && ctp === 0) {
	                setCookies(scName, JSON.stringify(queryData), 1, "/", twNewegg().twNeweggDomain);
	            }
	        }
	        if (typeof successMethods === "function") {
	            var content = JSON.parse(jQuery.cookie(scName));
	            if (jQuery.isArray(successParas)) {
	                successParas.push(content);
	                successMethods.apply(this, successParas);
	            } else {
	                successMethods.apply(this, [content]);
	            }
	        }
	    };

	    var addToCookies = function (itemDetail) {
	        if (!jQuery.cookie(scName)) {
	            var content = [];
	            content.push(itemDetail);
	            setCookies(scName, JSON.stringify(content), 1, "/", twNewegg().twNeweggDomain);
	        } else {
	            var content = JSON.parse(jQuery.cookie(scName));
	            var isSame = false;
	            if (content.length > 40) {
	                twNewegg().checkState("tips", "超過上限");
	                return;
	            }
	            for (i = 0; i < content.length; i++) {
	                if (content[i].iid == itemDetail.iid) {
	                    isSame = true;
	                    content[i].qty = itemDetail.qty;
	                    content[i].stu = itemDetail.stu;
	                    content[i].cid = itemDetail.cid;
	                    content[i].cty = itemDetail.cty;
	                    content[i].cpd = itemDetail.cpd;
	                }
	            }
	            if (!isSame) {
	                content.push(itemDetail);
	            }
	            setCookies(scName, JSON.stringify(content), 1, "/", twNewegg().twNeweggDomain);
	        }
	    };

	    var updateToCookies = function (itemDetail) {
	        if (!jQuery.cookie(scName)) {
	            twNewegg().checkState("tips", "無此商品");
	        } else {
	            var content = JSON.parse(jQuery.cookie(scName));
	            for (i = 0; i < content.length; i++) {
	                if (content[i].iid == itemDetail.iid) {
	                    content[i].qty = itemDetail.qty;
	                    content[i].stu = itemDetail.stu;
	                    content[i].cid = itemDetail.cid;
	                    content[i].cty = itemDetail.cty;
	                    content[i].cpd = itemDetail.cpd;
	                }
	            }
	            setCookies(scName, JSON.stringify(content), 1, "/", twNewegg().twNeweggDomain);
	        }
	    };

	    var deleteFromCookies = function (itemDetail) {
	        if (!jQuery.cookie(scName)) {
	            twNewegg().checkState("tips", "無此商品");
	        } else {
	            var content = JSON.parse(jQuery.cookie(scName));
	            var newContent = [];
	            for (i = 0; i < content.length; i++) {
	                if (content[i].iid !== itemDetail.iid) {
	                    newContent.push(content[i]);
	                }
	            }
	            setCookies(scName, JSON.stringify(newContent), 1, "/", twNewegg().twNeweggDomain);
	        }
	    };

	    var readFromCookies = function (getCondition) {
	        var oriContent = { all: [], cart: [], wish: [] };
	        if (!jQuery.cookie(scName)) {
	        } else {
	            var content = JSON.parse(jQuery.cookie(scName));
	            var categoryID = (typeof getCondition["categoryId"] === "number") ? getCondition["categoryId"] : 0;
	            var categoryType = (typeof getCondition["categoryType"] === "number") ? getCondition["categoryType"] : 0;
	            for (i = 0 ; i < content.length; i++) {
	                if (categoryID != 0 && categoryType != 0) {
	                    if (content[i].cid === categoryID && content[i].cty === categoryType) {
	                        oriContent.all.push(content[i]);
	                        if (content[i].stu === 0) {
	                            oriContent.cart.push(content[i]);
	                        }
	                        if (content[i].stu === 1) {
	                            oriContent.wish.push(content[i]);
	                        }
	                    }
	                } else if (categoryID != 0 && categoryType == 0) {
	                    if (content[i].cid === categoryID) {
	                        oriContent.all.push(content[i]);
	                        if (content[i].stu === 0) {
	                            oriContent.cart.push(content[i]);
	                        }
	                        if (content[i].stu === 1) {
	                            oriContent.wish.push(content[i]);
	                        }
	                    }
	                } else if (categoryID == 0 && categoryType != 0) {
	                    if (content[i].cty === categoryType) {
	                        oriContent.all.push(content[i]);
	                        if (content[i].stu === 0) {
	                            oriContent.cart.push(content[i]);
	                        }
	                        if (content[i].stu === 1) {
	                            oriContent.wish.push(content[i]);
	                        }
	                    }
	                } else {
	                    oriContent.all.push(content[i]);
	                    if (content[i].stu === 0) {
	                        oriContent.cart.push(content[i]);
	                    }
	                    if (content[i].stu === 1) {
	                        oriContent.wish.push(content[i]);
	                    }
	                }
	            }
	        }
	        return oriContent;
	    };

	    var readFromAllCookies = function (getCondition) {

	    };

	    var failAction = function () {
	        twNewegg().checkState("tips", "連線錯誤");
	    };

	    var parameterToModel = function (itemIds, qtys, status, categoryIds, categoryTypes, couponIds) {
	        var postData = [];
	        for (i = 0; i < itemIds.length ; i++) {
	            var singleData = {};
	            singleData["iid"] = itemIds[i];
	            singleData["qty"] = (qtys) ? qtys[i] : 1;
	            singleData["stu"] = status;
	            singleData["cid"] = (categoryIds) ? categoryIds[i] : null;
	            singleData["cty"] = (categoryTypes) ? categoryTypes[i] : null;
	            singleData["cpd"] = (couponIds) ? couponIds[i] : null;
	            postData.push(singleData);
	        }
	        return postData;
	    };

	    var queryModelToCookies = function (queryData) {
	        var cookieModel = [];
	        if (jQuery.isArray(queryData["cart"])) {
	            for (i = 0; i < queryData["cart"].length; i++) {
	                var singleData = {};
	                singleData["iid"] = queryData["cart"][i].ItemID;
	                singleData["qty"] = queryData["cart"][i].ItemQty;
	                singleData["stu"] = 0;
	                singleData["cid"] = queryData["cart"][i].CategoryID;
	                singleData["cty"] = queryData["cart"][i].CategoryType;
	                singleData["cpd"] = "";
	                cookieModel.push(singleData);
	            }
	        }
	        if (jQuery.isArray(queryData["wish"])) {
	            for (i = 0; i < queryData["wish"].length; i++) {
	                var singleData = {};
	                singleData["iid"] = queryData["wish"][i].ItemID;
	                singleData["qty"] = queryData["wish"][i].ItemQty;
	                singleData["stu"] = 1;
	                singleData["cid"] = queryData["wish"][i].CategoryID;
	                singleData["cty"] = queryData["wish"][i].CategoryType;
	                singleData["cpd"] = "";
	                cookieModel.push(singleData);
	                if (i > 9) {
	                    break;
	                }
	            }
	        }
	        if (jQuery.isArray(queryData["cartadditional"])) {
	            for (i = 0; i < queryData["cartadditional"].length; i++) {
	                var singleData = {};
	                singleData["iid"] = queryData["cartadditional"][i].ItemID;
	                singleData["qty"] = queryData["cartadditional"][i].ItemQty;
	                singleData["stu"] = 100;
	                singleData["cid"] = queryData["cartadditional"][i].CategoryID;
	                singleData["cty"] = queryData["cartadditional"][i].CategoryType;
	                singleData["cpd"] = "";
	                cookieModel.push(singleData);
	                if (i > 9) {
	                    break;
	                }
	            }
	        }
	        if (jQuery.isArray(queryData["chooseadditional"])) {
	            for (i = 0; i < queryData["chooseadditional"].length; i++) {
	                var singleData = {};
	                singleData["iid"] = queryData["chooseadditional"][i].ItemID;
	                singleData["qty"] = queryData["chooseadditional"][i].ItemQty;
	                singleData["stu"] = 102;
	                singleData["cid"] = queryData["chooseadditional"][i].CategoryID;
	                singleData["cty"] = queryData["chooseadditional"][i].CategoryType;
	                singleData["cpd"] = "";
	                cookieModel.push(singleData);
	                if (i > 9) {
	                    break;
	                }
	            }
	        }
	        if (jQuery.isArray(queryData["interadditional"])) {
	            for (i = 0; i < queryData["interadditional"].length; i++) {
	                var singleData = {};
	                singleData["iid"] = queryData["interadditional"][i].ItemID;
	                singleData["qty"] = queryData["interadditional"][i].ItemQty;
	                singleData["stu"] = 101;
	                singleData["cid"] = queryData["interadditional"][i].CategoryID;
	                singleData["cty"] = queryData["interadditional"][i].CategoryType;
	                singleData["cpd"] = "";
	                cookieModel.push(singleData);
	                if (i > 9) {
	                    break;
	                }
	            }
	        }
	        return cookieModel;
	    };

	    var parameterToQueryModel = function (categoryIds, categoryTypes, successMethods, successParas) {
	        var queryData = {};
	        for (i = 0; i < categoryIds.length && i < 1 ; i++) {
	            queryData["categoryId"] = categoryIds[i];
	        }
	        for (i = 0; i < categoryTypes.length && i < 1 ; i++) {
	            queryData["categoryType"] = categoryTypes[i];
	        }
	        for (i = 0; i < successMethods.length && i < 1 ; i++) {
	            queryData["successMethod"] = successMethods[i];
	        }
	        for (i = 0; i < successParas.length && i < 1 ; i++) {
	            queryData["successPara"] = successParas[i];
	        }
	        return queryData;
	    };

	    var addToCart = function (itemIds, qtys, categoryIds, categoryTypes, successMethods, successParas, auchk) {
	        var postData = parameterToModel(itemIds, qtys, 0, categoryIds, categoryTypes, null);
	        if (auchk) {
	            twNewegg().neWebApi.postNe(trackApi, { '': postData }, successAction, ["add", postData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < postData.length; i++) {
	                addToCookies(postData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
	    };

	    var addToAdditionalCart = function (itemIds, qtys, categoryIds, categoryTypes, CartType, successMethods, successParas, auchk) {
	        var postData = parameterToModel(itemIds, qtys, CartType, categoryIds, categoryTypes, null);
	        if (auchk) {
	            twNewegg().neWebApi.postNe(trackApi, { '': postData }, successAction, ["add", postData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < postData.length; i++) {
	                addToCookies(postData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
	    };

	    var addToWish = function (itemIds, qtys, categoryIds, categoryTypes, successMethods, successParas, auchk) {
	        var postData = parameterToModel(itemIds, qtys, 1, categoryIds, categoryTypes, null);
	        if (auchk) {
	            twNewegg().neWebApi.postNe(trackApi, { '': postData }, successAction, ["add", postData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < postData.length; i++) {
	                addToCookies(postData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
		};
	    var updateToCart = function (itemIds, qtys, categoryIds, categoryTypes, couponIds, successMethods, successParas, auchk) {
	        var putData = parameterToModel(itemIds, qtys, 0, categoryIds, categoryTypes, couponIds);
	        if (auchk) {
	            twNewegg().neWebApi.putNe(trackApi, { '': putData }, successAction, ["update", putData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < putData.length; i++) {
	                updateToCookies(putData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
	    };
	    var updateToAdditionalCart = function (itemIds, qtys, categoryIds, categoryTypes, CartType, couponIds, successMethods, successParas, auchk) {
	        var putData = parameterToModel(itemIds, qtys, CartType, categoryIds, categoryTypes, couponIds);
	        if (auchk) {
	            twNewegg().neWebApi.putNe(trackApi, { '': putData }, successAction, ["update", putData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < putData.length; i++) {
	                updateToCookies(putData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
	    };
	    var updateToWish = function (itemIds, qtys, categoryIds, categoryTypes, couponIds, successMethods, successParas, auchk) {
	        var putData = parameterToModel(itemIds, qtys, 1, categoryIds, categoryTypes, couponIds);
	        if (auchk) {
	            twNewegg().neWebApi.putNe(trackApi, { '': putData }, successAction, ["update", putData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < putData.length; i++) {
	                updateToCookies(putData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
		};
	    var removeFromCart = function (itemIds, CartType, successMethods, successParas, auchk) {
	        var deleteData = parameterToModel(itemIds, null, 0, null, CartType, null);
	        if (auchk) {
	            twNewegg().neWebApi.deleteNe(trackApi, { '': deleteData }, successAction, ["delete", deleteData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < deleteData.length; i++) {
	                deleteFromCookies(deleteData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
		};
	    var removeFromWish = function (itemIds, successMethods, successParas, auchk) {
	        var deleteData = parameterToModel(itemIds, null, 1, null, null, null);
	        if (auchk) {
	            twNewegg().neWebApi.deleteNe(trackApi, { '': deleteData }, successAction, ["delete", deleteData, successMethods, successParas], failAction, null);
	        } else {
	            for (i = 0; i < deleteData.length; i++) {
	                deleteFromCookies(deleteData[i]);
	            }
	            if (jQuery.isArray(successMethods)) {
	                if (typeof successMethods[0] === "function") {
	                    if (jQuery.isArray(successParas)) {
	                        successMethods[0].apply(this, successParas);
	                    } else {
	                        successMethods[0].apply(this, []);
	                    }
	                }
	            }
	        }
		};

	    var readFromCart = function (categoryIds, categoryTypes, successMethods, successParas, auchk) {
	        var queryData = parameterToQueryModel(categoryIds, categoryTypes, successMethods, successParas);
	        if (auchk) {
	            twNewegg().neWebApi.getNe(trackApi, (typeof queryData["categoryId"] === "number") ? "cid=" + queryData["categoryId"] : "", successQuery, ["read", queryData["categoryId"], queryData["categoryType"], queryData["successMethod"], queryData["successPara"]], failAction, null);
	        } else {
	            if (typeof queryData.successMethod === "function") {
	                var content = JSON.parse((jQuery.cookie(scName)) ? jQuery.cookie(scName) : "[]");
	                if (jQuery.isArray(queryData.successPara)) {
	                    queryData.successPara.push(content);
	                    queryData.successMethod.apply(this, queryData.successPara);
	                } else {
	                    queryData.successMethod.apply(this, [content]);
	                }
	            }
	        }
		};
	    var readAllCart = function (categoryIds, categoryTypes, successMethods, successParas, auchk) {
	        var queryData = parameterToQueryModel(categoryIds, categoryTypes, successMethods, successParas);
	        if (auchk) {
	            twNewegg().neWebApi.getNe(trackApi, (typeof queryData["categoryId"] === "number") ? "ac=all&cid=" + queryData["categoryId"] : "ac=all", queryData["successMethod"], queryData["successPara"], failAction, null);
	        } else {
	            //readFromAllCookies(queryData);
	            twNewegg().checkState("tips", "連線錯誤");
	        }
		};

		this.cartMethod = function (action, settingPara) {
		    jQuery.extend(setting, settingPara);
		    switch (action) {
		        case "addToCart":
		            twNewegg().checkNGO(addToCart, [setting["itemIds"], setting["qtys"], setting["categoryIds"], setting["categoryTypes"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "addToAdditionalCart":
		            twNewegg().checkNGO(addToAdditionalCart, [setting["itemIds"], setting["qtys"], setting["categoryIds"], setting["categoryTypes"], setting["status"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "addToWish":
		            twNewegg().checkNGO(addToWish, [setting["itemIds"], setting["qtys"], setting["categoryIds"], setting["categoryTypes"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "updateToCart":
		            twNewegg().checkNGO(updateToCart, [setting["itemIds"], setting["qtys"], setting["categoryIds"], setting["categoryTypes"], setting["couponIds"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "updateToAdditionalCart":
		            twNewegg().checkNGO(updateToAdditionalCart, [setting["itemIds"], setting["qtys"], setting["categoryIds"], setting["categoryTypes"], setting["status"], setting["couponIds"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "updateToWish":
		            twNewegg().checkNGO(updateToWish, [setting["itemIds"], setting["qtys"], setting["categoryIds"], setting["categoryTypes"], setting["couponIds"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "removeFromCart":
		            twNewegg().checkNGO(removeFromCart, [setting["itemIds"], setting["status"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "removeFromWish":
		            twNewegg().checkNGO(removeFromWish, [setting["itemIds"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "readFromCart":
		            twNewegg().checkNGO(readFromCart, [setting["categoryIds"], setting["categoryTypes"], setting["successMethods"], setting["successParas"]]);
		            break;
		        case "readAllCart":
		            twNewegg().checkNGO(readAllCart, [setting["categoryIds"], setting["categoryTypes"], setting["successMethods"], setting["successParas"]]);
		            break;
		        default:
		            break;
		    }
		};

	};
	twNewegg.prototype.cart = function () {
		return new shoppingCart();
	}
})(twNewegg, jQuery);

/* ------ 設定refer的CategoryId ------ */
function SetReferCategory()
{
    var strReferUrl = document.referrer;
    var numCategoryId = 0;
    var listCategory = null;
    var objCategory = null;
    var ni = 0;
    var boolSearch = false;

    if (document.location.href.toUpperCase().indexOf("ITEMID") < 0)
    {
        return;
    }

    if (strReferUrl != null && typeof (strReferUrl) != "undefined" && strReferUrl.length > 0)
    {
        strReferUrl = strReferUrl.toUpperCase();
        strReferUrl = strReferUrl.split("?");
        if (strReferUrl.length > 1)
        {
            //將參數整理成可化成物件的Json字串
            strReferUrl = "\"" + strReferUrl[1];
            strReferUrl = strReferUrl.replace(/\&/gi, "\",\"");
            strReferUrl = strReferUrl.replace(/\=/gi, "\":\"");
            strReferUrl = strReferUrl + "\"";
            strReferUrl = "{" + strReferUrl + "}";
            //console.log("before string : ", strReferUrl);
            strReferUrl = JSON.parse(strReferUrl);
            //console.log("refer object:", strReferUrl);
        }
    }
    else
    {
        strReferUrl = new Object();
        strReferUrl.CATEGORYID = $("#data_main_category").val();
    }
    if (strReferUrl.CATEGORYID == null || typeof (strReferUrl.CATEGORYID) == "undefined")
    {
        strReferUrl.CATEGORYID = $("#data_main_category").val();
    }

    listCategory = window.localStorage["ChooseItemCategory"];
    //console.log("sessionCategory:", listCategory);
    if (listCategory == null || typeof (listCategory) == "undefined")
    {
        listCategory = new Array();
    }
    else
    {
        listCategory = JSON.parse(listCategory);
    }
    for (ni = 0; ni < listCategory.length; ni++)
    {
        if (listCategory[ni].ItemId == $("#data_item_id").val())
        {
            boolSearch = true;
            listCategory[ni].CategoryId = strReferUrl.CATEGORYID;
            break;
        }
    }
    if (!boolSearch)
    {
        objCategory = new Object();
        objCategory.ItemId = $("#data_item_id").val();
        objCategory.CategoryId = strReferUrl.CATEGORYID;
        listCategory.push(objCategory);
    }

    window.localStorage["ChooseItemCategory"] = JSON.stringify(listCategory);
    //console.log("after ChooseItemCategory:", window.localStorage["ChooseItemCategory"]);

}

/* ------ 取得來源的CategoryId ------ */
function GetReferCategoryId(argItemId)
{
    var numCategoryId = -1;
    var listCategory = null;
    var ni = 0;

    if (argItemId == null || typeof (argItemId) == "undefined" || !$.isNumeric(argItemId) || argItemId <= 0)
    {
        return numCategoryId;
    }


    listCategory = window.localStorage["ChooseItemCategory"];
    if (listCategory == null || typeof (listCategory) == "undefined")
    {
        return numCategoryId;
    }

    listCategory = JSON.parse(listCategory);
    if (listCategory == null || typeof (listCategory) == "undefined" || listCategory.length <= 0)
    {
        return numCategoryId;
    }

    for (ni = 0; ni < listCategory.length; ni++)
    {
        if (listCategory[ni].ItemId == argItemId)
        {
            numCategoryId = parseInt(listCategory[ni].CategoryId, 10);
            if (isNaN(numCategoryId)) {
                numCategoryId = -1;
            }
            //console.log("numCategoryId:", numCategoryId);
            break;
        }
    }

    return numCategoryId;
}