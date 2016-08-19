using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.SendMail;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class SendMailController : Controller
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private string NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["Images"];
        private string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
        private string NoticeMail = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SendPOFailNoticeMail"];
        object backupController = "";

        public SendMailController(ControllerContext _context)
        {
            this.ControllerContext = _context;
            this.ControllerContext.Controller = this;
            backupController = this.ControllerContext.RouteData.Values["controller"];
        }

        public ActionResult Index()
        {
            return View();
        }

        /// 訂單失敗通知信，金流失敗通知信
        /// </summary>
        /// <param name="needReSendList"></param>
        /// <param name="payFail">當PayFail為F則為金流失敗通知信</param>
        /// <param name="rtnCode">rtncode</param>
        /// <param name="rtnMsg">rtnmsg</param>
        /// <returns>是否成功發送</returns>
        public bool PaymentFailureNotificationLetter(int SOGroupID, string payFail = "", string rtnCode = "", string rtnMsg = "")
        {
            string userEmail = NEUser.Email;
            try
            {
                logger.Info("Email [" + userEmail + "] SOReSendMail : [start] GroupID [" + SOGroupID + "] payFail [" + payFail + "] rtnCode [" + rtnCode + "] rtnMsg [" + rtnMsg + "]");
                TWNewEgg.DB.TWSqlDBContext db_before = new DB.TWSqlDBContext();
                string messageresult = "";

                var SOGroupPaymentFailureMailDataResult = Processor.Request<SOGroupPaymentFailureMailData, SOGroupPaymentFailureMailData>("SOServices.SOGroupInfoService", "GetSOGroupPaymentFailureMailData", SOGroupID);

                if (SOGroupPaymentFailureMailDataResult.results != null)
                {
                    SendMailDM SendMailDM = new SendMailDM();
                    ViewBag.NewLinkTitle = NewLinkTitle;
                    ViewBag.Environment = environment.ToUpper();
                    SOGroupPaymentFailureMailDataResult.results.payFail = payFail;
                    SOGroupPaymentFailureMailDataResult.results.rtnCode = rtnCode;
                    SOGroupPaymentFailureMailDataResult.results.rtnMsg = rtnMsg;

                    ViewBag.SOGroupPaymentFailureMailDataResult = SOGroupPaymentFailureMailDataResult.results;
                    using (StringWriter sw = new StringWriter())
                    {
                        ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "~/Views/MailManage/PaymentFailureNotificationLetter.cshtml");
                        ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                        viewResult.View.Render(viewContext, sw);
                        messageresult = sw.GetStringBuilder().ToString();
                    }

                    SendMailDM.bodyMessage = messageresult;
                    SendMailDM.reciver = NoticeMail.ToString();
                    SendMailDM.subject = "(Issue) 金流失敗 [ " + "GroupID:" + SOGroupID + "_Environment:" + environment.ToUpper() + " ]";
                    var SendMailResult = Processor.Request<bool, bool>("SendMailServices", "SendMail", SendMailDM);

                    if (SendMailResult.error != null)
                    {
                        logger.Info(SendMailResult.error);
                    }
                }

                logger.Info("Email [" + userEmail + "] SOReSendMail : [End]");
            }
            catch (Exception ex)
            {
                logger.Info("AllPayCredic:(寄發金流發生錯誤通知信失敗) : [Email] " + userEmail + " 寄發金流發生錯誤通知信失敗 [ErrorMessage] " + ex.ToString());
                return false;
            }
            return true;
        }
    }
}
