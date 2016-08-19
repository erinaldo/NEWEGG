using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using TWNewEgg.API.View.Attributes;

namespace TWNewEgg.API.View.Controllers
{
    public class FinancialController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
        TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();

        #region 主單
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageAccount)]
        [FunctionName(FunctionNameAttributeValues.Financial)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("對帳單")]
        [Filter.PermissionFilter]
        public ActionResult MainStatement()
        {
            TWNewEgg.API.View.MainStatementViewModel viewModel = new TWNewEgg.API.View.MainStatementViewModel();
            ViewBag.viewModel = viewModel;

            return View();
        }

        [HttpPost]
        [Filter.PermissionFilter]
        public JsonResult MainStatement_Search([DataSourceRequest] DataSourceRequest request, MainStatementSearchCondition searchCondition)
        {
            // 執行結果
            bool isSuccess = true;

            #region 設定搜尋條件

            TWNewEgg.API.Models.MainStatementSearchCondition searchCondition_API = new API.Models.MainStatementSearchCondition();

            #region 轉換 search condition model

            try
            {
                AutoMapper.Mapper.CreateMap<MainStatementSearchCondition, TWNewEgg.API.Models.MainStatementSearchCondition>();
                searchCondition_API = AutoMapper.Mapper.Map<TWNewEgg.API.Models.MainStatementSearchCondition>(searchCondition);
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
                TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();

                // 設定是否具有管理權限
                searchCondition_API.IsAdmin = sellerInfoService.IsAdmin;

                // 設定資料需求方為 VendorPortal
                searchCondition_API.WhosCall = API.Models.WhosCall.VendorPortal;

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
                if (!string.IsNullOrEmpty(searchCondition_API.SettlementID))
                {
                    searchCondition_API.SettlementID = searchCondition_API.SettlementID.Trim();
                }
            }

            #endregion 設定搜尋條件

            #region 取得 API 資料

            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.MainStatement>> getMainStatement = new API.Models.ActionResponse<List<API.Models.MainStatement>>();

            if (isSuccess)
            {
                try
                {
                    getMainStatement = connector.GetMainStatement(searchCondition_API);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    logger.Info(string.Format("取得 API 資料失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }


            ////負數測試------------------add by bruce 20160802
            //foreach (var each_info in getMainStatement.Body) each_info.PaymentAmount = -500;


            #endregion 取得 API 資料

            if (isSuccess)
            {
                if (getMainStatement.Body.Count > 0)
                {
                    getMainStatement.Body = getMainStatement.Body.OrderBy(x => x.SellerID).ThenBy(x => x.SettleMonth).ThenBy(x => x.SettlementID).ToList();
                    return Json(getMainStatement.Body.ToDataSourceResult(request));
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

        [Filter.PermissionFilter]
        /// <summary>
        /// 讀取子單內容
        /// </summary>
        /// <param name="settlementID">帳單編號</param>
        /// <param name="finanStatus">發票狀態</param>
        /// <returns>子單內容</returns>
        public ActionResult StatementDetail(string settlementID = "", string finanStatus = "", int sellerID = -1)
        {
            #region 判斷是否正確的把帳單編號傳進 VIEW
            if (string.IsNullOrEmpty(settlementID) == true)
            {
                logger.Error("settlementID 是空的");
                ViewBag.ErrorMessage = "沒有對帳單編號";
                return PartialView();
            }
            #endregion
            #region 呼叫對帳單的查詢 API 並判斷是否有發生 EXCEPTION
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.financialModel> getMasterDetailDate = new API.Models.ActionResponse<API.Models.financialModel>();
            bool isNoException = true;
            try
            {
                //int sellerid = sellerInfoService.currentSellerID;
                //呼叫查詢的 API 
                getMasterDetailDate = connector.getfinancialDetail(settlementID, sellerID);
                isNoException = true;
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[innerExceptionMsg]: " + this.InnerExceptionMsg(error));
                isNoException = false;
            }
            //發生 Exception
            if (isNoException == false)
            {
                ViewBag.ErrorMessage = "系統錯誤";
                return PartialView();
            }
            #endregion
            //呼叫 API　回傳錯誤
            if (getMasterDetailDate.IsSuccess == false)
            {
                ViewBag.ErrorMessage = getMasterDetailDate.Msg;
                return PartialView();
            }
            TWNewEgg.API.Models.ActionResponse<List<DetailForOrderReturnWarehouse>> automapToDetailResult = new API.Models.ActionResponse<List<DetailForOrderReturnWarehouse>>();
            TWNewEgg.API.View.Detail detail = new Detail();
            try
            {
                #region 把子單上頭要秀出來的訊息寫入對應要塞到 VIEW 的 MODEL
                //帳單年月份
                detail.SettleMonth = getMasterDetailDate.Body.masterTopData.SettleMonth;//automapToDetailResult.Body[0].masterTopData.SettleMonth;
                //帳單編號
                detail.SettlementID = getMasterDetailDate.Body.masterTopData.SettlementID;
                //結帳日期
                detail.SettleDate = getMasterDetailDate.Body.masterTopData.SettleDate.AddHours(8).Year.ToString() + "/" + getMasterDetailDate.Body.masterTopData.SettleDate.AddHours(8).Month.ToString() + "/" + getMasterDetailDate.Body.masterTopData.SettleDate.AddHours(8).Day.ToString();
                //結算日期區間(起)
                detail.DateStart = getMasterDetailDate.Body.masterTopData.DateStart.AddHours(8).Year.ToString() + "/" + getMasterDetailDate.Body.masterTopData.DateStart.AddHours(8).Month.ToString() + "/" + getMasterDetailDate.Body.masterTopData.DateStart.AddHours(8).Day.ToString();
                //結算日期區間(迄)
                detail.DateEnd = getMasterDetailDate.Body.masterTopData.DateEnd.AddHours(8).Year.ToString() + "/" + getMasterDetailDate.Body.masterTopData.DateEnd.AddHours(8).Month.ToString() + "/" + getMasterDetailDate.Body.masterTopData.DateEnd.AddHours(8).Day.ToString();
                //匯款日期
                detail.RemitDate = getMasterDetailDate.Body.masterTopData.RemitDate.HasValue ? getMasterDetailDate.Body.masterTopData.RemitDate.Value.AddHours(8).Year.ToString() + "/" + getMasterDetailDate.Body.masterTopData.RemitDate.Value.AddHours(8).Month.ToString() + "/" + getMasterDetailDate.Body.masterTopData.RemitDate.Value.AddHours(8).Day.ToString() : null;
                //發票日期
                detail.InvoDate = getMasterDetailDate.Body.masterTopData.InvoDate.HasValue ? getMasterDetailDate.Body.masterTopData.InvoDate.Value.AddHours(8).Year.ToString() + "/" + getMasterDetailDate.Body.masterTopData.InvoDate.Value.AddHours(8).Month.ToString() + "/" + getMasterDetailDate.Body.masterTopData.InvoDate.Value.AddHours(8).Day.ToString() : string.Empty;
                //發票號碼
                detail.InvoNumber = getMasterDetailDate.Body.masterTopData.InvoNumber;
                //付款方式
                detail.PayType = getMasterDetailDate.Body.masterTopData.BillingCycle;
                // 是否開放使用者押發票 (add by Smoke 20151013 12:08)
                detail.IsOpenInvoice = IsOpenInvoice(finanStatus, detail.PayType);
                // 商家編號 (add by Smoke 20151013 12:08)
                detail.SellerID = sellerID;
                #endregion
                #region 訂單畫面上要秀出來的資料
                ViewBag.Order = getMasterDetailDate.Body.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.訂單 || p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉).ToList();
                TWNewEgg.API.View.SubFooter OrderSubFooter = new SubFooter();
                OrderSubFooter.Count = getMasterDetailDate.Body.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.訂單 || p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉).ToList().Count;
                OrderSubFooter.POPrice = getMasterDetailDate.Body.masterTopData.POPrice;
                OrderSubFooter.Potax = getMasterDetailDate.Body.masterTopData.POTax;
                OrderSubFooter.Subtotal = OrderSubFooter.POPrice + OrderSubFooter.Potax;
                ViewBag.OrderSubFooter = OrderSubFooter;
                #endregion
                #region 退款畫面上要秀出來的資料
                ViewBag.Returned = getMasterDetailDate.Body.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.退貨).ToList();
                TWNewEgg.API.View.SubFooter RuturnedSubFooter = new SubFooter();
                RuturnedSubFooter.Count = getMasterDetailDate.Body.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.退貨).ToList().Count;
                RuturnedSubFooter.POPrice = getMasterDetailDate.Body.masterTopData.RMAPrice;
                RuturnedSubFooter.Potax = getMasterDetailDate.Body.masterTopData.RMATax;
                RuturnedSubFooter.Subtotal = RuturnedSubFooter.POPrice + RuturnedSubFooter.Potax;
                ViewBag.RuturnedSubFooter = RuturnedSubFooter;
                #endregion
                #region 寄倉畫面上要秀出來的資料
                getMasterDetailDate.Body.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉).ToList();
                ViewBag.WarehouseModel = getMasterDetailDate.Body.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉).ToList();
                TWNewEgg.API.View.SubFooter WarehouseSubFooter = new SubFooter();
                WarehouseSubFooter.Count = getMasterDetailDate.Body.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉).ToList().Count;
                WarehouseSubFooter.POPrice = getMasterDetailDate.Body.masterTopData.WarehousePrice;
                WarehouseSubFooter.Potax = getMasterDetailDate.Body.masterTopData.WarehouseTax;
                WarehouseSubFooter.Subtotal = WarehouseSubFooter.POPrice + WarehouseSubFooter.Potax;
                ViewBag.WarehouseSubFooter = WarehouseSubFooter;
                #endregion
                #region 子單畫面下面要秀出來的資料
                TWNewEgg.API.View.MainFooter MainFooter = new MainFooter();
                MainFooter.TotalAmount = getMasterDetailDate.Body.masterTopData.TotalAmount;
                MainFooter.TotalTax = getMasterDetailDate.Body.masterTopData.TotalTax;
                MainFooter.PaymentAmount = getMasterDetailDate.Body.masterTopData.PaymentAmount;
                ViewBag.mainfooter = MainFooter;
                #endregion
                isNoException = true;

                //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720

                //http://jira/browse/BSATW-232

                //合計金額(含稅)：訂單_總計金額-退貨_總計金額。
                MainFooter.TotalAmount = (MainFooter.TotalAmount + MainFooter.TotalTax);

                //調整項金額(含稅)：調整項金額加總。
                MainFooter.TotalAmount2 = 0;

                string user_name = string.Empty;

                var list_result = connector.Get_SellerCorrectionPrice_GroupBy("", "", finanStatus, sellerID, settlementID, user_name);
                List<TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceDM> list_info = list_result.Body.ToList();

                foreach (var each_info2 in list_info)
                {
                    if (each_info2.SellerID == sellerID && each_info2.SettlementID == settlementID)
                    {
                        //如沒有調整項，則不顯示此欄位。--------------add by bruce 20160726
                        MainFooter.TotalAmount2_Data_Records += 1;

                        MainFooter.TotalAmount2 += each_info2.TotalAmount;
                        //發票開立金額(含稅)：合計金額(含稅)+ 調整項金額(含稅)。
                        MainFooter.InvoicePrice = (MainFooter.TotalAmount + MainFooter.TotalAmount2);
                        //本期應付總金額
                        MainFooter.PaymentAmount = (MainFooter.InvoicePrice - MainFooter.SubWarehoursetotal);
                    }
                    else if (each_info2.SellerID == sellerID && string.IsNullOrEmpty(each_info2.SettlementID))
                    {
                        //如沒有調整項，則不顯示此欄位。--------------add by bruce 20160726
                        MainFooter.TotalAmount2_Data_Records += 1;

                        MainFooter.TotalAmount2 += each_info2.TotalAmount;
                        //發票開立金額(含稅)：合計金額(含稅)+ 調整項金額(含稅)。
                        MainFooter.InvoicePrice = (MainFooter.TotalAmount + MainFooter.TotalAmount2);
                        //本期應付總金額
                        MainFooter.PaymentAmount = (MainFooter.InvoicePrice - MainFooter.SubWarehoursetotal);
                    }                    

                }
                //end 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160720


            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerExceptionMsg]: " + this.InnerExceptionMsg(error));
                isNoException = false;
            }

            //發生 Exception
            if (isNoException == false)
            {
                ViewBag.ErrorMessage = "系統錯誤";
                return PartialView();
            }
            return PartialView(detail);
        }

        #region 是否開放使用者押發票

        /// <summary>
        /// 是否開放使用者押發票
        /// </summary>
        /// <param name="finanStatus">發票狀態</param>
        /// <param name="payType">Vendor 結算方式</param>
        /// <returns>ture:開放, false:關閉</returns>
        private bool IsOpenInvoice(string finanStatus = null, string payType = null)
        {
            bool result = false;

            if (CheckFinanStatusValue(finanStatus) && CheckPayTypeValue(payType))
            {
                if (finanStatus == "S" || finanStatus == "V")
                {
                    // 押發票的開始日期
                    DateTime invoicing_OpenDate = new DateTime();

                    // 押發票的結束日期
                    DateTime invoicing_CloseDate = new DateTime();

                    // 現在時間
                    DateTime now = DateTime.Now;

                    if (payType == "月結")
                    {
                        // 設定押發票的開始日期
                        if (now.Day <= 25)
                        {
                            invoicing_OpenDate = new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, 26).Date;
                        }
                        else
                        {
                            invoicing_OpenDate = new DateTime(now.Year, now.Month, 26).Date;
                        }

                        // 設定押發票的結束日期 (月結的押發票結束日期，為每月的 11 號，也就是從押發票開始日期開放到下個月 10 號的 23:59 分)
                        invoicing_CloseDate = new DateTime(invoicing_OpenDate.AddMonths(1).Year, invoicing_OpenDate.AddMonths(1).Month, 11).Date;
                    }

                    if (payType == "半月結")
                    {
                        // 設定押發票的開始日期
                        if (now.Day >= 1 && now.Day <= 10)
                        {
                            invoicing_OpenDate = new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, 26).Date;
                        }
                        else if (now.Day >= 11 && now.Day <= 25)
                        {
                            invoicing_OpenDate = new DateTime(now.Year, now.Month, 11).Date;
                        }
                        else
                        {
                            invoicing_OpenDate = new DateTime(now.Year, now.Month, 26).Date;
                        }

                        // 設定押發票的結束日期
                        invoicing_CloseDate = invoicing_OpenDate.AddDays(5).Date;
                    }

                    // 2015.11.02 BSA 提出不判斷時間限制 User 修改發票號碼
                    // 判斷現在的時間，是否位於可押發票日期的區間內
                    //if (now >= invoicing_OpenDate && now < invoicing_CloseDate)
                    //{
                    result = true;
                    //}
                }
            }
            else
            {
                logger.Info(string.Format("輸入參數錯誤; FinanStatus = {0}; PayType = {1}.", finanStatus, payType));
            }

            return result;
        }

        /// <summary>
        /// 檢查發票狀態值是否正確
        /// </summary>
        /// <param name="finanStatus">發票狀態</param>
        /// <returns>true:正確, false:輸入值錯誤</returns>
        private bool CheckFinanStatusValue(string finanStatus)
        {
            bool result = false;

            switch (finanStatus)
            {
                case "S":
                case "V":
                case "C":
                    {
                        result = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// 檢查 Seller 結算方式的值是否正確
        /// </summary>
        /// <param name="payType">Vendor 結算方式</param>
        /// <returns>true:正確, false:輸入值錯誤</returns>
        private bool CheckPayTypeValue(string payType)
        {
            bool result = false;

            switch (payType)
            {
                case "月結":
                case "半月結":
                    {
                        result = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return result;
        }

        #endregion 是否開放使用者押發票

        public JsonResult DetailsInvoice(string taxedNumber = "", string taxedDate = "", string SettlementID = "", int sellerID = -1)
        {
            #region 判斷是否有輸入發票號碼、日期或都不輸入

            if ((string.IsNullOrEmpty(taxedNumber) && !string.IsNullOrEmpty(taxedDate)) || (!string.IsNullOrEmpty(taxedNumber) && string.IsNullOrEmpty(taxedDate)))
            {
                if (string.IsNullOrEmpty(taxedDate))
                {
                    return Json(new { isSuccess = "F", Msg = "請輸入發票日期。" });
                }

                if (string.IsNullOrEmpty(taxedNumber))
                {
                    return Json(new { isSuccess = "F", Msg = "請輸入發票號碼。" });
                }
            }

            if (string.IsNullOrEmpty(SettlementID))
            {
                logger.Info(string.Format("押發票失敗; ErrorMessage = {0}.", "無帳單編號"));
                return Json(new { isSuccess = "F", Msg = "系統錯誤" });
            }

            #endregion 判斷是否有輸入發票號碼、日期或都不輸入

            #region 檢查時間格式正不正確

            bool dateTraExceptionNoError = true;

            if (!string.IsNullOrEmpty(taxedDate))
            {
                DateTime dateTra = DateTime.Now;

                try
                {
                    dateTra = Convert.ToDateTime(taxedDate);
                    dateTraExceptionNoError = true;
                }
                catch (Exception error)
                {
                    dateTraExceptionNoError = false;
                    logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerExceptionMsg]: " + this.InnerExceptionMsg(error));
                }
                if (dateTraExceptionNoError == false)
                {
                    return Json(new { isSuccess = "F", Msg = "請選擇正確的時間格式" });
                }
            }

            #endregion 檢查時間格式正不正確

            #region 押發票和發票日期

            TWNewEgg.API.Models.ActionResponse<string> pushInvoNumDate = new API.Models.ActionResponse<string>();

            try
            {
                //pushInvoNumDate = connector.pushInvoNumAndInvoDate(taxedDate, taxedNumber.ToUpper(), SettlementID);
                //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721
                pushInvoNumDate = connector.pushInvoNumAndInvoDate(taxedDate, taxedNumber.ToUpper(), SettlementID, sellerID);
                dateTraExceptionNoError = true;

            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerExceptionMsg]: " + this.InnerExceptionMsg(error));
                dateTraExceptionNoError = false;
            }

            if (dateTraExceptionNoError == false)
            {
                return Json(new { isSuccess = "F", Msg = "系統錯誤" });
            }

            return Json(new { isSuccess = "T", Msg = pushInvoNumDate.Msg });

            #endregion 押發票和發票日期
        }
        public ActionResult Index()
        {
            return View();
        }
        #region 產生 Excel 報表
        /// <summary>
        /// 匯出 Excel
        /// </summary>
        /// <param name="SettlementIDNumber"></param>
        /// <param name="sellerID">商家編號</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportExcelDetail(string SettlementIDNumber = "", int sellerID = -1)
        {
            if (string.IsNullOrEmpty(SettlementIDNumber) == true)
            {
                return Json(new { isSuccess = "F", Msg = "系統錯誤, 沒有帳單編號" });
            }
            //int sellerid = sellerInfoService.currentSellerID;
            bool noException = true;
            TWNewEgg.API.Models.ActionResponse<string> getExcelData = new API.Models.ActionResponse<string>();
            try
            {
                //呼叫產生 Excel 的 API
                getExcelData = connector._FinancialExportExcel(SettlementIDNumber, sellerID);
                noException = true;
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + ";[InnerExceptionMsg]:" + this.InnerExceptionMsg(error));
                noException = false;
            }
            //發生Exception
            if (noException == false)
            {
                return Json(new { isSuccess = "F", Msg = "系統錯誤" });
            }
            if (getExcelData.IsSuccess == false)
            {
                return Json(new { isSuccess = "F", Msg = getExcelData.Msg });
            }
            return Json(new { isSuccess = "T", Msg = getExcelData.Body });
        }
        #endregion
        public TWNewEgg.API.Models.ActionResponse<List<DetailForOrderReturnWarehouse>> automapToOrderModel(TWNewEgg.API.Models.financialModel list_financialModel)
        {
            TWNewEgg.API.Models.ActionResponse<List<DetailForOrderReturnWarehouse>> result = new API.Models.ActionResponse<List<DetailForOrderReturnWarehouse>>();
            result.Body = new List<DetailForOrderReturnWarehouse>();
            try
            {
                foreach (var item in list_financialModel.basicDomain)
                {
                    TWNewEgg.API.View.DetailForOrderReturnWarehouse detailForOrderReturnWarehouse = new DetailForOrderReturnWarehouse();
                    AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.basicDomain, TWNewEgg.API.View.DetailForOrderReturnWarehouse>();
                    AutoMapper.Mapper.Map(item, detailForOrderReturnWarehouse);
                    result.Body.Add(detailForOrderReturnWarehouse);
                }
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[InnerExceptionMsg]: " + this.InnerExceptionMsg(error));
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }

        #region 判斷是否有 innerException message
        public string InnerExceptionMsg(Exception error)
        {
            string returnMsg = string.Empty;
            returnMsg = error.InnerException == null ? "No innerException Msg" : error.InnerException.Message;
            return returnMsg;
        }
        #endregion
    }
}
