using AllPay.Payment.Integration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.Services.OldCart.CartService;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.ViewModels.AllPay;
using TWNewEgg.ECWeb.PrivilegeFilters;
using System.Web.Script.Serialization;
using TWNewEgg.Models.DomainModels.SendMail;
using System.IO;
using TWNewEgg.ECWeb.Auth;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowAnonymous]
    [AllowNonSecures]
    public class AllPayController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private AfterPayService afterService = new AfterPayService();
        private string HashKey;
        private string HashIV;
        private string MerchantID;
        private string ServiceURL;

        public AllPayController()
        {
            HashKey = System.Configuration.ConfigurationManager.AppSettings["HashKey"];
            HashIV = System.Configuration.ConfigurationManager.AppSettings["HashIV"];
            MerchantID = System.Configuration.ConfigurationManager.AppSettings["MerchantID"];
            ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"];
        }

        [HttpPost]
        public ActionResult Pay(int id)
        {
            try
            {
                var groupServiceResult = Processor.Request<SOGroupInfo, SOGroupInfo>("SOServices.SOGroupInfoService", "GetSOGroupInfo", id);
                if (!string.IsNullOrWhiteSpace(groupServiceResult.error))
                {
                    throw new Exception("我們找不到您的訂單，請洽客服人員。");
                }

                var group = groupServiceResult.results;
                var payServiceResult = Processor.Request<string, string>("AllPayProxy", "Pay", group);

                if (!string.IsNullOrWhiteSpace(payServiceResult.error))
                {
                    throw new Exception("訂單" + group.SalesOrders[0].Main.Code + "付款失敗，請洽客服人員。");
                }

                string html = payServiceResult.results;
                return Json(new { code = 0, data = html });
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                return Json(new { code = 0, data = e.Message });
            }
        }

        public ContentResult PaymentCheckOutFeedback(AllPayReturnData allPayReturnResult)
        {
            logger.Info("PaymentCheckOutFeedback: start");
            List<string> enErrors = new List<string>();

            try
            {
                Hashtable htFeedback = null;
                using (AllInOne oPayment = new AllInOne())
                {
                    oPayment.HashKey = this.HashKey;
                    oPayment.HashIV = this.HashIV;

                    /* 取回付款結果 */
                    enErrors.AddRange(oPayment.CheckOutFeedback(ref htFeedback));
                    logger.Info("PaymentCheckOutFeedback: Errors:" + String.Format("0|{0}", String.Join("\\r\\n", enErrors)));
                }

                if (enErrors.Count() == 0)
                {
                    var json = new JavaScriptSerializer().Serialize(allPayReturnResult);
                    logger.Info("PaymentCheckOutFeedback: 回傳內容" + json);

                    if (string.Equals(allPayReturnResult.RtnCode, "1"))
                    {
                        var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "PayComplete", Int32.Parse(allPayReturnResult.MerchantTradeNo));
                        if (!string.IsNullOrWhiteSpace(cartProxyResult.error))
                        {
                            throw new Exception(cartProxyResult.error);
                        }

                        TWNewEgg.DB.TWSQLDB.Models.Auth auth = this.CreateAuth(allPayReturnResult);
                        string errMsg = String.Format("0|{0}", String.Join("\\r\\n", enErrors));
                        afterService.CreatePOAndAuth(allPayReturnResult.MerchantTradeNo, auth, "1", "ALLPAY");
                    }
                    else
                    {
                        ///訂單失敗通知信，金流失敗通知信
                        TWNewEgg.ECWeb.Controllers.SendMailController SendMailController = new TWNewEgg.ECWeb.Controllers.SendMailController(this.ControllerContext);
                        SendMailController.PaymentFailureNotificationLetter(Int32.Parse(allPayReturnResult.MerchantTradeNo), "", allPayReturnResult.RtnCode, "OPCCartMachineProxy_Cancel");
                        var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "Cancel", Int32.Parse(allPayReturnResult.MerchantTradeNo));
                        if (!string.IsNullOrWhiteSpace(cartProxyResult.error))
                        {
                            enErrors.Add(cartProxyResult.error);
                        }
                    }
                }
                else
                {
                    string errMsg = String.Format("0|{0}", String.Join("\\r\\n", enErrors));
                    logger.Info("PaymentCheckOutFeedback: CheckOutFeedback Error，" + errMsg);
                }

                return Content("1|OK");
            }
            catch (Exception ex)
            {
                enErrors.Add(ex.Message);
                string errMsg = String.Format("0|{0}", String.Join("\\r\\n", enErrors));
                logger.Error("PaymentCheckOutFeedback: " + errMsg);
                return Content(errMsg);
            }
        }

        /// <summary>
        /// ATM通知付款結果
        /// </summary>
        public ContentResult ATMPaymentInfo(AllPayATMPaymentInfo ATMPaymentInfo)
        {
            var json = new JavaScriptSerializer().Serialize(ATMPaymentInfo);
            logger.Info("ATMPaymentInfo: start " + json);
            List<string> enErrors = new List<string>();

            try
            {
                using (AllInOne oPayment = new AllInOne())
                {
                    Hashtable htFeedback = null;
                    oPayment.HashKey = this.HashKey;
                    oPayment.HashIV = this.HashIV;
                    /* 取回付款結果 */
                    logger.Info("ATMPaymentInfo: getting payment info");
                    enErrors.AddRange(oPayment.CheckOutFeedback(ref htFeedback));
                    logger.Info(String.Format("0|{0}", String.Join("\\r\\n", enErrors)));
                }
                // 取回所有資料
                if (enErrors.Count() == 0)
                {
                    if (string.Equals(ATMPaymentInfo.RtnCode, "2"))
                    {
                        var groupServiceResult = Processor.Request<SOGroupInfo, SOGroupInfo>("SOServices.SOGroupInfoService", "GetSOGroupInfo", ATMPaymentInfo.TradeNo);
                        if (groupServiceResult.results != null)
                        {
                            SendMailDM sendMailDM = new SendMailDM();
                            sendMailDM.reciver = groupServiceResult.results.SalesOrders.FirstOrDefault().Main.Email;
                            ViewBag.OrderName = groupServiceResult.results.SalesOrders.FirstOrDefault().Main.Name;
                            ViewBag.appellation = "先生/女士";
                            ViewBag.LBOList = "";
                            foreach (var temp in groupServiceResult.results.SalesOrders) { 
                                if(ViewBag.LBOList != ""){
                                    ViewBag.LBOList = ViewBag.LBOList + "，";
                                }
                                ViewBag.LBOList = ViewBag.LBOList + temp.Main.Code;
                            }

                            using (StringWriter sw = new StringWriter())
                            {
                                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "~/Views/MailManage/Mail_ATMSuccess.cshtml");
                                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                                viewResult.View.Render(viewContext, sw);
                                sendMailDM.bodyMessage = sw.GetStringBuilder().ToString();
                            }
                            var SuccessfulMailResult = Processor.Request<bool, bool>("SendMailServices", "SendATMPaymentSuccessfulMail", sendMailDM);
                        }
                        logger.Info("ATMPaymentInfo: " + ATMPaymentInfo.MerchantTradeNo + " 取號結果:" + ATMPaymentInfo.RtnCode
                           + " 銀行代碼:" + ATMPaymentInfo.BankCode
                           + " 匯款帳號:" + ATMPaymentInfo.vAccount
                           + " 匯款期限:" + ATMPaymentInfo.ExpireDate);
                    }
                    else
                    {
                        ///訂單失敗通知信，金流失敗通知信
                        TWNewEgg.ECWeb.Controllers.SendMailController SendMailController = new TWNewEgg.ECWeb.Controllers.SendMailController(this.ControllerContext);
                        SendMailController.PaymentFailureNotificationLetter(Int32.Parse(ATMPaymentInfo.TradeNo), "", ATMPaymentInfo.RtnCode, "OPCCartMachineProxy_Cancel");
                        var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "Cancel", Int32.Parse(ATMPaymentInfo.TradeNo));
                        if (!string.IsNullOrWhiteSpace(cartProxyResult.error))
                        {
                            throw new Exception(cartProxyResult.error);
                        }

                        logger.Error("ATMPaymentInfo: RtnCode(" + ATMPaymentInfo.RtnCode + ")");
                    }
                }
                else
                {
                    string errMsg = String.Format("0|{0}", String.Join("\\r\\n", enErrors));
                    logger.Info("ATMPaymentInfo: CheckOutFeedback Error，" + errMsg);
                }

                return Content("1|OK");
            }
            catch (Exception ex)
            {
                // 例外錯誤處理。
                enErrors.Add(ex.Message);
                string errMsg = String.Format("0|{0}", String.Join("\\r\\n", enErrors));
                logger.Error("ATMPaymentInfo: " + errMsg);
                return Content(errMsg);
            }
        }

        public ActionResult ATMClientPaymentInfo(AllPayATMPaymentInfo ATMPaymentInfo)
        {
            var json = new JavaScriptSerializer().Serialize(ATMPaymentInfo);
            logger.Info("ATMClientPaymentInfo: start" + json);
            List<string> enErrors = new List<string>();

            try
            {
                using (AllInOne oPayment = new AllInOne())
                {
                    Hashtable htFeedback = null;
                    oPayment.HashKey = this.HashKey;
                    oPayment.HashIV = this.HashIV;
                    /* 取回付款結果 */
                    logger.Info("ATMClientPaymentInfo: getting payment info");
                    enErrors.AddRange(oPayment.CheckOutFeedback(ref htFeedback));
                    logger.Info("ATMClientPaymentInfo: " + String.Format("0|{0}", String.Join("\\r\\n", enErrors)));
                }

                if (enErrors.Count() == 0)
                {
                    if (string.Equals(ATMPaymentInfo.RtnCode, "2"))
                    {
                        logger.Info("ATMPaymentInfo: " + ATMPaymentInfo.MerchantTradeNo + " 取號結果:" + ATMPaymentInfo.RtnCode
                           + " 銀行代碼:" + ATMPaymentInfo.BankCode
                           + " 匯款帳號:" + ATMPaymentInfo.vAccount
                           + " 匯款期限:" + ATMPaymentInfo.ExpireDate);
                    }
                    else
                    {
                        logger.Info("ATMClientPaymentInfo: RtnCode (" + ATMPaymentInfo.RtnCode + ")");
                        throw new Exception(ATMPaymentInfo.RtnCode);
                    }
                }
                else
                {
                    throw new Exception("CheckOutFeedback Error");
                }

                DateTime expireDate;
                DateTime.TryParse(ATMPaymentInfo.ExpireDate, out expireDate);

                int soGroupId = int.Parse(ATMPaymentInfo.MerchantTradeNo);

                var upDateATMPaymentResult = Processor.Request<string, string>("SOServices.SOGroupInfoService", "UpdateATMPayment", soGroupId, ATMPaymentInfo.BankCode, ATMPaymentInfo.vAccount, expireDate);
                if (!string.IsNullOrWhiteSpace(upDateATMPaymentResult.error))
                {
                    throw new Exception("update PaymentInfo Error");
                }

                //var viewResult = cartController.Step3ResultPage(soGroupId);

                return RedirectToAction("Step3ResultPage", "Cart", new { SalesOrderGroupID = soGroupId });
            }
            catch (Exception ex)
            {
                int soGroupId = int.Parse(ATMPaymentInfo.MerchantTradeNo);
                // 例外錯誤處理。
                enErrors.Add(ex.Message);
                string errMsg = String.Format("0|{0}", String.Join("\\r\\n", enErrors));
                logger.Error("PaymentCheckOutFeedback: " + errMsg);
                ViewBag.ErrorMsg = "系統發生錯誤，請洽客服(" + ex.Message + ")";

                return RedirectToAction("Index", "Cart", new { SalesOrderGroupID = soGroupId, errMsg = "連線發生問題，請稍後再試。" });
            }
        }

        private TWNewEgg.DB.TWSQLDB.Models.Auth CreateAuth(AllPayReturnData allPayFeedback)
        {
            TWNewEgg.DB.TWSQLDB.Models.Auth objAuth = null;

            //僅RtnCode為1時表示付款成功,其餘皆為失敗
            objAuth = new TWNewEgg.DB.TWSQLDB.Models.Auth();
            if (allPayFeedback.RtnCode == "1")
            {
                objAuth.SuccessFlag = "1";
            }
            else
            {
                objAuth.SuccessFlag = "0";
            }

            objAuth.AcqBank = "10006";
            objAuth.AuthSN = allPayFeedback.TradeNo;
            objAuth.CustomerID = "AllPay";
            objAuth.OrderNO = allPayFeedback.MerchantTradeNo;
            objAuth.AuthDate = DateTime.Now;
            try
            {
                objAuth.Amount = Convert.ToInt32(allPayFeedback.TradeAmt);
            }
            catch
            {
                objAuth.Amount = 0;
            }

            objAuth.Bonus = 0;

            objAuth.RspCode = allPayFeedback.RtnCode.ToString();  //授權回應碼
            objAuth.RspMSG = allPayFeedback.RtnMsg;   //授權回應訊息
            objAuth.RspOther = "";   //額外資料
            objAuth.CreateUser = "lynn.p";
            objAuth.AgreementID = "";

            return objAuth;
        }
    }

    public class AllPayReturnData
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public string TradeNo { get; set; }
        public string TradeAmt { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public string PaymentTypeChargeFee { get; set; }
        public string TradeDate { get; set; }
        public string SimulatePaid { get; set; }
        public string CheckMacValue { get; set; }
    }

    public class AllPayATMPaymentInfo
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public string TradeNo { get; set; }
        public string TradeAmt { get; set; }
        public string PaymentType { get; set; }
        public string TradeDate { get; set; }
        public string BankCode { get; set; }
        public string vAccount { get; set; }
        public string ExpireDate { get; set; }
    }
}
