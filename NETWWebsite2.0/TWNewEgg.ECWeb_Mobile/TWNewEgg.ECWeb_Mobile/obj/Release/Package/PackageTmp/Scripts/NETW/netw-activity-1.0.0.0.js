(function ($) {

    var _defaultSettings = {
        itemMarketPrice: '<span class="marketPrice">${0}</span>',
        itemPrice: '<span class="price">${0}</span>',
        itemSoldOut: '<div class="soldOut">售完</div>',
        itemAddToCart: '<div class="addCart" onclick="purchaseItem({0})">加入購物車</div>',
        itemAddToWish: '<i class="fa fa-2x fa-heart orangetxt2" onclick="addToWishList({0})"></i>',
        itemAddToSC: '<div class="buyNow" onclick="purchaseItem({0})" >立即<br/>結帳</div>'
    };

    var _settings = _defaultSettings;

    var methods = {
        init: function (options) {
            _settings = $.extend(_defaultSettings, options);

            //利用each跑遍要被執行的元素
            return this.each(function () {
                //logJson(_settings);
            });
        },
        dispose: function (callback) {
            var tagBody = $(this);
            //logJson(_settings);

            var aryBoxCode = getTagVal(tagBody);
            //logJson(aryBoxCode);

            var tagName, strTmp, objTmp;
            var itemBox, itemBoxHtml;
            var eachItem;
            $.each(aryBoxCode, function (idx, val) {

                itemBox = tagBody.find(".itemBox[tag-id='" + val.tagId + "']");

                //取得區塊資料
                eachItem = getItemData(itemBox, val);
                //logJson(eachItem);
            });

            //callback
            return this.each(function () {
                if (callback && typeof callback === 'function') {
                    callback.call(this, aryBoxCode);
                }
            });

        },
        json: function (callback) {
            var data = getTagVal(this);
            //logJson(data);           

            //callback
            if (callback && typeof callback === 'function') {
                return this.each(function () {
                    callback.call(this, data);
                });
            } else {
                return data;
            }
        }
    };

    //Public
    $.fn.netwActivityPage = function (method) {
        //console.log(arguments);
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
            //} else if (typeof method === 'object' || !method) {
            //    console.log('object');
            //    return method.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.activityPage');
        }
    };

    //Private
    function getGuid() {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    }

    function getTagVal(target) {
        var itemBox = target.find('.itemBox');
        var boxCode = [], aryBoxCode = [];

        itemBox.hide();

        var tagBody = this;
        itemBox.each(function () {

            var tagGuid = getGuid();
            $(this).attr('tag-id', tagGuid);

            boxCode = [];
            //取得data-itemid
            var dataItemid = $(this).attr('data-itemid');

            parseHtml($(this).html(), boxCode);

            aryBoxCode.push({
                'tagId': tagGuid,
                'dataItemid': dataItemid,
                'tagVal': boxCode
            });
        });

        //logJson(aryBoxCode);
        return aryBoxCode;
    }

    function parseHtml(innerHtml, boxCode) {

        var str = innerHtml;

        var intIdx = 0, intS, intE;
        var innerHtml = '', temp = '', pictemp = '';

        var tagBox = this;
        while (str.indexOf('{{') != -1) {

            //取得起始符號的位址
            intS = str.indexOf('{{');
            if (intS > -1) {
                //console.log(str.substr(intS + 2));
                temp = str.substr(intS + 2);
                //console.log(temp);

                //取得{{pic_2_200}}內容
                intE = temp.indexOf('}}');
                //alert(intE);
                pictemp = temp.substr(0, intE);
                //console.log(pictemp);
                //ary.push(pictemp);

                ////組區塊訊息
                //var tagGuid = oTag.getGuid();
                //var picInfo = {
                //    tagId: 'tag_' + tagGuid,
                //    tagContent: pictemp
                //};
                //ary.push(picInfo);

                //組區塊訊息
                //console.log(pictemp.indexOf('item'));
                if (pictemp.indexOf('item') == 0)
                    boxCode.push(parseTagVal(pictemp));

                //取得尚未處理內容
                intIdx = intE + 2;
                str = temp.substr(intIdx);

            }
        }

        //this.logJson(boxCode);
    }

    function parseTagVal(value) {
        var ary = value.split('_');
        var tagVal = [];

        switch (ary[0]) {
            case 'itemLink':
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'src': '/item?itemid={0}'
                };
                break;
            case 'itemName':
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'length': ary[1]
                };
                break;
            case 'itemImgUrl':
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'PKey': ary[1],
                    'width': ary[2]
                };
                break;
            case 'itemMarketPrice':
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'template': _settings.itemMarketPrice
                };
                break;
            case 'itemPrice':
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'template': _settings.itemPrice
                };
                break;
            case 'itemQty':
                tagVal = {
                    'label': value,
                    'name': ary[0]
                };
                break;
            case 'itemSoldOut':
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'template': _settings.itemSoldOut
                };
                break;
            case 'itemAddToCart'://加入購物車
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'template': _settings.itemAddToCart
                };
                break;
            case 'itemAddToWish':
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'template': _settings.itemAddToWish
                };
                break;
            case 'itemAddToSC'://立即購買
                tagVal = {
                    'label': value,
                    'name': ary[0],
                    'template': _settings.itemAddToSC
                };
                break;
            case 'itemSubTitle':
                tagVal = {
                    'label': value,
                    'name': ary[0]
                };
                break;
            default:
                tagVal = {
                    'label': value,
                    'name': ary[0]
                };
                break;
        }

        return tagVal;
    }
    //var times=0;
    function getItemData(itemBox, val) {
        try {
            var itemBoxHtml = '';

            //times+=1;
            //if (times > 1) return;            

            $.ajax({
                url: '/api/Item/getItemDetailByItemId?itemid=' + val.dataItemid + '&' + getTicks(),
                type: 'GET',
                dataType: 'json',
                //timeout: 5000,
                success: function (jdata) {

                    //替代特定符號
                    itemBoxHtml = itemBox.html();

                    $.each(val.tagVal, function (idx2, val2) {
                        //console.log(val2.name);
                        tagName = '{{' + val2.label + '}}';
                        strTmp = '';
                        switch (val2.name) {
                            case 'itemLink':
                                strTmp = val2.src.replace('{0}', val.dataItemid);
                                break;
                            case 'itemName':
                                if (jdata.Name != null && $.trim(jdata.Name) != '') {
                                    if (val2.length == null) {
                                        strTmp = jdata.Name;
                                    } else {
                                        strTmp = jdata.Name.substr(0, val2.length);
                                        if (jdata.Name.length > val2.length)
                                            strTmp += ' …';
                                    }
                                }
                                break;
                            case 'itemImgUrl':
                                strTmp = getImage(jdata, val2);
                                break;
                            case 'itemMarketPrice':
                                strTmp = getMarketPrice(jdata);
                                break;
                            case 'itemPrice':
                                strTmp = getPrice(jdata);
                                break;
                            case 'itemQty':
                                strTmp = jdata.Amount;
                                break;
                            case 'itemSoldOut':
                                strTmp = getSoldOut(jdata);
                                break;
                            case 'itemAddToCart':
                                //logJson(jdata);
                                strTmp = getAddToCartOrWish(jdata);
                                break;
                            case 'itemAddToWish':
                                strTmp = getAddToWish(jdata);
                                break;
                            case 'itemAddToSC':
                                strTmp = getAddToSC(jdata);
                                break;
                            case 'itemSubTitle':
                                strTmp = (jdata.Title == null ? '' : jdata.Title);
                                break;
                        }

                        itemBoxHtml = itemBoxHtml.replace(tagName, strTmp);
                    });

                    //console.log(itemBoxHtml);
                    itemBox.html(itemBoxHtml);
                },
                error: function (jqXhr, textStatus) {
                    //logJson(jqXhr);
                    var err = jqXhr.status + ' (' + jqXhr.statusText + ')<br>[ItemId: ' + val.dataItemid + '] ' +
                        jQuery.parseJSON(jqXhr.responseText).Message;

                    itemBox.html('<div style="padding:10px;display:inline-block;color:#FF0000;background-color:#FFC8B4;line-height:1.5;">' + err + '</div>');
                },
                complete: function () {
                    itemBox.show();
                }
            });
        } catch (e) {
            console.log(e.message);
        }
    }

    function getImage(eachItem, itemSet) {
        var strTmp = '', objTmp;

        if (eachItem.ImgUrlList != null && eachItem.ImgUrlList.length > 0) {
            //logJson(itemSet.width);
            strTmp = eachItem.ImgUrlList[0];

            //newegg.com.tw 替換檔名
            if (strTmp.indexOf('.newegg.com.tw/') > -1) {
                var fileName = strTmp.split(/(\\|\/)/g).pop();
                //console.log(fileName);

                var aryFile = fileName.split('.');
                var aryTmp = aryFile[0].split('_');

                var newFileName = fileName.replace('_' + aryTmp[aryTmp.length - 1] + '.', '_' + itemSet.width + '.');
                //console.log(newFileName);

                strTmp = strTmp.replace('/' + fileName, '/' + newFileName);
            }
            //console.log(strTmp);
        }

        return strTmp;
    }

    function getMarketPrice(eachItem) {
        var innerHtml = _settings.itemMarketPrice;

        try {
            if (eachItem.Price == null || eachItem.Price == 0) {
                innerHtml = '';
            } else {
                innerHtml = innerHtml.replace('{0}', eachItem.Price);
            }
        } catch (e) {

        }
        return innerHtml;
    }

    function getPrice(eachItem) {
        var innerHtml = _settings.itemPrice;

        if (eachItem.PromotionPrice == null || eachItem.PromotionPrice == 0) {
            innerHtml = '';
        } else {
            innerHtml = innerHtml.replace('{0}', eachItem.PromotionPrice);
        }
        return innerHtml;
    }

    function getSoldOut(eachItem) {
        var innerHtml = _settings.itemSoldOut;
        try {
            if (eachItem.Amount != null && eachItem.Amount > 0) {
                innerHtml = '';
            }
        } catch (e) {

        }
        return innerHtml;
    }

    function getAddToCartOrWish(eachItem) {
        var innerHtml = '';//_settings.itemAddToWish;

        try {
            if (eachItem.Amount == null || eachItem.Amount == 0) {
                innerHtml = _settings.itemAddToWish.replace('{0}', eachItem.Id);
            } else {
                innerHtml = _settings.itemAddToCart.replace('{0}', eachItem.Id);
            }
        } catch (e) {

        }
        return innerHtml;
    }

    function getAddToWish(eachItem) {
        var innerHtml = _settings.itemAddToWish.replace('{0}', eachItem.Id);

        return innerHtml;
    }

    //立即購買
    function getAddToSC(eachItem) {
        var innerHtml = '';//_settings.itemAddToSC;

        try {
            innerHtml = _settings.itemAddToSC.replace('{0}', eachItem.Id);
        } catch (e) {

        }
        return innerHtml;
    }

    function getTicks() {
        var nowDate = new Date();
        var ticks = ((nowDate.getTime() * 10000) + 621355968000000000);

        return ticks;
    }

    function logJson(jsonData) {
        console.log(JSON.stringify(jsonData, null, '\t'));
    }
})(jQuery);

(function (window, jQuery, undefined) {
    var _netw = function () {
        if (!(this instanceof _netw)) {
            return new _netw();
        }
    };

    _netw.prototype = {
        buyNow: function (itemID) {
            //console.log('netw.buyNow：' + itemID);
            twNewegg().cart().cartMethod("addToCart", { itemIds: [itemID], qtys: [1], categoryIds: [], categoryTypes: [], successMethods: [], successParas: [[]] });
        },
        myWish: function (itemID) {
            //console.log('netw.myWish：' + itemID);
            twNewegg().cart().cartMethod("addToWish", { itemIds: [itemID], qtys: [1], categoryIds: [], categoryTypes: [], successMethods: [], successParas: [[]] });
        },
        logJson: function (jsonData) {
            console.log(JSON.stringify(jsonData, null, '\t'));
        }
    };

    window.netw = _netw;
})(window, jQuery);


