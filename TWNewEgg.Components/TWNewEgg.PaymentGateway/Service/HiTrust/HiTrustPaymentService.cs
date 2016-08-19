using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.HiTrustRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.PaymentGateway.Interface;
using AutoMapper;
using TWNewEgg.CommonService.Interface;
using TWNewEgg.CommonService.DomainModels;

namespace TWNewEgg.PaymentGateway.Service
{
    public class HiTrustPaymentService : IHiTrust
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private INotificationService _notificationService;
        private IHiTrustRepoAdapter HiTranService;
        public HiTrustPaymentService(IHiTrustRepoAdapter HiRepoAdapter, INotificationService notificationService)
        {
            this.HiTranService = HiRepoAdapter;
            this._notificationService = notificationService;
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustTrans, TWNewEgg.Models.DomainModels.PaymentGateway.HiTrustAuth>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustQuery, TWNewEgg.Models.DomainModels.PaymentGateway.HiTrustQueryData>().ReverseMap();
        }

        //Function：HiTrustSetting-Read HiTrust setting table
        private HiTrustSetting HiTrustSettingData(string bankID, DateTime Date, int isonce)
        {
            
            HiTrustSetting HiInfo = new HiTrustSetting();
            TWNewEgg.Models.DBModels.TWSQLDB.HiTrust Info = new TWNewEgg.Models.DBModels.TWSQLDB.HiTrust();

            var varInfo = HiTranService.GetAllHiTrustSetting().Where(x => x.BnkID == bankID && x.IsOnce == isonce).AsQueryable();

            Info = varInfo.Where(x => DateTime.Compare(x.DateStart, Date) <= 0 && DateTime.Compare(x.DateEnd, Date) >= 0).SingleOrDefault();
            HiInfo.MerConfigName = Info.MerConfig + Info.MerConfigName;
            HiInfo.SerConfigName = Info.HiServer;
            HiInfo.updateURL = Info.UpdateUrl;
            HiInfo.returnURL = Info.returnURL;
            HiInfo.queryflag = Info.QueryFlag;
            HiInfo.merupdateURL = Info.merupdateURL;

            return HiInfo;
        }

        /// <summary>
        /// Function：HiTrustTransSave-Write HiTrust transation table
        /// </summary>
        /// <param name="AData"></param>
        private void HiTrustTransSave(HiTrustAuth AData)
        {
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {
                var trans = ModelConverter.ConvertTo<HiTrustTrans>(AData);
                HiTranService.UpdateHiTrustTrans(trans);
                var log = ModelConverter.ConvertTo<HiTrustTransLog>(AData);
                HiTranService.UpdateHiTrustTransLog(log);
                ts.Complete();
            }
        }

        /// <summary>
        /// Function：HiTrustQuerySave-Write HiTrust Query log table
        /// </summary>
        /// <param name="QData"></param>
        private void HiTrustQuerySave(HiTrustQueryData QData)
        {
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {
                var query = ModelConverter.ConvertTo<HiTrustQuery>(QData);
                HiTranService.UpdateHiTrustQuery(query);
                var log = ModelConverter.ConvertTo<HiTrustQueryLog>(QData);
                HiTranService.UpdateHiTrustQueryLog(log);
                ts.Complete();
            }
        }

        //Function：HiTrustAuthData-依 function type 組 HiTrust data
        ////0. call HiTrustSetting
        ////1. Auth data
        private HiTrustAuth HiTrustAuthData(HiTrustInput inputData, HiTrustSetting HiData)
        {
            HiTrustAuth AuthData = new HiTrustAuth();
            AuthData.MerConfigName = HiData.MerConfigName;
            AuthData.ordernumber = inputData.ordernumber.ToString();
            AuthData.amount = decimal.ToInt32(inputData.amount * 100).ToString();
            AuthData.currency = inputData.currency;
            AuthData.orderdesc = inputData.orderdesc;
            AuthData.depositflag = ((int)HiTrustAuth.Depositflag.自動請款).ToString();
            AuthData.queryflag = HiData.queryflag;
            AuthData.returnURL = HiData.returnURL;
            AuthData.merupdateURL = HiData.merupdateURL;
            AuthData.updateURL = HiData.updateURL;
            AuthData.pan = inputData.cardNumber;
            AuthData.E01 = inputData.CVC2;
            AuthData.expiry = inputData.expiry;
            AuthData.E03 = inputData.HpType.ToString().PadLeft(2, '0');

            if (inputData.HpType > 1)
                AuthData.E04 = inputData.IsRedMoney.ToString();
            else
                AuthData.E04 = ((int)HiTrustInput.isredMoney.不啟用).ToString();
            
            AuthData.E05 = inputData.PayPage.ToString();
            AuthData.SerConfigName = HiData.SerConfigName;
            
            return AuthData;
        }
        //Function：HiTrustQueryData-依 function type 組 HiTrust data
        ////0. 讀交易查詢檔
        ////1. Query data
        public HiTrustQueryInput HiTrustQueryInData(string OrderNumber)
        {
            //用OrderNumber查HiTrustTrans，再查HiTrust
            HiTrustQueryInput QueryInput = new HiTrustQueryInput();
            var HiTran = HiTranService.GetAllHiTrustTrans().Where(x => x.ordernumber == OrderNumber).SingleOrDefault();
            QueryInput.ordernumber = OrderNumber.ToString();
            QueryInput.MerConfigName = HiTran.MerConfigName;
            QueryInput.retcode = HiTran.retcode;
            QueryInput.SerConfigName = HiTran.SerConfigName;

            return QueryInput;
        }

        private bool HiTrustPay(HiTrustAuth AuthData, HiTrustSetting HiData)
        {
            HiTrustTransLog LogInfo = new HiTrustTransLog();
            System.Type oType = System.Type.GetTypeFromProgID("HiB2CCom.eB2CCom.1");
            try
            {
                object tmp = System.Activator.CreateInstance(oType);
                //設定訂單資訊
                oType.InvokeMember("ordernumber", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.ordernumber });//訂單編號
                oType.InvokeMember("amount", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.amount });//金額
                oType.InvokeMember("currency", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.currency });//幣別
                oType.InvokeMember("orderdesc", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.orderdesc });//訂單描述
                oType.InvokeMember("ticketno", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.ticketno });//機票
                //oType.InvokeMember("returnURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Request.Url.Segments[1].Replace("/", "") + "/B2C_Return.aspx" });//return連結
                //oType.InvokeMember("merupdateURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Request.Url.Segments[1].Replace("/", "") + "/B2C_Update.aspx" });//update連結
                oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.MerConfigName }); //商家config檔名稱
                oType.InvokeMember("SerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { HiData.SerConfigName });//Server conf路徑
                //自動請款 depositflag
                oType.InvokeMember("depositflag", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.depositflag });
                //啟動查詢(回傳詳細交易結果)
                oType.InvokeMember("queryflag", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.queryflag });
                //oType.InvokeMember("returnURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.returnURL });
                //oType.InvokeMember("merupdateURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.merupdateURL });
                //oType.InvokeMember("updateURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.updateURL });
                //卡號及有效日期
                oType.InvokeMember("pan", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.pan });//卡號
                oType.InvokeMember("expiry", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.expiry });//效期
                oType.InvokeMember("E01", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.E01 });//末三碼
                oType.InvokeMember("E03", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.E03 });
                //紅利折抵: 不可與分期使用
                oType.InvokeMember("E04", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.E04 });
                oType.InvokeMember("E05", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { AuthData.E05 });
                //呼叫B2CAuth並取得token
                oType.InvokeMember("B2CAuth", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);//呼叫授權函式

                AuthData.token = (string)oType.InvokeMember("Token", System.Reflection.BindingFlags.GetProperty, null, tmp, null);//取得付款頁面token
                AuthData.retcode = (string)oType.InvokeMember("retcode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);//取得retcode

                if (AuthData.retcode != "00")
                {
                    throw new Exception("與HiTrust連線取得Token失敗, RetCode: " + AuthData.retcode);
                }
            }
            catch(Exception ex)
            {
                DateTime now = DateTime.Now;
                this._notificationService.Set(new NotificationModel
                {
                    PresetId = "PaymentGatewayError",
                    MailContent = "HiTrust(Pay method) connection problem! \nOrderNO: " + AuthData.ordernumber + ", \nErrMsg: " + string.Join(",", ex.ToString()),
                    Title = string.Format("台灣新蛋網金流元件發生異常（HiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now),
                    PhoneContent = string.Format("台灣新蛋網金流元件發生異常（HiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now)
                });
                this._notificationService.NotifyByMailAndSMS();

                LogInfo = ModelConverter.ConvertTo<HiTrustTransLog>(AuthData);
                LogInfo.SerConfigName = HiData.SerConfigName;
                LogInfo.except = (ex.InnerException != null) ? "InnerEx:" + ex.InnerException.Message : "例外發生: " + ex.Message; ;
                HiTranService.UpdateHiTrustTransLog(LogInfo);
                return false;
            }
            if (AuthData.retcode.Equals("00"))
                return true;
            else
                return false;
            
        }

        private HiTrustQueryData HiTrustQuery(HiTrustQueryInput QueryInput)
        {
            HiTrustQueryData QueryData = new HiTrustQueryData();
            HiTrustQueryLog LogInfo = new HiTrustQueryLog();

            try
            {
                System.Type oType = System.Type.GetTypeFromProgID("HiB2CCom.eB2CCom.1");
                object tmp = System.Activator.CreateInstance(oType);

                oType.InvokeMember("ordernumber", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { QueryInput.ordernumber });//訂單編號
                oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { QueryInput.MerConfigName }); //商家config檔路徑
                oType.InvokeMember("SerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { QueryInput.SerConfigName });//Server conf路徑

                oType.InvokeMember("B2CQuery", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);//呼叫查詢函式

                QueryData.MerConfigName = QueryInput.MerConfigName;
                QueryData.ordernumber = QueryInput.ordernumber;
                QueryData.SerConfigName = QueryInput.SerConfigName;
                QueryData.retcode = QueryInput.retcode;

                //銀行授權碼
                QueryData.authCode = (string)oType.InvokeMember("authCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //銀行調單編號
                QueryData.authRRN = (string)oType.InvokeMember("authRRN", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //訂單狀態碼
                QueryData.orderstatus = (string)oType.InvokeMember("orderstatus", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //核准金額
                QueryData.approveamount = (string)oType.InvokeMember("approveamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //請款金額
                QueryData.depositamount = (string)oType.InvokeMember("depositamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //退款金額
                QueryData.credamount = (string)oType.InvokeMember("credamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //訂單日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                QueryData.orderdate = (string)oType.InvokeMember("orderdate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //請款日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                QueryData.capDate = (string)oType.InvokeMember("capDate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //幣別
                QueryData.currency = (string)oType.InvokeMember("currency", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //授權方式(SSL, MIA, SET)
                QueryData.eci = (string)oType.InvokeMember("eci", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //分期期數
                QueryData.E06 = (string)oType.InvokeMember("e06", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //首期金額
                QueryData.E07 = (string)oType.InvokeMember("e07", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //每期金額
                QueryData.E08 = (string)oType.InvokeMember("e08", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //手續費
                QueryData.E09 = (string)oType.InvokeMember("e09", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //點點變現金銷帳編號
                QueryData.redemordernum = (string)oType.InvokeMember("redemordernum", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //本次折抵點數
                QueryData.redem_discount_point = (string)oType.InvokeMember("redem_discount_point", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //本次折抵金額
                QueryData.redem_discount_amount = (string)oType.InvokeMember("redem_discount_amount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //本次實付金額
                QueryData.redem_purchase_amount = (string)oType.InvokeMember("redem_purchase_amount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                //剩餘點數
                QueryData.redem_balance_point = (string)oType.InvokeMember("redem_balance_point", System.Reflection.BindingFlags.GetProperty, null, tmp, null);

                QueryData.acquirer = (string)oType.InvokeMember("acquirer", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                QueryData.cardtype = (string)oType.InvokeMember("cardtype", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            }
            catch (Exception ex)
            {
                DateTime now = DateTime.Now;
                this._notificationService.Set(new NotificationModel
                {   
                    PresetId = "PaymentGatewayError",
                    MailContent = "HiTrust(Query method) connection problem! OrderNO: " + QueryInput.ordernumber + ", ErrMsg: " + string.Join(",", ex.ToString()),
                    Title = string.Format("台灣新蛋網金流元件發生異常（HiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now),
                    PhoneContent = string.Format("台灣新蛋網金流元件發生異常（HiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now)
                });
                this._notificationService.NotifyByMailAndSMS();

                LogInfo = ModelConverter.ConvertTo<HiTrustQueryLog>(QueryData);
                LogInfo.except = (ex.InnerException != null) ? "InnerEx:" + ex.InnerException.Message : "例外發生: " + ex.Message; ;
                HiTranService.UpdateHiTrustQueryLog(LogInfo);
            }
            return QueryData;
        }

        //Function：Pay<T>
        ////1. call HiTrustData
        ////2. call HiTrustPay
        ////3. call HiTrustTransSave
        ////4. call HiTrustQuery
        ////5. call HiTrustQuerySave
        public string Pay(HiTrustInput inputData)
        {
            logger.Info("HiTrustPaymentService Start: Pay, ordernumber: " + inputData.ordernumber);
            HiTrustSetting HiInfo = new HiTrustSetting();
            HiTrustAuth AuthData = new HiTrustAuth();

            if (inputData.HpType > 0)
                HiInfo = HiTrustSettingData(inputData.BankID, inputData.orderDate, (int)HiTrustSetting.isOnce.分期);
            else
                HiInfo = HiTrustSettingData(inputData.BankID, inputData.orderDate, (int)HiTrustSetting.isOnce.一次);

            if (HiInfo != null)
            {
                AuthData = HiTrustAuthData(inputData, HiInfo);
                bool IsSuccess = HiTrustPay(AuthData, HiInfo);
                inputData.token = AuthData.token;
                //3. call HiTrustTransSave
                HiTrustTransSave(AuthData);
            }

            logger.Info("HiTrustPaymentService URL: "+AuthData.token);
            return AuthData.token;
        }

        //Function：IsPayed(string Id)
        ////1. call HiTrustData
        ////2. call HiTrustQueryLog table
        ////4. return true/false
        public HiTrustQueryData CheckPayResult(int Id)
        {
            HiTrustQueryInput QueryInput = new HiTrustQueryInput();
            HiTrustQueryData QueryData = new HiTrustQueryData();

            string strID = Id.ToString();
            var Info = HiTranService.GetAllHiTrustTrans().Where(x => x.ordernumber == strID).SingleOrDefault();

            if (Info != null)
            {
                QueryInput.MerConfigName = Info.MerConfigName;
                QueryInput.SerConfigName = Info.SerConfigName;
                QueryInput.ordernumber = Info.ordernumber;
                QueryInput.retcode = Info.retcode;
                //4. call HiTrustQuery 查詢HiTrustQueryData
                QueryData = HiTrustQuery(QueryInput);

                if (QueryData != null)
                {
                    //5. call HiTrustQuerySave
                    HiTrustQuerySave(QueryData);
                    return QueryData;
                }
                else
                    throw new Exception("HiTrustQuery returns NULL");
            }
            else
                throw new Exception("HiTrustTrans with ordernumber: " + strID + " can't be found");
        }
    }
}
