using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using AllPay.Payment.Integration;
using TWNewEgg.Models.DomainModels.PaymentGateway;
using TWNewEgg.PaymentGateway.Interface;
using System.Collections;
using System.Configuration;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using AutoMapper;
using TWNewEgg.CommonService;
using TWNewEgg.CommonService.Interface;
using TWNewEgg.CommonService.DomainModels;

namespace TWNewEgg.PaymentGateway.Service
{
    public class AllPaymentService : IAllPay 
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private INotificationService _notificationService;
        public AllPaymentService(INotificationService notificationService)
        {
            this._notificationService = notificationService;
        }
        /// <summary>
        /// 檢查是否付款成功
        /// </summary>
        /// <param name="Id">SOGroup 或 SO ID</param>
        /// <returns>是否成功</returns>
        public bool IsPayed(int Id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 付款
        /// </summary>
        /// <typeparam name="T">付款所需資料的model</typeparam>
        /// <param name="paymentInfo">付款所需資料</param>
        public string Pay(AllInOne oPayment, int paymentType)
        {
            logger.Info("AllPay PaymentGateway: Start Pay");

            AllInOne allPayment = ReGenerateData(oPayment);

            string paymentInfoUrl = System.Configuration.ConfigurationManager.AppSettings["PaymentInfoURL"];
            string clientRedirectUrl = System.Configuration.ConfigurationManager.AppSettings["ClientRedirectURL"];
            string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["AllPay_ReturnUrl"];
            List<AllPayItem> allPayItem = new List<AllPayItem>();
            List<string> enErrors = new List<string>();

            try
            {
                using (allPayment)
                {
                    /* 服務參數 */
                    string serviceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"];
                    string hashKey = System.Configuration.ConfigurationManager.AppSettings["HashKey"];
                    string hashIV = System.Configuration.ConfigurationManager.AppSettings["HashIV"];
                    string merchantID = System.Configuration.ConfigurationManager.AppSettings["MerchantID"];
                    allPayment.ServiceMethod = HttpMethod.HttpPOST;
                    allPayment.ServiceURL = serviceURL; //<<呼叫的服務位址>>
                    allPayment.HashKey = hashKey; //<<Hash Key>>
                    allPayment.HashIV = hashIV; //<<Hash IV>>
                    allPayment.MerchantID = merchantID; //<<特店編號>>
                    allPayment.Send.ReturnURL = ReturnUrl;
                    allPayment.Send.MerchantTradeDate = DateTime.Now; //<<您此筆訂單的交易時間>>
                    if (paymentType == (int)SOInfo.nPayType.歐付寶儲值支付)
                    {
                        allPayment.Send.ChoosePayment = PaymentMethod.TopUpUsed;
                    }
                    else if (paymentType == (int)SOInfo.nPayType.網路ATM)
                    {
                        allPayment.Send.ChoosePayment = PaymentMethod.WebATM;      
                    }
                    else if (paymentType == (int)SOInfo.nPayType.實體ATM)
                    {
                        allPayment.Send.ChoosePayment = PaymentMethod.ATM;
                        allPayment.SendExtend.ExpireDate = 2; // 您允許的繳費有效天數
                        allPayment.SendExtend.PaymentInfoURL = paymentInfoUrl;// 您要收到付款相關資訊的伺服器端網址
                        allPayment.SendExtend.ClientRedirectURL = clientRedirectUrl; // 您要收到虛擬帳號相關資訊的伺服器端網址
                    }
                    else if (paymentType == (int)SOInfo.nPayType.三期零利率)
                    {
                        allPayment.Send.ChoosePayment = PaymentMethod.Credit;
                        allPayment.Send.ChooseSubPayment = PaymentMethodItem.None;
                        allPayment.SendExtend.CreditInstallment = 3;
                        allPayment.SendExtend.UnionPay = false; // 是否為聯營卡(是True; 否False)
                    }
                    else if (paymentType == (int)SOInfo.nPayType.六期零利率)
                    {
                        allPayment.Send.ChoosePayment = PaymentMethod.Credit;
                        allPayment.Send.ChooseSubPayment = PaymentMethodItem.None;
                        allPayment.SendExtend.CreditInstallment = 6;
                        allPayment.SendExtend.UnionPay = false; // 是否為聯營卡(是True; 否False)
                    }
                    else if (paymentType == (int)SOInfo.nPayType.十期零利率)
                    {
                        allPayment.Send.ChoosePayment = PaymentMethod.Credit;
                        allPayment.Send.ChooseSubPayment = PaymentMethodItem.None;
                        allPayment.SendExtend.CreditInstallment = 10;
                        allPayment.SendExtend.UnionPay = false; // 是否為聯營卡(是True; 否False)
                    }

                    allPayment.Send.NeedExtraPaidInfo = ExtraPaymentInfo.No;
                    allPayment.Send.DeviceSource = DeviceType.PC;

                    logger.Info("AllPay PaymentGateway: 產生訂單" + allPayment.Send.TotalAmount);
                    /* 產生訂單 */
                    enErrors.AddRange(allPayment.CheckOut());
                    /* 產生產生訂單 Html Code 的方法 */
                    string szHtml = String.Empty;
                    enErrors.AddRange(allPayment.CheckOutString(ref szHtml));

                    if (enErrors.Count > 0)
                    {
                        DateTime now = DateTime.Now;
                        this._notificationService.Set(new NotificationModel
                        {
                            PresetId = "PaymentGatewayError",
                            MailContent = string.Format("Allpay(Pay method) connection problem! \nOrderNO: {0}, \nErrMsg: {1}"
                                , oPayment.Send.MerchantTradeNo
                                , string.Join(",", enErrors.ToArray())),
                            Title = string.Format("台灣新蛋網金流元件發生異常（Allpay）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now),
                            PhoneContent = string.Format("台灣新蛋網金流元件發生異常（Allpay）, 時間: {0:yyyy/MM/dd HH:mm:ss}", now)
                        });
                        this._notificationService.NotifyByMailAndSMS();
                        throw new Exception("歐付寶連線錯誤, PayType:" + paymentType);
                    }

                    return szHtml;
                }
            }
            catch (Exception ex)
            {
                // 例外錯誤處理。
                enErrors.Add(ex.Message);
                throw;
            }
            finally
            {
                // 顯示錯誤訊息。
                if (enErrors.Count() > 0)
                {
                    string szErrorMessage = String.Join("\\r\\n", enErrors);
                    logger.Error("AllPay PaymentGateway: " + szErrorMessage);
                }
            }
        }

        private AllInOne ReGenerateData(AllInOne allPayment)
        {
            AllInOne oPayment = new AllInOne();
            oPayment.SendExtend.Email = allPayment.SendExtend.Email;
            oPayment.SendExtend.PhoneNo = allPayment.SendExtend.PhoneNo;
            oPayment.SendExtend.UserName = allPayment.SendExtend.UserName;
            oPayment.Send.ChooseSubPayment = allPayment.Send.ChooseSubPayment;

            oPayment.Send.ClientBackURL = allPayment.Send.ClientBackURL;
            oPayment.Send.MerchantTradeNo = allPayment.Send.MerchantTradeNo;
            oPayment.Send.TotalAmount = Convert.ToInt32(allPayment.Send.TotalAmount);
            oPayment.Send.TradeDesc = allPayment.Send.TradeDesc;
            oPayment.Send.Remark = allPayment.Send.Remark;
            //Credit
            oPayment.SendExtend.InstallmentAmount = allPayment.SendExtend.InstallmentAmount;
            oPayment.SendExtend.UnionPay = allPayment.SendExtend.UnionPay;
            foreach (AllPay.Payment.Integration.Item item in allPayment.Send.Items)
            {
                oPayment.Send.Items.Add(new AllPay.Payment.Integration.Item()
                {
                    Name = item.Name,
                    Price = Convert.ToInt32(item.Price),
                    Currency = item.Currency,
                    Quantity = item.Quantity,
                    URL = item.URL
                });
            }

            return oPayment;
        }

        /// <summary>
        /// 取消付款
        /// </summary>
        /// <param name="Id">SOGroup 或 SO ID</param>
        public void Cancel(int Id)
        {
            List<string> enErrors = new List<string>();
            Hashtable htFeedback = null;
            try
            {
                using (AllInOne oPayment = new AllInOne())
                {
                    /* 服務參數 */
                    string serviceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"];
                    string hashKey = System.Configuration.ConfigurationManager.AppSettings["HashKey"];
                    string hashIV = System.Configuration.ConfigurationManager.AppSettings["HashIV"];
                    string merchantID = System.Configuration.ConfigurationManager.AppSettings["MerchantID"];

                    oPayment.ServiceMethod = HttpMethod.ServerPOST;
                    oPayment.ServiceURL = serviceURL; //<<呼叫的服務位址>>
                    oPayment.HashKey = hashKey; //<<Hash Key>>
                    oPayment.HashIV = hashIV; //<<Hash IV>>
                    oPayment.MerchantID = merchantID; //<<特店編號>>

                    /* 基本參數 */
                    oPayment.Action.MerchantTradeNo = "<<您要處理的訂單交易編號>>";
                    oPayment.Action.TradeNo = "<<AllPay的交易編號>>";
                    oPayment.Action.Action = ActionType.C;
                    oPayment.Action.TotalAmount = Decimal.Parse("<<訂單總金額>>");
                    enErrors.AddRange(oPayment.DoAction(ref htFeedback));
                }
                if (enErrors.Count() == 0)
                {
                    /* 執行後的回傳的基本參數 */

                    string szMerchantID = String.Empty;
                    string szMerchantTradeNo = String.Empty;
                    string szTradeNo = String.Empty;
                    string szRtnCode = String.Empty;
                    string szRtnMsg = String.Empty;
                    // 取得資料於畫面
                    foreach (string szKey in htFeedback.Keys)
                    {
                        switch (szKey)
                        {
                            /* 執行後的回傳的基本參數 */
                            case "MerchantID": szMerchantID = (string)htFeedback[szKey]; break;
                            default: break;
                        }
                    }
                    // 其他資料處理。

                }
                else
                {
                    // 其他資料處理。

                }
            }
            catch (Exception ex)
            {
                // 例外錯誤處理。
                enErrors.Add(ex.Message);
            }
            finally
            {
                // 顯示錯誤訊息。
                if (enErrors.Count() > 0)
                {
                    string szErrorMessage = String.Join("\\r\\n", enErrors);
                }
            }
        }

        public void Complete(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
