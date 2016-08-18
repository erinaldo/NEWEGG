using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;

using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.API.Service
{
    public class FinancialService
    {
        TWNewEgg.BackendService.Interface.IFinanDetailService FinanDetailService = new TWNewEgg.BackendService.Service.FinanDetailService();
        TWNewEgg.BackendService.Interface.ICategoryService CategoryService = new TWNewEgg.BackendService.Service.CategoryService();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);


        #region 主單

        /// <summary>
        /// 對帳單主單查詢
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>主單清單</returns>
        public ActionResponse<List<MainStatement>> GetMainStatement(MainStatementSearchCondition searchCondition)
        {
            ActionResponse<List<MainStatement>> result = new ActionResponse<List<MainStatement>>();
            result.Body = new List<MainStatement>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            #region 轉換 search condition model

            TWNewEgg.BackendService.Models.MainStatementSearchCondition searchcondition_Back = new BackendService.Models.MainStatementSearchCondition();
            searchcondition_Back.PageInfo = new TWNewEgg.BackendService.Models.PageInfo();

            try
            {
                // 查詢條件的起、迄日期，在 json 傳遞時會轉為 utc 格式，因此在轉換 model 時，要將 utc 的時間 +8 小時
                AutoMapper.Mapper.CreateMap<PageInfo, TWNewEgg.BackendService.Models.PageInfo>();
                AutoMapper.Mapper.CreateMap<MainStatementSearchCondition, TWNewEgg.BackendService.Models.MainStatementSearchCondition>()
                    .ForMember(x => x.DateStart, x => x.MapFrom(src => src.DateStart.AddHours(8)))
                    .ForMember(x => x.DateEnd, x => x.MapFrom(src => src.DateEnd.AddHours(8)))
                    .ForMember(x => x.PageInfo, x => x.MapFrom(src => src.PageInfo));
                searchcondition_Back = AutoMapper.Mapper.Map<TWNewEgg.BackendService.Models.MainStatementSearchCondition>(searchCondition);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                logger.Info(string.Format("轉換 search condition model 失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            #endregion 轉換 search condition model

            #region 若具有管理權限，且商家編號為 null，則改為不篩選 SellerID

            if (result.IsSuccess && (searchCondition.IsAdmin && searchCondition.SellerID == null))
            {
                searchcondition_Back.SellerID = null;
            }

            #endregion 若具有管理權限，且商家編號為 null，則改為不篩選 SellerID

            #region 取得 BackendService 的資料

            TWNewEgg.BackendService.Models.ActionResponse<List<TWNewEgg.BackendService.Models.MainStatement>> backendServiceResult = new BackendService.Models.ActionResponse<List<BackendService.Models.MainStatement>>();

            if (result.IsSuccess)
            {
                try
                {
                    // 連接 BackendService
                    TWNewEgg.BackendService.Interface.IFinanDetailService iFinanDetailService = new TWNewEgg.BackendService.Service.FinanDetailService();
                    backendServiceResult = iFinanDetailService.GetMainStatement(searchcondition_Back);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("取得 BackendService 資料失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            #endregion 取得 BackendService 的資料

            #region 轉換回傳 model

            if (result.IsSuccess)
            {
                try
                {
                    AutoMapper.Mapper.CreateMap<TWNewEgg.BackendService.Models.MainStatement, MainStatement>();
                    result.Body = AutoMapper.Mapper.Map<List<MainStatement>>(backendServiceResult.Body);

                    if (result.Body.Count > 0)
                    {
                        foreach (MainStatement mainStatement in result.Body)
                        {
                            mainStatement.POTotal = mainStatement.POPrice + mainStatement.POTax;
                            mainStatement.RMATotal = mainStatement.RMAPrice + mainStatement.RMATax;
                            mainStatement.WarehouseTotal = mainStatement.WarehousePrice + mainStatement.WarehouseTax;
                            mainStatement.SettleMonth = mainStatement.SettleMonth.Substring(0, 4) + "/" + mainStatement.SettleMonth.Substring(4, 2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("轉換回傳 model 失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            #endregion 轉換回傳 model

            #region 依發票狀態篩選

            if (result.IsSuccess && result.Body.Count > 0 && searchCondition.IsInvoiced.HasValue)
            {
                if (searchCondition.IsInvoiced.Value)
                {
                    // 篩選已開立發票
                    result.Body = result.Body.Where(x => x.InvoDate.HasValue && !string.IsNullOrEmpty(x.InvoNumber)).ToList();
                }
                else
                {
                    // 篩選未開立發票
                    result.Body = result.Body.Where(x => !x.InvoDate.HasValue && string.IsNullOrEmpty(x.InvoNumber)).ToList();
                }
            }

            #endregion 依發票狀態篩選

            #region 讀取商家名稱，並組合商家名稱及商家編號

            if (result.IsSuccess && result.Body.Count > 0)
            {
                List<int> sellerIDCell = result.Body.Select(x => x.SellerID).Distinct().ToList();
                ActionResponse<List<Seller_ID_Name>> sellerInfoCell = new ActionResponse<List<Seller_ID_Name>>();

                if (sellerIDCell != null && sellerIDCell.Count > 0)
                {
                    #region 查詢商家名稱

                    try
                    {
                        SellerBasicInfoService sellerBasicInfoService = new SellerBasicInfoService();
                        sellerInfoCell = sellerBasicInfoService.GetSellerNameBySellerID(sellerIDCell);
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        logger.Info(string.Format("讀取商家名稱失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                    }

                    #endregion

                    #region 填寫商家 => 商家名稱(商家編號)

                    if (sellerInfoCell.IsSuccess && sellerInfoCell != null && sellerInfoCell.Body.Count > 0)
                    {
                        foreach (MainStatement mainStatement in result.Body)
                        {
                            try
                            {
                                mainStatement.Seller = string.Format("{0}({1})", sellerInfoCell.Body.Where(x => x.ID == mainStatement.SellerID).Select(x => x.Name).FirstOrDefault(), mainStatement.SellerID);
                            }
                            catch (Exception ex)
                            {
                                logger.Info(string.Format("查無商家名稱(exception); SellerID = {2}; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace, mainStatement.SellerID));
                                continue;
                            }
                        }
                    }

                    #endregion 填寫商家 => 商家名稱(商家編號)
                }
            }

            #endregion 讀取商家名稱，並組合商家名稱及商家編號

            if (result.IsSuccess)
            {
                if (result.Body.Count > 0)
                {
                    result.Msg = string.Format("查詢主單成功，共有 {0} 筆資料。", result.Body.Count);
                }
                else
                {
                    result.Msg = "查無主單資料。";
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "查詢主單失敗。";
            }

            result.Code = SetResponseCode(result.IsSuccess);

            logger.Info(string.Format("對帳單主單查詢 API result message : {0}", result.Msg));

            return result;
        }

        /// <summary>
        /// 填寫 Response Code
        /// </summary>
        /// <param name="isSuccess">成功、失敗資訊</param>
        /// <returns>Response Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)ResponseCode.Success;
            }
            else
            {
                return (int)ResponseCode.Error;
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


        #region 查詢子單的資料 並且也把對應的主單資料查詢出來
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.financialModel> GetDetailData(string SettlementID, int sellerid)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.financialModel> result = new ActionResponse<financialModel>();
            BackendService.Models.SellerFinanceDetail_Domain sellerVPDomain = new BackendService.Models.SellerFinanceDetail_Domain();

            //判斷是否有傳入對帳單編號
            if (string.IsNullOrEmpty(SettlementID) == true)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            //把查詢條件放入要送到 backend 查詢的 model
            sellerVPDomain.SettlementID = SettlementID;
            sellerVPDomain.SellerID = sellerid;


            TWNewEgg.BackendService.Models.ActionResponse<BackendService.Models.ReturnDetailAndNeedModel> _ReturnDetailAndNeedModel = new BackendService.Models.ActionResponse<BackendService.Models.ReturnDetailAndNeedModel>();
            //用來判斷是否發生 Exception 
            bool isNoException = true;
            try
            {
                //呼叫 backend 並取得對應的資料
                _ReturnDetailAndNeedModel = FinanDetailService.getDetailAndNeedData(sellerVPDomain);
                isNoException = true;
            }
            catch (Exception error)
            {
                isNoException = false;
                string innerExceptionMsg = string.Empty;
                innerExceptionMsg = error.InnerException == null ? "" : error.InnerException.Message;
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[innerExceptionMsg]: " + innerExceptionMsg);
                isNoException = false;
            }
            //發生 Exception
            if (isNoException == false)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            //呼叫 backend 回傳的結果是錯誤的
            if (_ReturnDetailAndNeedModel.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = _ReturnDetailAndNeedModel.Msg;
                return result;
            }
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.basicDomain>> autoMapToShowDetail = new ActionResponse<List<basicDomain>>();

            ActionResponse<TWNewEgg.API.Models.MasterTopData> autoMapToShowDetailFooter = new ActionResponse<MasterTopData>();

            try
            {
                //把從 backend 查詢回來的資料(子單) automap 回要顯示的 View Model
                autoMapToShowDetail = this.SellerFinanceDetail_DomainVendorPortal_2_financialModel(_ReturnDetailAndNeedModel.Body.sellerFinanceDetail_Domain_Vendor_Portal.ToList());
                if (autoMapToShowDetail.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = autoMapToShowDetail.Msg;
                    result.Body = null;
                }
                else
                {
                    //把從 backend 查詢回來的資料(主單) automap 回要顯示的 View Model
                    autoMapToShowDetailFooter = this.masterTopData(_ReturnDetailAndNeedModel.Body.mastershowTop);
                    if (autoMapToShowDetailFooter.IsSuccess == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = autoMapToShowDetailFooter.Msg;
                        result.Body = null;
                    }
                    else
                    {
                        result.Body = new financialModel();
                        result.Body.masterTopData = autoMapToShowDetailFooter.Body;
                        result.Body.basicDomain = autoMapToShowDetail.Body;
                        result.IsSuccess = true;
                        result.Msg = "查詢成功";
                    }
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "系統處理錯誤";
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]:" + error.StackTrace + " ;[ExceptionInnerMessage]:" + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 把從 backend service 查詢回來的資料 automap 回要回傳給 view 的 model
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.basicDomain>> SellerFinanceDetail_DomainVendorPortal_2_financialModel(List<TWNewEgg.BackendService.Models.SellerFinanceDetail_Domain_Vendor_Portal> automapModel)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.basicDomain>> result = new Models.ActionResponse<List<basicDomain>>();
            result.Body = new List<basicDomain>();

            // 合併退貨單要取 訂單編號
            TWNewEgg.DB.TWBackendDBContext bakdb = new DB.TWBackendDBContext();

            try
            {

                foreach (var item in automapModel)
                {
                    TWNewEgg.API.Models.basicDomain financialModel_Temp = new basicDomain();

                    AutoMapper.Mapper.CreateMap<TWNewEgg.BackendService.Models.SellerFinanceDetail_Domain_Vendor_Portal, basicDomain>();
                    AutoMapper.Mapper.Map(item, financialModel_Temp);

                    if (item.SettleType == 2)
                    {
                        string regoodOrderID = bakdb.Retgood.Where(x => x.Code == item.OrderID).Select(r => r.CartID).FirstOrDefault();

                        financialModel_Temp.RegoodOrderID = regoodOrderID;
                    }

                    // 計算含稅單價
                    financialModel_Temp.UnitPrice_Total = financialModel_Temp.UnitPrice + financialModel_Temp.UnitTax;

                    // 計算含稅總額
                    financialModel_Temp.SumPrice_Total = financialModel_Temp.SumPrice + financialModel_Temp.SumTax;

                    if (item.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉)
                    {
                        // 計算含稅運費金額
                        financialModel_Temp.ShipFee_Total = financialModel_Temp.ShipFee + financialModel_Temp.ShipTax;

                        // 計算含稅出貨處理費
                        financialModel_Temp.LogisticAmount_Total = financialModel_Temp.LogisticAmount + financialModel_Temp.LogisticTax;

                        // 計算含稅寄倉總額
                        financialModel_Temp.WhereHouse_Total = financialModel_Temp.ShipFee_Total + financialModel_Temp.LogisticAmount_Total;
                    }

                    result.Body.Add(financialModel_Temp);
                }

                result.IsSuccess = true;
                result.Msg = "AutoMap 成功";
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "AutoMap 失敗";
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]: " + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 把子單上面要顯示的主單資料放回要顯示的 Model 裡
        public ActionResponse<TWNewEgg.API.Models.MasterTopData> masterTopData(TWNewEgg.BackendService.Models.MaterShowTop backendMasterShowData)
        {
            ActionResponse<TWNewEgg.API.Models.MasterTopData> result = new ActionResponse<MasterTopData>();
            result.Body = new MasterTopData();
            try
            {
                //對帳單年月份
                result.Body.SettleMonth = backendMasterShowData.SettleMonth.Substring(0, 4) + "/" + backendMasterShowData.SettleMonth.Substring(4, 2);
                //對帳單編號
                result.Body.SettlementID = backendMasterShowData.SettlementID;
                //結算日期
                result.Body.SettleDate = backendMasterShowData.SettleDate;
                //結算起始日
                result.Body.DateStart = backendMasterShowData.DateStart;
                //結算迄日
                result.Body.DateEnd = backendMasterShowData.DateEnd;
                //匯款日期
                result.Body.RemitDate = backendMasterShowData.RemitDate;
                //發票日期
                result.Body.InvoDate = backendMasterShowData.InvoDate;
                //發票號碼
                result.Body.InvoNumber = backendMasterShowData.InvoNumber;
                //採購總額(未稅)
                result.Body.POPrice = backendMasterShowData.POPrice;
                //採購稅額
                result.Body.POTax = backendMasterShowData.POTax;
                //本期退貨總額(未稅)
                result.Body.RMAPrice = backendMasterShowData.RMAPrice;
                //退貨稅額
                result.Body.RMATax = backendMasterShowData.RMATax;
                //本期寄倉費用總額(未稅)
                result.Body.WarehousePrice = backendMasterShowData.WarehousePrice;
                //WarehouseTax
                result.Body.WarehouseTax = backendMasterShowData.WarehouseTax;
                //合計總額(未稅)
                result.Body.TotalAmount = backendMasterShowData.TotalAmount;
                //營業稅
                result.Body.TotalTax = backendMasterShowData.TotalTax;
                //本期應付(應收)總額(含稅)
                result.Body.PaymentAmount = backendMasterShowData.PaymentAmount;
                //付款方式
                result.Body.BillingCycle = backendMasterShowData.BillingCycle;
                //供應商相關資訊
                result.Body.sellerInfo_IDNAMESAP =
                    new SellerInfo_IDNAMESAP
                    {
                        sellerid = backendMasterShowData.sellerInfo_IDNAMESAP.sellerid,
                        Sap = backendMasterShowData.sellerInfo_IDNAMESAP.Sap,
                        sellerName = backendMasterShowData.sellerInfo_IDNAMESAP.sellerName
                    };
                result.IsSuccess = true;
                result.Msg = "成功";
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理錯誤";
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[innerExceptionMsg]: " + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 壓發票和發票日期
        //public ActionResponse<string> pushInvoNumAndInvoDate(string InvoDate, string InvoNum, string SettlementID)
        //{
        //    int sellerID = -1;
        //    return pushInvoNumAndInvoDate(InvoDate, InvoNum, SettlementID, sellerID);
        //}

        public ActionResponse<string> pushInvoNumAndInvoDate(string InvoDate, string InvoNum, string SettlementID, int sellerID)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            TWNewEgg.DB.TWBackendDBContext dbBack = new DB.TWBackendDBContext();
            #region 檢查有帳單編號, 發票日期, 發票號碼
            if (string.IsNullOrEmpty(SettlementID) == true)
            {
                result.IsSuccess = false;
                result.Msg = "沒有帳單編號";
                return result;
            }
            if ((string.IsNullOrEmpty(InvoDate) && !string.IsNullOrEmpty(InvoNum)) || (!string.IsNullOrEmpty(InvoDate) && string.IsNullOrEmpty(InvoNum)))
            {
                if (string.IsNullOrEmpty(InvoDate))
                {
                    result.IsSuccess = false;
                    result.Msg = "沒有填入發票日期";
                    return result;
                }

                if (string.IsNullOrEmpty(InvoNum))
                {
                    result.IsSuccess = false;
                    result.Msg = "沒有填入發票號碼";
                    return result;
                }
            }
            #endregion
            #region 進行發票日期轉換

            DateTime? InvoDateTime = null;

            if (!string.IsNullOrEmpty(InvoDate))
            {
                try
                {
                    InvoDateTime = Convert.ToDateTime(InvoDate);
                    result.IsSuccess = true;
                }
                catch (Exception error)
                {
                    result.IsSuccess = false;
                    result.Msg = "時間格式錯誤";
                    logger.Error(this.ExceptionInnerMessage(error));
                }
                //時間格式轉換錯誤
                if (result.IsSuccess == false)
                {
                    return result;
                }
            }

            #endregion
            #region 判斷對應的帳單號碼存不存在資料庫
            TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanMaster sellerFinanMaster = new DB.TWBACKENDDB.Models.Seller_FinanMaster();
            sellerFinanMaster = dbBack.Seller_FinanMaster.Where(p => p.SettlementID == SettlementID).FirstOrDefault();
            if (sellerFinanMaster == null)
            {
                result.IsSuccess = false;
                result.Msg = "資料不存在";
                return result;
            }
            #endregion
            #region 判斷是否有匯款日期, 有就不能壓發票
            if (sellerFinanMaster.RemitDate.HasValue == true || sellerFinanMaster.FinanStatus == "C")
            {
                result.IsSuccess = false;
                result.Msg = "此筆資料不能進行存儲動作";
                return result;
            }
            #endregion
            #region 檢查發票號碼是否有中文字
            if (!string.IsNullOrEmpty(InvoNum))
            {
                if (this.isCheckChineseWord(InvoNum) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "發票號碼不能含有中文字";
                    return result;
                }
            }
            #endregion
            sellerFinanMaster.InvoDate = InvoDateTime;
            sellerFinanMaster.InvoNumber = InvoNum;

            string current_FinanStatus = sellerFinanMaster.FinanStatus;

            // 更改發票狀態 (add by Smoke 20151015 11:40)
            if (string.IsNullOrEmpty(InvoDate) && string.IsNullOrEmpty(InvoNum))
            {
                sellerFinanMaster.FinanStatus = "S";
            }
            else if (!string.IsNullOrEmpty(InvoDate) && !string.IsNullOrEmpty(InvoNum))
            {
                sellerFinanMaster.FinanStatus = "V";
            }
            else
            {
                result.IsSuccess = false;
                logger.Info(string.Format("押發票失敗; ErrorMessage = {0}.", "設定主單發票狀態時，出現例外狀況"));
            }

            try
            {
                if (result.IsSuccess)
                {
                    dbBack.SaveChanges();
                }
                result.IsSuccess = true;
                result.Msg = "操作成功";
            }
            catch (Exception error)
            {
                logger.Error(this.ExceptionInnerMessage(error));
                result.IsSuccess = false;
                result.Msg = "系統錯誤";
            }

            // 修改子單的發票狀態註記欄位 (add by Smoke 20151015 11:40)
            if (result.IsSuccess)
            {
                ActionResponse<bool> markInvoiced = new ActionResponse<bool>();

                if (string.IsNullOrEmpty(InvoDate) && string.IsNullOrEmpty(InvoNum))
                {
                    markInvoiced = MarkInvoiced(SettlementID, "N");
                }
                else if (!string.IsNullOrEmpty(InvoDate) && !string.IsNullOrEmpty(InvoNum))
                {
                    markInvoiced = MarkInvoiced(SettlementID, "Y");
                }
                else
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("押發票失敗; ErrorMessage = {0}.", "設定子單發票狀態註記欄位時，出現例外狀況"));
                }

                if (result.IsSuccess)
                {
                    result.IsSuccess = markInvoiced.IsSuccess;
                    result.Code = markInvoiced.Code;
                }
                else
                {
                    result.Code = SetResponseCode(result.IsSuccess);
                }

                result.Body = string.Empty;
            }

            //"S:已結算"  ,"V:已開發票", "C:已匯款"
            //與rehtt討論後結論, V跟C不計算--------add by bruce 20160729
            if (current_FinanStatus == "S")
            {
                //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721           
                //http://jira/browse/BSATW-232
                string settlementID = SettlementID;
                string user_name = string.Empty;
                if (sellerID > 0) user_name = sellerID.ToString();

                //系統面-對帳單編號：廠商統編+西元年+月。 觸發時機：於押入發票後寫入----------------add by bruce 20160727
                if (!string.IsNullOrEmpty(InvoNum))
                {
                    TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM input_info = new TWNewEgg.Models.DomainModels.Seller.SellerCorrectionPriceSearchDM();
                    input_info.FinanStatus = current_FinanStatus;
                    input_info.SellerIDs.Add(sellerID);
                    input_info.SettlementIDs.Add(settlementID);

                    var list_updated = Processor.Request<List<bool>, List<bool>>("SellerCorrectionPriceService", "Save1", input_info, user_name);
                }
                //end 依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721

            }



            if (result.IsSuccess)
            {
                result.Msg = "操作成功。";
            }
            else
            {
                result.Msg = "操作失敗，請稍後再試；若仍發生此錯誤，請聯繫客服人員。";
            }

            return result;
        }


        #endregion
        #region 產生對帳單 Excel 報表
        public ActionResponse<string> _FinancialExportExcel(string SettlementIDNumber, int sellerid)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            List<FinancialExportExcel> toGeneraExcelmodel = new List<FinancialExportExcel>();
            //先讀取要做成報表的資料
            var _GetDetailData = this.GetDetailData(SettlementIDNumber, sellerid);
            if (_GetDetailData.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = _GetDetailData.Msg;
                return result;
            }
            //根據查詢出來的資料, 再讀取類別名稱和 id
            ActionResponse<financialModel> joinAndGetCategoryData = this.automap_FinancialExportExcel(_GetDetailData.Body);
            if (joinAndGetCategoryData.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = joinAndGetCategoryData.Msg;
                return result;
            }
            ActionResponse<string> generanalExcelFinan = this.toGeneraExcelFile(joinAndGetCategoryData.Body);
            //產生 Excel 報表失敗
            if (generanalExcelFinan.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = generanalExcelFinan.Msg;
                return result;
            }
            result.IsSuccess = true;
            result.Msg = "Excel 產生成功";
            result.Body = generanalExcelFinan.Body;

            return result;
        }
        #endregion
        #region 利用查詢出來的資料再讀取對應的類別名稱和 id
        public ActionResponse<financialModel> automap_FinancialExportExcel(TWNewEgg.API.Models.financialModel _financialModel_basicDomain)
        {
            ActionResponse<financialModel> result = new ActionResponse<financialModel>();
            TWNewEgg.DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();
            //把要換成 Excel 資料的 productid 收集出來
            List<CartCategory> cartCategoryCell = _financialModel_basicDomain.basicDomain.Select(x => new CartCategory { CartID = x.OrderID }).ToList();
            //List<string> cartIDCell = _financialModel_basicDomain.basicDomain.Select(p => p.OrderID).ToList();
            try
            {
                List<int> itemCategory = new List<int>();

                // 查詢賣場編號
                ActionResponse<List<CartCategory>> getItemID = GetItemID(cartCategoryCell);

                // 查詢第 2 層分類
                ActionResponse<List<CartCategory>> getCategoryID = GetCategoryID(getItemID.Body);

                if (getItemID.IsSuccess && getCategoryID.IsSuccess)
                {
                    List<int> items = getItemID.Body.Select(x => x.ItemID).ToList();
                    var tempItem = dbFront.Item.Where(x => items.Contains(x.ID)).Select(r => new { r.ID, r.ProductID }).ToList();

                    var categoryLayer0_Desc = (from cartCategory in getCategoryID.Body
                                               join temp in tempItem on cartCategory.ItemID equals temp.ID
                                               join categoryLayer2 in dbFront.Category on cartCategory.Layer2.ID equals categoryLayer2.ID
                                               join categoryLayer1 in dbFront.Category on categoryLayer2.ParentID equals categoryLayer1.ID
                                               join categoryLayer0 in dbFront.Category on categoryLayer1.ParentID equals categoryLayer0.ID
                                               select new
                                               {
                                                   categoryid = categoryLayer0.ID,
                                                   Desc = categoryLayer0.Description,
                                                   Product_id = temp.ProductID
                                               }).ToList();

                    List<FinancialExportExcel> financial_List = new List<FinancialExportExcel>();
                    //把類別的名稱和 id 放回對應的 Model
                    foreach (var item in _financialModel_basicDomain.basicDomain)
                    {
                        var _categoryDescription = categoryLayer0_Desc.Where(p => p.Product_id == item.ProductID).FirstOrDefault();

                        // 若查不到給予空字串
                        if (_categoryDescription == null)
                        {
                            item.categoryDescription = string.Empty;
                        }
                        else
                        {
                            item.categoryDescription = _categoryDescription.Desc + ", " + _categoryDescription.categoryid.ToString();
                        }
                    }
                    result.IsSuccess = true;
                    result.Body = _financialModel_basicDomain;
                    result.Msg = "成功";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Body = null;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Body = null;
                result.Msg = "失敗";
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]:" + this.ExceptionInnerMessage(error));
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }


        /// <summary>
        /// 從子單透過主單 ID 查賣場編號
        /// </summary>
        /// <param name="cartCategoryCell">子單編號</param>
        /// <returns>賣場編號</returns>
        private ActionResponse<List<CartCategory>> GetItemID(List<CartCategory> cartCategoryCell)
        {
            ActionResponse<List<CartCategory>> result = new ActionResponse<List<CartCategory>>();
            result.Body = new List<CartCategory>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            #region 查詢賣場編號

            if (cartCategoryCell != null && cartCategoryCell.Count > 0)
            {
                DB.TWBackendDBContext dbBack = new DB.TWBackendDBContext();

                List<string> cartIDCell = cartCategoryCell.Select(x => x.CartID).Distinct().ToList();

                try
                {
                    result.Body = dbBack.Process.Where(x => cartIDCell.Contains(x.CartID) && x.StoreID.HasValue).Select(x => new CartCategory
                    {
                        CartID = x.CartID,
                        ItemID = x.StoreID.Value
                    }).ToList();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("查詢賣場編號失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                }
            }
            else
            {
                result.IsSuccess = false;
                logger.Info(string.Format("查詢賣場編號失敗; ErrorMessage = {0}.", "未傳入參數，或傳入參數的資料筆數為 0"));
            }

            #endregion 查詢賣場編號

            #region 組合輸入、查詢結果

            if (result.IsSuccess)
            {
                try
                {
                    foreach (CartCategory cartCategory in cartCategoryCell)
                    {
                        cartCategory.ItemID = result.Body.Where(x => x.CartID == cartCategory.CartID).Select(x => x.ItemID).FirstOrDefault();
                    }

                    result.Body = new List<CartCategory>();
                    result.Body = cartCategoryCell;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("填寫賣場編號失敗; ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                }
            }

            #endregion 組合輸入、查詢結果

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 從 Item 透過 Item ID 查第 2 層 Category ID
        /// </summary>
        /// <param name="cartCategoryCell">賣場 ID</param>
        /// <returns>第 2 層分類</returns>
        private ActionResponse<List<CartCategory>> GetCategoryID(List<CartCategory> cartCategoryCell)
        {
            ActionResponse<List<CartCategory>> result = new ActionResponse<List<CartCategory>>();
            result.Body = new List<CartCategory>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            #region 查詢第 2 層分類 ID

            if (cartCategoryCell != null && cartCategoryCell.Count > 0)
            {
                DB.TWSqlDBContext dbFront = new DB.TWSqlDBContext();

                List<int> itemIDCell = cartCategoryCell.Select(x => x.ItemID).Distinct().ToList();

                try
                {
                    result.Body = dbFront.Item.Where(x => itemIDCell.Contains(x.ID)).Select(x => new CartCategory
                    {
                        ItemID = x.ID,
                        Layer2 = new CategoryLayer()
                        {
                            ID = x.CategoryID
                        }
                    }).ToList();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("查詢第 2 層分類 ID 失敗(exception); ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                }
            }
            else
            {
                result.IsSuccess = false;
                logger.Info(string.Format("查詢第 2 層分類 ID 失敗; ErrorMessage = {0}.", "未傳入參數，或傳入參數的資料筆數為 0"));
            }

            #endregion 查詢第 2 層分類 ID

            #region 組合輸入、查詢結果

            if (result.IsSuccess)
            {
                try
                {
                    foreach (CartCategory cartCategory in cartCategoryCell)
                    {
                        int categoryID_layer2 = result.Body.Where(x => x.ItemID == cartCategory.ItemID).Select(x => x.Layer2.ID).FirstOrDefault();

                        cartCategory.Layer2.ID = categoryID_layer2 == 0 ? 0 : categoryID_layer2;
                    }

                    result.Body = new List<CartCategory>();
                    result.Body = cartCategoryCell;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("填寫第 2 層分類編號失敗; ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString()));
                }
            }

            #endregion 組合輸入、查詢結果

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #endregion
        #region 產生對帳單的 Excel 報表
        public ActionResponse<string> toGeneraExcelFile(financialModel GeneraExcelModel)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            if (GeneraExcelModel == null)
            {
                result.IsSuccess = false;
                result.Msg = "沒有資料傳入";
                result.Body = null;
                return result;
            }
            GeneraExcelModel.basicDomain.ForEach(p => p.CartDate.Value.AddHours(8));
            //抓取現在的時間用來當作檔案的命名成功
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            TWNewEgg.API.Service.ImageService imgService = new ImageService();
            string saveDate = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            string saveFolder = AppDomain.CurrentDomain.BaseDirectory + "ToExcel\\Financial";

            //檢查資料夾路徑
            if (Directory.Exists(saveFolder) == false)
            {
                Directory.CreateDirectory(saveFolder);
            }

            string FileName = "Finan_" + saveDate;
            string fileSavePath = saveFolder + "\\" + FileName + ".xlsx";
            //string fileSavePath = System.Web.HttpContext.Current.Server.MapPath("~/ToExcel/Financial/" + FileName + ".xlsx");


            try
            {
                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                OfficeOpenXml.ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("對帳單明細");
                string totalQty = string.Empty;
                string totalSumPrice = string.Empty;
                #region 預設 Column 的 Windth
                sheet.Column(1).Width = 14;
                sheet.Column(2).Width = 19;
                sheet.Column(3).Width = 19;
                sheet.Column(4).Width = 20;
                sheet.Column(5).Width = 20;
                sheet.Column(6).Width = 20;
                sheet.Column(7).Width = 25;
                sheet.Column(8).Width = 20;
                sheet.Column(9).Width = 60;
                sheet.Column(10).Width = 16;
                sheet.Column(11).Width = 16;
                sheet.Column(12).Width = 16;
                sheet.Column(13).Width = 16;
                #endregion
                #region 主單的資料(廠商名稱,帳單年月日, 結算區間 )
                sheet.Cells[1, 1].Value = "廠商名稱：";
                string _strSap = string.IsNullOrEmpty(GeneraExcelModel.masterTopData.sellerInfo_IDNAMESAP.Sap) == true ? "" : GeneraExcelModel.masterTopData.sellerInfo_IDNAMESAP.Sap;
                string _strSellerName = string.IsNullOrEmpty(GeneraExcelModel.masterTopData.sellerInfo_IDNAMESAP.sellerName) == true ? "" : GeneraExcelModel.masterTopData.sellerInfo_IDNAMESAP.sellerName;
                sheet.Cells[1, 2].Value = _strSap + " " + _strSellerName;

                sheet.Cells[2, 1].Value = "帳單年月日：";
                sheet.Cells[2, 2].Value = GeneraExcelModel.masterTopData.SettleMonth.ToString();

                sheet.Cells[3, 1].Value = "結算區間：";
                string startDate = GeneraExcelModel.masterTopData.DateStart.Year.ToString() + "/" + GeneraExcelModel.masterTopData.DateStart.Month.ToString() + "/" + GeneraExcelModel.masterTopData.DateStart.Day.ToString();
                string startEnd = GeneraExcelModel.masterTopData.DateEnd.Year.ToString() + "/" + GeneraExcelModel.masterTopData.DateEnd.Month.ToString() + "/" + GeneraExcelModel.masterTopData.DateEnd.Day.ToString();
                sheet.Cells[3, 2].Value = startDate + "~" + startEnd;
                #endregion

                #region 訂單明細
                #region 訂單明細 title
                sheet.Cells[6, 1].Value = "訂單明細";
                sheet.Cells[7, 1].Value = "序號";
                sheet.Cells[7, 2].Value = "訂單日期";
                sheet.Cells[7, 3].Value = "出貨日期";
                sheet.Cells[7, 4].Value = "訂單編號";
                sheet.Cells[7, 5].Value = "採購編號";
                sheet.Cells[7, 6].Value = "新蛋商品編號";
                sheet.Cells[7, 7].Value = "品項類別";
                sheet.Cells[7, 8].Value = "商家商品編號";
                sheet.Cells[7, 9].Value = "商品名稱";
                sheet.Cells[7, 10].Value = "單價";
                sheet.Cells[7, 11].Value = "數量";
                sheet.Cells[7, 12].Value = "總額";
                #endregion
                //設定置中, 從(7, 1)->(7, 11)
                sheet.Cells[7, 1, 7, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //畫線(畫: 下, 上, 右)
                sheet.Cells[7, 1, 7, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[7, 1, 7, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[7, 1, 7, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                //根據需要的 Excel row 位置設定初始位置
                int nowRaw = 8;
                //訂單明細的資料
                var orderData = GeneraExcelModel.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.訂單 || p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉).ToList();
                //判斷訂單明細是否有資料, 有在產生 Excel
                if (orderData.Count != 0)
                {
                    for (int i = 0; i < orderData.Count; i++)
                    {
                        #region 開始針對訂單明細部分開始把資料寫入 Excel
                        //序號
                        sheet.Cells[nowRaw, 1].Value = i + 1;
                        // 訂單日期
                        sheet.Cells[nowRaw, 2].Value = orderData[i].CartDate == null ? "" : orderData[i].CartDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                        // 出貨日期
                        sheet.Cells[nowRaw, 3].Value = orderData[i].TrackDate == null ? "" : orderData[i].TrackDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                        // 訂單編號
                        sheet.Cells[nowRaw, 4].Value = orderData[i].OrderID == null ? "" : orderData[i].OrderID.ToString();
                        // 採購編號
                        sheet.Cells[nowRaw, 5].Value = orderData[i].POID == null ? "" : orderData[i].POID.ToString();
                        // 新蛋商品編號
                        sheet.Cells[nowRaw, 6].Value = orderData[i].ProductID == null ? "" : orderData[i].ProductID.ToString();
                        // 品項類別
                        sheet.Cells[nowRaw, 7].Value = orderData[i].categoryDescription == null ? "" : orderData[i].categoryDescription.ToString();
                        // 商家商品編號
                        sheet.Cells[nowRaw, 8].Value = orderData[i].SellerProductID == null ? "" : orderData[i].SellerProductID.ToString();
                        // 商品名稱
                        sheet.Cells[nowRaw, 9].Value = orderData[i].ProductName == null ? "" : orderData[i].ProductName.ToString();
                        // 單價
                        sheet.Cells[nowRaw, 10].Value = orderData[i].UnitPrice_Total == null ? "" : orderData[i].UnitPrice_Total.ToString("N0");
                        // 數量
                        sheet.Cells[nowRaw, 11].Value = orderData[i].Qty == null ? "" : orderData[i].Qty.ToString();
                        // 總額
                        sheet.Cells[nowRaw, 12].Value = orderData[i].SumPrice_Total == null ? "" : orderData[i].SumPrice_Total.ToString("N0");
                        #endregion
                        //自動換行
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.WrapText = true;
                        //設定資料置中
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //畫線(畫: 下, 上, 右)
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        nowRaw++;
                    }
                    sheet.Cells[nowRaw, 1, nowRaw, 10].Merge = true;
                    sheet.Cells[nowRaw, 1].Value = "合計：";
                    sheet.Cells[nowRaw, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    totalQty = orderData.ToList().Sum(p => p.Qty).ToString();
                    totalSumPrice = orderData.ToList().Sum(p => p.SumPrice_Total).ToString("N0");
                    sheet.Cells[nowRaw, 11].Value = totalQty;
                    sheet.Cells[nowRaw, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[nowRaw, 12].Value = totalSumPrice;
                    sheet.Cells[nowRaw, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //畫線(畫: 下, 上, 右)
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                else
                {
                    sheet.Cells[nowRaw, 1, nowRaw, 10].Merge = true;
                    sheet.Cells[nowRaw, 1].Value = "合計：";
                    sheet.Cells[nowRaw, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[nowRaw, 11].Value = string.Empty;
                    sheet.Cells[nowRaw, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[nowRaw, 12].Value = string.Empty;
                    sheet.Cells[nowRaw, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //畫線(畫: 下, 上, 右)
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                #endregion
                nowRaw = nowRaw + 2;
                #region 退貨明細
                sheet.Cells[nowRaw, 1].Value = "退貨明細";
                nowRaw++;
                //讀取退貨明細的資料
                var ReturnedData = GeneraExcelModel.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.退貨).ToList();
                #region 寫入退貨明細的 title
                sheet.Cells[nowRaw, 1].Value = "序號";
                sheet.Cells[nowRaw, 2].Value = "出貨日期";
                sheet.Cells[nowRaw, 3].Value = "退貨日期";
                sheet.Cells[nowRaw, 4].Value = "訂單編號";
                sheet.Cells[nowRaw, 5].Value = "採購編號";
                sheet.Cells[nowRaw, 6].Value = "新蛋商品編號";
                sheet.Cells[nowRaw, 7].Value = "品項類別";
                sheet.Cells[nowRaw, 8].Value = "商家商品編號";
                sheet.Cells[nowRaw, 9].Value = "商品名稱";
                sheet.Cells[nowRaw, 10].Value = "單價";
                sheet.Cells[nowRaw, 11].Value = "數量";
                sheet.Cells[nowRaw, 12].Value = "總額";
                //畫線(上, 下, 右)
                sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                #endregion
                //設定置中
                sheet.Cells[nowRaw, 1, nowRaw, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                nowRaw++;
                if (ReturnedData.Count != 0)
                {
                    for (int i = 0; i < ReturnedData.Count; i++)
                    {
                        #region 開始針對退貨明細部分開始把資料寫入 Excel
                        //序號
                        sheet.Cells[nowRaw, 1].Value = i + 1;
                        // 出貨日期
                        sheet.Cells[nowRaw, 2].Value = ReturnedData[i].TrackDate == null ? "" : ReturnedData[i].TrackDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                        // 退貨日期
                        sheet.Cells[nowRaw, 3].Value = ReturnedData[i].RMADate == null ? "" : ReturnedData[i].RMADate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                        // 訂單編號
                        sheet.Cells[nowRaw, 4].Value = ReturnedData[i].RegoodOrderID == null ? "" : ReturnedData[i].RegoodOrderID.ToString();
                        // 採購編號
                        sheet.Cells[nowRaw, 5].Value = ReturnedData[i].POID == null ? "" : ReturnedData[i].POID.ToString();
                        // 新蛋商品編號
                        sheet.Cells[nowRaw, 6].Value = ReturnedData[i].ProductID == null ? "" : ReturnedData[i].ProductID.ToString();
                        // 品項類別
                        sheet.Cells[nowRaw, 7].Value = ReturnedData[i].categoryDescription == null ? "" : ReturnedData[i].categoryDescription.ToString();
                        // 商家商品編號
                        sheet.Cells[nowRaw, 8].Value = ReturnedData[i].SellerProductID == null ? "" : ReturnedData[i].SellerProductID.ToString();
                        // 商品名稱
                        sheet.Cells[nowRaw, 9].Value = ReturnedData[i].ProductName == null ? "" : ReturnedData[i].ProductName.ToString();
                        // 單價
                        sheet.Cells[nowRaw, 10].Value = ReturnedData[i].UnitPrice_Total == null ? "" : ReturnedData[i].UnitPrice_Total.ToString("N0");
                        // 數量
                        sheet.Cells[nowRaw, 11].Value = ReturnedData[i].Qty == null ? "" : ReturnedData[i].Qty.ToString();
                        // 總額
                        sheet.Cells[nowRaw, 12].Value = ReturnedData[i].SumPrice_Total == null ? "" : ReturnedData[i].SumPrice_Total.ToString("N0");
                        //自動換行
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.WrapText = true;
                        #endregion
                        //設定置中
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //畫線(上, 下, 右)
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        nowRaw++;
                    }
                    sheet.Cells[nowRaw, 1, nowRaw, 10].Merge = true;
                    sheet.Cells[nowRaw, 1].Value = "合計：";
                    sheet.Cells[nowRaw, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    totalQty = ReturnedData.ToList().Sum(p => p.Qty).ToString();
                    totalSumPrice = ReturnedData.ToList().Sum(p => p.SumPrice_Total).ToString("N0");
                    sheet.Cells[nowRaw, 11].Value = totalQty;
                    sheet.Cells[nowRaw, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[nowRaw, 12].Value = totalSumPrice;
                    sheet.Cells[nowRaw, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //畫線(上, 下, 右)
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                else
                {
                    sheet.Cells[nowRaw, 1, nowRaw, 10].Merge = true;
                    sheet.Cells[nowRaw, 1].Value = "合計：";
                    sheet.Cells[nowRaw, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[nowRaw, 11].Value = string.Empty;
                    sheet.Cells[nowRaw, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[nowRaw, 12].Value = string.Empty;
                    sheet.Cells[nowRaw, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //畫線(上, 下, 右)
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                #endregion
                nowRaw = nowRaw + 2;
                #region 寄倉明細
                sheet.Cells[nowRaw, 1].Value = "寄倉明細";
                nowRaw++;
                var WareHouseData = GeneraExcelModel.basicDomain.Where(p => p.SettleType == (int)TWNewEgg.DB.TWBACKENDDB.Models.Seller_FinanDetail.SettleType_Identify.寄倉).ToList();
                sheet.Cells[nowRaw, 1].Value = "序號";
                sheet.Cells[nowRaw, 2].Value = "訂單日期";
                sheet.Cells[nowRaw, 3].Value = "出貨日期";
                sheet.Cells[nowRaw, 4].Value = "訂單編號";
                sheet.Cells[nowRaw, 5].Value = "採購編號";
                sheet.Cells[nowRaw, 6].Value = "新蛋商品編號";
                sheet.Cells[nowRaw, 7].Value = "品項類別";
                sheet.Cells[nowRaw, 8].Value = "商家商品編號";
                sheet.Cells[nowRaw, 9].Value = "商品名稱";
                sheet.Cells[nowRaw, 10].Value = "材積";
                sheet.Cells[nowRaw, 11].Value = "運費金額";
                sheet.Cells[nowRaw, 12].Value = "出貨處理費";
                //sheet.Cells[nowRaw, 13].Value = "逆物流運費金額";
                //sheet.Cells[nowRaw, 14].Value = "逆物流出貨處理費";
                sheet.Cells[nowRaw, 13].Value = "總額";
                //設定置中
                sheet.Cells[nowRaw, 1, nowRaw, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //畫線(上, 下, 右)
                sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                nowRaw++;
                if (WareHouseData.Count != 0)
                {
                    for (int i = 0; i < WareHouseData.Count; i++)
                    {
                        #region 開始針對倉儲明細部分開始把資料寫入 Excel
                        // 序號
                        sheet.Cells[nowRaw, 1].Value = i + 1;
                        // 訂單日期
                        sheet.Cells[nowRaw, 2].Value = WareHouseData[i].CartDate == null ? "" : WareHouseData[i].CartDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                        // 出貨日期
                        sheet.Cells[nowRaw, 3].Value = WareHouseData[i].TrackDate == null ? "" : WareHouseData[i].TrackDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                        // 訂單編號
                        sheet.Cells[nowRaw, 4].Value = WareHouseData[i].OrderID == null ? "" : WareHouseData[i].OrderID.ToString();
                        // 採購編號
                        sheet.Cells[nowRaw, 5].Value = WareHouseData[i].POID == null ? "" : WareHouseData[i].POID.ToString();
                        // 新蛋商品編號
                        sheet.Cells[nowRaw, 6].Value = WareHouseData[i].ProductID == null ? "" : WareHouseData[i].ProductID.ToString();
                        // 品項類別
                        sheet.Cells[nowRaw, 7].Value = WareHouseData[i].categoryDescription == null ? "" : WareHouseData[i].categoryDescription.ToString();
                        // 新蛋商品編號
                        sheet.Cells[nowRaw, 8].Value = WareHouseData[i].SellerProductID == null ? "" : WareHouseData[i].SellerProductID.ToString();
                        // 商品名稱
                        sheet.Cells[nowRaw, 9].Value = WareHouseData[i].ProductName == null ? "" : WareHouseData[i].ProductName.ToString();
                        // 材積
                        sheet.Cells[nowRaw, 10].Value = WareHouseData[i].Size == null ? "" : WareHouseData[i].Size.Value.ToString("N0");
                        // 運費金額
                        sheet.Cells[nowRaw, 11].Value = WareHouseData[i].ShipFee_Total == null ? "" : WareHouseData[i].ShipFee_Total.ToString("N0");
                        // 出貨處理費
                        sheet.Cells[nowRaw, 12].Value = WareHouseData[i].LogisticAmount_Total == null ? "" : WareHouseData[i].LogisticAmount_Total.ToString("N0");
                        // 總額
                        sheet.Cells[nowRaw, 13].Value = WareHouseData[i].WhereHouse_Total == null ? "" : WareHouseData[i].WhereHouse_Total.ToString("N0");
                        #endregion
                        //自動換行
                        sheet.Cells[nowRaw, 1, nowRaw, 13].Style.WrapText = true;
                        //設定置中(nowRaw, 1) -> (nowRaw, 12)
                        sheet.Cells[nowRaw, 1, nowRaw, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //畫線(上, 下, 右)
                        sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        nowRaw++;
                    }
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Merge = true;
                    sheet.Cells[nowRaw, 1].Value = "合計：";
                    sheet.Cells[nowRaw, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    totalSumPrice = WareHouseData.ToList().Sum(p => p.WhereHouse_Total).ToString("N0");
                    sheet.Cells[nowRaw, 13].Value = totalSumPrice;
                    sheet.Cells[nowRaw, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //畫線(上, 下, 右)
                    sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                else
                {
                    sheet.Cells[nowRaw, 1, nowRaw, 12].Merge = true;
                    sheet.Cells[nowRaw, 1].Value = "合計：";
                    sheet.Cells[nowRaw, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    sheet.Cells[nowRaw, 13].Value = string.Empty;
                    sheet.Cells[nowRaw, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //畫線(上, 下, 右)
                    sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nowRaw, 1, nowRaw, 13].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                #endregion
                excel.SaveAs(new System.IO.FileInfo(fileSavePath));
                string fileSavePathDownLoad = System.Configuration.ConfigurationSettings.AppSettings["ReturnExcel"] + "Financial/" + FileName + ".xlsx";
                result.IsSuccess = true;
                result.Msg = "產生 Excel 成功";
                result.Body = fileSavePathDownLoad;
                return result;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "產生 Excel 失敗";
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMessage]:" + this.ExceptionInnerMessage(error));
            }
            return result;
        }
        #endregion
        #region 讀取 Exception Inner Message
        public string ExceptionInnerMessage(Exception error)
        {
            string returnMsg = string.Empty;
            string ExceptionMsg = "[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace;
            returnMsg = ExceptionMsg + (error.InnerException == null ? "" : error.InnerException.Message);
            return returnMsg;
        }
        #endregion
        #region 檢查字串中是否有中文字
        public bool isCheckChineseWord(string checkStr)
        {
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("^[\u4e00-\u9fa5]$");
            bool isHaveChiceseWord = true;
            for (int i = 0; i < checkStr.Length; i++)
            {
                //有中文字
                if (rx.IsMatch(checkStr[i].ToString()) == true)
                {
                    isHaveChiceseWord = true;
                    break;
                }
                else
                {
                    //沒有中文字
                    isHaveChiceseWord = false;
                }
            }
            return isHaveChiceseWord;
        }
        #endregion

        /// <summary>
        /// 修改子單的發票狀態註記欄位
        /// </summary>
        /// <param name="settlementID">主單編號</param>
        /// <param name="sellerID">商家編號</param>
        /// <returns>成功、失敗訊息</returns>
        private ActionResponse<bool> MarkInvoiced(string settlementID, string isCheck)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 連接後台
            DB.TWBackendDBContext dbBack = new DB.TWBackendDBContext();

            #region 讀取子單

            List<DB.TWBACKENDDB.Models.Seller_FinanDetail> dbData = new List<DB.TWBACKENDDB.Models.Seller_FinanDetail>();

            try
            {
                dbData = dbBack.Seller_FinanDetail.Where(x => x.SettlementID == settlementID).ToList();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                logger.Info(string.Format("讀取子單失敗(exception); 主單編號 = {2}; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace, settlementID));
            }

            if (dbData.Count <= 0)
            {
                result.IsSuccess = false;
                logger.Info(string.Format("讀取子單失敗; 主單編號 = {0}; ErrorMessage = {1}.", settlementID, "查無子單"));
            }

            #endregion 讀取子單

            #region 修改 IsCheck 欄位

            if (result.IsSuccess)
            {
                try
                {
                    foreach (DB.TWBACKENDDB.Models.Seller_FinanDetail data in dbData)
                    {
                        data.IsCheck = isCheck;
                        dbBack.Entry(data).State = System.Data.EntityState.Modified;
                    }

                    dbBack.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    logger.Info(string.Format("儲存子單失敗(exception); 主單編號 = {2}; ErrorMessage = {0}; Exception = {1}.", GetExceptionMessage(ex), ex.ToString(), settlementID));
                }
            }

            #endregion 修改 IsCheck 欄位

            result.Body = result.IsSuccess;
            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }
    }
}
