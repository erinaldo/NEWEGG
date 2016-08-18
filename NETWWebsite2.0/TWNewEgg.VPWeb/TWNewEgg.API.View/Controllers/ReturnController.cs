using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;
using TWNewEgg.API.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.API.View.Service;

namespace TWNewEgg.API.View.Controllers
{
    public class ReturnController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region 主單

        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageOrder)]
        [FunctionName(FunctionNameAttributeValues.RMAList)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("退貨處理")]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            RetgoodMainPage viewModel = new RetgoodMainPage();
            ViewBag.viewModel = viewModel;

            return View();
        }

        [HttpPost]
        [Filter.PermissionFilter]
        public JsonResult GetMainRetgood([DataSourceRequest] DataSourceRequest request, MainRetgoodSearchCondition searchCondition)
        {
            // 執行結果
            bool isSuccess = true;

            #region 設定搜尋條件

            TWNewEgg.API.Models.MainRetgoodSearchCondition searchCondition_API = new API.Models.MainRetgoodSearchCondition();

            #region 轉換 search condition model

            try
            {
                AutoMapper.Mapper.CreateMap<MainRetgoodSearchCondition, TWNewEgg.API.Models.MainRetgoodSearchCondition>();
                searchCondition_API = AutoMapper.Mapper.Map<TWNewEgg.API.Models.MainRetgoodSearchCondition>(searchCondition);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Info(string.Format("轉換 search condition model 失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            #endregion 轉換 search condition model

            if (isSuccess)
            {
                // 使用者資訊
                SellerInfoService sellerInfoService = new SellerInfoService();

                // 設定是否具有管理權限
                searchCondition_API.IsAdmin = sellerInfoService.IsAdmin;

                // 設定 SellerID 篩選方式
                if (sellerInfoService.IsAdmin && sellerInfoService.SellerID == sellerInfoService.currentSellerID)
                {
                    // 不篩選 SellerID
                    searchCondition_API.SellerID = null;
                }
                else
                {
                    searchCondition_API.SellerID = sellerInfoService.currentSellerID;
                }

                // 若有輸入對帳單號，清除輸入值前後的空白
                if (!string.IsNullOrEmpty(searchCondition_API.KeyWord))
                {
                    searchCondition_API.KeyWord = searchCondition_API.KeyWord.Trim();
                }

                // 排除錯誤的退貨狀態
                switch (searchCondition.RetgoodStatus)
                {
                    default:
                        {
                            searchCondition_API.RetgoodStatus = null;
                            break;
                        }
                    case (int)Retgood.status.退貨處理中:
                    case (int)Retgood.status.退貨中:
                    case (int)Retgood.status.完成退貨:
                    case (int)Retgood.status.退貨異常:
                    case (int)Retgood.status.退貨取消:
                        {
                            break;
                        }
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
            }

            #endregion 設定搜尋條件

            #region 取得 API 資料

            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.MainRetgood>> getMainRetgood = new API.Models.ActionResponse<List<API.Models.MainRetgood>>();

            if (isSuccess)
            {
                try
                {
                    //getMainRegood.Body = new List<MainRetgood>();
                    //getMainRegood.Body.Add(new MainRetgood()
                    //{ 
                    //    Status = 0,
                    //    StatusName = "退貨處理中",
                    //    CreateDate = DateTime.Today,
                    //    CartID = "cart01",
                    //    ProductName = "product01",
                    //    PayType = 0,
                    //    PayTypeName = "test",
                    //    Seller = "test(00)",
                    //    FrmName = "name",
                    //    FinReturnDate = DateTime.Today,
                    //    Date = DateTime.Today
                    //});
                    //getMainRegood.Body.Add(new MainRetgood()
                    //{
                    //    Status = 1,
                    //    StatusName = "退貨中",
                    //    CreateDate = DateTime.Today,
                    //    CartID = "cart02",
                    //    ProductName = "product02",
                    //    PayType = 0,
                    //    PayTypeName = "信用卡一次付清",
                    //    Seller = "test(00)",
                    //    FrmName = "name",
                    //    FinReturnDate = DateTime.Today,
                    //    Date = DateTime.Today
                    //});
                    //getMainRegood.Body.Add(new MainRetgood()
                    //{
                    //    Status = 2,
                    //    StatusName = "完成退貨",
                    //    CreateDate = DateTime.Today,
                    //    CartID = "cart03",
                    //    ProductName = "product03",
                    //    PayType = 0,
                    //    PayTypeName = "test",
                    //    Seller = "test(00)",
                    //    FrmName = "name",
                    //    FinReturnDate = DateTime.Today,
                    //    Date = DateTime.Today
                    //});
                    getMainRetgood = connector.GetMainRetgood(searchCondition_API);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    logger.Info(string.Format("取得 API 資料失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            #endregion 取得 API 資料

            if (isSuccess)
            {
                if (getMainRetgood.Body.Count > 0)
                {
                    return Json(getMainRetgood.Body.ToDataSourceResult(request));
                }
                else
                {
                    return Json("查無此資料，請重新確認！");
                }
            }
            else
            {
                return Json("查詢時發生錯誤，請稍後再試；若仍發生此錯誤，請聯繫客服人員。");
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

        #endregion 主單

        #region 派車功能
        /// <summary>
        /// 顯示派車功能畫面
        /// </summary>
        /// <param name="cart_id">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        public ActionResult RetgoodsInfo(string cart_id)
        {
            SellerInfoService sellerInfoService = new SellerInfoService();
            TWNewEgg.API.Models.Connector connector = new Connector();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodAPIModel> retgoodInfo = new ActionResponse<RetgoodAPIModel>();
            try
            {
                // 廠商已查看派車明細備註檢驗
                ActionResponse<bool> lookingInfo = connector.HasBeenViewed(sellerInfoService.UserID, cart_id);
                if (lookingInfo.IsSuccess == false)
                {
                    logger.Error("廠商已查看派車明細備註執行失敗 [ErrorMessage] " + lookingInfo.Msg);
                    return Json("資料讀取失敗");
                }
                // Retgood資訊取得
                retgoodInfo = connector.retgoodInfomation(cart_id);
                if (retgoodInfo.IsSuccess == true)
                {
                    ViewBag.retgoodUpperInfo = retgoodInfo.Body.retgoodUpper;
                    ViewBag.retgoodGridInfo = retgoodInfo.Body.retgoodgrid;
                    // 退款總金額計算
                    ActionResponse<decimal> total = this.ComputChildTotal(retgoodInfo.Body.retgoodgrid);
                    if (total.IsSuccess == false)
                    {
                        ViewBag.coupon_Total = 0;
                        ViewBag.Total = 0;
                    }
                    else
                    {
                        ViewBag.coupon_Total = retgoodInfo.Body.retgoodgrid.Select(x => x.process_coupon_total).Sum();
                        ViewBag.Total = total.Body;
                    }
                }
                else
                {
                    ViewBag.retgoodUpperInfo = null;
                    ViewBag.retgoodGridInfo = null;
                }
            }
            catch (Exception error)
            {
                logger.Error(this.errorMsg(error));
            }

            return PartialView();
        }

        /// <summary>
        /// 退貨商品退款金額計算
        /// </summary>
        /// <param name="retgoodgrid">退貨編號</param>
        /// <returns>返回計算金額</returns>
        public ActionResponse<decimal> ComputChildTotal(List<RetgoodGrid> retgoodgrid)
        {
            ActionResponse<decimal> result = new ActionResponse<decimal>();
            decimal total = 0;
            try
            {
                foreach (var item in retgoodgrid)
                {
                    total += item.retgood_Price;
                }

                result.IsSuccess = true;
                result.Body = total;
            }
            catch (Exception error)
            {
                this.errorMsg(error);
                result.IsSuccess = false;
                result.Body = 0;
            }

            return result;
        }

        /// <summary>
        /// 讀取貨運公司相關資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFreightCompanyInfo()
        {
            TWNewEgg.API.Models.Connector connector = new Connector();
            TWNewEgg.API.Models.ActionResponse<List<string>> shipCarrierResult = new ActionResponse<List<string>>();
            List<TWNewEgg.API.View.FreightCompany> freightCompanyList = new List<FreightCompany>();
            try
            {
                shipCarrierResult = connector.APIQueryShipCarrier(string.Empty, string.Empty);

                if (shipCarrierResult.IsSuccess == true)
                {
                    foreach (var item in shipCarrierResult.Body)
                    {
                        string[] strArray = item.Split('.');
                        freightCompanyList.Add(new TWNewEgg.API.View.FreightCompany { FreightCompanyID = Convert.ToInt32(strArray[0]), FreightCompanyName = strArray[1] });
                    }
                }
            }
            catch (Exception error)
            {
                logger.Error(this.errorMsg(error));
            }

            if (shipCarrierResult.IsSuccess == false)
            {
                return Json("");
            }

            freightCompanyList = freightCompanyList.OrderBy(p => p.FreightCompanyID).ToList();
            return Json(freightCompanyList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 儲存退貨概要資訊
        /// </summary>
        /// <param name="updateRetgoodStatus">所需更新資訊</param>
        /// <returns>返回儲存結果</returns>
        public JsonResult UpdateRetgoodInfo(UpdateRetgoodStatus updateRetgoodStatus)
        {
            UpdateRetGoodsInfo updateRetGoodsInfo = new UpdateRetGoodsInfo();
            ActionResponse<bool> result = new ActionResponse<bool>();
            TWNewEgg.API.Models.Connector connector = new Connector();
            try
            {
                // 組合更新退貨狀態所需資訊
                updateRetGoodsInfo = this.UpdateRetGoodsInfoCombine(updateRetgoodStatus);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = ex.Message;
                logger.Error("退貨資訊更新失敗 [ErrorMessage] " + ex.ToString());
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // 更新退貨商品相關資訊
                result = connector.UpdateRetGoods(updateRetGoodsInfo);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = "退貨資訊更新失敗";
                logger.Error("退貨資訊更新失敗 [ErrorMessage] " + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 組合更新退貨狀態所需資訊
        /// </summary>
        /// <param name="updateRetgoodStatus">頁面回傳更新資訊</param>
        /// <returns>返回組合結果</returns>
        private UpdateRetGoodsInfo UpdateRetGoodsInfoCombine(UpdateRetgoodStatus updateRetgoodStatus)
        {
            DateTime getDate = DateTime.UtcNow.AddHours(8);
            bool isDateFormate = DateTime.TryParse(updateRetgoodStatus.PickupDate, out getDate);
            if (!isDateFormate)
            {
                throw new Exception("時間格式錯誤");
            }

            SellerInfoService sellerInfoService = new SellerInfoService();
            UpdateRetGoodsInfo updateRetGoodsInfo = new UpdateRetGoodsInfo();
            updateRetGoodsInfo.Cart_ID = updateRetgoodStatus.CartID;
            updateRetGoodsInfo.Retgood_ShpCode = updateRetgoodStatus.FreightNumber;

            if (updateRetgoodStatus.FreightCompanyID == 99 || updateRetgoodStatus.FreightCompanyName == "Other")
            {
                updateRetGoodsInfo.OtherUpDataNote = string.Format(" VendorPortal : UserID({0}); 貨運公司名稱 : {1}; 貨運編號 : {2}; 取件日期 : {3}",
                    sellerInfoService.UserID,
                    updateRetgoodStatus.OtherFreightCompanyName,
                    updateRetgoodStatus.FreightNumber,
                    getDate.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            else
            {
                //updateRetGoodsInfo.OtherUpDataNote = " VendorPortal : UserID(" + sellerInfoService.UserID + "); 貨運公司名稱 : " + updateRetgoodStatus.FreightCompanyName + "(" + updateRetgoodStatus.FreightCompanyID + ")" + " 貨運編號 : " + updateRetgoodStatus.FreightNumber + " 取件日期 : " + getDate.ToString("yyyy/MM/dd HH:mm:ss");
                updateRetGoodsInfo.OtherUpDataNote = string.Format(" VendorPortal : UserID({0}); 貨運公司名稱 : {1}({2}); 貨運編號 : {3}; 取件日期 : {4}",
                    sellerInfoService.UserID,
                    updateRetgoodStatus.FreightCompanyName,
                    updateRetgoodStatus.FreightCompanyID,
                    updateRetgoodStatus.FreightNumber,
                    getDate.ToString("yyyy/MM/dd HH:mm:ss"));
            }

            updateRetGoodsInfo.Retgood_Status = (int)Retgood.status.退貨中;
            return updateRetGoodsInfo;
        }

        /// <summary>
        /// 頁面回傳更新資訊
        /// </summary>
        public class UpdateRetgoodStatus
        {
            public string CartID { get; set; }
            public int FreightCompanyID { get; set; }
            public string FreightCompanyName { get; set; }
            public string FreightNumber { get; set; }
            public string PickupDate { get; set; }
            public string OtherFreightCompanyName { get; set; }
        }

        /// <summary>
        /// 組合錯誤訊息
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string errorMsg(Exception error)
        {
            string innerMsg = string.Empty;
            innerMsg = error.InnerException == null ? string.Empty : error.InnerException.Message;
            return "[Msg]: " + error.Message + " ;[StackTrace]:" + error.StackTrace + " ;[innerMsg]: " + innerMsg;
        }
        #endregion

        #region 回報功能
        /// <summary>
        /// 轉換 Enum 成 List 回傳
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRetgoodStatus()
        {
            List<TWNewEgg.API.View.StatusSelect> statusSelect = new List<StatusSelect>();
            statusSelect = GetStatusSelect();
            return Json(statusSelect, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 顯示回報功能畫面
        /// </summary>
        /// <param name="cart_id"></param>
        /// <returns></returns>
        public ActionResult RetgoodsNote(string cart_id)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodNote> retgoodNote = new ActionResponse<RetgoodNote>();
            TWNewEgg.API.Models.Connector connector = new Connector();
            try
            {
                retgoodNote = connector.RetgoodsNote(cart_id);
                ViewBag.RetgoodsNote_CartID = cart_id;
                List<TWNewEgg.API.View.StatusSelect> statusSelect = new List<StatusSelect>();
                statusSelect = GetStatusSelect();
                string getStatus = statusSelect.Where(x => x.StatusId == retgoodNote.Body.Note_Status.ToString()).Select(x => x.StatusId).FirstOrDefault();
                ViewBag.Note_Status = getStatus;
            }
            catch (Exception error)
            {
                this.errorMsg(error);
            }

            return PartialView(retgoodNote.Body);
        }

        /// <summary>
        /// 取得退貨狀態選單資訊
        /// </summary>
        /// <returns>返回退貨選單</returns>
        private List<StatusSelect> GetStatusSelect()
        {
            var retgoodStatusEnum = Enum.GetValues(typeof(Retgood.status));
            List<TWNewEgg.API.View.StatusSelect> statusSelect = new List<StatusSelect>();
            foreach (var item in retgoodStatusEnum)
            {
                if ((Int16)item == (int)Retgood.status.完成退貨 || (Int16)item == (int)Retgood.status.退貨異常)
                {
                    TWNewEgg.API.View.StatusSelect StaSel = new StatusSelect { StatusId = ((Int16)item).ToString(), StatusDes = item.ToString() };
                    statusSelect.Add(StaSel);
                }
            }

            return statusSelect;
        }

        /// <summary>
        /// 退貨回報
        /// </summary>
        /// <param name="retgoodsReportNote">退貨回報資訊</param>
        /// <returns>返回執行結果</returns>
        public JsonResult RetgoodsReport(RetgoodsReportNote retgoodsReportNote)
        {
            UpdateRetGoodsInfo updateRetGoodsInfo = new UpdateRetGoodsInfo();
            ActionResponse<bool> result = new ActionResponse<bool>();
            TWNewEgg.API.Models.Connector connector = new Connector();
            try
            {
                // 組合更新退貨回報所需資訊
                updateRetGoodsInfo = this.RetgoodsReportInfoCombine(retgoodsReportNote);
                // 更新退貨商品相關資訊
                result = connector.UpdateRetGoods(updateRetGoodsInfo);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = "退貨回報狀態更新失敗";
                logger.Error("退貨回報狀態更新失敗 [ErrorMessage] " + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 組合更新退貨回報所需資訊
        /// </summary>
        /// <param name="retgoodsReportNote">頁面退貨回報資訊</param>
        /// <returns>返回組合結果</returns>
        private UpdateRetGoodsInfo RetgoodsReportInfoCombine(RetgoodsReportNote retgoodsReportNote)
        {
            SellerInfoService sellerInfoService = new SellerInfoService();
            UpdateRetGoodsInfo updateRetGoodsInfo = new UpdateRetGoodsInfo();
            updateRetGoodsInfo.Cart_ID = retgoodsReportNote.CartID;
            updateRetGoodsInfo.OtherUpDataNote = " VendorPortal : UserID(" + sellerInfoService.UserID + "); " + retgoodsReportNote.NoteDes;
            //updateRetGoodsInfo.Retgood_FinreturndateNote = updateRetGoodsInfo.OtherUpDataNote;
            updateRetGoodsInfo.Retgood_Status = retgoodsReportNote.ReportRetgoodStatus;
            return updateRetGoodsInfo;
        }

        /// <summary>
        /// 頁面退貨回報
        /// </summary>
        public class RetgoodsReportNote
        {
            public string CartID { get; set; }
            public int ReportRetgoodStatus { get; set; }
            public string NoteDes { get; set; }
        }
        #endregion

        #region 查看備註功能
        /// <summary>
        /// 顯示查看功能畫面
        /// </summary>
        /// <param name="cart_id"></param>
        /// <returns></returns>
        public ActionResult UpdateRetgoodsNote(string cart_id)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodNote> retgoodNote = new ActionResponse<RetgoodNote>();
            TWNewEgg.API.Models.Connector connector = new Connector();
            try
            {
                retgoodNote = connector.RetgoodsNote(cart_id);
                ViewBag.RetgoodsNote_CartID = cart_id;
            }
            catch (Exception error)
            {
                this.errorMsg(error);
            }

            return PartialView(retgoodNote.Body);
        }

        /// <summary>
        /// 退貨備註更新
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <param name="updateNoteDes">所要新增的備註</param>
        /// <returns>返回更新結果</returns>
        public JsonResult UpdateNote(string cartID, string updateNoteDes)
        {
            SellerInfoService sellerInfoService = new SellerInfoService();
            ActionResponse<bool> result = new ActionResponse<bool>();
            TWNewEgg.API.Models.Connector connector = new Connector();
            try
            {
                // 更新備註
                result = connector.UpdateRetGoodsNote(sellerInfoService.UserID, cartID, updateNoteDes);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Body = false;
                result.Msg = "退貨備註更新失敗";
                logger.Error("退貨備註更新失敗 [ErrorMessage] " + ex.ToString());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
