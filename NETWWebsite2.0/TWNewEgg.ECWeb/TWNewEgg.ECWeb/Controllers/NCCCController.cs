using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TWNewEgg.ECWeb.Services.OldCart.CartService;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.PaymentGateway;

namespace TWNewEgg.ECWeb.Controllers
{
    public class NCCCController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private AfterPayService afterService = new AfterPayService();

        [HttpPost]
        public ActionResult Pay(int id)
        {
            try
            {
                logger.Info("NCCC Pay, orderId:" + id);
                var groupServiceResult = Processor.Request<SOGroupInfo, SOGroupInfo>("SOServices.SOGroupInfoService", "GetSOGroupInfo", id);
                if (!string.IsNullOrWhiteSpace(groupServiceResult.error))
                {
                    throw new Exception("我們找不到您的訂單，請洽客服人員。");
                }

                var group = groupServiceResult.results;
                var payServiceResult = Processor.Request<string, string>("NCCCProxy", "Pay", group);

                if (!string.IsNullOrWhiteSpace(payServiceResult.error))
                {
                    throw new Exception("訂單" + group.SalesOrders[0].Main.Code + "付款失敗，請洽客服人員。");
                }

                string key = payServiceResult.results;
                ViewBag.Key = key;
                return PartialView();
            }
            catch (Exception e)
            {
                logger.Error("orderId: " + id + ", " + e.ToString());
                return Content(e.Message);
            }
        }

        public ActionResult Notify(string KEY)
        {
            ActionResult notifyResult;
            try
            {
                logger.Info("NCCC notify, key: " + KEY);

                var checkPayedResult = Processor.Request<NCCCResult, NCCCResult>("NCCCProxy", "CheckPayResultByKey", KEY);
                if (!string.IsNullOrWhiteSpace(checkPayedResult.error))
                {
                    throw new Exception("B2CReturn: NCCCProxy, check by key: " + checkPayedResult.error);
                }

                NCCCResult result = checkPayedResult.results;
                if (result.ResponseCode == "00")
                {
                    var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "PayComplete", Int32.Parse(result.OrderID));
                    if (!string.IsNullOrWhiteSpace(cartProxyResult.error))
                    {
                        throw new Exception("B2CReturn: OPCCartMachineProxy.PayComplete失敗, orderId: " + result.OrderID + ", " + cartProxyResult.error);
                    }

                    TWNewEgg.DB.TWSQLDB.Models.Auth auth = this.CreateAuth(result);
                    afterService.CreatePOAndAuth(result.OrderID, auth, result.ResponseCode, "NCCC");

                    notifyResult = RedirectToAction("Step3ResultPage", "Cart", new { SalesOrderGroupID = int.Parse(result.OrderID) });
                }
                else
                {
                    ///訂單失敗通知信，金流失敗通知信
                    TWNewEgg.ECWeb.Controllers.SendMailController SendMailController = new TWNewEgg.ECWeb.Controllers.SendMailController(this.ControllerContext);
                    SendMailController.PaymentFailureNotificationLetter(Int32.Parse(result.OrderID), "", result.ResponseCode, "OPCCartMachineProxy_Cancel");

                    var cartProxyResult = Processor.Request<string, string>("OPCCartMachineProxy", "Cancel", Int32.Parse(result.OrderID));
                    notifyResult = RedirectToAction("Index", "Cart", new { SalesOrderGroupID = int.Parse(result.OrderID), errMsg = "付款失敗，請聯絡銀行客服人員。" });
                }

                return notifyResult;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                notifyResult = RedirectToAction("Index", "Cart", new { SalesOrderGroupID = 1, errMsg = "連線發生問題，請洽客服人員。" });
                return notifyResult;
            }
        }

        public ActionResult JumpPage(string url)
        {
            ViewBag.Url = HttpUtility.JavaScriptStringEncode(url, false);
            return View();
        }

        private TWNewEgg.DB.TWSQLDB.Models.Auth CreateAuth(NCCCResult queryData)
        {
            var json = new JavaScriptSerializer().Serialize(queryData);
            logger.Info("HiTrust return result:" + json);
            TWNewEgg.DB.TWSQLDB.Models.Auth objAuth = null;

            //僅RtnCode為1時表示付款成功,其餘皆為失敗
            objAuth = new TWNewEgg.DB.TWSQLDB.Models.Auth();
            if (queryData.ResponseCode == "00")
            {
                objAuth.SuccessFlag = "1";
            }
            else
            {
                objAuth.SuccessFlag = "0";
            }

            //objAuth.AcqBank = ;
            //objAuth.AuthSN = ;
            objAuth.CustomerID = "NCCC";
            objAuth.OrderNO = queryData.OrderID;
            objAuth.AuthDate = DateTime.Now;
            try
            {
                objAuth.Amount = Int32.Parse(queryData.TransAmt);
            }
            catch
            {
                objAuth.Amount = 0;
            }

            objAuth.Bonus = 0;
            objAuth.AuthCode = queryData.ApproveCode;//銀行授權碼
            objAuth.RspCode = queryData.ResponseCode;  //授權回應碼
            objAuth.RspMSG = queryData.ResponseMsg;   //授權回應訊息
            objAuth.RspOther = "";   //額外資料
            objAuth.CreateUser = "lynn.p";
            objAuth.AgreementID = "";

            return objAuth;
        }
    }
}
