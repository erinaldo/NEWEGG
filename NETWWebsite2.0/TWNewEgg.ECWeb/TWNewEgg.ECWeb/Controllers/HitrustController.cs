using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.ECWeb.Services.OldCart.CartService;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.PaymentGateway;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowAnonymous]
    [AllowNonSecures]
    public class HitrustController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private AfterPayService afterService = new AfterPayService();
        

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Pay(int id)
        {
            try
            {
                logger.Info("HiTrust Pay: start, groupId:" + id);
                var groupServiceResult = Processor.Request<SOGroupInfo, SOGroupInfo>("SOServices.SOGroupInfoService", "GetSOGroupInfo", id);
                if (!string.IsNullOrWhiteSpace(groupServiceResult.error))
                {
                    throw new Exception("我們找不到您的訂單，請洽客服人員。");
                }

                var group = groupServiceResult.results;
                var payServiceResult = Processor.Request<string, string>("HiTrustProxy", "Pay", group);

                if (!string.IsNullOrWhiteSpace(payServiceResult.error))
                {
                    throw new Exception("訂單" + group.SalesOrders[0].Main.Code + "付款失敗，請洽客服人員。");
                }

                string url = payServiceResult.results;
                return Json(new { code = 0, data = url });
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                return Json(new { code = 1, data = e.Message });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Mac"></param>
        /// <param name="Cipher"></param>
        /// <param name="merconfigname"></param>
        /// <returns></returns>
        public ActionResult B2CUpdate(string Key, string Mac, string Cipher, string merconfigname)
        {
            logger.Info("----------------------------");
            logger.Info("B2CUpdate = B2CUpdate start");
            logger.Info("Key: " + Key + ", Mac: " + Mac + ", Cipher: " + Cipher + ", merconfigname: " + merconfigname);
            //logger.Info("key:" + Key + " mac:" + Mac + " Cipher:" + Cipher + " merconfigname:" + merconfigname);
            //System.Type oType = System.Type.GetTypeFromProgID("HiB2CCom.eB2CCom.1");
            //object tmp = System.Activator.CreateInstance(oType);
            //oType.InvokeMember("Key", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { Key });
            //oType.InvokeMember("Mac", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { Mac });//訂單編號
            //oType.InvokeMember("Cipher", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { Cipher });//訂單編號
            //oType.InvokeMember("merconfigname", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { merconfigname });//訂單編號
            //oType.InvokeMember("B2CUpdateNoPost", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);//呼叫查詢函式
            //string retcode = (string)oType.InvokeMember("retcode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //string ordernumber = (string)oType.InvokeMember("ordernumber", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //logger.Info("retcode:" + retcode + " ordernumber:" + ordernumber);
            logger.Info("B2CUpdate = B2CUpdate end");
            logger.Info("----------------------------");
            return Content("R01=00");
        }
        
        /// <summary>
        /// 由 HiTrust 回傳交易結果, 根據交易結果對內部資料進行處理
        /// </summary>
        /// <param name="hitrustResult"></param>
        /// <returns></returns>
        public ActionResult MerUpdateURL(HitrustReturnResult hitrustResult)
        {
            logger.Info("retcode: " + hitrustResult.retcode + ";ordernumber: " + hitrustResult.ordernumber);
            try
            {
                var checkPayedResult = Processor.Request<HiTrustQueryData, HiTrustQueryData>("HiTrustProxy", "CheckPayResult", Int32.Parse(hitrustResult.ordernumber));
                if (!string.IsNullOrWhiteSpace(checkPayedResult.error))
                {
                    logger.Error("B2CReturn: HiTrustProxy.IsPayed " + checkPayedResult.error);
                }
                HiTrustQueryData queryData = checkPayedResult.results;
                if (hitrustResult.retcode == "00")
                {
                    var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "PayComplete", Int32.Parse(hitrustResult.ordernumber));
                    //var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "PayComplete", Int32.Parse(hitrustResult.ordernumber));
                    if (!string.IsNullOrWhiteSpace(cartProxyResult.error))
                    {
                        throw new Exception("B2CReturn: OPCCartMachineProxy.PayComplete " + cartProxyResult.error);
                    }
                    TWNewEgg.DB.TWSQLDB.Models.Auth auth = this.CreateAuth(queryData);
                    afterService.CreatePOAndAuth(hitrustResult.ordernumber, auth, hitrustResult.retcode, "HITRUST");
                }
                else
                {
                    ///訂單失敗通知信，金流失敗通知信
                    TWNewEgg.ECWeb.Controllers.SendMailController SendMailController = new TWNewEgg.ECWeb.Controllers.SendMailController(this.ControllerContext);
                    SendMailController.PaymentFailureNotificationLetter(Int32.Parse(hitrustResult.ordernumber), "", hitrustResult.retcode, "OPCCartMachineProxy_Cancel");
                    var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "Cancel", Int32.Parse(hitrustResult.ordernumber));
                }
            }
            catch (Exception error)
            {
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[innerException]: " + (error.InnerException == null ? "No innerException" : error.InnerException.Message));
            }
            //不管結果是哪一個都回傳 00 ( 跟 HiTrust 確認 )
            return Content("R01=00");
        }
        /// <summary>
        /// 由 HiTrust 導回後根據付款狀態判斷要導向哪一個畫面, 成功頁或失敗頁
        /// </summary>
        /// <param name="hitrustResult">由 HiTrust 帶回</param>
        /// <returns></returns>
        public ActionResult B2CReturn(HitrustReturnResult hitrustResult)
        {
            logger.Info("retcode: " + hitrustResult.retcode + " ;ordernumber: " + hitrustResult.ordernumber);
            RedirectToRouteResult result;
            try
            {
                var checkPayedResult = Processor.Request<HiTrustQueryData, HiTrustQueryData>("HiTrustProxy", "CheckPayResult", Int32.Parse(hitrustResult.ordernumber));
                if (!string.IsNullOrWhiteSpace(checkPayedResult.error))
                {
                    logger.Error("B2CReturn: HiTrustProxy.IsPayed " + checkPayedResult.error);
                }
                HiTrustQueryData queryData = checkPayedResult.results;
                //00 為成功狀態
                if (hitrustResult.retcode == "00")
                {
                    var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "PayComplete", Int32.Parse(hitrustResult.ordernumber));
                    //var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "PayComplete", Int32.Parse(hitrustResult.ordernumber));
                    if (string.IsNullOrWhiteSpace(cartProxyResult.error))
                    {
                        TWNewEgg.DB.TWSQLDB.Models.Auth auth = this.CreateAuth(queryData);
                        afterService.CreatePOAndAuth(hitrustResult.ordernumber, auth, hitrustResult.retcode, "HITRUST");
                    }
                    result = RedirectToAction("Step3ResultPage", "Cart", new { SalesOrderGroupID = int.Parse(hitrustResult.ordernumber) });
                }
                else
                {
                    result = RedirectToAction("Index", "Cart", new { SalesOrderGroupID = int.Parse(hitrustResult.ordernumber), errMsg = "付款失敗，請聯絡銀行客服人員。" });
                }
            }
            catch (Exception e)
            {
                logger.Error("B2CReturn: " + e.ToString());
                result = RedirectToAction("Index", "Cart", new { SalesOrderGroupID = int.Parse(hitrustResult.ordernumber), errMsg = "連線發生問題，請聯絡客服人員。" });
            }
            return result;
        }

        private TWNewEgg.DB.TWSQLDB.Models.Auth CreateAuth(HiTrustQueryData queryData)
        {
            var json = new JavaScriptSerializer().Serialize(queryData);
            logger.Info("HiTrust return result:" + json);
            TWNewEgg.DB.TWSQLDB.Models.Auth objAuth = null;

            //僅RtnCode為1時表示付款成功,其餘皆為失敗
            objAuth = new TWNewEgg.DB.TWSQLDB.Models.Auth();
            if (queryData.retcode == "00")
            {
                objAuth.SuccessFlag = "1";
            }
            else
            {
                objAuth.SuccessFlag = "0";
            }

            objAuth.AcqBank = "812";
            objAuth.AuthSN = queryData.authRRN;
            objAuth.CustomerID = "Hitrust";
            objAuth.OrderNO = queryData.ordernumber;
            objAuth.AuthDate = DateTime.Now;
            try
            {
                objAuth.Amount = Convert.ToInt32(queryData.depositamount) / 100;
            }
            catch
            {
                objAuth.Amount = 0;
            }

            objAuth.Bonus = 0;
            objAuth.AuthCode = queryData.authCode;//銀行授權碼
            objAuth.RspCode = queryData.retcode;  //授權回應碼
            objAuth.RspMSG = queryData.retcode;   //授權回應訊息
            objAuth.RspOther = "";   //額外資料
            objAuth.CreateUser = "lynn.p";
            objAuth.AgreementID = "";

            return objAuth;
        }
    }

    public class HitrustReturnResult
    {
        /// <summary>
        /// 訂單號碼
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 交易結果
        /// </summary>
        public string retcode { get; set; }
        /// <summary>
        /// 付款金額
        /// </summary>
        public string amount { get; set; }
    }//end class

    public class HitrustB2CUpdateResult
    {
        public string Key { get; set; }
        public string Mac { get; set; }
        public string Cipher { get; set; }
        public string merconfigname { get; set; }
    }
}
