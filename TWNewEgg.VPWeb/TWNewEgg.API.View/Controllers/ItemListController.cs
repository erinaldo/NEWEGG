using KendoGridBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.View.Attributes;
using Kendo.Mvc.Extensions;
using AutoMapper;
using Kendo.Mvc.UI;


namespace TWNewEgg.API.View.Controllers
{
    public class ItemListController : Controller
    {
        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        TWNewEgg.API.Models.Connector connector = new Connector();
        private log4net.ILog logger;

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
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }

            return result;
        }


        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.ItemList)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("商品列表")]
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
        [HttpPost]
        public JsonResult GetItemList(KendoGridRequest request, string keywd)
        {
            Connector conn = new Connector();
            TWNewEgg.API.Models.ItemSearchCondition searchModel = new ItemSearchCondition();
            List<TWNewEgg.API.Models.ItemInfoList> itemInfo = new List<ItemInfoList>();

            int totalItems = 0;

            searchModel.SearchMode = 4;
            searchModel.SellerID = sellerInfo.currentSellerID;
            searchModel.Keyword = keywd;
            searchModel.PageInfo.PageIndex = request.Page - 1;
            searchModel.PageInfo.PageSize = request.PageSize;

            //searchModel.PageInfo.PageIndex = request.Page;
            //searchModel.PageInfo.PageSize = request.PageSize;
            try
            {
                var result = conn.APIGetItemList(null, null, searchModel);

                if (result.Body != null)
                {
                    itemInfo = result.Body;
                    totalItems = Convert.ToInt32(result.Msg);
                }
            }
            catch (Exception)
            {
                
                throw;
            }


            return Json(new { itemInfo, total = totalItems });
        }
        
        public JsonResult ManufactureNameSearch(string text/*, string isQuery*/)
        {
            TWNewEgg.API.Models.SearchDataModel searchData = new SearchDataModel();
            List<TWNewEgg.API.View.ItemListManufacturer> _itemManufacturer = new List<ItemListManufacturer>();
            TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> _manufacturer = new ActionResponse<List<Manufacturer>>();
            List<Manufacturer> result = new List<Manufacturer>();
            Manufacturer _insertMan = new Manufacturer();
            searchData.SearchType = TWNewEgg.API.Models.SearchType.SearchofficialALLInfo;
            try
            {
                result = connector.SearchManufacturerInfo(searchData).Body;
                if (text!=null/*isQuery == "true"*/)
                {
                    result = result.Where(x => x.ManufactureName.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                }
                //_insertMan.SN = -1;
                //_insertMan.ManufactureName = "";
                //result.Insert(0, _insertMan);
            }
            catch (Exception error)
            {
            }
                var resultM = result.Select(p => new TWNewEgg.API.View.ItemListManufacturer
                {
                    ManufactureName = p.ManufactureName,
                    SN = p.SN
                });
                resultM = resultM.OrderBy(p => p.ManufactureName).ToList();
                return Json(resultM, JsonRequestBehavior.AllowGet);
            
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_jsonupdateData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult update(string _jsonupdateData)
        {
            System.Text.RegularExpressions.Regex numberReg = new System.Text.RegularExpressions.Regex(@"^-?\d+$");
            TWNewEgg.API.View.Service.CheckModelElememtTypeService checkAttr = new Service.CheckModelElememtTypeService();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            bool jsonstringCheck = true;
            List<TWNewEgg.API.Models.ItemSketch> modifyData = new List<ItemSketch>();
            List<TWNewEgg.API.View.ItemSketchSelect> _listModifyData = new List<ItemSketchSelect>();
            List<string> responseMsg = new List<string>();
            ActionResponse<string> checkAttrAction = new ActionResponse<string>();
            checkAttrAction.IsSuccess = true;
            try
            {
                //Regex numberReg = new Regex(@"^-?\d+$");
                _listModifyData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TWNewEgg.API.View.ItemSketchSelect>>(_jsonupdateData);
                modifyData = autoMapItemSketch(_listModifyData);
                checkAttrAction = checkAttr.modelStatusCheckAttr(modifyData, "sketchFirst");
            }
            catch (Exception error)
            {
                logger.Error("/ItemList/update error : " + error.Message);
                jsonstringCheck = false;
            }
            //檢查格式
            if (checkAttrAction.IsSuccess == false)
            {
                responseMsg.Add("Updata");
                responseMsg.Add("CheckAttr");
                responseMsg.Add(checkAttrAction.Msg);
                return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = responseMsg });
            }
            //Exception 錯誤
            if (jsonstringCheck == false)
            {
                responseMsg.Add("Updata");
                responseMsg.Add("DataError");
                responseMsg.Add("資料錯誤");
                return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = responseMsg });
            }
            try
            {
                var EditResult = connector.EditItemSketch(ItemSketchEditType.ListEdit, modifyData);
                if (EditResult.Code == (int)ResponseCode.Success && EditResult.IsSuccess == true)
                {
                    responseMsg.Add("Updata");
                    responseMsg.Add("DataSuccess");
                    responseMsg.Add("修改完成");
                }
                else
                {
                    responseMsg.Add("Updata");
                    responseMsg.Add("DataError");
                    responseMsg.Add(EditResult.Msg);
                }
            }
            catch (Exception error)
            {
                logger.Error("/ItemList/update error: " + error.Message);
                responseMsg.Add("Updata");
                responseMsg.Add("DataError");
                responseMsg.Add("資料錯誤");
            }
            return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = responseMsg });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeleteId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ItemStetchDelete(int DeleteId)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            ActionResponse<string> result = new ActionResponse<string>();
            string returnMsg = string.Empty;
            try
            {
                //connect to delete api
                result = connector.DeleteStetch(DeleteId);
            }
            catch (Exception error)
            {
                //connect to api error
                returnMsg = "[Error]: 資料錯誤";
                logger.Error("ItemList/ItemStetchDelete/ api error: " + error.Message);
            }
            //delete success
            if (result.IsSuccess == true && result.Code == (int)ResponseCode.Success)
            {
                returnMsg = "[Success]: 刪除成功";
            }
            else if (result.IsSuccess == false && result.Code == (int)ResponseCode.Error)
            {//some reason that delete error
                returnMsg = "[Error]: 刪除失敗";
            }
            else
            {
                returnMsg = "[Error]: 資料錯誤";
                //record the error message that not focus
                logger.Error("ItemList/ItemStetchDelete/ else statement error");
            }
            return Json(returnMsg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManufactureID"></param>
        /// <param name="DateType"></param>
        /// <param name="StartData"></param>
        /// <param name="EndData"></param>
        /// <param name="Stock"></param>
        /// <param name="ItemCategory1"></param>
        /// <param name="ItemCategory2"></param>
        /// <param name="ItemCategory3"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketchSearchCondition> BindDataAction(int? ManufactureID, int DateType, string StartData, string EndData, int Stock, int? ItemCategory1, int? ItemCategory2, int? ItemCategory3, int CheckStatus, int GoodsStatus, int? ShowOrder, string IsRecover)
        {
            TWNewEgg.API.Models.ItemSketchSearchCondition bindData = new ItemSketchSearchCondition();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketchSearchCondition> result = new ActionResponse<ItemSketchSearchCondition>();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            try
            {
                //是否有選擇加價購狀態
                if (ShowOrder != null)
                {
                    bindData.ShowOrder = ShowOrder;
                }
                //是否有選擇審核狀態
                if (CheckStatus != -1)
                {
                    bindData.Status = CheckStatus;
                }
                //是否有商品狀態
                if (GoodsStatus != -1)
                {
                    bindData.ItemStatus = GoodsStatus;
                }
                //判斷是否有選擇製造商
                if (ManufactureID != null)
                {
                    bindData.ManufactureID = ManufactureID.GetValueOrDefault(); ;
                }
                if (IsRecover == "Y") 
                {
                    bindData.IsRecover = IsRecover;
                
                
                }
                //初始化所選擇"創建日期"的選擇條件
                bindData.createDate = (TWNewEgg.API.Models.ItemSketchCreateDate)Enum.ToObject(typeof(TWNewEgg.API.Models.ItemSketchCreateDate), DateType);
                
                //如果創建日期選擇的是指定日期
                if (bindData.createDate == TWNewEgg.API.Models.ItemSketchCreateDate.SpecifyDate)
                {
                    //判斷是時間是否有選擇
                    if (string.IsNullOrEmpty(StartData) == false)
                    {
                        DateTime _sDate = Convert.ToDateTime(StartData);
                        bindData.startDate = _sDate;
                        bindData.endDate = _sDate;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "[Error]: 請填寫時間";
                        return result;
                    }
                }

                else if (bindData.createDate == TWNewEgg.API.Models.ItemSketchCreateDate.DateRange)
                {//如果創建日期選擇的是時間範圍
                    //判斷是否有選擇開始時間
                    if (string.IsNullOrEmpty(StartData) == true)
                    {
                        result.IsSuccess = false;
                        result.Msg = "[Error]: 請填寫時間";
                        return result;
                    }
                    else if (string.IsNullOrEmpty(EndData) == true)
                    {//判斷是否有選擇結束時間
                        result.IsSuccess = false;
                        result.Msg = "[Error]: 請填寫時間";
                        return result;
                    }
                    else
                    {
                        //開始時間跟結束時間都有選擇
                        DateTime _sDate = Convert.ToDateTime(StartData);
                        DateTime _eDate = Convert.ToDateTime(EndData);
                        if (_sDate > _eDate)
                        {
                            result.IsSuccess = false;
                            result.Msg = "[Error]: 時間區間錯誤";
                            return result;
                        }
                        else
                        {
                            bindData.startDate = _sDate;
                            bindData.endDate = _eDate;
                        }
                    }
                }
                //初始化所選擇庫存搜索條件
                bindData.canSellQty = (TWNewEgg.API.Models.ItemSketchCanSellQty)Enum.ToObject(typeof(TWNewEgg.API.Models.ItemSketchCanSellQty), Stock);
                //判斷是否有選擇主分類一
                if (ItemCategory1 != null)
                {
                    bindData.categoryID_Layer0 = ItemCategory1.GetValueOrDefault();
                }
                //判斷是否有選擇主分類二
                if (ItemCategory2 != null)
                {
                    bindData.categoryID_Layer1 = ItemCategory2.GetValueOrDefault();
                }
                //判斷是否有選擇主分類三
                if (ItemCategory3 != null)
                {
                    bindData.categoryID_Layer2 = ItemCategory3.GetValueOrDefault();
                }
                result.Body = bindData;
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "[Error]: 資料錯誤";
                logger.Error("/ItemList/BindDataAction error: " + error.Message);
            }
            return result;
        }
        [HttpPost]
        public JsonResult read([Kendo.Mvc.UI.DataSourceRequest] Kendo.Mvc.UI.DataSourceRequest request, int SearCon, string searchText, int? ManufactureID, int DateType, string StartData, string EndData, int Stock, int? ItemCategory1, int? ItemCategory2, int? ItemCategory3)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.ItemSketchSearchCondition _itemSketchSearchCondition = new ItemSketchSearchCondition();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketchSearchCondition> result = new ActionResponse<ItemSketchSearchCondition>();
            ActionResponse<List<ItemSketch>> searchresult = new ActionResponse<List<ItemSketch>>();
            List<TWNewEgg.API.View.ItemSketchSelect> _itemSketchSelect = new List<ItemSketchSelect>();
            List<string> returnMsg = new List<string>();
            bool tryflag = true;
            //利用BindDataAction檢查資料完整性
            result = BindDataAction(ManufactureID, DateType, StartData, EndData, Stock, ItemCategory1, ItemCategory2, ItemCategory3, -1, -1,null,"N");
            string _strErrorMsg = string.Empty;
            //資料檢查無誤
            if (result.IsSuccess == true)
            {
                //是否為管理者
                result.Body.IsAdmin = sellerInfo.IsAdmin;
                //欲查詢的sellerID
                result.Body.SellerID = sellerInfo.currentSellerID;
                //要查詢的相關資訊
                result.Body.KeyWord = searchText;
                try
                {
                    result.Body.KeyWordScarchTarget = (ItemSketchKeyWordSearchTarget)Enum.ToObject(typeof(ItemSketchKeyWordSearchTarget), SearCon);
                    //connect to GetItemSketchList api
                    searchresult = connector.GetItemSketchListRemoveDes(result.Body);
                    //api return error data
                    if (searchresult.IsSuccess == false)
                    {
                        tryflag = false;
                        returnMsg.Add("Read");
                        returnMsg.Add(searchresult.Msg);
                    }
                    else
                    {
                        //若為vender濾掉加價購草稿
                        var UserInfo = sellerInfo.IsAdmin;
                        if (!UserInfo)
                        {
                            searchresult.Body = searchresult.Body.Where(x => x.Item.ShowOrder != (int)AdditionalPurchase.ShowOrderType.加價購).ToList();
                        }
                        //has datas
                        if (searchresult.Body.Count != 0)
                        {
                            _itemSketchSelect = autoMapItemSketchSelect(searchresult.Body);
                            foreach (var index in _itemSketchSelect)
                            {
                                index.Item.Sdesc = "";
                                index.Product.Description = "";
                                index.Item.Spechead = "";
                                index.ItemStock.CanSaleQty = index.ItemStock.InventoryQty;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    tryflag = false;
                    returnMsg.Add("Read");
                    returnMsg.Add("資料錯誤");
                    logger.Error("/ItemList/read error: " + error.Message);
                }
                if (tryflag == true)
                {
                    return Json(_itemSketchSelect.ToDataSourceResult(request));
                }
                else
                {
                    return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnMsg });
                }
            }
            else
            {
                returnMsg.Add("Read");
                returnMsg.Add(result.Msg);
                
                //return message to view to alert to user
                return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnMsg });
            }
        }

        [HttpPost]
        public JsonResult ToCheck(List<int> _intCheck)
        {
            string userid = sellerInfo.UserID.ToString();
            string _strReturnMsg = string.Empty;
            if (_intCheck == null)
            {
                return Json("[Error]: 請勾選欲審核的資料");
            }
            ActionResponse<string> result = new ActionResponse<string>();
            try
            {
                //connect to verifyStetch api
                result = connector.VerifyStetch(_intCheck, userid);
                if (result.IsSuccess = true && result.Code == (int)ResponseCode.Success)
                {
                    _strReturnMsg = "[Success]: " + result.Msg;
                }
                else if (result.IsSuccess == false && result.Code == (int)ResponseCode.Error)
                {
                    _strReturnMsg = "[Error]: " + result.Msg;
                }
                else
                {
                    _strReturnMsg = "資料錯誤";
                }
            }
            catch (Exception error)
            {
                logger.Error("/ItemList/ToCheck error, api connect error: " + error.Message);
                _strReturnMsg = "[Error]: 資料錯誤";
            }
            return Json(_strReturnMsg);
        }
        public List<TWNewEgg.API.View.ItemSketchSelect> autoMapItemSketchSelect(List<TWNewEgg.API.Models.ItemSketch> toMapData)
        {
            List<TWNewEgg.API.View.ItemSketchSelect> autoResult = new List<ItemSketchSelect>();

            foreach (var item in toMapData)
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.View.ItemSketchSelect>();
                TWNewEgg.API.View.ItemSketchSelect autoData = AutoMapper.Mapper.Map<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.View.ItemSketchSelect>(item);
                TWNewEgg.API.View.CategoryViewModel cateTempData = new CategoryViewModel();
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
                autoData.Category = cateTempData;
                autoResult.Add(autoData);
            }
            return autoResult;
        }
        public List<TWNewEgg.API.Models.ItemSketch> autoMapItemSketch(List<TWNewEgg.API.View.ItemSketchSelect> toMapData)
        {
            List<TWNewEgg.API.Models.ItemSketch> result = new List<ItemSketch>();
            foreach (var item in toMapData)
            {
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.View.ItemSketchSelect, TWNewEgg.API.Models.ItemSketch>();
                TWNewEgg.API.Models.ItemSketch autoData = AutoMapper.Mapper.Map<TWNewEgg.API.View.ItemSketchSelect, TWNewEgg.API.Models.ItemSketch>(item);
                autoData.Item.ShipType = item.Category.shiptypeCode;
                autoData.CreateAndUpdate.CreateUser = sellerInfo.UserID;
                autoData.CreateAndUpdate.UpdateUser = sellerInfo.UserID;
                result.Add(autoData);
            }
            return result;
        }

        #region RequestList function area

        TWNewEgg.API.View.ServiceAPI.APIConnector apiConn = new ServiceAPI.APIConnector();

        [HttpPost]
        public JsonResult readList([Kendo.Mvc.UI.DataSourceRequest] Kendo.Mvc.UI.DataSourceRequest request, int SearCon, string searchText, int? ManufactureID, int DateType, string StartData, string EndData, int Stock, int? ItemCategory1, int? ItemCategory2, int? ItemCategory3, int CheckStatus, int GoodsStatus, int? ShowOrder, string IsRecover)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.ItemSketchSearchCondition _itemSketchSearchCondition = new ItemSketchSearchCondition();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketchSearchCondition> result = new ActionResponse<ItemSketchSearchCondition>();
            List<TWNewEgg.API.Models.ItemSketch> _readResult = new List<ItemSketch>();
            List<TWNewEgg.API.View.ItemSketchSelect> _itemSketchSelect = new List<ItemSketchSelect>();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> searchResult = new ActionResponse<List<ItemSketch>>();
            result = BindDataAction(ManufactureID, DateType, StartData, EndData, Stock, ItemCategory1, ItemCategory2, ItemCategory3, CheckStatus, GoodsStatus, ShowOrder, IsRecover);
            //用於回傳到前端的訊息
            List<string> returnKendoMsg = new List<string>();
            returnKendoMsg.Add("Search");
            if (result.IsSuccess == true)
            {

                //是否為管理者
                result.Body.IsAdmin = sellerInfo.IsAdmin;
                //欲查詢的sellerID
                result.Body.SellerID = sellerInfo.currentSellerID;
                //要查詢的相關資訊
                result.Body.KeyWord = searchText;
                //搜索的條件選擇
                result.Body.KeyWordScarchTarget = (ItemSketchKeyWordSearchTarget)Enum.ToObject(typeof(ItemSketchKeyWordSearchTarget), SearCon);

                result.Body.pageInfo.PageIndex = request.Page - 1;

                result.Body.pageInfo.PageSize = request.PageSize;

                try
                {
                    //Connect to api
                    searchResult = connector.GetItemTempList(result.Body, false);
                    //searchResult = apiConn.GetItemTempList(result.Body);
                }
                catch (Exception error)
                {
                    logger.Error("ItemList/readList api connect error: " + error.Message + "[StackTrace]" + error.StackTrace);
                    searchResult.IsSuccess = false;
                    searchResult.Msg = "查詢失敗";
                }
                if (searchResult.IsSuccess == true)
                {
                    //logger.Error("API START 查詢結束");
                    //查詢有資料
                    if (searchResult.Body.Count != 0)
                    {
                        //logger.Error("API START 有資料");
                        _itemSketchSelect = autoMapItemSketchSelect(searchResult.Body);
                        //Encode特定欄位，防止某些欄位因為特定符號，造成轉換成Json字串的時候錯誤
                        foreach (var index in _itemSketchSelect)
                        {
                            index.Item.Sdesc = "";
                            index.Product.Description = "";
                            index.Item.Spechead = "";
                        }
                    }
                    else
                    {
                        //logger.Error("API START 無資料");
                        returnKendoMsg.Add("[Error]: 查無資料");
                        return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                    }
                }
                else if (searchResult.IsSuccess == false)
                {
                    returnKendoMsg.Add("[Error]: " + searchResult.Msg);
                    return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                }
                else
                {
                    returnKendoMsg.Add("[Error]: 資料錯誤");
                    return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                }
                return Json(new DataSourceResult { Data = _itemSketchSelect, Total = Convert.ToInt32(searchResult.Msg) });
            }
            else
            {
                returnKendoMsg.Add(result.Msg);
                return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deleteId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteList(List<int> deleteId)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            int _intTemp;
            ActionResponse<string> check_result = new ActionResponse<string>();
            ActionResponse<List<string>> deleteResult = new ActionResponse<List<string>>();
            check_result.IsSuccess = true;
            #region 判斷是否有傳入要刪除資料的ID
            if (deleteId == null)
            {
                return Json("[Error]: 資料錯誤");
            }
            #endregion
            #region 檢查從前端傳回來的資料格式是否確
            foreach (var item in deleteId)
            {
                _intTemp = 0;
                if (int.TryParse(item.ToString(), out _intTemp) == false)
                {
                    check_result.IsSuccess = false;
                    break;
                }
            }
            if (check_result.IsSuccess == false)
            {
                return Json("[Error]: 資料錯誤");
            }
            #endregion
            check_result = null;
            check_result = new ActionResponse<string>();
            try
            {
                //connect to api
                deleteResult = connector.DeleteItemTemp(deleteId);
                if (deleteResult.IsSuccess == false)
                {
                    check_result.IsSuccess = false;
                    check_result.Msg = deleteResult.Msg;
                }
                else
                {
                    check_result.IsSuccess = true;
                    check_result.Msg = deleteResult.Msg;
                }
            }
            catch (Exception error)
            {
                logger.Error("/ItemList/DeleteList error: " + error.Message);
                check_result.IsSuccess = false;
                check_result.Msg = deleteResult.Msg;
            }
            if (check_result.IsSuccess == true)
            {
                return Json(new { isSuccess = "T", Msg = check_result.Msg });
            }
            else
            {
                return Json(new { isSuccess = "F", Msg = check_result.Msg });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_jsonupdateDataList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult updateList(string _jsonupdateDataList)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            List<TWNewEgg.API.View.ItemSketchSelect> _listModifyData = new List<ItemSketchSelect>();
            List<TWNewEgg.API.Models.ItemSketch> MapResult = new List<ItemSketch>();
            ActionResponse<List<string>> updateResult = new ActionResponse<List<string>>(); 
            ActionResponse<string> returnResult = new ActionResponse<string>();
            List<string> returnKendoMsg = new List<string>();
            TWNewEgg.API.View.Service.CheckModelElememtTypeService checkAttr = new Service.CheckModelElememtTypeService();
            bool UserType = sellerInfo.IsAdmin;

            returnKendoMsg.Add("Update");
            returnResult.IsSuccess = true;
            #region 防止 Json 反序列畫錯誤，錯誤則回傳錯誤，並寫入logger
            try
            {
                _listModifyData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TWNewEgg.API.View.ItemSketchSelect>>(_jsonupdateDataList);
                if (_listModifyData == null)
                {
                    returnResult.IsSuccess = false;
                }
            }
            catch (Exception error)
            {
                logger.Error("/ItemList/updateList json DeserializeObject error: " + error.Message);
                returnResult.IsSuccess = false;
            }
            if (returnResult.IsSuccess == false)
            {
                returnKendoMsg.Add("F");
                returnKendoMsg.Add("資料錯誤");
                returnKendoMsg.Add("Body=F");
                return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                //return Json(new { isSuccess = "F", Msg = "資料錯誤", Body = "F" });
            }
            #endregion
            bool ischeckFirst = true;
            foreach (var itemProductID in _listModifyData)
            {
                int itemid = itemProductID.Item.ItemID.GetValueOrDefault();
                int productid = itemProductID.Product.ProductID.GetValueOrDefault();
                if (itemid == 0 || productid == 0)
                {
                    ischeckFirst = false;
                    returnKendoMsg.Add("F");
                    returnKendoMsg.Add("無新蛋賣場編號和新蛋產品編號不能進行修改");
                    returnKendoMsg.Add("Body=F");
                    break;
                }

                if (itemProductID.Item.ShowOrder == (int)TWNewEgg.DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart
                && UserType == false)
                {
                    ischeckFirst = false;
                    returnKendoMsg.Add("F");
                    returnKendoMsg.Add("修改的商品包含加價購商品，請聯繫相關 PM 編輯!");
                    returnKendoMsg.Add("Body=F");
                    break;
                }
            }
            if (ischeckFirst == false)
            {
                return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
            }
            #region connect to api
            try
            {
                MapResult = autoMapItemSketch(_listModifyData);

                // 格式檢查
                var modelCheckResult = checkAttr.modelStatusCheckAttr(MapResult, "sketchFirst");

                if (modelCheckResult.IsSuccess == true)
                {
                    // 送 API 進行修改
                    updateResult = connector.EditListTemp(MapResult);
                }
                else
                {
                    updateResult.IsSuccess = false;
                    updateResult.Msg = modelCheckResult.Msg;
                    updateResult.Code = (int)ResponseCode.Error;
                }
            }
            catch (Exception error)
            {
                returnResult.IsSuccess = false;
                returnResult.Msg = "資料錯誤";
                logger.Error("/ItemList/updateList error: " + error.Message);
            }
            //修改成功
            if (updateResult.IsSuccess == true)
            {
                returnKendoMsg.Add("T");
                returnKendoMsg.Add(updateResult.Msg);
                returnKendoMsg.Add("Body=T");
                return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                //return Json(new { isSuccess = "T", Msg = updateResult.Msg, Body = "F" });
            }
            else
            {
                if (updateResult.IsSuccess == false && updateResult.Code == (int)ResponseCode.AccessError)
                {
                    string _strCombineMsg = string.Empty;
                    foreach (var item in updateResult.Body)
                    {
                        _strCombineMsg = _strCombineMsg + item + "</br>";
                    }
                    returnKendoMsg.Add("F");
                    returnKendoMsg.Add(_strCombineMsg);
                    returnKendoMsg.Add("Body=T");
                    return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                }
                else if (updateResult.IsSuccess == false && updateResult.Code == (int)ResponseCode.Error)
                {
                    returnKendoMsg.Add("F");
                    returnKendoMsg.Add(updateResult.Msg);
                    returnKendoMsg.Add("Body=F");
                    return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                }
                else
                {
                    returnKendoMsg.Add("F");
                    returnKendoMsg.Add("資料錯誤");
                    returnKendoMsg.Add("Body=F");
                    return this.Json(new Kendo.Mvc.UI.DataSourceResult { Errors = returnKendoMsg });
                }
            }
            #endregion

        }

        #endregion

        [HttpPost]
        public ActionResult ItemListToExcel(ItemSearchCondition ToExcel)
        {
            TWNewEgg.API.Models.DownloadItemListModel toexcelModel = new DownloadItemListModel();

            ActionResponse<string> result = new ActionResponse<string>();

            try
            {
                ToExcel.SellerID = sellerInfo.currentSellerID;
                ToExcel.SearchMode = 4;
                ToExcel.CreateDateBefore = ToExcel.CreateDateBefore == 0 ? null : ToExcel.CreateDateBefore;
                ToExcel.Status = ToExcel.Status == -1 ? null : ToExcel.Status;

                toexcelModel.fileName = "商品清單匯出";
                toexcelModel.sheetName = "WorkSheet";
                toexcelModel.titleLine = 2;
                toexcelModel.itemSearchCondition = ToExcel;
                result = connector.DownloadItemList(toexcelModel);              
    }
            catch (Exception ex)
            {
                logger.Error("Msg: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                return Json(new { Msg = "[Error]: " + "發生意外錯誤，請稍後在試!", Url = "" });               
}

            //TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>> SearchResult = _connector.QueryOrderInfos<List<API.Models.OrderInfo>>("", "", _queryCartCondition);
            if (result.IsSuccess == true)
            {
                return Json(new { Msg = "[Success]: " + result.Msg, Url = result.Body });
            }
            else
            {
                return Json(new { Msg = "[Error]: " + result.Msg, Url = "" });
            }
        }
        [HttpPost]
        public ActionResult ItemListExportToExcel(ItemSearchCondition ToExcel)
        {
            TWNewEgg.API.Models.DownloadItemListModel toexcelModel = new DownloadItemListModel();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            ActionResponse<string> result = new ActionResponse<string>();

            try
            {
                ToExcel.SellerID = sellerInfo.currentSellerID;
                ToExcel.SearchMode = 4;
                ToExcel.CreateDateBefore = ToExcel.CreateDateBefore == 0 ? null : ToExcel.CreateDateBefore;
                ToExcel.Status = ToExcel.Status == -1 ? null : ToExcel.Status;

                toexcelModel.fileName = "商品清單匯出";
                toexcelModel.sheetName = "WorkSheet";
                toexcelModel.titleLine = 2;
                toexcelModel.itemSearchCondition = ToExcel;
                result = connector.DownloadItemListToExcel(toexcelModel);
            }
            catch (Exception ex)
            {
                logger.Error("Msg: " + ex.Message + ", StackTrace: " + ex.StackTrace);
                return Json(new { Msg = "[Error]: " + "發生意外錯誤，請稍後在試!", Url = "" });
            }

            //TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>> SearchResult = _connector.QueryOrderInfos<List<API.Models.OrderInfo>>("", "", _queryCartCondition);
            if (result.IsSuccess == true)
            {
                return Json(new { Msg = "[Success]: " + result.Msg, Url = result.Body });
            }
            else
            {
                return Json(new { Msg = "[Error]: " + result.Msg, Url = "" });
            }
        }
    }
}
