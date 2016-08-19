using AutoMapper;
using HppApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TWNewEgg.CommonService.DomainModels;
using TWNewEgg.CommonService.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.NCCCRepoAdapters.Interface;
using TWNewEgg.PaymentGateway.Interface;

namespace TWNewEgg.PaymentGateway.Service.NCCC
{
    public class NCCCPaymentService: INCCC
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private INotificationService _notificationService;
        private INCCCRepoAdapter _ncccRepoAdapter;
        public NCCCPaymentService(INCCCRepoAdapter ncccRepoAdapter, INotificationService notificationService)
        {
            this._notificationService = notificationService;
            this._ncccRepoAdapter = ncccRepoAdapter;
            Mapper.CreateMap<ApiClient, NCCCTrans>();
            Mapper.CreateMap<NCCCTrans, NCCCResult>();
        }

        public string Pay(NCCCInput inputData)
        {
            var inputJson = new JavaScriptSerializer().Serialize(inputData);
            ApiClient apiClient = new ApiClient();
            apiClient.setMERCHANTID(System.Configuration.ConfigurationManager.AppSettings["NCCC_MERCHANTID"]);
            apiClient.setTERMINALID(System.Configuration.ConfigurationManager.AppSettings["NCCC_TERMINALID"]);
            apiClient.setORDERID(inputData.OrderID);
            apiClient.setTRANSAMT(inputData.TransAmt);
            apiClient.setTRANSMODE(inputData.TransMode);
            apiClient.setINSTALLMENT(inputData.Installment);
            apiClient.setBankNo(inputData.BankNo);
            apiClient.setNotifyURL(System.Configuration.ConfigurationManager.AppSettings["NCCC_NotifyURL"]);
            apiClient.setURL(System.Configuration.ConfigurationManager.AppSettings["NCCC_domainNAME"], System.Configuration.ConfigurationManager.AppSettings["NCCC_requestURL"]);

            NCCCTrans trans = ModelConverter.ConvertTo<NCCCTrans>(apiClient);
            trans = this._ncccRepoAdapter.Create(trans);
            NCCCPostTransaction(inputData, apiClient);
            
            string key = "";
            trans.ResponseCode = apiClient.getRESPONSECODE();

            trans = this._ncccRepoAdapter.Update(trans);
            if ("00".Equals(trans.ResponseCode))
            { // 作業執行成功
                key = apiClient.getKEY(); //交易金鑰
            }
            else
            {
                logger.Error("NCCC授權失敗，NCCCInput: " + inputJson + " RtnCode:" + trans.ResponseCode + ", RtnMsg:" + trans.ResponseMsg);
            }
            
            return key;
        }

        private void NCCCPostTransaction(NCCCInput inputData, ApiClient apiClient)
        {
            var inputJson = new JavaScriptSerializer().Serialize(inputData);
            try
            {
                int rtnCode = apiClient.postTransaction();
                if (rtnCode <= 0)
                {
                    throw new Exception("與NCCC連線取得交易金鑰失敗，RtnCode: " + rtnCode);
                }
            }
            catch (Exception e)
            {
                DateTime now = DateTime.Now;
                this._notificationService.Set(new NotificationModel
                {
                    PresetId = "PaymentGatewayError",
                    MailContent = "NCCC(Pay method) connection problem! OrderNO: " + inputData.OrderID + ", \nErrMsg:  " + e.ToString(),
                    Title = string.Format("台灣新蛋網金流元件發生異常（NCCC）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now),
                    PhoneContent = string.Format("台灣新蛋網金流元件發生異常（NCCC）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now)
                });
                this._notificationService.NotifyByMailAndSMS();

                logger.Error("NCCCInput: " + inputJson + "與NCCC連線取得交易金鑰失敗");
                throw;
            }
        }

        public NCCCResult CheckPayResultByOrderId(string orderGroupId)
        {
            ApiClient apiClient = new ApiClient();
            apiClient.setMERCHANTID("6600800020");
            apiClient.setORDERID(orderGroupId);
            apiClient.setURL(System.Configuration.ConfigurationManager.AppSettings["NCCC_domainNAME"], System.Configuration.ConfigurationManager.AppSettings["NCCC_requestURL"]);
            int rtnCode = apiClient.postQuery();
            if (rtnCode <= 0)
            {
                DateTime now = DateTime.Now;
                this._notificationService.Set(new NotificationModel
                {
                    PresetId = "PaymentGatewayError",
                    MailContent = "NCCC(Check method) connection problem! OrderNO: " + orderGroupId + ", Return Code: " + string.Join(",", rtnCode),
                    Title = string.Format("台灣新蛋網金流元件發生異常（NCCC）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now),
                    PhoneContent = string.Format("台灣新蛋網金流元件發生異常（NCCC）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now)
                });
                this._notificationService.NotifyByMailAndSMS();
                logger.Info("NCCC 查詢錯誤: code:" + apiClient.getRESPONSECODE() + ",MSG: " + apiClient.getRESPONSEMSG());
                throw new Exception("與NCCC的連線查詢失敗");
            }

            NCCCTrans trans = ModelConverter.ConvertTo<NCCCTrans>(apiClient);
            trans = this._ncccRepoAdapter.Update(trans);
            NCCCResult result = ModelConverter.ConvertTo<NCCCResult>(trans);
            return result;
        }

        public NCCCResult CheckPayResultByKey(string key)
        {
            ApiClient apiClient = new ApiClient();
            apiClient.setKEY(key);
            apiClient.setURL(System.Configuration.ConfigurationManager.AppSettings["NCCC_domainNAME"], System.Configuration.ConfigurationManager.AppSettings["NCCC_requestURL"]);
            int rtnCode = apiClient.postQuery();
            if (rtnCode <= 0)
            {
                logger.Info("NCCC 查詢錯誤: code:" + apiClient.getRESPONSECODE() + ",MSG: " + apiClient.getRESPONSEMSG());
                throw new Exception("與NCCC的連線查詢失敗");
            }

            NCCCTrans trans = ModelConverter.ConvertTo<NCCCTrans>(apiClient);
            trans = this._ncccRepoAdapter.Update(trans);
            NCCCResult result = ModelConverter.ConvertTo<NCCCResult>(trans);
            return result;
        }
    }
    public class NCCCNotifyInfo
    {
        public string TransType { get; set; }
        public string RespCode { get; set; }
        public string RespMsg { get; set; }
        public string MerAbbr { get; set; }
        public string MerId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderAmount { get; set; }
        public string RespTime { get; set; }
        public string CupReserved { get; set; }
    }
}
