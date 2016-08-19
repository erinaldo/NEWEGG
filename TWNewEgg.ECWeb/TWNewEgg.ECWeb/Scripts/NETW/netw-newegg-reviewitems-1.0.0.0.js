(function (twNewegg)
{
    twNewegg.prototype.ReviewItems = {

        /* 此函式主要在儲存資料於Cookie內, 可利用於購物車、最近瀏覽品項、推廌等等 */

        //預設CookieName
        cookie_name: "newegg_items",
        //預設group_name
        group_name : "item_reviews",
        //預設記錄最大數量, 若為0, 表示不限
        group_maxlength: 0,

        /* ------ 從Cookie內取得所有的Item項 ------ */
        GetAll: function ()
        {
            var listItem = null;
            var listAllGroupData = null;

            //取得所有品項
            listAllGroupData = this.GetAllCookieGroupData();
            if (listAllGroupData == null || typeof (listAllGroupData) == "undefined")
                return null;

            listItem = listAllGroupData[this.group_name];

            return listItem;
        },

        /* ------ 根據Id取得Item物件 ------ */
        GetItemById: function (arg_ItemId)
        {
            var listItem = null;
            var newItemList = null;
            var obj = null;

            listItem = this.GetAll();
            if (listItem != null && typeof (listItem) != "undefined")
            {
                newItemList = $.grep(listItem, function (obj)
                {
                    return obj.Id === arg_ItemId;
                });
            }

            if (newItemList != null && typeof (newItemList) != "undefnined" && newItemList.length <= 0)
                newItemList = null;

            return newItemList;
        },
        
        /* ------ 從Cookie內新增Item項, 陣列愈前面的資料愈舊 ------ */
        AddItem: function (argItem)
        {
            if (argItem == null || typeof (argItem) == "undefined")
            {
                return false;
            }

            var listItem = null;
            var strTemp = "";
            var listAllGroupData = null;
            var objTemp = null;
            var i = 0;

            //取得品項
            listItem = this.GetAll();

            if (listItem == null || typeof (listItem) == "undefined")
            {
                listItem = new Array();
            }

            //先比對是否有重覆Id的資訊, 若有重覆的話, 直接移除重覆的資料
            listItem = $.grep(listItem, function (obj)
            {
                return obj.Id !== argItem.Id;
            });
            
            //若是超過最大數, 就要先扣除最前面一項, 陳列愈前面的資料愈舊
            if (this.group_maxlength != 0 && listItem.length >= this.group_maxlength)
            {
                listItem.shift();
            }
            
            //加入最新的Item
            listItem.push(argItem);

            //將新的資料轉為JSON string 存入Cookie
            listAllGroupData = this.GetAllCookieGroupData();
            if (listAllGroupData == null || typeof (listAllGroupData) == "undefined")
            {
                //若cookie不存在, 就先建一個空的新物件
                listAllGroupData = new Object();
            }
            listAllGroupData[this.group_name] = listItem;//取代Cookie內同群組的物件
            strTemp = JSON.stringify(listAllGroupData);
            $.cookie(this.cookie_name, strTemp);


            return true;;
        },

        /* ------ 從Cookie內移除Id等於傳入值的Item項 ------ */
        RemoveItem: function (arg_ItemId)
        {
            var listItem = null
            var newItemList = null;
            var i = 0;
            var strTemp = "";
            var boolExec = false;
            var listAllGroupData = null;
            
            //取得所有的Item列表
            listItem = this.GetAll();

            if (listItem == null || typeof (listItem) == "undefined")
                return false;

            //扣除此項
            newItemList = $.grep(listItem, function (obj)
            {
                return obj.Id !== arg_ItemId;
            });

            //將newListItem重新組成JSON字串存入Cookie, 若無則清空
            strTemp = $.cookie(this.cookie_name);
            listAllGroupData = JSON.parse(strTemp);//取得所有的Cookie物件

            if (newItemList != null || typeof (newItemList) != "undefined" && newItemList.Length > 0)
            {
                listAllGroupData[this.group_name] = newItemList;//將舊資料取代掉
            }
            else
            {
                listAllGroupData[this.group_name] = null;
            }
            strTemp = JSON.stringify(listAllGroupData);
            boolExec = $.cookie(this.cookie_name, strTemp);
            
            return boolExec;
        },

        RemoveGroup: function ()
        {
            var i = 0;
            var strTemp = "";
            var listAllGroupData = null;
            var objRemoveItem = null;

            listAllGroupData = this.GetAllCookieGroupData();
            if (listAllGroupData != null && typeof (listAllGroupData) != "undefined")
            {
                listAllGroupData[this.group_name] = null;
            }
            strTemp = JSON.stringify(listAllGroupData);
            $.cookie(this.cookie_name, strTemp);
        },

        GetAllCookieGroupData: function(){
            var listAllGroupData = null;
            var strTemp = "";

            strTemp = $.cookie(this.cookie_name);
            if (strTemp != null && typeof (strTemp) != "undefined" && strTemp.length > 0)
            {
                listAllGroupData = JSON.parse(strTemp);
            }

            return listAllGroupData;
        },

        Settings: function (options)
        {
            //預設值
            var argsettings = {
                cookie_name: "newegg_items",
                group_name : "item_reviews",
                group_maxlength : 0
            }

            if (options)
            {
                $.extend(argsettings, options);
            }

            this.cookie_name = argsettings.cookie_name;
            this.group_maxlength = argsettings.group_maxlength;
            this.group_name = argsettings.group_name;
        }

    };
})(twNewegg);

/* ------ 使用Cookie範例 ------ */
//註1:一個cookie內允許存在多個Group, 一個Group裡存在多筆資料, 資料以Array(Object)的方式儲存
//註2:每個objItem一定要有欄位Id做為Key值, 同一個Group中的資料, 不會有2筆相同Key值的資料, 同Key值的新資料會取代舊資料
function TestDataFromLynnCookie()
{
    var cookie_name = "cookie_test"; //若未設定cookie_name , 則使用預設值為newegg_items
    var group_name = "test_reviews";  //若未設定group_name , 則使用預設值為item_reviews
    var group_maxlength = 8;//若設為0, 表示資料列最大值不限, 若有值,則會取代最舊的資料
    var Settings = { "cookie_name": cookie_name, "group_name": group_name, "group_maxlength": group_maxlength };
    var objItem = { "Id": 99, "Name": "TTT", Title: "For Testing" };
    var listItems = null;

    //設定Cookie, 每次讀取應先設好自己要使用的Cookie資料,否則會使用預設值的資料
    twNewegg().ReviewItems.Settings(Settings);

    //加入一筆資料於Group內, 若無Group會自動建立
    twNewegg().ReviewItems.AddItem(objItem);

    //取得Group內的所有資料清單, 若無資料則回傳null
    listItems = twNewegg().ReviewItems.GetAll();

    //取得Group內Id為傳入值的資料清單, 傳回值為Array(object), 若無資料則回傳null
    listItems = twNewegg().ReviewItems.GetItemById(objItem.Id);

    //刪除Group內Id為傳入值的資料
    twNewegg().ReviewItems.RemoveItem(objItem.Id);

    //刪除該Group內的所有資料
    twNewegg().ReviewItems.RemoveGroup();

    //取得此Cookie內, 所有的Group及其下的所有資料, 傳回值為Array(object), 若無資料則回傳null
    //特殊之處在於, 若要使用此Array, 無法使用Array的index, 而是直接以 listItems[group_name]取得該Group旗下資料
    listItems = twNewegg().ReviewItems.GetAllCookieGroupData();
}

