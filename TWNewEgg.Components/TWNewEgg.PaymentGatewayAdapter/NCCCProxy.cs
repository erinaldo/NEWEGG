using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.PaymentGatewayAdapter.Interface;

namespace TWNewEgg.PaymentGatewayAdapter
{
    public class NCCCProxy : INCCCProxy
    {
        private ICartMachine _cartMachine;
        private string env = System.Configuration.ConfigurationManager.AppSettings["Environment"];

        public NCCCProxy(ICartMachine cartMachine)
        {
            this._cartMachine = cartMachine;
        }

        public string Pay(SOGroupInfo soGroupInfo)
        {
            this._cartMachine.InitialMachine(soGroupInfo);
            string soBankNo = soGroupInfo.SalesOrders[0].Main.CardBank;
            string trueBankNo = soBankNo.Substring(soBankNo.Length-3);
            NCCCInput input = new NCCCInput()
            {
                OrderID = soGroupInfo.Main.ID + "",
                TransAmt = decimal.ToInt32(this._cartMachine.GetTotalPrice()) + "",
                TransMode = getHpType(soGroupInfo.SalesOrders[0].Main.PayType.Value),
                Installment = getInstallment(soGroupInfo.SalesOrders[0].Main.PayType.Value)
            };

            if (!string.IsNullOrWhiteSpace(trueBankNo) && env == "PRD")
            {
                input.BankNo = trueBankNo;
            }

            var payServiceResult = Processor.Request<string, string>("Service.NCCCPaymentService", "Pay", input);

            if (!string.IsNullOrWhiteSpace(payServiceResult.error))
            {
                throw new Exception("訂單" + soGroupInfo.SalesOrders[0].Main.Code + "付款失敗。");
            }

            string url = payServiceResult.results;
            this._cartMachine.Pay((int)SalesOrder.status.NCCC處理中);

            return url;
        }

        private string getInstallment(int payType)
        {
            string installment = "";
            switch (payType)
            {
                case (int)PayType.nPayType.信用卡一次付清:
                case (int)PayType.nPayType.信用卡紅利折抵:
                    installment = "00";
                    break;
                case (int)PayType.nPayType.三期零利率:
                    installment = "03";
                    break;
                case (int)PayType.nPayType.六期零利率:
                    installment = "06";
                    break;
                default:
                    throw new Exception("並未提供此種交易模式");
            }

            return installment;
        }

        private string getHpType(int payType)
        {
            string transMode = "";
            switch (payType){
                case (int)PayType.nPayType.信用卡一次付清:
                    transMode = "0";
                    break;
                case (int)PayType.nPayType.三期零利率:
                case (int)PayType.nPayType.六期零利率:
                    transMode = "1";
                    break;
                case (int)PayType.nPayType.信用卡紅利折抵:
                    transMode = "2";
                    break;
                default:
                    throw new Exception("並未提供此種交易模式");
            }

            return transMode;
        }

        public NCCCResult CheckPayResultByOrderId(string orderId)
        {
            var payServiceResult = Processor.Request<NCCCResult, NCCCResult>("Service.NCCCPaymentService", "CheckPayResultByOrderId", orderId);

            if (!string.IsNullOrWhiteSpace(payServiceResult.error))
            {
                throw new Exception("訂單查詢失敗。");
            }

            return payServiceResult.results;
        }

        public NCCCResult CheckPayResultByKey(string key)
        {
            var payServiceResult = Processor.Request<NCCCResult, NCCCResult>("Service.NCCCPaymentService", "CheckPayResultByKey", key);

            if (!string.IsNullOrWhiteSpace(payServiceResult.error))
            {
                throw new Exception("訂單查詢失敗。");
            }

            return payServiceResult.results;
        }
    }
}
