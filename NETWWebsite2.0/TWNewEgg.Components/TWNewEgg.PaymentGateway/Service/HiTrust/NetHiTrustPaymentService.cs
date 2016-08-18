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
using com.hitrust.b2ctoolkit.b2cpay;
using TWNewEgg.CommonService.DomainModels;
using TWNewEgg.CommonService.Interface;
using TWNewEgg.PaymentGateway.Models;
using System.Xml.Serialization;
using System.Xml;

namespace TWNewEgg.PaymentGateway.Service.HiTrust
{
    public class NetHiTrustPaymentService : IHiTrust
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private INotificationService _notificationService;
        private IHiTrustRepoAdapter HiTranService;
        public NetHiTrustPaymentService(IHiTrustRepoAdapter HiRepoAdapter, INotificationService notificationService)
        {
            this.HiTranService = HiRepoAdapter;
            this._notificationService = notificationService;
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustTrans, TWNewEgg.Models.DomainModels.PaymentGateway.HiTrustAuth>().ReverseMap();
            Mapper.CreateMap<TWNewEgg.Models.DBModels.TWSQLDB.HiTrustQuery, TWNewEgg.Models.DomainModels.PaymentGateway.HiTrustQueryData>().ReverseMap();
        }

        //Function：HiTrustSetting-Read HiTrust setting table
        private HiTrustSetting HiTrustSettingData(string bankID, DateTime Date, int isonce)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MerConfigs));
            string path = AppDomain.CurrentDomain.BaseDirectory + "Configurations\\PaymentGateway\\hitrust.xml";
            MerConfigs configs;
            using (XmlReader reader = XmlReader.Create(path))
            {
                configs = (MerConfigs)ser.Deserialize(reader);
            }

            MerchantConfig merConfig = configs.Merchants.Where(x => x.BankID == bankID && x.IsOnce == isonce).First();

            HiTrustSetting HiInfo = new HiTrustSetting();
            
            string[] GetStoreID = merConfig.Name.ToLower().Split('.', 'c', 'o', 'n', 'f');
            HiInfo.StoreID = GetStoreID[0];

            HiInfo.MerConfigName = merConfig.MerConfigURI + merConfig.Name;
            HiInfo.SerConfigName = merConfig.HiServerURI;
            HiInfo.updateURL = merConfig.UpdateURL;
            HiInfo.returnURL = merConfig.ReturnURL;
            HiInfo.queryflag = merConfig.QueryFlag;
            HiInfo.merupdateURL = merConfig.MerUpdateURL;

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

            AuthData.StoreID = HiData.StoreID;
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
            try
            {
                /*Request 請求參數*/
                B2CPayAuth auth = new B2CPayAuth()
                {
                    OrderNo = AuthData.ordernumber.ToString(), //訂單編號
                    Amount = AuthData.amount.ToString(), //金額
                    Currency = AuthData.currency, //幣別
                    OrderDesc = AuthData.orderdesc, //訂單描述
                    TicketNo = AuthData.ticketno, //機票
                    ReturnURL = AuthData.returnURL, //return連結
                    MerUpdateURL = AuthData.merupdateURL, //update連結(交易結果網址)
                    UpdateURL = AuthData.updateURL, //加密結果回傳網址
                    StoreId = AuthData.StoreID, // 商家config檔名稱 (xxxxx 舊的含.config，新的不含)
                    //SerConfigName = HiData.SerConfigName, //Server conf路徑(Net沒有)
                    DepositFlag = AuthData.depositflag, //自動請款 depositflag
                    QueryFlag = AuthData.queryflag, //啟動查詢(回傳詳細交易結果)
                    Pan = AuthData.pan, //卡號
                    Expiry = AuthData.expiry, //效期
                    E01 = AuthData.E01, //末三碼
                    E03 = AuthData.E03,
                    E04 = AuthData.E04,
                    E05 = AuthData.E05
                };

                /*Process 處理(呼叫授權函式)*/
                auth.transaction();

                /*Response 回應參數*/
                AuthData.token = auth.Token; //取得付款頁面token
                AuthData.retcode = auth.RetCode; //取得retcode

                if (auth.RetCode != "00")
                {
                    throw new Exception("與HiTrust連線取得Token失敗, RetCode: " + auth.RetCode);
                }
            }
            catch (Exception ex)
            {
                DateTime now = DateTime.Now;
                this._notificationService.Set(new NotificationModel
                {
                    PresetId = "PaymentGatewayError",
                    MailContent = "NetHiTrust(Pay method) connection problem! \nOrderNO: " + AuthData.ordernumber + ", \nErrMsg: " + ex.ToString(),
                    Title = string.Format("台灣新蛋網金流元件發生異常（NetHiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now),
                    PhoneContent = string.Format("台灣新蛋網金流元件發生異常（NetHiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now)
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

            QueryData.MerConfigName = QueryInput.MerConfigName;
            QueryData.ordernumber = QueryInput.ordernumber;
            QueryData.SerConfigName = QueryInput.SerConfigName;
            QueryData.retcode = QueryInput.retcode;

            try
            {
                string[] GetStoreID = QueryInput.MerConfigName.ToLower().Split('\\');

                string strGetStoreID = GetStoreID[GetStoreID.Length - 1].Substring(0, 5);

                /*Request 請求參數*/
                B2CPayOther trx = new B2CPayOther()
                {
                    OrderNo = QueryInput.ordernumber, //訂單編號
                    StoreId = strGetStoreID, // 商家config檔名稱 (xxxxx 舊的含.config，新的不含)
                    Type = B2CPay.QUERY
                };

                /*Process 處理(呼叫授權函式)*/
                trx.transaction();

                /*Response 回應參數*/
                QueryData.authCode = trx.AuthCode; //銀行授權碼
                QueryData.authRRN = trx.AuthRRN; //銀行調單編號
                QueryData.orderstatus = trx.OrderStatus; //訂單狀態碼
                QueryData.approveamount = trx.ApproveAmount; //核准金額
                QueryData.depositamount = trx.CaptureAmount; //請款金額
                QueryData.credamount = trx.RefundAmount; //退款金額
                QueryData.orderdate = trx.OrderDate; //訂單日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                QueryData.capDate = trx.CaptureDate; //請款日期(YYYY-MM-DD.HH.MM.SS.XXXXXX)
                QueryData.currency = trx.Currency; //幣別
                QueryData.eci = trx.Eci; //授權方式(SSL, MIA, SET)
                QueryData.E06 = trx.E06; //分期期數
                QueryData.E07 = trx.E07; //首期金額
                QueryData.E08 = trx.E08; //每期金額
                QueryData.E09 = trx.E09; //手續費
                QueryData.redemordernum = trx.Redemordernum; //點點變現金銷帳編號
                QueryData.redem_discount_point = trx.Redem_discount_point; //本次折抵點數
                QueryData.redem_discount_amount = trx.Redem_discount_amount; //本次折抵金額
                QueryData.redem_purchase_amount = trx.Redem_purchase_amount; //本次實付金額
                QueryData.redem_balance_point = trx.Redem_balance_point; //剩餘點數
                QueryData.acquirer = trx.Acquirer;
                QueryData.cardtype = trx.CardType;
            }
            catch (Exception ex)
            {
                DateTime now = DateTime.Now;
                this._notificationService.Set(new NotificationModel
                {
                    PresetId = "PaymentGatewayError",
                    MailContent = "NetHiTrust(Query method) connection problem! OrderNO: " + QueryInput.ordernumber + ", ErrMsg: " + string.Join(",", ex.ToString()),
                    Title = string.Format("台灣新蛋網金流元件發生異常（NetHiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now),
                    PhoneContent = string.Format("台灣新蛋網金流元件發生異常（NetHiTrust）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now)
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
            logger.Info("NetHiTrustPaymentService Start: Pay, ordernumber: " + inputData.ordernumber);
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

            logger.Info("NetHiTrustPaymentService URL: " + AuthData.token);
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
