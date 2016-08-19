using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TWNewEgg.FinanceServices.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Finance;
using TWNewEgg.FinanceRepoAdapters.Interface;
using System.Threading;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.DAL.Interface;
//using TWNewEgg.DAL.Repository;
using Autofac;
//using TWNewEgg.CommonService.DomainModels;
//using TWNewEgg.CommonService.Interface;

namespace TWNewEgg.FinanceServices
{
    public partial class FinanceDocumentService : IFinanceDocumentService
    {
        log4net.ILog _logger = log4net.LogManager.GetLogger(LoggerInfo.FinanceLog);

        IAccountsProfileRepoAdapter _accProfileRepoAdapter;
        IFinanceRepoAdapter _finanRepoAdapter;
        ISapBapiAccDocumentRepoAdapter _sapRepoAdapter;
        ISellerFinanceRepoAdapter _sellerFinanRepoAdapter;

        ICompanyFinanceDataService _twNewEggFinanService;
        
        /// <summary>
        /// TWNewEgg會計基本資料
        /// </summary>
        List<FinanceDataListFinanceData> _twNewEggFinanList = new List<FinanceDataListFinanceData>();
        
        public FinanceDocumentService(IAccountsProfileRepoAdapter accProfileRepoAdapter, IFinanceRepoAdapter finanRepoAdapter, ISapBapiAccDocumentRepoAdapter sapRepoAdapter,
            ISellerFinanceRepoAdapter sellerFinanRepoAdapter)
        {
            this._accProfileRepoAdapter = accProfileRepoAdapter;
            this._finanRepoAdapter = finanRepoAdapter;
            this._sapRepoAdapter = sapRepoAdapter;
            this._sellerFinanRepoAdapter = sellerFinanRepoAdapter;

            //取得TWNewEgg會計基本資料
            this._twNewEggFinanService = new CompanyFinanceDataService();
            this._twNewEggFinanList = this._twNewEggFinanService.GetAll().FinanceData;
        }

        enum DateRangeEnum
        {
            Start,
            End
        }
        private DateTime DateTimeParse(DateTime oDate, DateRangeEnum rangeType)
        {
            string strTime = "00:00:00";
            if (rangeType == DateRangeEnum.End)
                strTime = "23:59:59";

            return DateTime.Parse(string.Format("{0:yyyy/MM/dd} {1}", oDate, strTime));
        }

        /// <summary>
        /// 建立會計文件 (XQ、XD、XI、XIRMA)
        /// </summary>
        /// <param name="finanDocType"></param>
        /// <returns></returns>
        public ResponseMessage<string> Create(DateTime startFinanDate, DateTime endFinanDate, FinanDocTypeEnum finanDocType)
        {
            DateTime startDate, endDate, finishDate;
            ResponseMessage<string> result = new ResponseMessage<string>();

            List<AccountsDocumentType> accDocTypeList = null;
            List<SapDocumentInfo> sapList = null;
            FinanceDataListFinanceData twNewEggData = null;

            bool blnSuccess = true;

            try
            {
                //記錄待執行的會計文件類型
                ArrayList docTypeList = new ArrayList();
                switch (finanDocType)
                {
                    case FinanDocTypeEnum.XQ:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XQ);
                        break;
                    case FinanDocTypeEnum.XD:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XD);
                        break;
                    case FinanDocTypeEnum.XI:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XI);
                        break;
                    case FinanDocTypeEnum.XIRMA:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XIRMA);
                        break;
                    default:
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XQ);
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XD);
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XI);
                        docTypeList.Add(AccountsDocumentType.DocTypeEnum.XIRMA);
                        break;
                }

                //起始日期
                startDate = DateTimeParse(startFinanDate, DateRangeEnum.Start);
                //執行完成日期
                finishDate = DateTimeParse(endFinanDate, DateRangeEnum.End);

                //起始日期需為新版會計文件程式上線日期(2016/6/26)以後
                if (startDate < DateTime.Parse("2016/6/26"))
                    startDate = DateTime.Parse("2016/6/26");

                //每日各別產生會計文件
                do
                {
                    //迄止日期
                    endDate = DateTimeParse(startDate, DateRangeEnum.End);

                    foreach (AccountsDocumentType.DocTypeEnum docType in docTypeList)
                    {
                        #region -- 依台蛋訂單產生會計文件 --
                        //取得會計文件所需的購物車List
                        List<CartGroupInfo> cartGrpList = new List<CartGroupInfo>();
                        switch (docType)
                        {
                            case AccountsDocumentType.DocTypeEnum.XQ:
                                cartGrpList = this._finanRepoAdapter.GetXQData(startDate, endDate);
                                break;
                            case AccountsDocumentType.DocTypeEnum.XD:
                                cartGrpList.AddRange(this._finanRepoAdapter.GetXDData_Offline(startDate, endDate));
                                cartGrpList.AddRange(this._finanRepoAdapter.GetXDData_PayOnDelivery(startDate, endDate));
                                break;
                            case AccountsDocumentType.DocTypeEnum.XI:
                                cartGrpList = this._finanRepoAdapter.GetXIData(startDate, endDate);
                                break;
                            case AccountsDocumentType.DocTypeEnum.XIRMA:
                                cartGrpList = this._finanRepoAdapter.GetXIRMAData(startDate, endDate);
                                break;
                        }                        

                        if (cartGrpList.Count > 0)
                        {
                            //讀取設定檔 AccountsDocumentType (排除海外切貨XI-美金)
                            accDocTypeList = this._accProfileRepoAdapter.GetAccDocument(docType)
                                .Where(x => x.Code != AccountsDocumentType.OverSeaBuyOutUSDCode).ToList();
                            sapList = null;

                            foreach (CartGroupInfo cartGrpInfo in cartGrpList)
                            {
                                try
                                {
                                    //銀行資料
                                    BankAccountsInfo bankInfo = null;
                                    switch (docType)
                                    {
                                        case AccountsDocumentType.DocTypeEnum.XQ:
                                            //需取得銀行會計科目
                                            bankInfo = this._finanRepoAdapter.GetBank(cartGrpInfo.SalesOrderList.First().CardBank, true);
                                            break;
                                        default:
                                            //不需取得銀行會計科目
                                            bankInfo = this._finanRepoAdapter.GetBank(cartGrpInfo.SalesOrderList.First().CardBank);
                                            break;
                                    }

                                    //符合條件的 TWNewEgg會計基本資料
                                    twNewEggData = this._twNewEggFinanService.Get(cartGrpInfo.DocDate, this._twNewEggFinanList);
                                    if (twNewEggData == null)
                                        throw new Exception(string.Format("FinanceList.xml 無符合的資料 ({0})。", cartGrpInfo.SalesOrderList.First().ID));

                                    //產生SAP資料
                                    sapList = new List<SapDocumentInfo>();
                                    foreach (AccountsDocumentType accDocTypeInfo in accDocTypeList)
                                    {
                                        bool blnExec = true;
                                        //區分webATM與貨到付款
                                        if (docType == AccountsDocumentType.DocTypeEnum.XD)
                                        {
                                            switch (accDocTypeInfo.Code)
                                            {
                                                case AccountsDocumentType.PayOnDeliveryCode:
                                                    //貨到付款
                                                    //if (!PayOnDeliveryPayTypeList.Contains(cartGrpInfo.SalesOrderList.First().PayType ?? 0))
                                                    //    blnExec = false;

                                                    if (cartGrpInfo.DataType != CartGroupInfo.DataTypeEnum.XD_PayOnDelivery)
                                                        blnExec = false;
                                                    break;

                                                default:
                                                    //webATM
                                                    //if (PayOnDeliveryPayTypeList.Contains(cartGrpInfo.SalesOrderList.First().PayType ?? 0))
                                                    //    blnExec = false;

                                                    if (cartGrpInfo.DataType == CartGroupInfo.DataTypeEnum.XD_PayOnDelivery)
                                                        blnExec = false;

                                                    break;
                                            }
                                        }

                                        if (blnExec)
                                            CreateSAPData(sapList, accDocTypeInfo, cartGrpInfo, twNewEggData, bankInfo);
                                    }

                                    //儲存SAP資料
                                    SaveSAPData(sapList);
                                }
                                catch (Exception ex)
                                {
                                    this._logger.Error(string.Format("DocType: {0}, SalesOrderGroupID: {1}", docType.ToString(), cartGrpInfo.SalesOrderGroupID), ex);
                                    //result.Error.Detail += "部份資料異常，請檢示LOG。";
                                    blnSuccess = false;
                                }
                            }
                        }
                        #endregion

                        #region -- 依美蛋訂單(台蛋採購單)產生會計文件 --
                        //海外切貨XI-美金
                        if (docType == AccountsDocumentType.DocTypeEnum.XI)
                        {
                            //取得會計文件所需的採購單List
                            List<PurchaseOrderItemGroupInfo> poGrpList = new List<PurchaseOrderItemGroupInfo>();
                            poGrpList = this._finanRepoAdapter.GetXIData_OverSeaBuyOutUSD(startDate, endDate);

                            sapList = null;
                            foreach (PurchaseOrderItemGroupInfo poGrpInfo in poGrpList)
                            {
                                try
                                {
                                    //符合條件的 TWNewEgg會計基本資料
                                    twNewEggData = this._twNewEggFinanService.Get(poGrpInfo.DocDate, this._twNewEggFinanList);
                                    if (twNewEggData == null)
                                        throw new Exception(string.Format("FinanceList.xml 無符合的資料 ({0})。", poGrpInfo.PurchaseOrderItemList.First().SellerorderCode));

                                    //產生SAP資料
                                    sapList = new List<SapDocumentInfo>();
                                    CreateSAPData_Newegg(sapList, poGrpInfo, twNewEggData);

                                    //儲存SAP資料
                                    SaveSAPData(sapList);
                                }
                                catch (Exception ex)
                                {
                                    this._logger.Error(string.Format("DocType: {0}, PurchaseOrderitem.Code: {1}", docType.ToString(), poGrpInfo.PurchaseOrderItemList.First().Code), ex);
                                    blnSuccess = false;
                                }
                            }
                        }
                        #endregion  
                    }

                    startDate = startDate.AddDays(1);
                } while (startDate <= finishDate);

                if (!blnSuccess)
                {
                    throw new Exception("部份資料異常，請檢示LOG。");
                }

                result.IsSuccess = true;
                result.Message = "FinanceDocument Done";
            }
            catch (Exception ex)
            {
                this._logger.Error(ex);

                result.IsSuccess = false;
                result.Error.Detail += (ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 儲存會計文件相關資料
        /// </summary>
        /// <param name="sapList"></param>
        private void SaveSAPData(List<SapDocumentInfo> sapList)
        {
            if (sapList.Count > 0)
            {
                //寫入資料 (Thread)
                Task newTask = Task.Factory.StartNew(() =>
                {
                    using (var scope = AutofacConfig.Container.BeginLifetimeScope())
                    {
                        ISapBapiAccDocumentRepoAdapter sapRepoAdapter = scope.Resolve<ISapBapiAccDocumentRepoAdapter>();

                        foreach (SapDocumentInfo sapInfo in sapList.Where(x => x.DocDetail.Count > 0))
                        {
                            try
                            {                                
                                sapRepoAdapter.SaveDocument(sapInfo);
                            }
                            catch (Exception ex)
                            {
                                if (sapInfo.SalesOrderList != null && sapInfo.SalesOrderList.Count() > 0)
                                    this._logger.Error(string.Format("DocType: {0}, SalesorderGroupID: {1}", sapInfo.DocType.ToString(), sapInfo.SalesOrderList.First().SalesorderGroupID), ex);
                                else
                                    this._logger.Error(string.Format("DocType: {0}, PurchaseOrderitem.Code: {1}", sapInfo.DocType.ToString(), sapInfo.PurchaseOrderItemList.First().Code), ex);
                            }
                        }
                    }
                }).ContinueWith(task =>
                {
                    //this._logger.Info(string.Format("LBO:{0}=>IsCompleted:{1}, IsFaulted:{2}",
                    //    sapInfo.SalesOrder.ID, task.IsCompleted, task.IsFaulted));
                });
            }
        }

        /// <summary>
        /// 依台蛋訂單產生會計文件
        /// </summary>
        /// <param name="sapList"></param>
        /// <param name="accDocTypeInfo"></param>
        /// <param name="cartGrpInfo"></param>
        /// <param name="twNewEggData"></param>
        /// <param name="bankInfo"></param>
        private void CreateSAPData(List<SapDocumentInfo> sapList, AccountsDocumentType accDocTypeInfo, CartGroupInfo cartGrpInfo, FinanceDataListFinanceData twNewEggData, BankAccountsInfo bankInfo)
        {
            AccountsDocumentType.DocTypeEnum docType;

            SapDocumentInfo sapInfo = null;
            List<Cart> cartList = null;
            IEnumerable<Cart> cartSubList = null;

            List<ItemInStock_trans> itemInStockTransList =null;
            List<Process> procList = null;
            SapPriceInfo priceInfo = null;

            List<int> shipTypeList = new List<int>();
            List<ChartOfAccountsProfile> coaProfileList = new List<ChartOfAccountsProfile>();
            
            try
            {
                docType = (AccountsDocumentType.DocTypeEnum)Enum.Parse(typeof(AccountsDocumentType.DocTypeEnum), accDocTypeInfo.DocType, true);

                //Sap_BapiAccDocument_DocHeader
                switch (docType)
                {
                    case AccountsDocumentType.DocTypeEnum.XQ:
                        sapInfo = new SapDocumentInfo();
                        sapInfo.DocType = docType;
                        sapInfo.AccDocTypeCode = accDocTypeInfo.Code;

                        //sapInfo.DocHeader = CreateSAPDocHeaderForXQ(sapInfo.DocType, cartGrpInfo, twNewEggData, bankInfo);
                        sapInfo.DocHeader = CreateSAPDocHeaderForXQ(accDocTypeInfo, cartGrpInfo, twNewEggData, bankInfo);

                        //XQ所有交易模式下，會計科目都相同
                        coaProfileList = this._accProfileRepoAdapter.GetChartOfAccPorfile(accDocTypeInfo.Code, 0).ToList();

                        sapInfo.SalesOrderList = cartGrpInfo.SalesOrderList.ToList();

                        //產生一車的應收金額                        
                        CreateSAPDetailForLBO(accDocTypeInfo, coaProfileList, cartGrpInfo, twNewEggData, cartGrpInfo.SalesOrderList.First(), bankInfo, sapInfo, priceInfo);

                        sapList.Add(sapInfo);
                        break;

                    case AccountsDocumentType.DocTypeEnum.XD:
                        sapInfo = new SapDocumentInfo();
                        sapInfo.DocType = docType;
                        sapInfo.AccDocTypeCode = accDocTypeInfo.Code;

                        //sapInfo.DocHeader = CreateSAPDocHeaderForXD(sapInfo.DocType, cartGrpInfo, twNewEggData, bankInfo);
                        sapInfo.DocHeader = CreateSAPDocHeaderForXD(accDocTypeInfo, cartGrpInfo, twNewEggData, bankInfo);

                        //XD所有交易模式下，會計科目都相同
                        coaProfileList = this._accProfileRepoAdapter.GetChartOfAccPorfile(accDocTypeInfo.Code, 0).ToList();

                        sapInfo.SalesOrderList = cartGrpInfo.SalesOrderList.ToList();

                        //產生一車的應收金額                        
                        CreateSAPDetailForLBO(accDocTypeInfo, coaProfileList, cartGrpInfo, twNewEggData, cartGrpInfo.SalesOrderList.First(), bankInfo, sapInfo, priceInfo);

                        sapList.Add(sapInfo);
                        break;

                    case AccountsDocumentType.DocTypeEnum.XI:
                        //一車內有開立發票的訂單
                        cartList = cartGrpInfo.SalesOrderList.ToList();

                        //依交易模式產生XI會計文件
                        shipTypeList = cartList.Select(x => x.ShipType ?? 0).Distinct().ToList();
                        foreach (int intShipType in shipTypeList)
                        {
                            //取得會計文件設定檔
                            coaProfileList = this._accProfileRepoAdapter.GetChartOfAccPorfile(accDocTypeInfo.Code, intShipType).ToList();
                            if (coaProfileList.Count > 0)
                            {
                                sapInfo = new SapDocumentInfo();
                                sapInfo.DocType = docType;
                                sapInfo.AccDocTypeCode = accDocTypeInfo.Code;
                                
                                //明細記錄購物車內同一交易模式的訂單
                                cartSubList = cartList.Where(x => x.ShipType == intShipType);//.ToList();

                                cartGrpInfo.SalesOrderList = cartSubList;
                                sapInfo.SalesOrderList = cartSubList.ToList();

                                //sapInfo.DocHeader = CreateSAPDocHeaderForXI(sapInfo.DocType, cartGrpInfo, twNewEggData, bankInfo, intShipType);
                                sapInfo.DocHeader = CreateSAPDocHeaderForXI(accDocTypeInfo, cartGrpInfo, twNewEggData, bankInfo, intShipType);

                                foreach (Cart cartInfo in cartSubList)
                                {
                                    //金額計算(LBO)
                                    itemInStockTransList = this._sellerFinanRepoAdapter.GetItemInStockTrans(AccountsDocumentType.DocTypeEnum.XI, cartInfo.ID).ToList();
                                    procList = this._finanRepoAdapter.GetCartProcess(cartInfo.ID).ToList();

                                    priceInfo = new SapPriceInfo(AccountsDocumentType.DocTypeEnum.XI, cartInfo.ShipType ?? 0, itemInStockTransList, procList);

                                    CreateSAPDetailForLBO(accDocTypeInfo, coaProfileList, cartGrpInfo, twNewEggData, cartInfo, bankInfo, sapInfo, priceInfo);
                                }

                                sapList.Add(sapInfo);
                            }
                        }
                        break;

                    case AccountsDocumentType.DocTypeEnum.XIRMA:
                        //一車內的所有訂單
                        cartList = cartGrpInfo.SalesOrderList.ToList();
                        Retgood retgoodInfo = null;

                        //依交易模式產生XIRMA會計文件
                        shipTypeList = cartList.Select(x => x.ShipType ?? 0).Distinct().ToList();
                        foreach (int intShipType in shipTypeList)
                        {
                            //取得會計文件設定檔
                            coaProfileList = this._accProfileRepoAdapter.GetChartOfAccPorfile(accDocTypeInfo.Code, intShipType).ToList();
                            if (coaProfileList.Count > 0)
                            {
                                sapInfo = new SapDocumentInfo();
                                sapInfo.DocType = docType;
                                sapInfo.AccDocTypeCode = accDocTypeInfo.Code;
                                
                                //明細記錄購物車內同一交易模式的訂單
                                cartSubList = cartList.Where(x => x.ShipType == intShipType);//.ToList();

                                cartGrpInfo.SalesOrderList = cartSubList;
                                sapInfo.SalesOrderList = cartSubList.ToList();

                                //sapInfo.DocHeader = CreateSAPDocHeaderForXIRMA(sapInfo.DocType, cartGrpInfo, twNewEggData, bankInfo, intShipType);
                                sapInfo.DocHeader = CreateSAPDocHeaderForXIRMA(accDocTypeInfo, cartGrpInfo, twNewEggData, bankInfo, intShipType);

                                foreach (Cart cartInfo in cartSubList)
                                {
                                    //金額計算(LBR)
                                    retgoodInfo = this._finanRepoAdapter.GetRetgoodByCartID(cartInfo.ID);
                                    itemInStockTransList = this._sellerFinanRepoAdapter.GetItemInStockTrans(AccountsDocumentType.DocTypeEnum.XIRMA, retgoodInfo.Code).ToList();
                                    
                                    procList = this._finanRepoAdapter.GetCartProcess(cartInfo.ID).ToList();

                                    priceInfo = new SapPriceInfo(AccountsDocumentType.DocTypeEnum.XIRMA, cartInfo.ShipType ?? 0, itemInStockTransList, procList);

                                    CreateSAPDetailForLBO(accDocTypeInfo, coaProfileList, cartGrpInfo, twNewEggData, cartInfo, bankInfo, sapInfo, priceInfo);
                                }

                                sapList.Add(sapInfo);
                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// 依LBO建立Sap_BapiAccDocument_DocDetail、Seller_FinanDetail
        /// </summary>
        /// <param name="accDocTypeInfo"></param>
        /// <param name="coaProfileList"></param>
        /// <param name="cartGrpInfo"></param>
        /// <param name="twNewEggData"></param>
        /// <param name="cartInfo"></param>
        /// <param name="bankInfo"></param>
        /// <param name="sapInfo"></param>
        /// <param name="priceInfo"></param>
        private void CreateSAPDetailForLBO(AccountsDocumentType accDocTypeInfo, List<ChartOfAccountsProfile> coaProfileList, CartGroupInfo cartGrpInfo, FinanceDataListFinanceData twNewEggData, Cart cartInfo, BankAccountsInfo bankInfo, 
            SapDocumentInfo sapInfo, SapPriceInfo priceInfo)
        {
            try
            {
                int intItemNoAcc = sapInfo.DocDetail.Count();

                //Sap_BapiAccDocument_DocDetail 依設定檔產生資料
                foreach (ChartOfAccountsProfile coaProfile in coaProfileList)
                {
                    intItemNoAcc += 1;

                    switch (sapInfo.DocType)
                    {
                        case AccountsDocumentType.DocTypeEnum.XQ:
                            sapInfo.DocDetail.Add(CreateSAPDocDetailForXQ(sapInfo.DocHeader, coaProfile, cartGrpInfo, cartInfo, bankInfo, intItemNoAcc));
                            break;

                        case AccountsDocumentType.DocTypeEnum.XD:
                            sapInfo.DocDetail.Add(CreateSAPDocDetailForXD(sapInfo.DocHeader, coaProfile, cartGrpInfo, cartInfo, bankInfo, intItemNoAcc));
                            break;

                        case AccountsDocumentType.DocTypeEnum.XI:                            
                            sapInfo.DocDetail.Add(CreateSAPDocDetailForXI(sapInfo.DocHeader, coaProfile, cartGrpInfo, cartInfo, bankInfo, intItemNoAcc, priceInfo));
                            break;

                        case AccountsDocumentType.DocTypeEnum.XIRMA: 
                            sapInfo.DocDetail.Add(CreateSAPDocDetailForXIRMA(sapInfo.DocHeader, coaProfile, cartGrpInfo, cartInfo, bankInfo, intItemNoAcc, priceInfo));
                            break;
                    }
                }

                //Seller_FinanDetail
                switch (accDocTypeInfo.Code)
                {
                    case 3: //XI
                    case 4: //XIRMA
                        switch (cartInfo.ShipType ?? 0)
                        {
                            case 7: //B2C直配
                            case 9: //B2C寄倉
                                sapInfo.SellerFinanDetail.Add(CreateSellerFinanDetail(sapInfo.DocType, cartGrpInfo, cartInfo, priceInfo));
                                break;
                        }
                        break;
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// 依美蛋訂單(台蛋採購單)產生會計文件
        /// </summary>
        /// <param name="sapList"></param>
        /// <param name="poGrpInfo"></param>
        /// <param name="twNewEggData"></param>
        private void CreateSAPData_Newegg(List<SapDocumentInfo> sapList, PurchaseOrderItemGroupInfo poGrpInfo, FinanceDataListFinanceData twNewEggData)
        {
            SapDocumentInfo sapInfo = null;
            List<ChartOfAccountsProfile> coaProfileList = new List<ChartOfAccountsProfile>();

            try
            {
                //取得會計文件設定檔
                coaProfileList = this._accProfileRepoAdapter.GetChartOfAccPorfile(AccountsDocumentType.OverSeaBuyOutUSDCode, 6).ToList();
                if (coaProfileList.Count > 0)
                {
                    sapInfo = new SapDocumentInfo();
                    sapInfo.DocType = AccountsDocumentType.DocTypeEnum.XI; //docType;
                    sapInfo.AccDocTypeCode = AccountsDocumentType.OverSeaBuyOutUSDCode;

                    sapInfo.PurchaseOrderItemList = poGrpInfo.PurchaseOrderItemList.ToList();

                    //sapInfo.DocHeader = CreateSAPDocHeaderForXI(sapInfo.DocType, cartGrpInfo, twNewEggData, bankInfo, intShipType);
                    sapInfo.DocHeader = CreateSAPDocHeaderForXI_Newegg(poGrpInfo, twNewEggData, sapInfo.AccDocTypeCode);

                    int intItemNoAcc = sapInfo.DocDetail.Count();

                    //Sap_BapiAccDocument_DocDetail 依設定檔產生資料
                    foreach (ChartOfAccountsProfile coaProfile in coaProfileList)
                    {
                        intItemNoAcc += 1;
                        sapInfo.DocDetail.Add(CreateSAPDocDetailForXI_Newegg(sapInfo.DocHeader, coaProfile, poGrpInfo, intItemNoAcc));
                    }

                    sapList.Add(sapInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private string GetFinanDocGLAccount(ChartOfAccountsProfile coaProfile, BankAccountsInfo bankInfo)
        {
            string glAccount = "";
            switch (coaProfile.AccPattern.Trim())
            {
                case "S":
                    if (bankInfo.Accounts == null)
                        throw new Exception("查無銀行的會計科目");
                    glAccount = bankInfo.Accounts.AccNumber.PadLeft(10, '0');
                    break;
                case "A":
                    glAccount = coaProfile.AccNumber.PadLeft(10, '0');
                    break;
                default: //C、V、L
                    glAccount = "";
                    break;
            }
            return glAccount;
        }

        /// <summary>
        /// XI(LBO)與XIRMA(LBR)建立對帳單資料
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="cartGrpInfo"></param>
        /// <param name="cartInfo"></param>
        /// <param name="priceInfo"></param>
        /// <returns></returns>
        private Seller_FinanDetail CreateSellerFinanDetail(AccountsDocumentType.DocTypeEnum docType, CartGroupInfo cartGrpInfo, Cart cartInfo, SapPriceInfo priceInfo)
        {
            Seller_FinanDetail finanDetail = new Seller_FinanDetail();
            Seller_FinanDetail.SettleType_Identify settleType = Seller_FinanDetail.SettleType_Identify.訂單;
            Retgood retgInfo = null;
            decimal dSumPrice = 0;

            try
            {
                int intShipType = cartInfo.ShipType.Value;
                List<Process> procList = this._finanRepoAdapter.GetCartProcess(cartInfo.ID).ToList();

                //取得第一筆Process
                Process procFirstInfo = procList.OrderBy(x => x.ID).FirstOrDefault();
                PurchaseOrderitemTWBACK poItemInfo = this._finanRepoAdapter.GetOrderPOItem(cartInfo.ID);

                finanDetail.IsCheck = "N";
              
                switch (docType)
                {
                    case AccountsDocumentType.DocTypeEnum.XI:
                        finanDetail.OrderID = cartInfo.ID;
                        //發票日期
                        finanDetail.TrackDate = cartGrpInfo.DocDate; //InDate                        

                        switch (cartInfo.ShipType)
                        {
                            case 8:
                            case 9:
                                settleType = Seller_FinanDetail.SettleType_Identify.寄倉;
                                break;
                            default:
                                settleType = Seller_FinanDetail.SettleType_Identify.訂單;
                                break;
                        }
                        break;

                    case AccountsDocumentType.DocTypeEnum.XIRMA:
                        //退貨
                        retgInfo = this._finanRepoAdapter.GetRetgoodByCartID(cartInfo.ID);
                        if (retgInfo == null)
                            throw new Exception("Retgood查無資料。");

                        finanDetail.OrderID = retgInfo.Code;
                        finanDetail.RMADate = retgInfo.FinReturnDate;

                        settleType = Seller_FinanDetail.SettleType_Identify.退貨;

                        //發票日期
                        InvoiceList invoicInfo = this._finanRepoAdapter.GetCartInvoice(cartInfo.ID);
                        if (invoicInfo == null)
                            throw new Exception("InvoiceList查無資料。");
                        finanDetail.TrackDate = invoicInfo.InDate;                        
                        break;

                    default:
                        throw new Exception(string.Format("Seller_FinanDetail不處理{0}的會計文件。", docType.ToString()));
                }

                finanDetail.SettleType = (int)settleType;

                finanDetail.OrderDetailID = procFirstInfo.ID;
                finanDetail.CartDate = cartInfo.CreateDate;

                if (poItemInfo == null)
                {
                    finanDetail.ProductID = procFirstInfo.ProductID.GetValueOrDefault();
                }
                else
                {
                    finanDetail.POID = poItemInfo.PurchaseorderCode;
                    finanDetail.ProductID = poItemInfo.ProductID;
                    finanDetail.SellerProductID = poItemInfo.SellerProductID;
                }

                finanDetail.ProductName = procFirstInfo.Title;

                if (intShipType == 3)
                    //三角
                    finanDetail.BaseCurrency = "USD";
                else
                    finanDetail.BaseCurrency = "TWD";
                
                IQueryable<ItemInStock_trans> itemInStockTransList = this._sellerFinanRepoAdapter.GetItemInStockTrans(docType, finanDetail.OrderID);

                //XI、XIRMA有發票，ItemInStock_trans一定會有資料
                if (itemInStockTransList.Count() > 0)
                    finanDetail.Qty = itemInStockTransList.Sum(x => x.Qty);
                else
                    throw new Exception("Seller_FinanDetail.Qty錯誤(ItemInStock_trans查無資料)。");

                //Seller
                Seller sellerInfo;
                switch (intShipType)
                {
                    //case 31:
                    case 3:
                        sellerInfo = this._sellerFinanRepoAdapter.GetSellerVendorForPurchaseOrder(poItemInfo.SellerID ?? 2);
                        break;
                    default:
                        sellerInfo = this._sellerFinanRepoAdapter.GetSellerVendor(procFirstInfo.StoreID.GetValueOrDefault());
                        break;
                }

                if (sellerInfo == null)
                    finanDetail.SellerID = 0;
                else
                    finanDetail.SellerID = sellerInfo.ID;

                switch (docType)
                {
                    case AccountsDocumentType.DocTypeEnum.XI:
                        switch (intShipType)
                        {
                            //case 0: //切貨
                            //    dSumPrice = priceInfo.PreAVGCost + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            //    break;
                            //case 1: //間配
                            //    dSumPrice = priceInfo.PreAVGCost + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            //    break;
                            ////case 2: //直配
                            ////    dSumPrice = priceInfo.amount0;
                            ////    break;
                            //case 3: //三角-美金
                            //    dSumPrice = poItemInfo.SourcePrice * poItemInfo.Qty;
                            //    break;
                            ////case 4: //借賣網
                            ////    dSumPrice = priceInfo.PreAVGCost + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            ////    break;
                            //case 6: //海外切貨-台蛋報關
                            //    dSumPrice = priceInfo.PreAVGCost;
                            //    break;
                            case 7: //B2C直配
                                dSumPrice = priceInfo.PreAVGCost;
                                break;
                            ////case 8: //MKPL寄倉
                            ////    dSumPrice = priceInfo.amount0;
                            ////    break;
                            case 9: //B2C寄倉
                                dSumPrice = priceInfo.PreAVGCost;
                                break;
                        }
                        break;
                    case AccountsDocumentType.DocTypeEnum.XIRMA:
                        switch (intShipType)
                        {
                            //case 0: //切貨
                            //    dSumPrice = priceInfo.PreAVGCost + priceInfo.PreRetgoodPrice + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            //    break;
                            //case 1: //間配
                            //    dSumPrice = priceInfo.PreAVGCost + priceInfo.PreRetgoodPrice + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            //    break;
                            ////case 2: //直配
                            ////    dSumPrice = (priceInfo.amount0 + priceInfo.amount2tax + priceInfo.InstallmentFee) - priceInfo.Process_Pricecoupon;
                            ////    break;
                            //case 3: //三角

                            //    break;
                            ////case 4: //借賣網
                            ////    dSumPrice = priceInfo.PreAVGCost + priceInfo.PreRetgoodPrice + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            ////    break;
                            //case 6: //海外切貨-台蛋報關
                            //    dSumPrice = priceInfo.PreAVGCost + priceInfo.PreRetgoodPrice + priceInfo.PreAVGCostShipandTax + priceInfo.PreAVGCostTaxandDuty + priceInfo.PreAVGCostCustoms_Charge;
                            //    break;
                            case 7: //B2C直配
                                dSumPrice = priceInfo.PreAVGCost;
                                break;
                            ////case 8: //MKPL寄倉
                            ////    dSumPrice = priceInfo.amount0;
                            ////    break;
                            case 9: //B2C寄倉
                                //if (priceInfo.PreAVGCost == 0)
                                //    dSumPrice = priceInfo.PreRetgoodPrice;
                                //else
                                //    dSumPrice = priceInfo.PreAVGCost;
                                dSumPrice = priceInfo.PreAVGCost;
                                break;
                        }
                        break;
                }
               
                //待確認，國內小農廠商，其商品不含稅，現行系統都需要課稅

                finanDetail.SumPrice = dSumPrice;
                //PO單含稅價
                finanDetail.SumTax = (poItemInfo.LocalPrice * poItemInfo.Qty) - dSumPrice;

                if (finanDetail.SumPrice > 0)
                {
                    finanDetail.UnitPrice = Math.Round(poItemInfo.LocalPrice / (decimal)1.05, 0, MidpointRounding.AwayFromZero);
                    finanDetail.UnitTax = poItemInfo.LocalPrice - finanDetail.UnitPrice;
                }
                else
                {
                    finanDetail.UnitPrice = 0;
                    finanDetail.UnitTax = 0;
                }

                finanDetail.InDate = DateTime.UtcNow.AddHours(8);
                finanDetail.InUserID = "Finance";    

                return finanDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }    
    }
}
