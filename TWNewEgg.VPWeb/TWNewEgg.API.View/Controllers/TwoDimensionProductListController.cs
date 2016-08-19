using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using TWNewEgg.API.Models;
using log4net;
using log4net.Config;
using AutoMapper;


namespace TWNewEgg.API.View.Controllers
{
    public class TwoDimensionProductListController : Controller
    {
        //private log4net.ILog logger;
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        TWNewEgg.API.Models.Connector connector = new Connector();

        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.TwoDimensionItemList)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("規格商品清單")]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            List<TWNewEgg.API.View.CategoryViewModel> _category = new List<CategoryViewModel>();
            _category.Add(new TWNewEgg.API.View.CategoryViewModel { shiptype = "Newegg", shiptypeCode = "N" });
            _category.Add(new TWNewEgg.API.View.CategoryViewModel { shiptype = "供應商", shiptypeCode = "S" });
            ViewBag.isSeller = sellerInfo.AccountTypeCode;
            ViewData["selectItem"] = _category.First();
            ViewData["categories"] = _category;
            ViewBag.userType = sellerInfo.IsAdmin;
            return View();
        }
        #region 草稿區
        #region 草稿區查詢
        [HttpPost]
        public JsonResult readSketchProperty([Kendo.Mvc.UI.DataSourceRequest] Kendo.Mvc.UI.DataSourceRequest request, TWNewEgg.API.View.JsonDataSkechModel jsonData/*int searchSketchProperty, string searchTextSketchProperty, int MarkerNameSketchProperty, int DateSketchProperty, string StartDataSketchProperty, string EndDataSketchProperty, int ItemCategorySketchProperty1, int ItemCategorySketchProperty2, int ItemCategorySketchProperty3*/)
        {
            ActionResponse<ItemSketchSearchCondition> SearchData = new  ActionResponse<ItemSketchSearchCondition>();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> apiSearchResult = new ActionResponse<List<ItemSketch>>();
            List<TWNewEgg.API.View.ItemSketchSelect> autoResult = new List<ItemSketchSelect>();
            ActionResponse<string> searchFlagResult = new ActionResponse<string>();
            List<string> returnMSG = new List<string>();
            //binding search data
            SearchData = this.bindSearchSketchPropertyData(jsonData);
            //binding data 有錯誤
            if (SearchData.IsSuccess == false)
            {
                //returnMSG.Add(SearchData.Msg);
                return Json(SearchData.Msg);
            }
            //是否為管理者
            SearchData.Body.IsAdmin = sellerInfo.IsAdmin;
            //欲查詢的sellerID
            SearchData.Body.SellerID = sellerInfo.currentSellerID;
            //要查詢的相關資訊
            SearchData.Body.KeyWord = jsonData.searchTextSketchProperty;
            try
            {
                //初始或一開始選的條件(商家商品編號, 廠商產品編號, 草稿ID, 商品名稱)
                SearchData.Body.KeyWordScarchTarget = (ItemSketchKeyWordSearchTarget)Enum.ToObject(typeof(ItemSketchKeyWordSearchTarget), jsonData.searchSketchProperty);
                //連接 API
                apiSearchResult = connector.propertySketchSearch(SearchData.Body);
                if (apiSearchResult.IsSuccess == false)
                {
                    searchFlagResult.IsSuccess = false;
                    searchFlagResult.Msg = apiSearchResult.Msg;
                }
                else
                {
                    //若為vender濾掉加價購草稿
                    var UserInfo = sellerInfo.IsAdmin;
                    if (!UserInfo)
                    {
                        apiSearchResult.Body = apiSearchResult.Body.Where(x => x.Item.ShowOrder != (int)AdditionalPurchase.ShowOrderType.加價購).ToList();
                    }
                    //搜索回傳有資料再進行 autoMap
                    if (apiSearchResult.Body.Count != 0)
                    {
                        //把 API 回傳的資料 MODEL 轉換成要秀在畫面的資料 MODEL
                        autoResult = this.autoMapItemSketchSelect(apiSearchResult.Body);
                        searchFlagResult.IsSuccess = true;
                    }
                }
            }
            catch (Exception error)
            {
                searchFlagResult.IsSuccess = false;
                searchFlagResult.Msg = "資料處理錯誤";
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (searchFlagResult.IsSuccess == false)
            {
                return Json(searchFlagResult.Msg);
            }
            else
            {
                return Json(autoResult.ToDataSourceResult(request));
            }
        }
        #endregion
        #region 草稿區編輯
        [HttpPost]
        public JsonResult updateSketchProperty(string _jsonupdateData)
        {
            TWNewEgg.API.View.Service.CheckModelElememtTypeService checkAttr = new Service.CheckModelElememtTypeService();
            List<TWNewEgg.API.Models.ItemSketch> modifyData = new List<ItemSketch>();
            List<TWNewEgg.API.View.ItemSketchSelect> _listModifyData = new List<ItemSketchSelect>();
            ActionResponse<string> returnMsg = new ActionResponse<string>();
            ActionResponse<string> checkAttrAction = new ActionResponse<string>();
            bool errorCheck = true;
            List<string> returnDataMsg = new List<string>();
            #region 反序列化, automap 回對應的 model, 檢查資料型態
            try
            {
                //反序列化
                _listModifyData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TWNewEgg.API.View.ItemSketchSelect>>(_jsonupdateData);
                //把反序列的 MODEL 用 autoMap 成要送到 API 進行修改的 MODEL
                modifyData = autoMapItemSketch(_listModifyData);
                //檢查資料格式是否有錯誤
                checkAttrAction = checkAttr.modelStatusCheckAttr(modifyData, "propertySketchProperty");
                // try 執行沒有跳到 exception
                errorCheck = true;
            }
            catch (Exception error)
            {
                //try 跳到 exception
                errorCheck = false;
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            //這邊的錯誤可能是反序列化或 AutoMap 時候錯誤
            if (errorCheck == false)
            {
                returnDataMsg.Add("F");
                returnDataMsg.Add("資料內容處理錯誤");
                return Json(returnDataMsg);
            }
            #endregion
            //資料形式有錯誤
            if (checkAttrAction.IsSuccess == false)
            {
                returnDataMsg.Add("F");
                if (string.IsNullOrEmpty(checkAttrAction.Msg) == true)
                {
                    returnDataMsg.Add("資料組合錯誤");
                    return Json(returnDataMsg);
                }
                returnDataMsg.Add(checkAttrAction.Msg);
                return Json(returnDataMsg);
            }

            ActionResponse<string> EditResult = new ActionResponse<string>();
            try
            {
                EditResult = connector.propertySketchEdit(ItemSketchEditType.ListEdit, modifyData);
                errorCheck = true;
            }
            catch (Exception error)
            {
                errorCheck = false;
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            //連接 API 錯誤
            if (errorCheck == false)
            {
                returnDataMsg.Add("F");
                returnDataMsg.Add("資料修改錯誤");
                return Json(returnDataMsg);
            }
            else
            {
                //api return true
                if (EditResult.IsSuccess == true)
                {
                    returnDataMsg.Add("T");
                    returnDataMsg.Add(EditResult.Msg);
                    return Json(returnDataMsg);
                }
                else
                {
                    //api resurn false
                    returnDataMsg.Add("F");
                    returnDataMsg.Add(EditResult.Msg);
                    return Json(returnDataMsg);
                }
            }
        }
        #endregion 草稿區編輯
        #region 草稿區送審
        [HttpPost]
        public JsonResult tosketchCheckProperty(List<int> _intCheck)
        {
            int userid = sellerInfo.UserID;
            int sellerid = sellerInfo.currentSellerID;
            ActionResponse<string> result = new ActionResponse<string>();
            if (_intCheck == null)
            {
                return Json("請勾選要送審的資料");
            }
            bool isNoExceprion = true;
            try
            {
                result = connector.propertySketchExamine(_intCheck, userid, sellerid);
                isNoExceprion = true;
            }
            catch (Exception error)
            {
                isNoExceprion = false;
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (isNoExceprion == false)
            {
                return Json("送審資料處理錯誤，請稍後再試");
            }
            else
            {
                return Json(result.Msg);
            }
        }
        #endregion 送審
        #region 草稿區刪除
        [HttpPost]
        public JsonResult DeleteSketchProperty(int DeleteId = -1)
        {
            List<string> returnJsonMsg = new List<string>();
            //沒有傳入任何要刪除的 id
            if (DeleteId == -1)
            {
                returnJsonMsg.Add("F");
                returnJsonMsg.Add("資料錯誤");
                return Json(returnJsonMsg);
            }
            try
            {
                var deleteResult = connector.propertySketchDelete(DeleteId);
                if (deleteResult.IsSuccess == true)
                {
                    returnJsonMsg.Add("T");
                    returnJsonMsg.Add("刪除成功");
                }
                else
                {
                    //判斷 api 回傳的訊息是不是有包含 not exists 有的話 則轉換成中文訊息
                    if (deleteResult.Msg.Contains("not exists") == true)
                    {
                        returnJsonMsg.Add("F");
                        returnJsonMsg.Add("刪除資料不存在");
                    }
                    else
                    {
                        returnJsonMsg.Add("F");
                        returnJsonMsg.Add("刪除資料處理錯誤");
                    }
                }
            }
            catch (Exception error)
            {
                returnJsonMsg.Add("F");
                returnJsonMsg.Add("資料錯誤，請稍後再試");
                logger.Error("[Msg]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }

            return Json(returnJsonMsg);
        }
        #endregion
        #region 把搜索回傳的結果 automap 回秀在 VIEW 上面的 MODEL
        public List<TWNewEgg.API.View.ItemSketchSelect> autoMapItemSketchSelect(List<TWNewEgg.API.Models.ItemSketch> toMapData)
        {
            List<TWNewEgg.API.View.ItemSketchSelect> autoResult = new List<ItemSketchSelect>();

            foreach (var item in toMapData)
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.View.ItemSketchSelect>();
                TWNewEgg.API.View.ItemSketchSelect autoData = AutoMapper.Mapper.Map<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.View.ItemSketchSelect>(item);
                TWNewEgg.API.View.CategoryViewModel cateTempData = new CategoryViewModel();
                #region Kendo dropdownlist 設定
                if (item.Item.ShipType == "N")
                {
                    cateTempData.shiptype = "Newegg";
                    cateTempData.shiptypeCode = "N";
                }
                else
                {
                    cateTempData.shiptype = "Seller";
                    cateTempData.shiptypeCode = "S";
                }
                #endregion
                autoData.startDateSketch = item.Item.DateStart;
                autoData.Category = cateTempData;
                autoResult.Add(autoData);
            }
            return autoResult;
        }
        #endregion
        #region 把從 VIEW 傳過來的 MODEL AUTOMAP 成進行修改的 MODEL
        public List<TWNewEgg.API.Models.ItemSketch> autoMapItemSketch(List<TWNewEgg.API.View.ItemSketchSelect> toMapData)
        {
            List<TWNewEgg.API.Models.ItemSketch> result = new List<ItemSketch>();
            foreach (var item in toMapData)
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.View.ItemSketchSelect, TWNewEgg.API.Models.ItemSketch>();
                TWNewEgg.API.Models.ItemSketch autoData = AutoMapper.Mapper.Map<TWNewEgg.API.View.ItemSketchSelect, TWNewEgg.API.Models.ItemSketch>(item);
                autoData.Item.ShipType = item.Category.shiptypeCode;
                //autoData.CreateAndUpdate.CreateUser = sellerInfo.UserID;
                autoData.CreateAndUpdate.UpdateUser = sellerInfo.UserID;
                autoData.Item.DateStart = item.startDateSketch.GetValueOrDefault();
                result.Add(autoData);
            }
            return result;
        }
        #endregion 把從 VIEW 傳過來的 MODEL AUTOMAP 成進行修改的 MODEL
        #region 把搜索的條件 放回要送到 API 的 MODEL
        public TWNewEgg.API.Models.ActionResponse<ItemSketchSearchCondition> bindSearchSketchPropertyData(TWNewEgg.API.View.JsonDataSkechModel SearchsonData)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.ActionResponse<ItemSketchSearchCondition> result = new ActionResponse<ItemSketchSearchCondition>();
            ItemSketchSearchCondition itemSketckModel = new ItemSketchSearchCondition();
            #region 判斷是否有選擇製造商
            //判斷是否有選擇製造商
            if (SearchsonData.MarkerNameSketchProperty != 0)
            {
                itemSketckModel.ManufactureID = SearchsonData.MarkerNameSketchProperty;
            }
            #endregion 
            #region 時間上的資料回填到要送到 API 的 MODEL
            //初始化所選擇"創建日期"的選擇條件
            try
            {
                //先把時間選擇的類型放回 MODEL
                itemSketckModel.createDate = (TWNewEgg.API.Models.ItemSketchCreateDate)Enum.ToObject(typeof(TWNewEgg.API.Models.ItemSketchCreateDate), SearchsonData.DateSketchProperty);
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "創建日期錯誤";
                logger.Error("[Msg]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            //選擇時間類型初始化失敗
            if (result.IsSuccess == false)
            {
                return result;
            }
            //指定日期
            if (itemSketckModel.createDate == TWNewEgg.API.Models.ItemSketchCreateDate.SpecifyDate)
            {
                try
                {
                    DateTime StartDate = Convert.ToDateTime(SearchsonData.StartDataSketchProperty);
                    //指定日期的話, 開始日期跟結束日期都給開始日期
                    itemSketckModel.startDate = StartDate;
                    itemSketckModel.endDate = StartDate;
                    result.IsSuccess = true;
                }
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = "時間格式錯誤";
                    logger.Error("[Msg]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
                }
                //時間型別轉換錯誤, 則不繼續, 回傳錯誤
                if (result.IsSuccess == false)
                {
                    return result;
                }
            }
            else if (itemSketckModel.createDate == TWNewEgg.API.Models.ItemSketchCreateDate.DateRange)
            {
                //日期範圍
                try
                {
                    DateTime StartDate = Convert.ToDateTime(SearchsonData.StartDataSketchProperty);
                    DateTime EndDate = Convert.ToDateTime(SearchsonData.EndDataSketchProperty);
                    itemSketckModel.startDate = StartDate;
                    itemSketckModel.endDate = EndDate;
                    result.IsSuccess = true;
                }
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = "查詢失敗";
                    logger.Error("[Msg]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
                }
            }
            else
            {
                //如果不是選指定日期或是日期範圍，則把開始時間跟結束時間都給 NULL
                itemSketckModel.startDate = null;
                itemSketckModel.endDate = null;
            }
            #endregion 時間上的資料回填到要送到 API 的 MODEL
            #region 初始化選的可售數量和判斷是否有錯誤
            try
            {
                //初始化選的可售數量
                itemSketckModel.canSellQty = (TWNewEgg.API.Models.ItemSketchCanSellQty)Enum.ToObject(typeof(TWNewEgg.API.Models.ItemSketchCanSellQty), SearchsonData.StockSketchProperty);
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "可售數量錯誤";
                logger.Error("[Msg]: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            //初始化可售數量失敗
            if (result.IsSuccess == false)
            {
                return result;
            }
            #endregion
            #region 主分類和誇分類的資料回填要送到 API 的 MODEL
            //主分類
            if (SearchsonData.ItemCategorySketchProperty1 != 0)
            {
                itemSketckModel.categoryID_Layer0 = SearchsonData.ItemCategorySketchProperty1;
            }
            //誇分類
            if (SearchsonData.ItemCategorySketchProperty2 != 0)
            {
                itemSketckModel.categoryID_Layer1 = SearchsonData.ItemCategorySketchProperty2;
            }
            //誇分類
            if (SearchsonData.ItemCategorySketchProperty3 != 0)
            {
                itemSketckModel.categoryID_Layer2 = SearchsonData.ItemCategorySketchProperty3;
            }
            #endregion 主分類和誇分類的資料回填要送到 API 的 MODEL
            //把資料放回搜索要用的 MODEL 沒錯誤
            result.IsSuccess = true;
            result.Body = itemSketckModel;
            return result;
        }
        #endregion 把搜索的條件 放回要送到 API 的 MODEL
        #endregion 草稿區










        #region 待審區
        #region 查詢
        public JsonResult readSketchPropertyList([Kendo.Mvc.UI.DataSourceRequest] Kendo.Mvc.UI.DataSourceRequest request, TWNewEgg.API.View.JsonDataSketchPropertyModel jsonData, bool isSearch = true)
        {
            ActionResponse<string> haserrorMsg = new ActionResponse<string>();
            TWNewEgg.API.Models.ItemSketchSearchCondition bindData = new ItemSketchSearchCondition();
            ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> listSketchPropertyExamine = new ActionResponse<List<sketchPropertyExamine>>();
            try
            {
                bindData = this.bindDataExamineSketch(jsonData).Body;
                haserrorMsg.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                haserrorMsg.IsSuccess = false;
                haserrorMsg.Msg = "查詢失敗，請稍後再試";
            }
            if (haserrorMsg.IsSuccess == false)
            {
                return Json(haserrorMsg);
            }
            //是否為管理者
            bindData.IsAdmin = sellerInfo.IsAdmin;
            //欲查詢的sellerID
            bindData.SellerID = sellerInfo.currentSellerID;
            //搜索的條件選擇
            bindData.KeyWordScarchTarget = (ItemSketchKeyWordSearchTarget)Enum.ToObject(typeof(ItemSketchKeyWordSearchTarget), jsonData.searchRequestListProperty);
            // 分頁功能
            bindData.pageInfo = new PageInfo();
            bindData.pageInfo.PageIndex = request.Page - 1;
            bindData.pageInfo.PageSize = request.PageSize;
            
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.View.itemsketchPropertyExamine>> returnResult = new ActionResponse<List<itemsketchPropertyExamine>>();
            try
            {
                //呼叫查詢的 API
                listSketchPropertyExamine = connector.propertySketchExamineSearch(bindData, isSearch);
                if (listSketchPropertyExamine.IsSuccess == false)
                {
                    returnResult.IsSuccess = false;
                    returnResult.Msg = listSketchPropertyExamine.Msg;
                }
                else
                {
                    //查無資料
                    if (listSketchPropertyExamine.Body.Count == 0)
                    {
                        returnResult.IsSuccess = false;
                        returnResult.Msg = listSketchPropertyExamine.Msg;
                    }
                    else
                    {
                        //把查詢的資料轉回在 View 上秀出的資料
                        var autobindData = this.autoMapData(listSketchPropertyExamine.Body);
                        returnResult.IsSuccess = true;
                        returnResult.Body = autobindData.Body;
                        returnResult.Msg = listSketchPropertyExamine.Msg;
                    }
                }
            }
            catch (Exception error)
            {
                returnResult.IsSuccess = false;
                returnResult.Msg = "資料處理錯誤，請稍後再試";
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (returnResult.IsSuccess == true)
            {
                DataSourceResult itemSearchResult = new DataSourceResult();

                request.Page = 1;
                itemSearchResult = returnResult.Body.ToDataSourceResult(request);
                itemSearchResult.Total = Convert.ToInt32(returnResult.Msg);

                return Json(itemSearchResult);
            }
            else
            {
                return Json(returnResult.Msg);
            }
        }
        #endregion
        #region 組合查詢條件的 model
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketchSearchCondition> bindDataExamineSketch(TWNewEgg.API.View.JsonDataSketchPropertyModel jsonData)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketchSearchCondition> returnModel = new ActionResponse<ItemSketchSearchCondition>();
            //初始化 returnModel
            returnModel.IsSuccess = true;
            returnModel.Msg = string.Empty;
            returnModel.Body = new ItemSketchSearchCondition();
            //returnModel.Body.Status = jsonData.CheckStatus;
            //returnModel.Body.ItemStatus = jsonData.GoodsStatus;
            //是否有帶入查詢的條件
            if (string.IsNullOrEmpty(jsonData.searchTextListProperty) == false)
            {
                returnModel.Body.KeyWord = jsonData.searchTextListProperty;
            }
            //判斷是否有選擇加價購
            if (jsonData.ShowOrder != null)
            {
                returnModel.Body.ShowOrder = jsonData.ShowOrder;
            }
            //判斷是否有選擇製造商
            if (jsonData.MarkerNameListProperty != 0)
            {
                returnModel.Body.ManufactureID = jsonData.MarkerNameListProperty;
            }
            //是否有選擇審核狀態
            if (jsonData.CheckStatus != -1)
            {
                returnModel.Body.Status = jsonData.CheckStatus;
            }
            //是否有商品狀態
            if (jsonData.GoodsStatus != -1)
            {
                returnModel.Body.ItemStatus = jsonData.GoodsStatus;
            }
            #region 時間區間判斷
            //先看查詢間的區間是哪一個
            returnModel.Body.createDate = (TWNewEgg.API.Models.ItemSketchCreateDate)Enum.ToObject(typeof(TWNewEgg.API.Models.ItemSketchCreateDate), jsonData.DateListProperty);
            //指定時間
            if (returnModel.Body.createDate == TWNewEgg.API.Models.ItemSketchCreateDate.SpecifyDate)
            {
                //Flag 用於紀錄時間格式是否轉換正確
                bool isDataFormat = true;
                if (string.IsNullOrEmpty(jsonData.StartDataListProperty) == false)
                {
                    try
                    {
                        DateTime _sDate = Convert.ToDateTime(jsonData.StartDataListProperty);
                        returnModel.Body.startDate = _sDate;
                        returnModel.Body.endDate = _sDate;
                    }
                    catch (Exception error)
                    {
                        isDataFormat = false;
                        logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                    }
                }
                else
                {
                    returnModel.IsSuccess = false;
                    returnModel.Msg = "[Error]: 請填寫時間";
                    return returnModel;
                }
                //時間格式轉換錯誤，則回傳
                if (isDataFormat == false)
                {
                    returnModel.IsSuccess = false;
                    returnModel.Msg = "[Error]: 時間格式錯誤";
                    return returnModel;
                }
            }//else if->時間範圍
            else if (returnModel.Body.createDate == TWNewEgg.API.Models.ItemSketchCreateDate.DateRange)
            {
                //Flag 用於紀錄時間格式是否轉換正確
                bool dateFormatCheck = true;
                //是否有選填時間
                if (string.IsNullOrEmpty(jsonData.StartDataListProperty) == true || (string.IsNullOrEmpty(jsonData.EndDataListProperty) == true))
                {
                    returnModel.IsSuccess = false;
                    returnModel.Msg = "請選擇時間";
                    return returnModel;
                }
                DateTime _sDate = new DateTime();
                DateTime _eDate = new DateTime();
                try
                {
                    _sDate = Convert.ToDateTime(jsonData.StartDataListProperty);
                    _eDate = Convert.ToDateTime(jsonData.EndDataListProperty);
                }
                catch (Exception error)
                {
                    dateFormatCheck = false;
                    logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                }
                if (dateFormatCheck == false)
                {
                    returnModel.IsSuccess = false;
                    returnModel.Msg = "時間格式錯誤";
                    return returnModel;
                }
                if (_sDate > _eDate)
                {
                    returnModel.IsSuccess = false;
                    returnModel.Msg = "[Error]: 時間區間錯誤";
                    return returnModel;
                }
                else
                {
                    returnModel.Body.startDate = _sDate;
                    returnModel.Body.endDate = _eDate;
                }
            }
            #endregion 時間區間判斷
            //初始化所選擇庫存搜索條件
            returnModel.Body.canSellQty = (TWNewEgg.API.Models.ItemSketchCanSellQty)Enum.ToObject(typeof(TWNewEgg.API.Models.ItemSketchCanSellQty), jsonData.StockListProperty);
            if (jsonData.ItemCategory1ListProperty != 0)
            {
                returnModel.Body.categoryID_Layer0 = jsonData.ItemCategory1ListProperty;
            }
            if (jsonData.ItemCategory2ListProperty != 0)
            {
                returnModel.Body.categoryID_Layer1 = jsonData.ItemCategory2ListProperty;
            }
            if (jsonData.ItemCategory3ListProperty != 0)
            {
                returnModel.Body.categoryID_Layer2 = jsonData.ItemCategory3ListProperty;
            }
            return returnModel;
        }
        #endregion
        #region 把草稿區搜索回來資料轉換成秀在 view 上面的 model
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.View.itemsketchPropertyExamine>> autoMapData(List<TWNewEgg.API.Models.sketchPropertyExamine> bindData)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.View.itemsketchPropertyExamine>> result = new ActionResponse<List<itemsketchPropertyExamine>>();
            result.Body = new List<itemsketchPropertyExamine>();
            foreach (var item in bindData)
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.sketchPropertyExamine, TWNewEgg.API.View.itemsketchPropertyExamine>();
                TWNewEgg.API.View.itemsketchPropertyExamine autoData = AutoMapper.Mapper.Map<TWNewEgg.API.Models.sketchPropertyExamine, TWNewEgg.API.View.itemsketchPropertyExamine>(item);
                TWNewEgg.API.View.CategoryViewModel cateTempData = new CategoryViewModel();
                #region 組合 Kendo 的 dropdownlist 用
                if (item.Item.ShipType == "N")
                {
                    cateTempData.shiptype = "Newegg";
                    cateTempData.shiptypeCode = "N";
                }
                else
                {
                    cateTempData.shiptype = "Seller";
                    cateTempData.shiptypeCode = "S";
                }
                #endregion
                //有自定義顏色就秀出自定義顏色, 沒有就秀出預設值
                autoData.color = string.IsNullOrEmpty(item.definitions) == true ? item.color : item.definitions;
                //autoData.color = item.color;
                autoData.size = item.size;
                autoData.groupid = item.group_id;
                //autoData.inputValue = item.definitions;
                autoData.startDate = item.Item.DateStart;
                autoData.Category = cateTempData;
                result.Body.Add(autoData);
            }
            return result;
        }
        #endregion
        #region 刪除
        [HttpPost]
        public JsonResult DeleteSketchPropertyList(List<int> deleteId)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            logger.Info("deleteId = " + deleteId);
            //判斷是否有傳入deleteId
            if (deleteId == null)
            {
                return Json("刪除資料處理錯誤");
            }
            try
            {
                result = connector.propertySketchExamineDelete(deleteId);
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "刪除失敗，請稍後再試";
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (result.IsSuccess == false)
            {
                return Json(result.Msg);
            }
            else
            {
                return Json("刪除成功");
            }
            
        }
        #endregion
        #region 修改(GRID)
        [HttpPost]
        public JsonResult UpdateSketchPropertyList(string _jsonupdateData)
        {
            bool isSucess = true;
            List<TWNewEgg.API.View.itemsketchPropertyExamine> _listSkPropertyEx = new List<itemsketchPropertyExamine>();
            List<TWNewEgg.API.Models.ItemSketch> _listItemSketch = new List<ItemSketch>();
            ActionResponse<string> updateResult = new ActionResponse<string>();
            TWNewEgg.API.View.Service.CheckModelElememtTypeService chMoElTyService = new Service.CheckModelElememtTypeService();
            List<string> returnContent = new List<string>();
            bool isSuccessEdit = true;
            try
            {
                _listSkPropertyEx = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TWNewEgg.API.View.itemsketchPropertyExamine>>(_jsonupdateData);
                _listItemSketch = this.autoMapFromPropertyExamineToItemSketch(_listSkPropertyEx).Body;
                isSucess = true;
            }
            catch (Exception error)
            {
                isSucess = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (isSucess == false)
            {
                returnContent.Add("F");
                returnContent.Add("修改處理資料失敗，是否有資料欄位填錯格式");
                return Json(returnContent);
            }

            bool UserType = sellerInfo.IsAdmin;

            foreach (var item in _listSkPropertyEx)
            {
                int itemid = item.Item.ItemID.GetValueOrDefault();
                int productid = item.Product.ProductID.GetValueOrDefault();
                if (itemid == 0 || productid == 0)
                {
                    returnContent.Add("F");
                    returnContent.Add("無新蛋賣場編號和新蛋產品編號不能進行修改");
                    isSuccessEdit = false;
                    break;
                }

                if (item.Item.ShowOrder == (int)TWNewEgg.DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart
                && UserType == false)
                {
                    isSuccessEdit = false;
                    returnContent.Add("F");
                    returnContent.Add("修改的商品包含加價購商品，請聯繫相關 PM 編輯!");
                    break;
                }
            }
            if (isSuccessEdit == false)
            {               
                return Json(returnContent);
            }
            ActionResponse<string> checkElementResult = chMoElTyService.modelStatusCheckAttr(_listItemSketch, "propertyListCheck");
            if (checkElementResult.IsSuccess == false)
            {
                returnContent.Add("F");
                returnContent.Add(checkElementResult.Msg);
                return Json(returnContent);
            }
            try
            {
                updateResult = connector.propertySketchExamineUpdate(_listItemSketch);
                isSuccessEdit = true;
            }
            catch (Exception error)
            {
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                isSuccessEdit = false;
            }
            if (isSuccessEdit == false)
            {
                returnContent.Add("F");
                returnContent.Add("修改資料發生錯誤，請稍後再試");
                return Json(returnContent);
            }
            else
            {
                returnContent.Add("T");
                returnContent.Add(updateResult.Msg);
                return Json(returnContent);
            }
        }
        #endregion

        #region 詳細內容修改

        public ActionResult TwoDimensionProductPendingDetailEdit(int itemTempID)
        {
            TWNewEgg.API.View.ItemCreationVM creationVM = new ItemCreationVM();

            #region 讀取編輯資料

            TWNewEgg.API.Models.ItemSketchSearchCondition searchCondition = new ItemSketchSearchCondition();
            searchCondition.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemTempID;
            searchCondition.KeyWord = itemTempID.ToString();
            searchCondition.SellerID = sellerInfo.currentSellerID;
            searchCondition.IsAdmin = sellerInfo.IsAdmin;

            TWNewEgg.API.Models.ActionResponse<List<sketchPropertyExamine>> getPendingData = new ActionResponse<List<sketchPropertyExamine>>();

            TWNewEgg.API.Models.Connector connector = new Connector();
            try
            {
                getPendingData = connector.propertySketchExamineSearch(searchCondition, false);
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("讀取待審資料失敗(exception); ItemTempID = {0}; ExceptionMessage = {1}; StackTrace = {2}.", itemTempID, GetExceptionMessage(ex), ex.StackTrace));
            }

            #endregion 讀取編輯資料

            if (getPendingData.IsSuccess && getPendingData.Body.Count == 1)
            {
                AutoMapper.Mapper.CreateMap<sketchPropertyExamine, ItemCreationVM>();
                TWNewEgg.API.View.Service.AES aes = new Service.AES();

                try
                {
                    AutoMapper.Mapper.Map(getPendingData.Body[0], creationVM);

                    creationVM.AesInventoryQtyReg = aes.AesEncrypt(creationVM.ItemStock.InventoryQtyReg.ToString());
                    creationVM.AesItemQtyReg = aes.AesEncrypt(creationVM.Item.ItemQtyReg.ToString());
                }
                catch (Exception ex)
                {
                    logger.Info(string.Format("轉換待審資料失敗(exception); ItemTempID = {0}; ExceptionMessage = {1}; StackTrace = {2}.", itemTempID, GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            ViewBag.readDataSuccess = getPendingData.IsSuccess;

            ViewBag.userType = sellerInfo.IsAdmin;

            return PartialView(creationVM);
        }

        /// <summary>
        /// 產生各層分類下拉式選單UI
        /// </summary>
        /// <returns>各層分類下拉式選單UI</returns>
        [HttpGet]
        public ActionResult Category(TWNewEgg.API.Models.ItemSketch_ItemCategory itemCategory)
        {
            return PartialView("~/Views/TwoDimensionProductCreation/Category.cshtml", itemCategory);
        }
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult ProductProperty(int? categoryID, int? ProductID)
        {
            bool isSuccess = true;

            List<PropertyResult> propertyResultCell = new List<PropertyResult>();
            List<PropertyResult> propertyResult = new List<PropertyResult>();
            List<SaveProductProperty> sppList = new List<SaveProductProperty>();
            API.Models.Connector apiConnector = new Connector();
            if (categoryID.HasValue && categoryID > 0)
            {

                try
                {
                    API.Models.ActionResponse<List<API.Models.PropertyResult>> getPropertyResult = apiConnector.GetProperty(null, null, categoryID.Value);
                    if (getPropertyResult.IsSuccess)
                    {
                        propertyResultCell = getPropertyResult.Body;
                    }
                    else
                    {
                        isSuccess = false;
                        logger.Info(string.Format("取得商品屬性清單失敗; CategoryID = {1}; APIErrorMessage = {2}.", categoryID, getPropertyResult.Msg));
                    }
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    logger.Info(string.Format("取得商品屬性清單失敗; CategoryID = {1}; ErrorMessage = {2}; StackTrace = {3}.", categoryID, GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            try
            {
                if (ProductID.HasValue)
                {
                    sppList = apiConnector.GetProductPropertyTemp(ProductID.Value).Body;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QueryProductProperty ProductTempID.HasValue [ErrorMessage] " + ex.Message + " [StackTrace] " + ex.StackTrace);
            }
            List<ProductPropertyEdit> result = new List<ProductPropertyEdit>();
            if (propertyResultCell != null && sppList != null)
            {
                result = (from pr in propertyResultCell.ToList()
                          join spp in sppList.ToList() on pr.PropertyID equals spp.PropertyID into ps
                          from spp in ps.DefaultIfEmpty()
                          select new ProductPropertyEdit
                          {
                              CategoryID = pr.CategoryID,
                              GroupID = pr.GroupID,
                              PropertyID = pr.PropertyID,
                              PropertyName = pr.PropertyName,
                              ValueInfo = pr.ValueInfo,
                              ValueOption = ((spp == null || string.IsNullOrWhiteSpace(spp.InputValue)) ? 0 : 1),
                              ValueID = ((spp == null || !string.IsNullOrWhiteSpace(spp.InputValue)) ? 0 : spp.ValueID),
                              InputValue = (spp == null ? "" : spp.InputValue),
                          }
                 ).ToList();
            }
            ViewBag.propertyDataList = result;

            string partialView = string.Empty;

            if (isSuccess)
            {
                partialView = RenderView("ProductProperty");
            }

            if (propertyResultCell.Count == 0 || isSuccess == false)
            {
                partialView = "<div id=\"withoutPropertyList\">" +
                                  "<br />" +
                                  "<span>查無所選類別屬性資料，請洽管理員或PM建立相關類別屬性。</span>" +
                              "</div>";
            }

            return Json(new { IsSuccess = isSuccess, ViewHtml = partialView }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        private string RenderView(string partialView)
        {
            string result = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                try
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    result = sw.GetStringBuilder().ToString();
                }
                catch (Exception ex)
                {
                    logger.Info(string.Format("將 View 轉成 string 時發生錯誤(expection); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            return result;
        }

        #endregion 詳細內容修改

        public ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> autoMapFromPropertyExamineToItemSketch(List<TWNewEgg.API.View.itemsketchPropertyExamine> _listItemPropertyEx)
        {
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            result.Body = new List<ItemSketch>();
            AutoMapper.Mapper.CreateMap<TWNewEgg.API.View.itemsketchPropertyExamine, TWNewEgg.API.Models.ItemSketch>();
            foreach (var item in _listItemPropertyEx)
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.View.itemsketchPropertyExamine, TWNewEgg.API.Models.ItemSketch>();
                TWNewEgg.API.Models.ItemSketch autoData = AutoMapper.Mapper.Map<TWNewEgg.API.View.itemsketchPropertyExamine, TWNewEgg.API.Models.ItemSketch>(item);
                autoData.Item.ShipType = item.Category.shiptypeCode;
                autoData.Item.DateStart = item.startDate.GetValueOrDefault();
                autoData.CreateAndUpdate.UpdateUser = sellerInfo.UserID;
                result.Body.Add(autoData);
            }

            return result;
        }
        #endregion

        #region 待審區資料匯出 Excel
        [HttpPost]
        public JsonResult excelSearchProperty(TWNewEgg.API.View.JsonDataSketchPropertyModel jsonData, bool isSearch = true)
        {
            ActionResponse<string> haserrorMsg = new ActionResponse<string>();
            TWNewEgg.API.Models.ItemSketchSearchCondition bindData = new ItemSketchSearchCondition();
            ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> listSketchPropertyExamine = new ActionResponse<List<sketchPropertyExamine>>();
            ActionResponse<string> Excelresult = new ActionResponse<string>();
            bool isExcelSuccess = true;
            try
            {
                bindData = this.bindDataExamineSketch(jsonData).Body;
                //是否為管理者
                bindData.IsAdmin = sellerInfo.IsAdmin;
                //欲查詢的sellerID
                bindData.SellerID = sellerInfo.currentSellerID;
                //搜索的條件選擇
                bindData.KeyWordScarchTarget = (ItemSketchKeyWordSearchTarget)Enum.ToObject(typeof(ItemSketchKeyWordSearchTarget), jsonData.searchRequestListProperty);
                Excelresult = connector.excelSearchProperty(bindData);
            }
            catch (Exception error)
            {
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                isExcelSuccess = false;
                //haserrorMsg.IsSuccess = false;
                //haserrorMsg.Msg = "匯出 Excel 失敗，請稍後再試";
            }
            if (isExcelSuccess == false)
            {
                return Json(new { ststus = "F", message = "匯出 Excel 失敗", url = "" });
            }
            if (Excelresult.IsSuccess == false)
            {
                return Json(new { _Ststus = "F", message = Excelresult.Msg, url = "" });
            }
            else
            {
                return Json(new { _Ststus = "T", message = Excelresult.Msg, url = Excelresult.Body });
            }
        }
        #endregion

        #region 草稿待審共用
        public JsonResult GetManufacturerList()
        {
            TWNewEgg.API.View.Service.GetManufacturerListService getManuListService = new Service.GetManufacturerListService();
            var result = getManuListService.GetManufacturer();
            if (result.IsSuccess == true)
            {
                return Json(result.Body, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
        #endregion

    }
    
}
