using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    public class MailController : Controller
    {
        //private string NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["Images"];
        private string NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["TWSPHost"];
        private static ILog log = LogManager.GetLogger(typeof(MailController));
        private string NeweggCompanyName = System.Configuration.ConfigurationManager.AppSettings["GlobalCompanyName"];
        /// <summary>
        /// 寄送信件
        /// </summary>
        /// <returns></returns>
        ///
        [Attributes.ActionDescriptionAttribute("寄送信件")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult SendMail(API.Models.Mail mail)
        {
            //Service.SellerInvitationService SIService = new Service.SellerInvitationService();

            ////回傳成功與否
            //API.Models.ActionResponse<TWNewEgg.API.Models.SendInvitationEmailResult> apiResult = new ActionResponse<TWNewEgg.API.Models.SendInvitationEmailResult>();
            //apiResult = SIService.SendInvitationEmail(sendInvitationEmail);

            API.Models.MailTransformation mailTransformation = new MailTransformation();
            DB.TWSELLERPORTALDB.Models.Seller_MailLog mail_Log = new DB.TWSELLERPORTALDB.Models.Seller_MailLog();
            //回傳成功與否
            API.Models.ActionResponse<TWNewEgg.API.Models.MailResult> apiResult = new ActionResponse<TWNewEgg.API.Models.MailResult>();
            //這邊不寫入整個 Model Log 雜亂訊息太多
            //var json_mail = Newtonsoft.Json.JsonConvert.SerializeObject(mail);
            //log.Info(json_mail);
            try
            {
                string Messageresult = "";

                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = null;
                    ViewBag.NewLinkTitle = NewLinkTitle;
                    DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

                    switch (mail.MailType)
                    {
                        // Seller 邀請信
                        case Mail.MailTypeEnum.SallerInvitationEmail: //Saller Invitation Email
                            DB.TWSELLERPORTALDB.Models.Seller_User user = spdb.Seller_User.Where(x => x.UserEmail == mail.UserEmail).FirstOrDefault();

                            ViewBag.UserEmail = mail.UserEmail;
                            ViewBag.RanCode = user.RanCode;

                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_SellerInvitation");
                            ViewBag.MailLink = NewLinkTitle + "/Account/SetPassword?UserEmail=" + mail.UserEmail + "&RanCo=" + user.RanCode;

                            //紀錄邀請連結
                            mail_Log.ID = "http://sellerportal.newegg.com.tw/Account/SetPassword?UserEmail=" + mail.UserEmail + "&RanCo=" + user.RanCode;

                            mailTransformation.MailSubject = "【" + NeweggCompanyName + "】會員邀請信";
                            break;

                        // 製造商審核結果通知 Email
                        case Mail.MailTypeEnum.ManufactureRequestNotification:
                            string[] mailMessage = mail.MailMessage.ToString().Split(';');
                            ViewBag.Result = mailMessage[0].Trim();
                            ViewBag.DeclineReason = mailMessage[1].Trim();
                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_ManufactureRequestNotification");

                            mailTransformation.MailSubject = "【" + NeweggCompanyName + "】製造商審核結果通知";
                            break;

                        // 庫存警示 Email
                        case Mail.MailTypeEnum.InventoryAlertEmail:
                            ViewBag.UserEmail = mail.UserEmail;
                            ViewBag.UserName = mail.UserName;
                            ViewBag.MailMessage = mail.MailMessage;
                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_ItemInventoryAlert");

                            mailTransformation.MailSubject = "【" + NeweggCompanyName + "】庫存異常系統通知信";
                            break;

                        // 重設密碼 Email
                        case Mail.MailTypeEnum.ResetPassword:
                            DB.TWSELLERPORTALDB.Models.Seller_User resetUser = spdb.Seller_User.Where(x => x.UserEmail == mail.UserEmail).FirstOrDefault();

                            ViewBag.UserEmail = mail.UserEmail;
                            ViewBag.RanCode = resetUser.RanCode;

                            string mailLink = NewLinkTitle + "/Account/SetPassword?UserEmail=" + mail.UserEmail + "&RanCo=" + resetUser.RanCode;

                            ViewBag.MailLink = mailLink;
                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_ResetPassword");

                            // 紀錄寄送連結
                            mail_Log.ID = mailLink;
                            mailTransformation.MailSubject = "【" + NeweggCompanyName + "】會員密碼重設信";
                            break;

                        // Seller提出製造商審核通知管理者 Jack.W.Wu
                        case Mail.MailTypeEnum.NewManufactureVerifyEmail:

                            ViewBag.UserEmail = mail.UserEmail;

                            ViewBag.UserName = mail.UserName;
                            // 待審核清單
                            ViewData.Model = spdb.Seller_ManufactureInfo_Edit.Where(x => x.ManufactureStatus == "P").ToList();
                            // 待審核數目
                            ViewBag.MailMessage = spdb.Seller_ManufactureInfo_Edit.Where(x => x.ManufactureStatus == "P").Count();

                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_NewManufactureVerify");

                            mailTransformation.MailSubject = "【Seller Portal】用戶製造商申請通知信";
                            break;

                        // 新訂單通知Seller Email
                        case Mail.MailTypeEnum.InformSellerNewSalesOrder:
                            ViewBag.UserEmail = mail.UserEmail;
                            ViewBag.UserName = mail.UserName;
                            ViewBag.MailMessage = mail.MailMessage;
                            ViewBag.SellerName = mail.UserName;
                            string[] MessageArray = mail.MailMessage.Split(',');
                            ViewBag.OrderNumber = MessageArray[0];
                            ViewBag.OrderDate = MessageArray[1];

                            // Mail_log 訂單編號
                            mail_Log.ID = MessageArray[0];

                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_InformSellerNewOrder");

                            mailTransformation.MailSubject = "【 " + NeweggCompanyName + "】訂單通知信";
                            break;

                        // 訂單取消通知
                        case Mail.MailTypeEnum.InformSellerCancelOrder:
                            ViewBag.SellerName = mail.UserName;
                            ViewBag.orderNumber = mail.MailMessage;

                            // 訂單編號
                            mail_Log.ID = mail.MailMessage;

                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_InformSellerCancelOrder");

                            mailTransformation.MailSubject = "【" + NeweggCompanyName + "】訂單取消通知信";
                            break;
                        // 通知PM Seller 新上架商品
                        case Mail.MailTypeEnum.Mail_TO_PMs:
                            ViewBag.UserEmail = mail.UserEmail;
                            ViewData.Model = mail.ItemInfoList;

                            viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_ItemInfoToPM");

                            // 紀錄新上架商品數量
                            mail_Log.ID = mail.ItemInfoList.Count().ToString();

                            mailTransformation.MailSubject = "【Seller Portal】待審核商品通知信";
                            break;

                        // 通知PM Seller修改商品售價 Jack.W.Wu
                        case Mail.MailTypeEnum.PriceChangedMail:
                            {

                                ViewBag.UserEmail = mail.UserEmail;

                                ViewBag.UserName = mail.UserName;
                                // 修改清單
                                ViewData.Model = mail.ItemInfoList.ToList();
                                // 修改數量
                                ViewBag.MailMessage = mail.ItemInfoList.Count();

                                viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_PriceChanged");

                                mailTransformation.MailSubject = "【Seller Portal】Seller商品價格變動通知信";
                            }
                            break;
                        case Mail.MailTypeEnum.ErrorInfo:
                            {
                                ViewBag.UserEmail = mail.UserEmail;

                                ViewBag.MailMessage = mail.MailMessage;

                                viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_ErrorInfo");

                                string[] errorInfo = mail.MailMessage.Split(',');

                                mail_Log.ID = errorInfo[0];

                                mailTransformation.MailSubject = "【Seller Portal】錯誤發生通知信";
                            }
                            break;

                        // 出貨提醒通知信
                        case Mail.MailTypeEnum.RemindSellerToSendPackage:
                            {
                                ViewBag.UserEmail = mail.UserEmail;
                                ViewBag.UserName = mail.UserName;
                                //ViewBag.MailMessage = mail.MailMessage;
                                ViewBag.SellerName = mail.UserName;

                                if (mail.IsAdmin == false)
                                { ViewBag.SellerID = mail.UnshipList.FirstOrDefault().SellerID; }
                                if (mail.IsAdmin == true)
                                { ViewBag.SellerID = ""; }

                                ViewBag.Unshipcount = mail.UnshipList.Sum(x => x.Unshipcount); //MessageArray2[1];
                                ViewBag.IsAdmin = mail.IsAdmin;
                                ViewBag.unshipList = mail.UnshipList;

                                mail_Log.ID = mail.UnshipList.FirstOrDefault().SellerID + " Unshipcount:" + mail.UnshipList.FirstOrDefault().Unshipcount;

                                viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_RemindSellerToSendPackage");

                                mailTransformation.MailSubject = "【" + NeweggCompanyName + "】出貨提醒通知信";
                            }
                            break;
                        case Mail.MailTypeEnum.RemindGrossMargin:
                            {
                                ViewBag.UserEmail = mail.UserEmail;
                                ViewBag.MailMessage = mail.MailMessage;

                                viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_RemindGrossMargin");

                                mailTransformation.MailSubject = "【" + NeweggCompanyName + "】館價低於毛利率通知信";
                            }
                            break;
                        case Mail.MailTypeEnum.UserResponse:
                            {
                                string responseMail = System.Configuration.ConfigurationManager.AppSettings["UserResponse"];

                                if (string.IsNullOrEmpty(responseMail))
                                    responseMail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];

                                var responseMails = responseMail.Split(',');
                                mail.UserEmail = responseMails[0];
                                mail.RecipientBcc = responseMail;
                                ViewBag.MailMessage = mail.MailMessage;
                                ViewBag.UserName = mail.UserName;
                                viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_UserResponse");


                                mail_Log.ID = mail.UserName + ", " + mail.MailMessage;

                                mailTransformation.MailSubject = "【" + NeweggCompanyName + "】用戶回饋通知信";
                            }
                            break;
                        case Mail.MailTypeEnum.RMAInfo:
                            {
                                ViewBag.SellerName = mail.UserName;
                                ViewBag.OrderNumber = mail.OrderInfo.OrderID;
                                ViewBag.ProductName = mail.OrderInfo.ProductName;

                                // Mail_log 訂單編號
                                mail_Log.ID = mail.OrderInfo.OrderID;

                                viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_InforSellerRMA");

                                mailTransformation.MailSubject = "【 " + NeweggCompanyName + "】退貨通知信";
                            }
                            break;
                        case Mail.MailTypeEnum.RMASuccessInfo:
                            {
                                ViewBag.UserEmail = mail.UserEmail;
                                ViewBag.MailMessage = mail.MailMessage;
                                ViewBag.SellerName = mail.UserName;

                                // Mail_log 訂單編號
                                mail_Log.ID = mail.MailMessage;

                                viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_InforSellerRMASuccess");

                                mailTransformation.MailSubject = "【 " + NeweggCompanyName + "】退貨完成通知信";
                            }
                            break;
                        default:
                            break;
                    }

                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    
                    viewResult.View.Render(viewContext, sw);

                    Messageresult = sw.GetStringBuilder().ToString();
                }

                mailTransformation.MailContent = Messageresult;
           
                //return Success;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }

            DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();           

            mailTransformation.UserEmail = mail.UserEmail;
            mailTransformation.RecipientBcc = mail.RecipientBcc;
            Service.MailService mailService = new Service.MailService();

            apiResult = mailService.SendEmail(mailTransformation);
            
            // 判斷 SendEmail 是否有內容
            if (apiResult != null)
            {
                mail_Log.MailType = mail.MailType.ToString();
                mail_Log.Email = mail.UserEmail == null ? "" : mail.UserEmail;
                mail_Log.IsSuccess = apiResult.IsSuccess ? "T" : "F";
                mail_Log.InDate = DateTime.Now;
                mail_Log.Msg = apiResult.Msg.Length > 500 ? apiResult.Msg.Substring(0, 500) : apiResult.Msg;

                if (apiResult.IsSuccess == false &&　mail.MailType == Mail.MailTypeEnum.SallerInvitationEmail)
                { 
                    var json_mail_Content = Newtonsoft.Json.JsonConvert.SerializeObject(mailTransformation);
                    var json_mail_sendResult = Newtonsoft.Json.JsonConvert.SerializeObject(apiResult);

                    log.Error(json_mail_sendResult);
                    log.Error("SendMail API Result: " + json_mail_Content);

                    API.Models.Connector conn = new Connector();

                    string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];

                    string APIHost = System.Configuration.ConfigurationManager.AppSettings["TWSPHost"];  //2014.1.24 寄認證信path改讀AppSettings.Config mark by ice
                    DB.TWSELLERPORTALDB.Models.Seller_User user = db.Seller_User.Where(x => x.UserEmail == mail.UserEmail).FirstOrDefault();

                    var MailLink = APIHost + "/Account/SetPassword?UserEmail=" + mail.UserEmail + "&RanCo=" + user.RanCode; 

                    Models.Mail adminmailContent = new Models.Mail();
                    adminmailContent.MailType = Mail.MailTypeEnum.ErrorInfo;
                    adminmailContent.MailMessage = "商家邀請信件未寄出，商家邀請連結如下: " + MailLink;
                    
                    string[] errorInfo = adminEmail.Split(',');

                    foreach (var adminmail in errorInfo)
                    {
                        adminmailContent.UserEmail = adminmail;
                        
                        Thread.Sleep(1000);
                        conn.SendMail(null, null, adminmailContent);
                    }
                }
            }
            else 
            {
                mail_Log.MailType = mail.MailType.ToString();
                mail_Log.Email = mail.UserEmail == null ? "" : mail.UserEmail;
                mail_Log.IsSuccess = "F";
                mail_Log.InDate = DateTime.Now;
                mail_Log.Msg = "SendMail return is null, see log info";
                var json_mail_inputMail = Newtonsoft.Json.JsonConvert.SerializeObject(mail);
                var json_mail_sendResult = Newtonsoft.Json.JsonConvert.SerializeObject(apiResult);
                log.Error("API Result: " + json_mail_sendResult);
                log.Error("Input Par 'mail': " + json_mail_inputMail);
            }

            try
            {

                db.Seller_MailLog.Add(mail_Log);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                var json_mail_log = Newtonsoft.Json.JsonConvert.SerializeObject(mail_Log);
                var json_mail_inputMail = Newtonsoft.Json.JsonConvert.SerializeObject(mail);
                log.Error("In Exception, Input Par 'mail': " + json_mail_inputMail);
                log.Error(ex.Message);
                log.Error(json_mail_log);

            }

            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }
    }
}