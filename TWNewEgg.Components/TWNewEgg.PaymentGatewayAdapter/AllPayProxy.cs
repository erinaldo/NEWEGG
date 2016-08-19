using AllPay.Payment.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.PaymentGatewayAdapter.Interface;

namespace TWNewEgg.PaymentGatewayAdapter
{
    public class AllPayProxy : IAllPayProxy
    {
        private ICartMachine _cartMachine;
        private IPayTypeRepoAdapter _payTypeRepoAdapter;
        public AllPayProxy(ICartMachine cartMachine, IPayTypeRepoAdapter payTypeRepoAdapter)
        {
            this._cartMachine = cartMachine;
            this._payTypeRepoAdapter = payTypeRepoAdapter;
        }

        public string Pay(SOGroupInfo soGroupInfo)
        {
            try
            {
                string clientBackURL = System.Configuration.ConfigurationManager.AppSettings["AllPay_ClientBackURL"];
                AllInOne oPayment = new AllInOne();
                Models.DomainModels.Cart.SOInfo soInfo = new Models.DomainModels.Cart.SOInfo();
                List<AllPayItem> allPayItem = new List<AllPayItem>();


                this._cartMachine.InitialMachine(soGroupInfo);
                switch (soGroupInfo.SalesOrders[0].Main.PayType.Value)
                {
                    case (int)Models.DomainModels.Cart.SOInfo.nPayType.歐付寶儲值支付:
                        oPayment.SendExtend.Email = soGroupInfo.SalesOrders[0].Main.Email;
                        oPayment.SendExtend.PhoneNo = soGroupInfo.SalesOrders[0].Main.Mobile;
                        oPayment.SendExtend.UserName = soGroupInfo.SalesOrders[0].Main.Name;
                        this._cartMachine.Pay((int)SalesOrder.status.歐付寶儲值支付處理中);
                        break;
                    case (int)Models.DomainModels.Cart.SOInfo.nPayType.網路ATM:
                        this._cartMachine.Pay((int)SalesOrder.status.歐付寶WebATM處理中);
                        break;
                    case (int)Models.DomainModels.Cart.SOInfo.nPayType.實體ATM:
                        this._cartMachine.Pay((int)SalesOrder.status.歐付寶線下ATM處理中);
                        PayType paytype = this._payTypeRepoAdapter.GetPayType(soGroupInfo.SalesOrders[0].Main.PayTypeID.Value);
                        oPayment.Send.ChooseSubPayment = GetPaymentMethodItem(paytype.BankID.Value);
                        break;
                    case (int)Models.DomainModels.Cart.SOInfo.nPayType.三期零利率:
                    case (int)Models.DomainModels.Cart.SOInfo.nPayType.六期零利率:
                    case (int)Models.DomainModels.Cart.SOInfo.nPayType.十期零利率:
                        oPayment.SendExtend.InstallmentAmount = this._cartMachine.GetTotalPrice(); // 刷卡分期的付款金額(預設不提供分期，預設0)
                        this._cartMachine.Pay((int)SalesOrder.status.歐付寶分期處理中);
                        break;
                    default:
                        break;
                }

                int paymentType = soGroupInfo.SalesOrders[0].Main.PayType.Value; //付款方式
                oPayment.Send.ClientBackURL = clientBackURL + soGroupInfo.Main.ID; // 歐付寶返回按鈕導向的瀏覽器端網址
                oPayment.Send.MerchantTradeNo = soGroupInfo.Main.ID.ToString(); // 此筆訂單交易編號
                oPayment.Send.TotalAmount = this._cartMachine.GetTotalPrice(); //您此筆訂單的交易總金額
                oPayment.Send.TradeDesc = soGroupInfo.Main.Note = soGroupInfo.Main.ID.ToString(); // 該筆訂單的描述
                oPayment.Send.Remark = ""; // 備註欄位
                
                foreach (SOItemBase itemBase in soGroupInfo.SalesOrders[0].SOItems)
                {
                    oPayment.Send.Items.Add(new AllPay.Payment.Integration.Item()
                    {
                        Name = itemBase.Name,
                        Price = itemBase.Price,
                        Currency = "元",
                        Quantity = itemBase.Qty,
                        URL = "https://secure.newegg.com.tw/item?itemid=" + itemBase.ItemID
                    });
                }

                var payServiceResult = Processor.Request<string, string>("Service.AllPaymentService", "Pay", oPayment, paymentType);

                if (!string.IsNullOrWhiteSpace(payServiceResult.error))
                {
                    throw new Exception("訂單" + soGroupInfo.SalesOrders[0].Main.Code + "付款失敗。");
                }

                string html = payServiceResult.results;

                return html;
            }
            catch
            {
                throw;
            }
        }

        private PaymentMethodItem GetPaymentMethodItem(int bankId)
        {
            PaymentMethodItem result;
            switch (bankId)
            {
                case 93:
                    result = PaymentMethodItem.ATM_FIRST;
                    break;
                case 205:
                    result = PaymentMethodItem.ATM_TAISHIN;
                    break;
                case 206:
                    result = PaymentMethodItem.ATM_ESUN;
                    break;
                case 207:
                    result = PaymentMethodItem.ATM_HUANAN;
                    break;
                case 208:
                    result = PaymentMethodItem.ATM_BOT;
                    break;
                case 209:
                    result = PaymentMethodItem.ATM_FUBON;
                    break;
                case 211:
                    result = PaymentMethodItem.ATM_CHINATRUST;
                    break;
                default:
                    throw new Exception("未選擇銀行");
            }

            return result;
        }
    }
}
