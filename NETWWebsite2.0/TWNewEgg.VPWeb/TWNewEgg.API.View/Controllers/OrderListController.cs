using AutoMapper;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using KendoGridBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.View.Attributes;
using TWNewEgg.API.View.Service;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.API.View.Controllers
{
    /// <summary>
    /// 訂單清單
    /// </summary>
    public partial class OrderListController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region 主單

        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageOrder)]
        [FunctionName(FunctionNameAttributeValues.OrderList)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [Filter.PermissionFilter]
        //[Filter.LoginAuthorize]
        [ActionDescription("訂單清單")]
        public ActionResult Index()
        {
            OrderMainPage viewModel = new OrderMainPage();
            ViewBag.viewModel = viewModel;

            return View();
        }

        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageOrder)]
        [FunctionName(FunctionNameAttributeValues.OrderList)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [Filter.PermissionFilter]
        [ActionDescription("訂單清單")]
        public ActionResult GetMainOrder([DataSourceRequest] DataSourceRequest request, MainOrderSearchCondition searchCondition)
        {
            #region 變數宣告

            // API 的搜尋絛件 Model
            TWNewEgg.API.Models.MainOrderSearchCondition searchCondition_API = new API.Models.MainOrderSearchCondition();

            // 建立搜尋條件 AutoMapper 規則
            AutoMapper.Mapper.CreateMap<MainOrderSearchCondition, TWNewEgg.API.Models.MainOrderSearchCondition>();

            // 連接使用者資訊 Service
            SellerInfoService sellerInfoService = new SellerInfoService();

            // 連接 API
            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();

            // 訂單主單查詢結果
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.MainOrderResult> result = new API.Models.ActionResponse<TWNewEgg.API.Models.MainOrderResult>();

            // Kendo Grid
            DataSourceResult kendoDataSourceResult;

            #endregion 變數宣告

            #region 設定搜尋條件

            #region 轉換 search condition model

            try
            {
                searchCondition_API = AutoMapper.Mapper.Map<TWNewEgg.API.Models.MainOrderSearchCondition>(searchCondition);
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("轉換 search condition model 失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                return Json(MainOrderSearchError());
            }

            #endregion 轉換 search condition model

            // 設定 SellerID
            searchCondition_API.SellerID = sellerInfoService.currentSellerID;

            // 若有輸入關鍵字，清除輸入值前後的空白
            if (!string.IsNullOrEmpty(searchCondition_API.KeyWord))
            {
                searchCondition_API.KeyWord = searchCondition_API.KeyWord.Trim();
            }

            // 排除不需要的日期設定
            switch (searchCondition_API.CreateDateSearchType)
            {
                default:
                    {
                        searchCondition_API.StartDate = null;
                        searchCondition_API.EndDate = null;
                        break;
                    }
                case (int)RetgoodCreateDateSearchType.指定日期:
                    {
                        searchCondition_API.EndDate = null;
                        break;
                    }
                case (int)RetgoodCreateDateSearchType.定制日期範圍:
                    {
                        break;
                    }
            }

            // 設定分頁
            searchCondition_API.PageInfo.PageIndex = request.Page - 1;

            // 設定資料筆數
            searchCondition_API.PageInfo.PageSize = request.PageSize;

            #endregion 設定搜尋條件

            #region 取得 API 資料

            try
            {
                result = connector.GetMainOrder(searchCondition_API);
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("取得 API 資料失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                return Json(MainOrderSearchError());
            }

            if (result == null || result.Body == null || result.Body.Grid == null)
            {
                if (result == null)
                {
                    logger.Info(string.Format("取得 API 訂單主單清單失敗; ErrorMessage = {0}.", "ActionResponse is null"));
                }

                return Json(MainOrderSearchError());
            }

            #endregion 取得 API 資料

            #region 顯示查詢結果

            if (result.Body.Grid.Count == 0)
            {
                return Json("查無此資料，請重新確認！");
            }

            // 資料的分頁、筆數在 API 已經處理完成，所以結果都只有第 1 個分頁
            request.Page = 1;

            kendoDataSourceResult = result.Body.Grid.ToDataSourceResult(request);

            kendoDataSourceResult.Total = result.Body.DataCount;

            return Json(kendoDataSourceResult);

            #endregion 顯示查詢結果
        }

        /// <summary>
        /// 主單查詢失敗訊息
        /// </summary>
        private string MainOrderSearchError()
        {
            return "查詢時發生錯誤，請稍後再試；若仍發生此錯誤，請聯繫客服人員。";
        }

        #endregion 主單

        /// <summary>
        /// 增加一個描述用
        /// </summary>
        /// <param name="_orderInfo">OrderInfo Model</param>
        /// <returns>要秀出畫面的model</returns>
        public List<TWNewEgg.API.View.OrderInfoAddDes> AddDesMapper(List<TWNewEgg.API.Models.OrderInfo> _orderInfo)
        {
            List<TWNewEgg.API.View.OrderInfoAddDes> _listOrderInfoAddDes = new List<OrderInfoAddDes>();

            TWNewEgg.API.View.Service.AES aes = new Service.AES();

            if (_orderInfo != null)
            {
                foreach (TWNewEgg.API.Models.OrderInfo _orderInfoAddDes in _orderInfo)
                {
                    Mapper.CreateMap<TWNewEgg.API.Models.OrderInfo, TWNewEgg.API.View.OrderInfoAddDes>();
                    TWNewEgg.API.View.OrderInfoAddDes orderinfoadd = new OrderInfoAddDes();
                    orderinfoadd = Mapper.Map<OrderInfoAddDes>(_orderInfoAddDes);
                    orderinfoadd.Detail = aes.AesEncrypt(JsonConvert.SerializeObject(_orderInfoAddDes));
                    string ItemNamedata = string.Empty;
                    //判斷是Seller or Vendor，兩者所組合的資料不相同
                    if (_orderInfoAddDes.AccountTypeCode.ToString() == "Vendor")
                    {
                        ItemNamedata = ItemNamedata + _orderInfoAddDes.SOCode_POCode + "\r\n" + _orderInfoAddDes.FirstProductTitle + "\r\n" + "數量: " + _orderInfoAddDes.TotalQty + "; " + "廠商產品編號: " + _orderInfoAddDes.FirstMenufacturePartNum + "; " + "狀態: " + _orderInfoAddDes.DelvStatusStr;
                    }
                    else
                    {
                        ItemNamedata = ItemNamedata + _orderInfoAddDes.SOCode + "\r\n" + _orderInfoAddDes.FirstProductTitle + "\r\n" + "數量: " + _orderInfoAddDes.TotalQty + "; " + "廠商產品編號: " + _orderInfoAddDes.FirstMenufacturePartNum + "; " + "狀態: " + _orderInfoAddDes.DelvStatusStr;
                    }
                    orderinfoadd.Des = ItemNamedata;
                    _listOrderInfoAddDes.Add(orderinfoadd);
                }
            }
            return _listOrderInfoAddDes;

        }

        #region 遞送畫面

        // 遞送頁面錯誤訊息
        private string shipPageErrorMessage = string.Empty;
        #region 舊版子單
        ///// <summary>
        ///// 遞送頁面
        ///// </summary>
        ///// <param name="orderInfo">訂單資料</param>
        ///// <returns>view 呈現內容</returns>
        //[HttpPost]
        //[Filter.PermissionFilter]
        ////[Filter.LoginAuthorize]
        //public ActionResult ShipPage(string detail, int _Page)
        //{
        //    log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        //    // 遞送頁面是否成功生成
        //    bool isSuccess = true;

        //    // 錯誤訊息
        //    string errorMessage = string.Empty;

        //    // 加解密
        //    TWNewEgg.API.View.Service.AES aes = new Service.AES();

        //    // 訂單內容
        //    TWNewEgg.API.Models.OrderInfo orderInfo = new TWNewEgg.API.Models.OrderInfo();

        //    // 取得 cookie 資訊
        //    TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

        //    // 由前頁傳來的訂單資料，解密後，從 JSON 轉為 model
        //    try
        //    {
        //        orderInfo = JsonConvert.DeserializeObject<TWNewEgg.API.Models.OrderInfo>(aes.AesDecrypt(detail));
        //    }
        //    catch (Exception ex)
        //    {
        //        isSuccess = false;
        //        errorMessage = "讀取資料發生錯誤。";
        //        logger.Info(string.Format("訂單資料解密失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
        //    }

        //    ViewBag.DelvStatusStr = orderInfo.DelvStatusStr;

        //    // 若訂單狀態為已成立，先更新訂單狀態至待出貨
        //    if (isSuccess && orderInfo.DelvStatusStr == "已成立")
        //    {
        //        // 訂單狀態
        //        int delvStatus = (int)(TWNewEgg.API.Models.OrderInfo.EnumDelvStatus.待出貨);

        //        // 連接至 API 的 Connector 
        //        TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();
        //        TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>> orderInfoCell = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>>();
        //        try
        //        {
        //            // 取得更新過後的訂單清單
        //            orderInfoCell = connector.UpdateDelvStatus("", "", orderInfo.SOCode, delvStatus, sellerInfo.currentSellerID.ToString(), sellerInfo.UserID.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            isSuccess = false;
        //            errorMessage = "讀取資料發生錯誤。";
        //            logger.Info(string.Format("取得更新過後的訂單失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
        //        }

        //        if (orderInfoCell.IsSuccess)
        //        {
        //            // 訂單編號
        //            string salseOrderCode = orderInfo.SOCode;

        //            // 更新要顯示的訂單資訊
        //            orderInfo = null;
        //            orderInfo = (from p in orderInfoCell.Body where p.SOCode == salseOrderCode select p).FirstOrDefault();
        //        }
        //        else
        //        {
        //            isSuccess = false;
        //            errorMessage = orderInfoCell.Msg;
        //            logger.Info(string.Format("更新狀態錯誤; API Message = {0}.", orderInfoCell.Msg));
        //        }
        //    }

        //    // 更改訂單時間格式
        //    orderInfo.CreateDate = string.Format("{0:yyyy/MM/dd HH:mm:ss}", orderInfo.CreateDate);

        //    // 傳到 view 的 account type
        //    ViewBag.accountTypeCode = sellerInfo.AccountTypeCode;

        //    // 傳到 view 的 model
        //    ViewBag.orderInfo = orderInfo;

        //    // 將 OrderDetails 轉成 Jason 並加密，待 ShipPackage 時，將整個 model 傳回來使用
        //    try
        //    {
        //        ViewBag.OrderDetails = aes.AesEncrypt(JsonConvert.SerializeObject(orderInfo.OrderDetails));
        //    }
        //    catch (Exception ex)
        //    {
        //        isSuccess = false;
        //        errorMessage = "讀取資料發生錯誤。";
        //        logger.Info(string.Format("訂單資料加密失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
        //    }

        //    ViewBag.Page = _Page;

        //    string openShipPage = RenderView("ShipPage");
        //    return Json(new { IsSuccess = isSuccess, ViewHtml = openShipPage, ErrorMessage = errorMessage });
        //}
        #endregion
        #region 新版子單
        /// <summary>
        /// 遞送頁面
        /// </summary>
        /// <param name="orderInfo">訂單資料</param>
        /// <returns>view 呈現內容</returns>
        [Filter.PermissionFilter]
        //[Filter.LoginAuthorize]
        public ActionResult ShipPage(string cartID)
        {
            // 加解密
            TWNewEgg.API.View.Service.AES aes = new Service.AES();

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
            TWNewEgg.API.Models.Connector connector = new Connector();

            // 訂單資料查詢
            ActionResponse<TinyCart> getCartInfo = null;
            ActionResponse<TinyCart> updateCartDelvStatus = null;
            ActionResponse<OrderDetail> getOrderDetail = null;

            try
            {
                getCartInfo = connector.GetCartInfo(cartID);
                if (getCartInfo == null || getCartInfo.IsSuccess == false)
                {
                    // 返回錯誤訊息
                    logger.Error(string.Format("讀取 Cart 失敗; CartID = {0}; SellerID = {1}; UserID = {2}.", cartID, sellerInfo.SellerID, sellerInfo.UserID));
                    ViewBag.ErrorMessage = "資料讀取失敗。";
                    return PartialView(null);
                }
            }
            catch (Exception ex)
            {
                // 返回錯誤訊息
                logger.Error(string.Format("讀取 Cart 失敗(exception); CartID = {0}; SellerID = {1}; UserID = {2}; ExceptionMessage = {3}; Exception = {4}.", cartID, sellerInfo.SellerID, sellerInfo.UserID, GetExceptionMessage(ex), ex.ToString()));
                ViewBag.ErrorMessage = "資料讀取失敗。";
                return PartialView(null);
            }

            if (getCartInfo.Body.ShipType != (int)Item.tradestatus.直配 && getCartInfo.Body.ShipType != (int)Item.tradestatus.B2C直配)
            {
                // 非直配與B2C直配者不可打開子單操作畫面
                logger.Error(string.Format("ShipType 錯誤，非直配與B2C直配者不可打開子單操作畫面; CartID = {0}; ShipType = {1}; SellerID = {2}; UserID = {3}.", cartID, getCartInfo.Body.ShipType, sellerInfo.SellerID, sellerInfo.UserID));
                ViewBag.ErrorMessage = "此訂單為寄倉商品，將由台蛋網進行出貨事宜。";
                return PartialView(null);
            }

            // 若訂單狀態為已成立，先更新訂單狀態至待出貨
            if (getCartInfo.Body.DelvStatus == (int)Cart.cartstatus.已成立)
            {
                try
                {
                    updateCartDelvStatus = connector.UpdateCartDelvStatus(cartID, (int)Cart.cartstatus.待出貨, sellerInfo.UserID.ToString());
                    if (updateCartDelvStatus.IsSuccess == false)
                    {
                        // 返回錯誤訊息
                        logger.Error(string.Format("更新狀態(從已成立至待出貨)失敗; CartID = {0}; SellerID = {1}; UserID = {2}.", cartID, sellerInfo.SellerID, sellerInfo.UserID));
                        ViewBag.ErrorMessage = "資料讀取失敗。";
                        return PartialView(null);
                    }
                }
                catch (Exception ex)
                {
                    // 返回錯誤訊息
                    logger.Error(string.Format("更新狀態(從已成立至待出貨)失敗(exception); CartID = {0}; SellerID = {1}; UserID = {2}; ExceptionMessage = {3}; Exception = {4}.", cartID, sellerInfo.SellerID, sellerInfo.UserID, GetExceptionMessage(ex), ex.ToString()));
                    ViewBag.ErrorMessage = "資料讀取失敗。";
                    return PartialView(null);
                }
            }

            // 取得顯示頁面所需相關資訊
            try
            {
                getOrderDetail = connector.OrderDetail(cartID);
                if (getOrderDetail.IsSuccess == false)
                {
                    // 返回錯誤訊息
                    logger.Error(string.Format("取得顯示頁面所需相關資訊失敗; CartID = {0}; SellerID = {1}; UserID = {2}.", cartID, sellerInfo.SellerID, sellerInfo.UserID));
                    ViewBag.ErrorMessage = "資料讀取失敗。";
                    return PartialView(null);
                }
            }
            catch (Exception ex)
            {
                // 返回錯誤訊息
                logger.Error(string.Format("取得顯示頁面所需相關資訊失敗(exception); CartID = {0}; SellerID = {1}; UserID = {2}; ExceptionMessage = {3}; Exception = {4}.", cartID, sellerInfo.SellerID, sellerInfo.UserID, GetExceptionMessage(ex), ex.ToString()));
                ViewBag.ErrorMessage = "資料讀取失敗。";
                return PartialView(null);
            }


            ViewBag.OrderDetail = getOrderDetail.Body;

            // 將 OrderDetail 轉成 Jason 並加密，待 ShipPackage 時，將整個 model 傳回來使用
            try
            {
                ViewBag.AesOrderDetail = aes.AesEncrypt(JsonConvert.SerializeObject(getOrderDetail.Body));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("訂單資料加密失敗(exception); CartID = {0}; SellerID = {1}; UserID = {2}; ExceptionMessage = {3}; Exception = {4}.", cartID, sellerInfo.SellerID, sellerInfo.UserID, GetExceptionMessage(ex), ex.ToString()));
                ViewBag.ErrorMessage = "資料讀取失敗。";
                return PartialView(null);
            }

            // 依據 BSATW-173 廢四機需求增加
            // 癈四機回收四聯單-----------add by bruce 20160505
            ViewBag.salesorderCode = cartID;

            return PartialView();
        }
        #endregion
        #region 按鈕事件

        #region 回押單號

        /// <summary>
        /// 點擊回押單號按鈕
        /// </summary>
        /// <returns>遞送包裹 UI 界面</returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult CreatePackage(string sellerShippingAddress = "")
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\CreatePackage：Start");

            // 連接至 API 的 Connector
            TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

            // 物流公司清單
            TWNewEgg.API.Models.ActionResponse<List<string>> shipCarrierResult = new TWNewEgg.API.Models.ActionResponse<List<string>>();

            try
            {
                // 取得物流公司清單
                shipCarrierResult = connector.APIQueryShipCarrier(string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Info("OrderList\\CreatePackage: API 取得物流公司清單錯誤：" + ex.ToString());
            }

            logger.Info(string.Format("OrderList\\CreatePackage：{0}", shipCarrierResult.Msg));
            ViewBag.shippingAddress = sellerShippingAddress;
            if (shipCarrierResult.IsSuccess)
            {
                // 將物流公司清單傳至 view
                ViewBag.shipCarrier = shipCarrierResult.Body;

                // 產生回押單號 UI 界面
                string resultView = RenderView("ShipPackageUI");

                return Json(new { isSuccess = shipCarrierResult.IsSuccess, shipPackageUI = resultView });
            }
            else
            {
                return Json(new { isSuccess = shipCarrierResult.IsSuccess, errorMessage = ReloadPageMessage() });
            }
        }

        #endregion 回押單號


        



        #region 確認出貨

        /// <summary>
        /// 點擊確認出貨按鈕
        /// </summary>
        /// <remarks>將出貨狀態由待出貨，更改為已出貨</remarks>
        /// <param name="salseOrderCode">訂單編號</param>
        /// <param name="orderDetails">訂單商品資訊</param>
        /// <param name="trackingNumber">貨運編號</param>
        /// <param name="businessAddress">發件地址</param>
        /// <param name="shipDateString">遞送日期</param>
        /// <param name="deliverID">遞送公司編號</param>
        /// <returns>錯誤訊息</returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult ShipPackage(string salseOrderCode, string orderDetails, string trackingNumber, string businessAddress, string shipDateString, int deliverID)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\ShipPackage: Start。");

            // 判斷是否符合 Tracking Number 的格式及發件地址是否有輸入
            if (IsTrackingNumber(trackingNumber) && !string.IsNullOrEmpty(businessAddress) && deliverID != -1)
            {
                // 重新讀取資料庫內目前的訂單狀態
                TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.OrderInfo.EnumDelvStatus?> delvStatus = LoadDelvStatus(salseOrderCode);

                #region 更新訂單狀態 (待出貨 -> 己出貨)

                // 若出貨狀態為待出貨，才能更新狀態
                if (delvStatus.IsSuccess && delvStatus.Body == TWNewEgg.API.Models.OrderInfo.EnumDelvStatus.待出貨)
                {
                    // 組合更新訂單狀態的 model
                    TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_DelvTrack>> getDelvTrack = GetSellerDelvTrack(orderDetails, trackingNumber, businessAddress, shipDateString, deliverID);

                    if (getDelvTrack.IsSuccess)
                    {
                        // 連接至 API 的 Connector
                        TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

                        // 更新訂單狀態為已出貨
                        TWNewEgg.API.Models.ActionResponse<bool> sendPackageResult = connector.SendPackage(null, null, getDelvTrack.Body);

                        // 判斷更新是否成功
                        if (sendPackageResult.IsSuccess)
                        {
                            logger.Info("OrderList\\ShipPackage: Success。");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sendPackageResult.Msg))
                            {
                                shipPageErrorMessage = TrackingNumberErrorMessage();
                            }
                            else
                            {
                                shipPageErrorMessage = sendPackageResult.Msg;
                            }

                            logger.Info("OrderList\\ShipPackage: SalseOrderCode " + salseOrderCode + "登錄 Tracking Number 失敗。");
                        }
                    }
                    else
                    {
                        shipPageErrorMessage = getDelvTrack.Msg;
                    }
                }
                else
                {
                    if (delvStatus.IsSuccess == false)
                    {
                        shipPageErrorMessage = delvStatus.Msg;
                    }
                    else
                    {
                        shipPageErrorMessage = RenewListMessage();
                    }
                }

                #endregion 更新訂單狀態 (待出貨 -> 己出貨)
            }
            else
            {
                if (string.IsNullOrEmpty(businessAddress))
                {
                    shipPageErrorMessage += "出貨地址為必填欄位。";
                }

                if (deliverID == -1)
                {
                    shipPageErrorMessage += "請選擇物流公司。";
                }

                logger.Info("OrderList\\ShipPackage: 輸入格式錯誤：" + shipPageErrorMessage);
            }

            logger.Info("OrderList\\ShipPackage: End。");

            return Json(shipPageErrorMessage);
        }

        #endregion 確認出貨

        #region 配達

        /// <summary>
        /// 點擊配達按鈕
        /// </summary>
        /// <param name="salseOrderCode">訂單編號</param>
        /// <returns>錯誤訊息</returns>
        public JsonResult Arrival(string salseOrderCode)
        {
            // 更新訂單狀態為配達結果
            bool arrivalResult = true;
            string arrivalResultMessage = string.Empty;

            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\Arrival: Start。");

            // 重新讀取資料庫內目前的訂單狀態
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.OrderInfo.EnumDelvStatus?> delvStatus = LoadDelvStatus(salseOrderCode);

            // 判斷重新讀取訂單狀態是否成功，並且狀態是否為已出貨
            if (delvStatus.IsSuccess && delvStatus.Body == TWNewEgg.API.Models.OrderInfo.EnumDelvStatus.已出貨)
            {
                // 連接至 API 的 Connector
                TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

                // 取得 cookie 資訊
                TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

                try
                {
                    // 更新訂單狀態為配達
                    var result = connector.Arrival(
                    null,
                    null,
                    salseOrderCode,
                    Convert.ToInt32(TWNewEgg.API.Models.OrderInfo.EnumDelvStatus.配達),
                    Convert.ToInt32(TWNewEgg.API.Models.OrderInfo.EnumCartStatus.完成),
                    sellerInfo.currentSellerID.ToString(),
                        sellerInfo.UserID.ToString());

                    arrivalResult = result.IsSuccess;
                    arrivalResultMessage = result.Msg;
                }
                catch (Exception ex)
                {
                    arrivalResult = false;
                    shipPageErrorMessage = TryAgainMessage();
                    logger.Info("OrderList\\Arrival: 訂單更新狀態為配達失敗(exception)：" + ex.ToString());
                    logger.Info("OrderList\\Arrival: End。");

                    return Json(shipPageErrorMessage);
                }

                // 判斷更新訂單狀態為配達是否成功
                if (arrivalResult)
                {
                    logger.Info("OrderList\\Arrival: Success。");
                }
                else
                {
                    shipPageErrorMessage = ReLoginMessage();
                    logger.Info("OrderList\\Arrival: 訂單更新狀態為配達失敗：" + arrivalResultMessage);
                }
            }
            else
            {
                if (delvStatus.IsSuccess == false)
                {
                    // 重新讀取訂單狀態錯誤訊息
                    shipPageErrorMessage = delvStatus.Msg;
                }
                else
                {
                    // 訂單狀態已被更新訊息
                    shipPageErrorMessage = RenewListMessage();
                }
            }

            logger.Info("OrderList\\Arrival: End。");

            return Json(shipPageErrorMessage);
        }

        #endregion 配達

        #region 列印出貨明細

        /// <summary>
        /// 點擊列印出貨明細連結
        /// </summary>
        /// <param name="salseOrderCode">訂單編號</param>
        /// <param name="receiver">收件人姓名</param>
        /// <param name="receiverCellphone">收件人手機</param>
        /// <param name="address">收件人地址</param>
        /// <param name="orderDetails">訂單商品資訊</param>
        /// <param name="note">備註</param>
        /// <returns>列印頁面</returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public ActionResult PrintPackingList(string salseOrderCode, string receiver, string receiverCellphone, string address, string orderDetails, string note)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\PrintPackingList: Start。");

            // 傳到列印頁面的 model
            ViewBag.printPackingList = GetPrintPackingList(salseOrderCode, receiver, receiverCellphone, address, orderDetails, note);

            logger.Info("OrderList\\PrintPackingList: End。");

            return View();
        }

        #endregion 列印出貨明細

        #endregion 按鈕事件

        /// <summary>
        /// 將該 View 轉成 string
        /// </summary>
        /// <param name="partialView">View 的名稱</param>
        /// <returns>返回 string</returns>
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

        #region 驗證

        /// <summary>
        /// 重新讀取資料庫內目前的訂單狀態
        /// </summary>
        /// <param name="salseOrderCode">訂單編號</param>
        /// <returns>訂單狀態</returns>
        private TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.OrderInfo.EnumDelvStatus?> LoadDelvStatus(string salseOrderCode)
        {
            // 訂單狀態結果
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.OrderInfo.EnumDelvStatus?> loadDelvStatusResult = new TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.OrderInfo.EnumDelvStatus?>();
            loadDelvStatusResult.Body = new TWNewEgg.API.Models.OrderInfo.EnumDelvStatus();
            loadDelvStatusResult.IsSuccess = true;
            loadDelvStatusResult.Msg = string.Empty;

            // 組合搜尋條件
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.QueryCartCondition> condition = GetQueryCartCondition(salseOrderCode);

            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\LoadDelvStatus: Start。");

            if (condition.IsSuccess)
            {
                // 連接至 API 的 Connector
                TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();

                // 查詢訂單結果
                TWNewEgg.API.Models.ActionResponse<List<API.Models.OrderInfo>> orderInfoResult = new TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>>();

                try
                {
                    // 查詢訂單
                    orderInfoResult = connector.QueryOrderInfos<List<API.Models.OrderInfo>>(null, null, condition.Body);
                }
                catch (Exception ex)
                {
                    loadDelvStatusResult.IsSuccess = false;
                    logger.Info("OrderList\\LoadDelvStatus: 查詢訂單失敗(exception)：" + ex.ToString());
                }

                // 判斷是否成功查詢到訂單資訊
                if (orderInfoResult.IsSuccess)
                {
                    // 讀取訂單狀態
                    loadDelvStatusResult.Body = orderInfoResult.Body.Select(x => x.DelvStatus).FirstOrDefault();

                    if (loadDelvStatusResult.Body != null)
                    {
                        loadDelvStatusResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                        logger.Info("OrderList\\LoadDelvStatus: Success。");
                    }
                    else
                    {
                        loadDelvStatusResult.IsSuccess = false;
                        loadDelvStatusResult.Msg = ReLoginMessage();
                        loadDelvStatusResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        logger.Info("OrderList\\LoadDelvStatus: 此訂單編號不在訂單列表中，無法查詢訂單資訊。");
                    }
                }
                else
                {
                    loadDelvStatusResult.Body = null;
                    loadDelvStatusResult.IsSuccess = false;
                    loadDelvStatusResult.Msg = TryAgainMessage();
                    loadDelvStatusResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    logger.Info("OrderList\\LoadDelvStatus: 查詢訂單失敗：" + orderInfoResult.Msg);
                }
            }
            else
            {
                loadDelvStatusResult.Body = null;
                loadDelvStatusResult.IsSuccess = false;
                loadDelvStatusResult.Msg = condition.Msg;
                loadDelvStatusResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
            }

            logger.Info("OrderList\\LoadDelvStatus: End。");

            return loadDelvStatusResult;
        }

        /// <summary>
        /// 判斷是否符合 Tracking Number 的格式
        /// </summary>
        /// <param name="trackingNumber">貨運編號</param>
        /// <returns>ture:符合；false:不符合</returns>
        private bool IsTrackingNumber(string trackingNumber)
        {
            // 判斷是否有填入貨運編號
            if (!string.IsNullOrEmpty(trackingNumber.Trim()))
            {
                // Tracking Number 的格式，只能輸入 英文、數字、-、_、空白等字元
                Regex reg = new Regex(@"^[0-9a-zA-Z_\-\s]+$");

                // 判斷是否符合 Tracking Number 的格式
                if (reg.IsMatch(trackingNumber.Trim()))
                {
                    return true;
                }
                else
                {
                    // 顯示錯誤提示
                    shipPageErrorMessage += "物流單號只能是英文、數字、_、-、空白等字元組成，請檢查修改。";

                    return false;
                }
            }
            else
            {
                // 顯示錯誤提示
                shipPageErrorMessage += "物流單號為必填欄位。";

                return false;
            }
        }

        #endregion 輸入資料的格式驗證

        #region 組 model

        /// <summary>
        /// 組合搜尋條件
        /// </summary>
        /// <param name="salseOrderCode">訂單編號</param>
        /// <returns>搜尋條件</returns>
        private TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.QueryCartCondition> GetQueryCartCondition(string salseOrderCode = null)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetQueryCartCondition: Start。");

            // 組合搜尋條件結果
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.QueryCartCondition> conditionResult = new TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.QueryCartCondition>();
            conditionResult.IsSuccess = true;
            conditionResult.Body = new TWNewEgg.API.Models.QueryCartCondition();
            conditionResult.Msg = string.Empty;

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            // 搜尋條件
            TWNewEgg.API.Models.QueryCartCondition condition = new TWNewEgg.API.Models.QueryCartCondition();

            #region 組合搜尋條件

            // 填寫 SellerID
            var getSellerID = GetSellerID();

            if (getSellerID.IsSuccess)
            {
                condition.SellerID = getSellerID.Body.ToString();
            }
            else
            {
                conditionResult.IsSuccess = false;
                conditionResult.Msg = getSellerID.Msg;
            }

            // 填寫訂單編號
            if (!string.IsNullOrEmpty(salseOrderCode))
            {
                condition.SOCode = salseOrderCode;
            }
            else
            {
                conditionResult.IsSuccess = false;
                conditionResult.Msg = ReloadPageMessage();
                logger.Info("OrderList\\GetQueryCartCondition: 訂單編號為空值。");
            }

            #endregion 組合搜尋條件

            if (conditionResult.IsSuccess)
            {
                conditionResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                conditionResult.Body = condition;
                logger.Info("OrderList\\GetQueryCartCondition: Success。");
            }
            else
            {
                conditionResult.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                conditionResult.Body = null;
            }

            logger.Info("OrderList\\GetQueryCartCondition: End。");

            return conditionResult;
        }

        /// <summary>
        /// 組合更新訂單狀態的 model
        /// </summary>
        /// <param name="orderDetails">訂單商品資訊</param>
        /// <param name="trackingNumber">貨運編號</param>
        /// <param name="businessAddress">發件地址</param>
        /// <param name="shipDateString">遞送日期</param>
        /// <param name="deliverID">遞送公司編號</param>
        /// <returns>更新訂單狀態的 model</returns>
        private TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_DelvTrack>> GetSellerDelvTrack(string aesOrderDetail, string trackingNumber, string businessAddress, string shipDateString, int deliverID)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetSellerDelvTrack: Start。");

            // 組合更新訂單狀態的 model 結果
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_DelvTrack>> result = new TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack>>();
            result.Body = null;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得訂單商品資訊
            //List<TWNewEgg.API.Models.OrderDetailsInfo> orderDetailList = GetOrderDetailList(orderDetails);
            OrderDetail orderDetail = this.DecryptOrderDetail(aesOrderDetail);

            // 取得出貨日期(string to DateTime)
            TWNewEgg.API.Models.ActionResponse<DateTime> getShipDate = GetDateTime(shipDateString);

            // 取得 Seller ID
            TWNewEgg.API.Models.ActionResponse<int> getSellerID = GetSellerID();

            // 取得 User ID
            TWNewEgg.API.Models.ActionResponse<int> getUserID = GetUserID();

            if (orderDetail != null && getShipDate.IsSuccess && getSellerID.IsSuccess && getUserID.IsSuccess)
            {
                // 更新訂單狀態的 model
                List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_DelvTrack> delvTrack = new List<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack>();

                // 目前時間(更新 InDate、UpdateDate 使用)
                DateTime nowTime = DateTime.Now;
                try
                {
                    foreach (string processID in orderDetail.ProcessIDList)
                    {
                        delvTrack.Add(new DB.TWSELLERPORTALDB.Models.Seller_DelvTrack()
                        {
                            // 建立包裏輸入內容
                            DeliverID = deliverID,
                            DelvDate = getShipDate.Body,
                            Address = businessAddress.Trim(),
                            TrackingNum = trackingNumber.Trim(),

                            // 訂單商品資訊
                            ManufactureID = orderDetail.ManufactureID,
                            ManufacturePartNum = orderDetail.MenufacturePartNum,
                            ProcessID = processID,
                            ProductID = orderDetail.ProductID,
                            Qty = orderDetail.Qty,
                            SellerID = getSellerID.Body,
                            SellerProductID = orderDetail.SellerProductID,
                            UPC = orderDetail.UPC,

                            // 資料更新日期、更新人
                            InDate = nowTime,
                            InUserID = getUserID.Body,
                            UpdateDate = nowTime,
                            UpdateUserID = getUserID.Body
                        });
                    }
                }
                catch (Exception ex)
                {
                    logger.Info("OrderList\\GetSellerDelvTrack: 組合訂單狀態 model 失敗 (CartID = " + orderDetail.CartID + ")(expection)：" + ex.ToString());
                }

                if (delvTrack.Count > 0)
                {
                    result.Body = delvTrack;

                    // 判斷要更新訂單狀態的 model 中，是不是其中幾筆有問題
                    logger.Info("OrderList\\GetSellerDelvTrack: Success。");
                    //if (delvTrack.Count == orderDetailList.Count)
                    //{
                    //    logger.Info("OrderList\\GetSellerDelvTrack: Success。");
                    //}
                    //else
                    //{
                    //    result.IsSuccess = false;
                    //    result.Msg = TryAgainMessage();
                    //}
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg = ReloadPageMessage();
                }
            }
            else
            {
                result.IsSuccess = false;

                if (getSellerID.IsSuccess == false || getUserID.IsSuccess == false)
                {
                    result.Msg = ReLoginMessage();
                }
                else
                {
                    result.Msg = ReloadPageMessage();
                }
            }

            if (result.IsSuccess)
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            }
            else
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
            }

            logger.Info("OrderList\\GetSellerDelvTrack: End。");

            return result;
        }

        /// <summary>
        /// 組合列印頁面的 model
        /// </summary>
        /// <param name="salseOrderCode">訂單編號</param>
        /// <param name="receiver">收件人姓名</param>
        /// <param name="receiverCellphone">收件人手機</param>
        /// <param name="address">收件人地址</param>
        /// <param name="orderDetails">訂單商品資訊</param>
        /// <param name="note">備註</param>
        /// <returns>列印頁面的 model</returns>
        private PrintPackingList GetPrintPackingList(string salseOrderCode, string receiver, string receiverCellphone, string address, string orderDetails, string note)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetPrintPackingList: Start。");

            PrintPackingList printPackingList = new PrintPackingList();

            // 訂單編號
            printPackingList.SOCode = salseOrderCode;

            // 訂購人名稱
            printPackingList.UserName = receiver;

            // 訂購人手機
            printPackingList.Mobile = receiverCellphone;

            // 收件人地址
            printPackingList.Address = address;

            // 備註
            printPackingList.Note = note;

            // 訂單商品資訊
            printPackingList.packageDetials = GetPackageDetials(orderDetails);

            logger.Info("OrderList\\GetPrintPackingList: End。");

            return printPackingList;
        }

        /// <summary>
        /// 取得列印頁面的訂單商品資訊
        /// </summary>
        /// <param name="orderDetails">訂單商品資訊(json 格式、已加密)</param>
        /// <returns>列印頁面的訂單商品資訊(model 格式)</returns>
        private List<PackageDetials> GetPackageDetials(string orderDetails)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetPackageDetials: Start。");

            // 取得訂單商品資訊
            OrderDetail orderDetail = GetOrderDetailList(orderDetails);

            // 訂單商品資訊(列印頁 model)
            List<PackageDetials> packageDetialsList = new List<PackageDetials>();

            // 判斷訂單商品資訊是否有資料
            if (orderDetail != null)
            {
                // 轉換訂單商品資訊的 model 為列印畫面的 model
                packageDetialsList.Add(
                    new PackageDetials
                    {
                        Qty = orderDetail.Qty.ToString(),
                        SellerProductID = orderDetail.SellerProductID,
                        Title = orderDetail.ItemTitle
                    });

                if (packageDetialsList.Count > 0)
                {
                    logger.Info("OrderList\\GetPackageDetials: Success。");
                }
                else
                {
                    logger.Info("OrderList\\GetPackageDetials: Auto Mapper 失敗。");
                }
            }
            else
            {
                // 空資料串 model (供 view 顯示時，避免錯誤使用)
                packageDetialsList.Add(new PackageDetials()
                {
                    SellerProductID = string.Empty,
                    Title = string.Empty,
                    Qty = string.Empty
                });

                logger.Info("OrderList\\GetPackageDetials: 取得空資料串 model。");
            }

            logger.Info("OrderList\\GetPackageDetials: End。");

            return packageDetialsList;
        }

        /// <summary>
        /// 取得訂單商品資訊
        /// </summary>
        /// <remarks>解密訂單商品資訊，並轉成 model 格式</remarks>
        /// <param name="orderDetails">訂單商品資訊(json 格式、已加密)</param>
        /// <returns>訂單商品資訊(model 格式)</returns>
        private OrderDetail GetOrderDetailList(string orderDetails)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetOrderDetailList: Start。");
            OrderDetail desOrderDetail = new OrderDetail();
            // 訂單商品資訊
            List<TWNewEgg.API.Models.OrderDetailsInfo> orderDetailList = new List<TWNewEgg.API.Models.OrderDetailsInfo>();

            try
            {
                // 加解密
                TWNewEgg.API.View.Service.AES aes = new Service.AES();
                // 1.將訂單商品資訊，由解密成 JSON 格式
                // 2.將 JSON 格式 的訂單商品資訊，轉為 model
                desOrderDetail = JsonConvert.DeserializeObject<OrderDetail>(aes.AesDecrypt(orderDetails));

                logger.Info("OrderList\\GetOrderDetailList: Success。");
            }
            catch (Exception ex)
            {
                logger.Info("OrderList\\GetOrderDetailList: 訂單商品資訊轉換 model 失敗(exception)：" + ex.ToString());
            }

            logger.Info("OrderList\\GetOrderDetailList: End。");

            return desOrderDetail;
        }

        /// <summary>
        /// 取得訂單商品資訊
        /// </summary>
        /// <remarks>解密訂單商品資訊，並轉成 model 格式</remarks>
        /// <param name="aesOrderDetail">訂單商品資訊(json 格式、已加密)</param>
        /// <returns>訂單商品資訊(model 格式)</returns>
        private OrderDetail DecryptOrderDetail(string aesOrderDetail)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetOrderDetail: Start。");

            // 訂單商品資訊
            OrderDetail orderDetail = null;
            try
            {
                // 加解密
                TWNewEgg.API.View.Service.AES aes = new Service.AES();

                // 1.將訂單商品資訊，由解密成 JSON 格式
                // 2.將 JSON 格式 的訂單商品資訊，轉為 model
                orderDetail = JsonConvert.DeserializeObject<OrderDetail>(aes.AesDecrypt(aesOrderDetail));

                logger.Info("OrderList\\GetOrderDetail: Success。");
            }
            catch (Exception ex)
            {
                logger.Info("OrderList\\GetOrderDetail: 訂單商品資訊轉換 model 失敗(exception)：" + ex.ToString());
            }

            logger.Info("OrderList\\GetOrderDetail: End。");

            return orderDetail;
        }

        /// <summary>
        /// 取得 cookie 的 Current Seller ID
        /// </summary>
        /// <returns>Seller ID</returns>
        private TWNewEgg.API.Models.ActionResponse<int> GetSellerID()
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetSellerID: Start。");

            TWNewEgg.API.Models.ActionResponse<int> result = new TWNewEgg.API.Models.ActionResponse<int>();
            result.Body = 0;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            try
            {
                result.Body = sellerInfo.currentSellerID;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("OrderList\\GetSellerID: Cookie 的 SellerID 讀取失敗(exception)：" + ex.ToString());

                return result;
            }

            if (result.Body == 0)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("OrderList\\GetSellerID: Cookie 的 SellerID 為 0。");
            }
            else
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("OrderList\\GetSellerID: Success。");
            }

            logger.Info("OrderList\\GetSellerID: End。");

            return result;
        }

        /// <summary>
        /// 取得 cookie 的 User ID
        /// </summary>
        /// <returns>User ID</returns>
        private TWNewEgg.API.Models.ActionResponse<int> GetUserID()
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetUserID: Start。");

            TWNewEgg.API.Models.ActionResponse<int> result = new TWNewEgg.API.Models.ActionResponse<int>();
            result.Body = 0;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 取得 cookie 資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            try
            {
                result.Body = sellerInfo.UserID;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("OrderList\\GetUserID: Cookie 的 UserID 讀取失敗(exception)：" + ex.ToString());

                return result;
            }

            if (result.Body == 0)
            {
                result.IsSuccess = false;
                result.Msg = ReLoginMessage();
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                logger.Info("OrderList\\GetUserID: Cookie 的 UserID 為 0。");
            }
            else
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("OrderList\\GetUserID: Success。");
            }

            logger.Info("OrderList\\GetUserID: End。");

            return result;
        }

        /// <summary>
        /// 轉換日期格式
        /// </summary>
        /// <param name="dateTiem">日期(String)</param>
        /// <returns>日期(DateTime)</returns>
        private TWNewEgg.API.Models.ActionResponse<DateTime> GetDateTime(string dateTiem)
        {
            // 記錄執行訊息
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("OrderList\\GetDateTime: Start。");

            TWNewEgg.API.Models.ActionResponse<DateTime> result = new TWNewEgg.API.Models.ActionResponse<DateTime>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                // 轉換日期格式
                result.Body = Convert.ToDateTime(dateTiem);
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                logger.Info("OrderList\\GetDateTime: Success。");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.Msg = ReloadPageMessage();
                logger.Info("OrderList\\GetDateTime: 轉換日期格式失敗(expcetion)：" + ex.ToString());
            }

            logger.Info("OrderList\\GetDateTime: End。");

            return result;
        }

        #endregion 組 model

        #region Error Message

        private string ReloadPageMessage()
        {
            return "畫面讀取錯誤，請重新整理畫面後，再重新進行操作，若仍發生此錯誤，請聯繫客服人員。";
        }

        private string ReLoginMessage()
        {
            return "訂單發生錯誤，請先登出後，再重新登入繼續進行操作，若仍發生此錯誤，請聯繫客服人員。";
        }

        private string TryAgainMessage()
        {
            return "伺服器忙線中，請稍後再試，若仍持續發生此錯誤，請聯繫客服人員。";
        }

        private string RenewListMessage()
        {
            return "訂單狀態已被更新，請點選返回按鈕回到列表，並更新列表資訊。";
        }

        private string TrackingNumberErrorMessage()
        {
            return "回押單號失敗，請聯繫客服人員。";
        }

        #endregion Error Message

        #endregion 遞送畫面
        /// <summary>
        /// 轉換資料成Excel
        /// </summary>
        /// <param name="_queryCartCondition"></param>
        /// <param name="keywd"></param>
        /// <param name="choose"></param>
        /// <param name="SelectDate"></param>
        /// <param name="_BeginDate"></param>
        /// <param name="_EndDate"></param>
        /// <returns></returns>
        public ActionResult ToExcel(TWNewEgg.API.Models.QueryCartCondition _queryCartCondition, string keywd, int choose, string SelectDate, string _BeginDate, string _EndDate)
        {
            TWNewEgg.API.Models.Connector _connector = new TWNewEgg.API.Models.Connector();
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            AES aes = new AES();
            if (Request.Cookies["CSD"] != null)
            {
                string _StrSellerId = aes.AesDecrypt(Request.Cookies["CSD"].Value);
                _queryCartCondition.SellerID = _StrSellerId;
            }
            //根據 Enum model 取回對應的值
            //string _strChoose = Enum.GetName(typeof(TWNewEgg.API.View.QueryCartCondition.QuerySO), choose).ToString();
            #region 選擇"選擇描述"的事件
            switch (choose)
            {
                default:
                //case "訂單編號":
                case (int)TWNewEgg.API.Models.OrderKeyWordSearchType.訂單編號:
                    {
                        _queryCartCondition.SOCode = keywd.Trim();
                        break;
                    }
                //case "收據編號":
                //    {
                //        _queryCartCondition.InvoiceNumber = keywd.Trim();
                //        break;
                //    }
                //case "客戶名稱":
                case (int)TWNewEgg.API.Models.OrderKeyWordSearchType.訂購人姓名:
                    {
                        _queryCartCondition.CustomerName = keywd.Trim();
                        break;
                    }
                //case "商家商品編號":
                case (int)TWNewEgg.API.Models.OrderKeyWordSearchType.商家商品編號:
                    {
                        _queryCartCondition.SellerProductID = keywd.Trim();
                        break;
                    }
                //case "新蛋商品編號":
                case (int)TWNewEgg.API.Models.OrderKeyWordSearchType.新蛋商品編號:
                    {
                        _queryCartCondition.ProductID = keywd.Trim();
                        break;
                    }
                //case "客戶電話":
                //    {
                //        _queryCartCondition.CustomerPhone = keywd.Trim();
                //        break;
                //    }
                //case "商品標題":
                case (int)TWNewEgg.API.Models.OrderKeyWordSearchType.商品名稱:
                    {
                        _queryCartCondition.Title = keywd.Trim();
                        break;
                    }
                //case "生產廠商":
                //    {
                //        _queryCartCondition.Manufacture = keywd.Trim();
                //        break;
                //    }
            }
            #endregion
            _queryCartCondition.BeginDate = null;
            _queryCartCondition.EndDate = null;
            _queryCartCondition.DayBefore = null;
            switch (SelectDate)
            {
                case "0":
                    {
                        break;
                    }
                case "1":
                    {
                        _queryCartCondition.BeginDate = DateTime.Now.ToString();
                        break;
                    }
                case "2":
                    {
                        _queryCartCondition.DayBefore = 3;
                        break;
                    }
                case "3":
                    {
                        _queryCartCondition.DayBefore = 7;
                        break;
                    }
                case "4":
                    {
                        _queryCartCondition.DayBefore = 30;
                        break;
                    }
                case "5":
                    {
                        _queryCartCondition.BeginDate = _BeginDate;
                        break;
                    }
                case "6":
                    {
                        _queryCartCondition.BeginDate = _BeginDate;
                        _queryCartCondition.EndDate = _EndDate;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            // 指定要搜尋的訂單狀態
            _queryCartCondition.Status = _queryCartCondition.OrderSearchMode;

            TWNewEgg.API.Models.OrderInfo.DownloadSalesOrderListModel _downLoadSalesOrder = new TWNewEgg.API.Models.OrderInfo.DownloadSalesOrderListModel();
            TWNewEgg.API.View.Service.SellerInfoService _sellerInfoServive = new SellerInfoService();

            string _strVS = _sellerInfoServive.AccountTypeCode;
            //判斷是Vendor or Seller ，因為兩所組合出來的Excel內容不相同
            if (_strVS == "V")
            {
                _downLoadSalesOrder.AccountType = TWNewEgg.API.Models.OrderInfo.EnumAccountTypeCode.Vendor;
            }
            else
            {
                _downLoadSalesOrder.AccountType = TWNewEgg.API.Models.OrderInfo.EnumAccountTypeCode.Seller;
            }
            #region 組合匯出 Excel 所需的資料
            _downLoadSalesOrder.SellerID = _queryCartCondition.SellerID;
            _downLoadSalesOrder.queryCartCondition = _queryCartCondition;
            _downLoadSalesOrder.fileName = "OrderList";
            _downLoadSalesOrder.sheetName = "Order";
            _downLoadSalesOrder.titleLine = 1;
            #endregion
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            try
            {
                result = _connector.DownloadSalesOrderList(_downLoadSalesOrder);
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                logger.Error("//OrderList/ToExcel connect to api error: " + error.Message);
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

        /// <summary>
        /// 訂單子單資料蒐集
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回蒐集結果</returns>
        public JsonResult GetOrderDetail(string cartID)
        {
            ActionResponse<OrderDetail> result = new ActionResponse<OrderDetail>();
            // 連接至 API 的 Connector 
            TWNewEgg.API.Models.Connector connector = new TWNewEgg.API.Models.Connector();
            try
            {
                result = connector.OrderDetail(cartID);
            }
            catch (Exception ex)
            {
                logger.Error("訂單編號 : " + cartID + " 系統意外結束 : [ErrorMessage] " + ex.ToString());
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "訂單編號 : " + cartID + " 系統執行失敗，請與客服聯繫";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
