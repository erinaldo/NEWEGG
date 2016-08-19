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
    public class HiTrustProxy : IHiTrustProxy
    {
        private string _IsHiTrustPaymentService = System.Configuration.ConfigurationManager.AppSettings["IsHiTrustPaymentService"];
        private ICartMachine _cartMachine;
        public HiTrustProxy(ICartMachine cartMachine)
        {
            this._cartMachine = cartMachine;
        }

        public string Pay(SOGroupInfo soGroupInfo)
        {
            this._cartMachine.InitialMachine(soGroupInfo);
            HiTrustInput input = new HiTrustInput()
            {
                amount = this._cartMachine.GetTotalPrice(),
                BankID = soGroupInfo.SalesOrders[0].Main.CardBank,
                currency = "NTD",
                HpType = getHpType(soGroupInfo.SalesOrders[0].Main.PayType.Value),
                IsRedMoney = 0,
                orderDate = DateTime.Now,
                orderdesc = "index No: " + soGroupInfo.Main.ID + "_" + soGroupInfo.SalesOrders[0].Main.Code,
                ordernumber = soGroupInfo.Main.ID,
                PayPage = 3,
                UpdateUser = "SYS"
            };

            //Hi-trust的Toolkit版本替換成.NET：Service.HiTrustPaymentService or Service.NetHiTrustPaymentService
            var payServiceResult = Processor.Request<string, string>(_IsHiTrustPaymentService, "Pay", input);

            if (!string.IsNullOrWhiteSpace(payServiceResult.error))
            {
                throw new Exception("訂單" + soGroupInfo.SalesOrders[0].Main.Code + "付款失敗。");
            }

            string url = payServiceResult.results;
            this._cartMachine.Pay((int)SalesOrder.status.台新分期處理中);
            
            return url;
        }

        public HiTrustQueryData CheckPayResult(int id)
        {
            //Hi-trust的Toolkit版本替換成.NET：Service.HiTrustPaymentService or Service.NetHiTrustPaymentService
            var payServiceResult = Processor.Request<HiTrustQueryData, HiTrustQueryData>(_IsHiTrustPaymentService, "CheckPayResult", id);

            if (!string.IsNullOrWhiteSpace(payServiceResult.error))
            {
                throw new Exception("訂單查詢失敗。");
            }

            return payServiceResult.results;
        }

        private int getHpType(int payType)
        {
            if (payType == (int)PayType.nPayType.信用卡一次付清)
            {
                return 0;
            }
            else if (payType == (int)PayType.nPayType.三期零利率)
            {
                return 3;
            }
            else if (payType == (int)PayType.nPayType.六期零利率)
            {
                return 6;
            }
            else if (payType == (int)PayType.nPayType.十期零利率 || payType == (int)PayType.nPayType.十期分期)
            {
                return 10;
            }
            else if (payType == (int)PayType.nPayType.十二期分期 || payType == (int)PayType.nPayType.十二期零利率)
            {
                return 12;
            }
            else if (payType == (int)PayType.nPayType.十八期分期 || payType == (int)PayType.nPayType.十八期零利率)
            {
                return 18;
            }
            else if (payType == (int)PayType.nPayType.二十四期分期 || payType == (int)PayType.nPayType.二十四期零利率)
            {
                return 24;
            }
            else
            {
                throw new Exception("no such a paytype in HiTrust");
            }
        }
    }
}
