using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.GetConfigData.Service;

namespace TWNewEgg.Website.ECWeb.Controllers
{
    public class MailManageController : Controller
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);
        string NewLinkTitle = System.Configuration.ConfigurationManager.AppSettings["Images"];
        string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
        string ECDomain2 = System.Configuration.ConfigurationManager.AppSettings["ECSSLDomain"];
        string NoticeMail = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SendPOFailNoticeMail"];
        object backupController = "";

        public MailManageController(ControllerContext _context)
        {
            this.ControllerContext = _context;
            this.ControllerContext.Controller = this;
            backupController = this.ControllerContext.RouteData.Values["controller"];
        }

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 寄發交易成功通知信，輸入參數:除國際運費及服務費外所有訂單資訊, 密件收件人
        /// </summary>
        /// <param name="SOList"></param>
        /// <returns></returns>
        public bool DealSuccess(List<InsertSalesOrdersBySellerOutput> SOList, string BCCMail = "")
        {
            TWSqlDBContext DB_Before = new TWSqlDBContext();
            bool SuccessFlag = false;
            bool Is602 = false, IsOver1000 = false;
            List<InsertSalesOrdersBySellerOutput> SOListTemp = new List<InsertSalesOrdersBySellerOutput>();
            string MainDelvType = ""; // 訂購成功通知信細節分類
            string CorS = SOList.First().salesorder_invoid.Trim().Length > 0 ? CorS = "C" : CorS = "S"; // 公司 or 個人
            try
            {
                List<int> SODelvtypeList = SOList.Select(x => (int)x.salesorder_delivtype).Distinct().ToList(); // 找出需要寄出哪些類型的信
                foreach (var row in SODelvtypeList)
                {
                    switch (row)
                    {
                        case (int)Item.tradestatus.切貨:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.切貨).ToList();
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.切貨, "2", "", "", BCCMail); // 發信SO資料List, 信件選擇, 到達天數 , 額外主旨附註
                            SuccessFlag = true;
                            break;
                        case (int)Item.tradestatus.間配:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.間配).ToList();
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.間配, "7~9", "", "", BCCMail);
                            SuccessFlag = true;
                            break;
                        case (int)Item.tradestatus.直配:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.直配).ToList();
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.直配, "3~5", "", "", BCCMail);
                            SuccessFlag = true;
                            break;
                        case (int)Item.tradestatus.三角:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.三角).ToList();
                            Is602 = false; IsOver1000 = false;
                            foreach (var sorow in SOListTemp)
                            {
                                Product ProductTemp = DB_Before.Product.Where(x => x.ID == sorow.salesorderitem_productid).FirstOrDefault();
                                if (ProductTemp.SourceTable == "productfromws")
                                {
                                    ProductFromWS ProductFromWSTemp = DB_Before.ProductFromWS.Where(x => x.ID == ProductTemp.FK).FirstOrDefault();
                                    if (ProductFromWSTemp.CCC != null && ProductFromWSTemp.CCC != "")
                                    {
                                        TwTradeTax TwTradeTaxTemp = DB_Before.TwTradeTax.Where(x => x.ID == ProductFromWSTemp.CCC.Replace("\n", "")).FirstOrDefault();
                                        if (TwTradeTaxTemp.ImportRule != null && TwTradeTaxTemp.ImportRule != "")
                                        {
                                            string[] ImportRuleTemp = TwTradeTaxTemp.ImportRule.Split(' ');
                                            foreach (string IRrow in ImportRuleTemp) if (IRrow == "602") Is602 = true;
                                        }
                                    }
                                }
                                // if 其中一個超過 1000m 則 true 並決定哪種寄信的類型，0、1多加報驗書、2多加報驗+電信、3多加電信自用
                                decimal cos = (sorow.salesorderitem_price + sorow.salesorderitem_shippingexpense + sorow.salesorderitem_serviceexpense) / 29; // 賣場價 + 國際物流處理費 
                                IsOver1000 = cos > 1000m ? true : false;
                                if (IsOver1000 == false && Is602 == false) { MainDelvType = "3_0_"; } // 0.無多加
                                if (IsOver1000 == true && Is602 == false) { MainDelvType = "3_1_"; }  // 1.多加報驗書
                                if (IsOver1000 == true && Is602 == true) { MainDelvType = "3_2_"; } // 2.多加報驗+電信
                                if (IsOver1000 == false && Is602 == true) { MainDelvType = "3_3_"; } // 3.多加電信自用
                            }
                            //int ItemID = SOListTemp.First().salesorderitem_itemid;
                            //Item ItemTemp = DB_Before.Item.Where(x => x.ID == ItemID).FirstOrDefault();
                            //if (ItemTemp.SellerID == 4) // 中蛋
                            //{
                            //    MainDelvType += "C_" + CorS;
                            //    Mail_DSuccess(SOListTemp, 3, "4~5", MainDelvType, "【海外直購】", BCCMail); // 發信SO資料List, 信件選擇, 到達天數 , 額外主旨附註
                            //}
                            //if (ItemTemp.SellerID == 2) // 美蛋
                            //{
                            //    MainDelvType += "A_" + CorS;
                            //    Mail_DSuccess(SOListTemp, 3, "7~9", MainDelvType, "【海外直購】", BCCMail);
                            //}
                            MainDelvType += CorS;
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.三角, "7~9", MainDelvType, "【海外直購】", BCCMail); // 發信SO資料List, 信件選擇, 到達天數 , 額外主旨附註
                            SuccessFlag = true;
                            break;
                        case (int)Item.tradestatus.國外直購: // 國外直配
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.國外直購).ToList();
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.國外直購, "3~5", "", "", BCCMail);
                            SuccessFlag = true;
                            break;
                        case 5:
                            //SOListTemp = SOList.Where(x => x.salesorder_delivtype == 5).ToList();
                            //Mail_DSuccess(SOList, 5, "7~9", "");
                            SuccessFlag = false;
                            break;
                        case (int)Item.tradestatus.海外切貨:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.海外切貨).ToList();
                            Is602 = false; IsOver1000 = false;
                            foreach (var sorow in SOListTemp)
                            {
                                Product ProductTemp = DB_Before.Product.Where(x => x.ID == sorow.salesorderitem_productid).FirstOrDefault();
                                if (ProductTemp.SourceTable == "productfromws")
                                {
                                    ProductFromWS ProductFromWSTemp = DB_Before.ProductFromWS.Where(x => x.ID == ProductTemp.FK).FirstOrDefault();
                                    if (ProductFromWSTemp.CCC != null && ProductFromWSTemp.CCC != "")
                                    {
                                        TwTradeTax TwTradeTaxTemp = DB_Before.TwTradeTax.Where(x => x.ID == ProductFromWSTemp.CCC).FirstOrDefault();
                                        if (TwTradeTaxTemp.ImportRule != null && TwTradeTaxTemp.ImportRule != "")
                                        {
                                            string[] ImportRuleTemp = TwTradeTaxTemp.ImportRule.Split(' ');
                                            foreach (string IRrow in ImportRuleTemp) if (IRrow == "602") Is602 = true;
                                        }
                                    }
                                }
                                // if 其中一個超過 1000m 則 true 並決定哪種寄信的類型，0、1多加報驗書、2多加報驗+電信、3多加電信自用
                                decimal cos = (sorow.salesorderitem_price + sorow.salesorderitem_shippingexpense - sorow.salesorderitem_tax) / 29; // 賣場價 + 國際物流處理費 
                                IsOver1000 = cos > 1000m ? true : false;
                                //if (IsOver1000 == false && Is602 == false) { MainDelvType = "6_0_A_" + CorS; } // 0.無多加
                                //if (IsOver1000 == true && Is602 == false) { MainDelvType = "6_1_A_" + CorS; }  // 1.多加報驗書
                                //if (IsOver1000 == true && Is602 == true) { MainDelvType = "6_2_A_" + CorS; } // 2.多加報驗+電信
                                //if (IsOver1000 == false && Is602 == true) { MainDelvType = "6_3_A_" + CorS; } // 3.多加電信自用
                                if (IsOver1000 == false && Is602 == false) { MainDelvType = "6_0_" + CorS; } // 0.無多加
                                if (IsOver1000 == true && Is602 == false) { MainDelvType = "6_1_" + CorS; }  // 1.多加報驗書
                                if (IsOver1000 == true && Is602 == true) { MainDelvType = "6_2_" + CorS; } // 2.多加報驗+電信
                                if (IsOver1000 == false && Is602 == true) { MainDelvType = "6_3_" + CorS; } // 3.多加電信自用
                            }
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.海外切貨, "7~9", MainDelvType, "【海外限定】", BCCMail); // 發信SO資料List, 信件選擇, 到達天數 , 額外主旨附註
                            SuccessFlag = true;
                            break;
                        case (int)Item.tradestatus.B2C直配:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.B2C直配).ToList();
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.B2C直配, "3~5", "", "", BCCMail);
                            SuccessFlag = true;
                            break;
                        case (int)Item.tradestatus.MKPL寄倉:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.MKPL寄倉).ToList();
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.MKPL寄倉, "2", "", "", BCCMail); // 發信SO資料List, 信件選擇, 到達天數 , 額外主旨附註
                            SuccessFlag = true;
                            break;
                        case (int)Item.tradestatus.B2c寄倉:
                            SOListTemp = SOList.Where(x => x.salesorder_delivtype == (int)Item.tradestatus.B2c寄倉).ToList();
                            Mail_DSuccess(SOListTemp, (int)Item.tradestatus.B2c寄倉, "2", "", "", BCCMail); // 發信SO資料List, 信件選擇, 到達天數 , 額外主旨附註
                            SuccessFlag = true;
                            break;
                    }
                }
                return SuccessFlag;
            }
            catch 
            {
                throw;
            }
        }
        /// <summary>
        /// 發信SO資料List, 信件選擇, 到達天數, 訂購成功通知信細節分類, 額外主旨附註, 密件收件人
        /// </summary>
        /// <param name="SOList"></param>
        /// <param name="MailChoose"></param>
        /// <param name="DelvDate"></param>
        /// <param name="MainDelvType"></param>
        /// <param name="TypeMessage"></param>
        /// <param name="BCCMail"></param>
        /// <returns></returns>
        public bool Mail_DSuccess(List<InsertSalesOrdersBySellerOutput> SOList, int MailChoose, string DelvDate, string MainDelvType, string TypeMessage, string BCCMail)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            List<string> FilePaths = new List<string>();
            object backupAction = null;
            string Mail_Choose = "";
            string Messageresult = "";
            string LBOList = "";
            try
            {
                switch (MailChoose)
                {
                    case (int)Item.tradestatus.切貨:
                    case (int)Item.tradestatus.間配:
                    case (int)Item.tradestatus.直配:
                    case (int)Item.tradestatus.國外直購:
                    case (int)Item.tradestatus.B2C直配:
                    case (int)Item.tradestatus.MKPL寄倉:
                    case (int)Item.tradestatus.B2c寄倉:
                        Mail_Choose = "Mail_DealSuccess_0";
                        break;
                    case (int)Item.tradestatus.三角:
                        Mail_Choose = "Mail_DealSuccess_3";
                        break;
                    case 5:
                        break;
                    case (int)Item.tradestatus.海外切貨:
                        Mail_Choose = "Mail_DealSuccess_6";
                        break;
                }

                List<string> SOListTemp = new List<string>();
                SOListTemp = SOList.Select(x => x.salesorder_code).Distinct().ToList();
                foreach (var row in SOListTemp) LBOList += row + ", ";
                if (LBOList.Length > 2) LBOList = LBOList.Substring(0, LBOList.Length - 2);
                int accountID = SOList.First().salesorder_accountid;
                int sex = db_before.Member.Where(x => x.AccID == accountID).Select(x => (int)x.Sex).FirstOrDefault();
                string appellation = sex == 1 ? "先生" : sex == 0 ? "女士" : "先生/女士";
                ViewBag.appellation = appellation;
                ViewBag.NewLinkTitle = NewLinkTitle;
                ViewBag.LBO = LBOList;
                ViewBag.SOList = SOList;
                ViewBag.DelvType = MailChoose;
                ViewBag.DelvDate = DelvDate;
                ViewBag.MainDelvType = MainDelvType;
                using (StringWriter sw = new StringWriter())
                {
                    backupAction = this.ControllerContext.RouteData.Values["action"];
                    this.ControllerContext.RouteData.Values["controller"] = "MailManage";
                    this.ControllerContext.RouteData.Values["action"] = Mail_Choose;
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, Mail_Choose);
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    Messageresult = sw.GetStringBuilder().ToString();
                    this.ControllerContext.RouteData.Values["controller"] = this.backupController;
                    this.ControllerContext.RouteData.Values["action"] = backupAction;
                }
                string Recipient = SOList.First().salesorder_email; // 收件人信箱
                string MailSubJect = SOList.First().salesorder_name + " " + appellation + " 您好:" + WebSiteData.SiteName + "通知-訂購成功通知信" + TypeMessage; // 信件主旨

                // 寄信資料寫入Server的Mail Log中做紀錄
                string MailSubJectTemp = "訂購成功通知信" + TypeMessage + "_" + MailChoose + "_" + MainDelvType + "_" + SOList.First().salesorder_email + "_" + SOList.First().salesorder_recvname;
                string path = Server.MapPath("~/Log/Mail");
                LogtoFileWrite(path, MailSubJectTemp, "");

                switch (MailChoose)
                {
                    case (int)Item.tradestatus.切貨:
                    case (int)Item.tradestatus.間配:
                    case (int)Item.tradestatus.直配:
                    case (int)Item.tradestatus.B2C直配:
                    case (int)Item.tradestatus.MKPL寄倉:
                    case (int)Item.tradestatus.B2c寄倉:
                        return send_email(Messageresult, MailSubJect, Recipient, BCCMail);
                        break;
                    case (int)Item.tradestatus.三角:
                        FilePaths = FilePathsCombine(MainDelvType);
                        return send_email(Messageresult, MailSubJect, Recipient, BCCMail, FilePaths);
                        break;
                    case (int)Item.tradestatus.國外直購:
                        return send_email(Messageresult, MailSubJect, Recipient, BCCMail, FilePaths);
                        break;
                    //case 5:
                    //    break;
                    case (int)Item.tradestatus.海外切貨:
                        FilePaths = FilePathsCombine(MainDelvType);
                        return send_email(Messageresult, MailSubJect, Recipient, BCCMail, FilePaths);
                        break;
                    default:
                        return send_email(Messageresult, MailSubJect, Recipient, BCCMail);
                        break;
                }
            }
            catch (Exception e)
            {
                this.ControllerContext.RouteData.Values["controller"] = this.backupController;
                this.ControllerContext.RouteData.Values["action"] = backupAction;
                throw e;
            }
        }
        /// <summary>
        /// 忘記密碼-重設密碼
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool Mail_ReSetEmail(Account account)
        {
            //string path = Server.MapPath("~/Log/Mail/");
            try
            {
                //var tt = System.Configuration.ConfigurationManager.AppSettings["Images"];
                ViewBag.NewLinkTitle = NewLinkTitle;
                string Messageresult = "";
                // 將URL與信件做連結
                ViewBag.url = ECDomain2 + "/MyNewegg/ReSetMailAddr?NewLinks=" + account.NewLinks + "&Email=" + account.Email2; // 重設新密碼的路徑
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "Mail_changeMail");

                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);

                    viewResult.View.Render(viewContext, sw);
                    Messageresult = sw.GetStringBuilder().ToString();
                }

                string Recipient = account.Email2;
                return send_email(Messageresult, " Newegg" + WebSiteData.SiteName + "通知-請立刻按[確認]－啟動您修改後的新會員帳號！", Recipient, "");
            }
            catch { return false; }
        }
        /// <summary>
        /// 信件訊息，信件主旨，收件人
        /// </summary>
        /// <param name="MailMessage"></param>
        /// <param name="mysubject"></param>
        /// <param name="Recipient"></param>
        /// <returns></returns>
        public bool send_email(string MailMessage, string mysubject, string Recipient)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg = MailBasicSettings(msg, MailMessage, mysubject, Recipient);
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                MySmtp.Send(msg);
                #region // 釋放物件，避免程式lock住檔案
                MySmtp.Dispose();
                MySmtp = null;
                #endregion // 釋放物件，避免程式lock住檔案
                return true;
            }
            catch (Exception e) { throw e; }
        }
        /// <summary>
        /// 信件訊息，信件主旨，收件人，密件收件人
        /// </summary>
        /// <param name="MailMessage"></param>
        /// <param name="mysubject"></param>
        /// <param name="Recipient"></param>
        /// <param name="RecipientBcc"></param>
        /// <returns></returns>
        public bool send_email(string MailMessage, string mysubject, string Recipient, string RecipientBcc)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg = MailBasicSettings(msg, MailMessage, mysubject, Recipient);
                if (RecipientBcc != "") msg.Bcc.Add(RecipientBcc); // 密件副本
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                MySmtp.Send(msg);
                #region // 釋放物件，避免程式lock住檔案
                MySmtp.Dispose();
                MySmtp = null;
                #endregion // 釋放物件，避免程式lock住檔案
                return true;
            }
            catch (Exception e) { throw e; }
        }
        /// <summary>
        /// 信件訊息，信件主旨，收件人，附件路徑
        /// </summary>
        /// <param name="MailMessage"></param>
        /// <param name="mysubject"></param>
        /// <param name="Recipient"></param>
        /// <param name="FilePaths"></param>
        /// <returns></returns>
        public bool send_email(string MailMessage, string mysubject, string Recipient, List<string> FilePaths)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg = MailBasicSettings(msg, MailMessage, mysubject, Recipient);
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                Attachment file = null; // 信件附檔
                foreach (string path in FilePaths)
                {
                    file = new Attachment(Server.MapPath(path));
                    msg.Attachments.Add(file);
                }
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                MySmtp.Send(msg);
                #region // 釋放物件，避免程式lock住檔案
                if (file != null)
                {
                    file.Dispose();
                    file = null;
                }
                MySmtp.Dispose();
                MySmtp = null;
                #endregion // 釋放物件，避免程式lock住檔案
                return true;
            }
            catch (Exception e) { throw e; }
        }
        /// <summary>
        /// 信件訊息，信件主旨，收件人，密件收件人，附件路徑
        /// </summary>
        /// <param name="MailMessage"></param>
        /// <param name="mysubject"></param>
        /// <param name="Recipient"></param>
        /// <param name="RecipientBcc"></param>
        /// <param name="FilePaths"></param>
        /// <returns></returns>
        public bool send_email(string MailMessage, string mysubject, string Recipient, string RecipientBcc, List<string> FilePaths)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg = MailBasicSettings(msg, MailMessage, mysubject, Recipient);
                if (RecipientBcc != "") msg.Bcc.Add(RecipientBcc); // 密件副本
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                Attachment file = null; // 信件附檔
                foreach (string path in FilePaths)
                {
                    file = new Attachment(Server.MapPath(path));
                    msg.Attachments.Add(file);
                }
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                MySmtp.Send(msg);
                #region // 釋放物件，避免程式lock住檔案
                if (file != null)
                {
                    file.Dispose();
                    file = null;
                }
                MySmtp.Dispose();
                MySmtp = null;
                #endregion // 釋放物件，避免程式lock住檔案
                return true;
            }
            catch (Exception e) { throw e; }
        }
        /// <summary>
        /// 信件基本設定
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="MailMessage"></param>
        /// <param name="mysubject"></param>
        /// <param name="Recipient"></param>
        /// <returns></returns>
        public MailMessage MailBasicSettings(MailMessage msg, string MailMessage, string mysubject, string Recipient)
        {
            // 收件者，以逗號分隔不同收件者
            msg.To.Add(Recipient);
            // msg.CC.Add("c@msn.com"); // 副本
            // 3個參數分別是發件人地址(可以隨便寫)，發件人姓名，編碼
            msg.From = new MailAddress("service@newegg.com.tw", WebSiteData.SiteName, System.Text.Encoding.UTF8);
            msg.Subject = mysubject; // 郵件主旨
            msg.SubjectEncoding = System.Text.Encoding.UTF8; // 郵件主旨編碼
            msg.Body = MailMessage; // 郵件內容
            msg.BodyEncoding = System.Text.Encoding.UTF8; // 郵件內容編碼
            msg.IsBodyHtml = true; // 是否為HTML郵件
            msg.Priority = MailPriority.Normal; // 郵件優先等級
            return msg;
        }
        /// <summary>
        /// 檔案路徑組合 : 訂購成功通知信細節分類
        /// </summary>
        /// <param name="MailDelvType"></param>
        /// <returns></returns>
        public List<string> FilePathsCombine(string MailDelvType)
        {
            List<string> filePaths = new List<string>();
            //string path = "~/MailDocuments/DealSuccessMail/AddFile", FirstPath = "", SecondPath = "", ThirdPath = "", Fourth = "";
            string path = "~/MailDocuments/DealSuccessMail/AddFileV2", FirstPath = "", SecondPath = "", ThirdPath = "", Fourth = "";
            switch (MailDelvType.Split('_')[0])
            {
                case "3":
                    FirstPath = "/3.海外直購商品";
                    break;
                case "6":
                    FirstPath = "/6.海外限定";
                    break;
            }
            switch (MailDelvType.Split('_')[1])
            {
                case "0":
                    SecondPath = "/0.無多加";
                    break;
                case "1":
                    SecondPath = "/1.多加報驗授權書";
                    break;
                case "2":
                    SecondPath = "/2.多加報驗授權與電信自用切結";
                    break;
                case "3":
                    SecondPath = "/3.多加電信自用切結";
                    break;
            }
            //switch (MailDelvType.Split('_')[2])
            //{
            //    case "A":
            //        ThirdPath = "/A.美國";
            //        break;
            //    case "C":
            //        ThirdPath = "/C.中國";
            //        break;
            //}
            //switch (MailDelvType.Split('_')[3])
            switch (MailDelvType.Split('_')[2])
            {
                case "C":
                    Fourth = "/C.公司/";
                    break;
                case "S":
                    Fourth = "/S.個人/";
                    break;
            }
            path += FirstPath + SecondPath + ThirdPath + Fourth;
            switch (MailDelvType)
            {
                #region // DelvType : 3
                // 3_0
                case "3_0_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    break;
                case "3_0_S":
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    break;
                // 3_1
                case "3_1_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    break;
                case "3_1_S":
                    filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    break;
                // 3_2
                case "3_2_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
                    break;
                case "3_2_S":
                    filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
                    break;
                // 3_3
                case "3_3_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
                    break;
                case "3_3_S":
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
                    break;
                #endregion // DelvType : 3
                #region // DelvType : 6
                // 6_0
                case "6_0_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    break;
                case "6_0_S":
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    break;
                // 6_1
                case "6_1_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    break;
                case "6_1_S":
                    filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    break;
                // 6_2
                case "6_2_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
                    break;
                case "6_2_S":
                    filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
                    break;
                // 6_3
                case "6_3_C":
                    filePaths.Add(path + "公司範本自用切結書.doc");
                    filePaths.Add(path + "個案委任書_公司.doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
                    break;
                case "6_3_S":
                    filePaths.Add(path + "個人自用切結書(空白表格).doc");
                    filePaths.Add(path + "個人自用切結書(範本).doc");
                    filePaths.Add(path + "個案委任書_個人(空白表格).doc");
                    filePaths.Add(path + "個案委任書_個人(範本).doc");
                    filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
                    break;
                #endregion // DelvType : 6
                default:
                    break;
            }
            return filePaths;
        }
        #region // 舊版 有分中國與美國
        //public List<string> FilePathsCombine(string MailDelvType) 
        //{
        //    List<string> filePaths = new List<string>();
        //    string path = "~/MailDocuments/DealSuccessMail/AddFile", FirstPath = "", SecondPath = "", ThirdPath = "", Fourth = "";
        //    switch (MailDelvType.Split('_')[0])
        //    { 
        //        case "3":
        //            FirstPath = "/3.海外直購商品";
        //            break;
        //        case "6":
        //            FirstPath = "/6.海外限定";
        //            break;
        //    }
        //    switch (MailDelvType.Split('_')[1])
        //    {
        //        case "0":
        //            SecondPath = "/0.無多加";
        //            break;
        //        case "1":
        //            SecondPath = "/1.多加報驗授權書";
        //            break;
        //        case "2":
        //            SecondPath = "/2.多加報驗授權與電信自用切結";
        //            break;
        //        case "3":
        //            SecondPath = "/3.多加電信自用切結";
        //            break;
        //    }
        //    switch (MailDelvType.Split('_')[2])
        //    {
        //        case "A":
        //            ThirdPath = "/A.美國";
        //            break;
        //        case "C":
        //            ThirdPath = "/C.中國";
        //            break;
        //    }
        //    switch (MailDelvType.Split('_')[3])
        //    {
        //        case "C":
        //            Fourth = "/C.公司/";
        //            break;
        //        case "S":
        //            Fourth = "/S.個人/";
        //            break;
        //    }
        //    path += FirstPath + SecondPath + ThirdPath + Fourth;
        //    switch (MailDelvType)
        //    {
        //        #region // DelvType : 3
        //        // 3_0
        //        case "3_0_C_C":
        //            filePaths.Add(path + "公司範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個案委任書(中國)_公司.doc");
        //            break;
        //        case "3_0_C_S":
        //            filePaths.Add(path + "個人範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個案委任書(中國)_個人.doc");
        //            break;
        //        case "3_0_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            break;
        //        case "3_0_A_S":
        //            filePaths.Add(path + "個人範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_個人.doc");
        //            break;
        //        // 3_1
        //        case "3_1_C_C":
        //            filePaths.Add(path + "公司範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
        //            filePaths.Add(path + "個案委任書(中國)_公司.doc");
        //            break;
        //        case "3_1_C_S":
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
        //            filePaths.Add(path + "個人範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個案委任書(中國)_個人.doc");
        //            break;
        //        case "3_1_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            break;
        //        case "3_1_A_S":
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
        //            filePaths.Add(path + "個人範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_個人.doc");
        //            break;
        //        // 3_2
        //        case "3_2_C_C":
        //            filePaths.Add(path + "公司範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
        //            filePaths.Add(path + "個案委任書(中國)_公司.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
        //            break;
        //        case "3_2_C_S":
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
        //            filePaths.Add(path + "個人範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個案委任書(中國)_個人.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
        //            break;
        //        case "3_2_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
        //            break;
        //        case "3_2_A_S":
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
        //            filePaths.Add(path + "個人範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_個人.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
        //            break;
        //        // 3_3
        //        case "3_3_C_C":
        //            filePaths.Add(path + "公司範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個案委任書(中國)_公司.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
        //            break;
        //        case "3_3_C_S":
        //            filePaths.Add(path + "個人範本自用切結書(中國).doc");
        //            filePaths.Add(path + "個案委任書(中國)_個人.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
        //            break;
        //        case "3_3_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
        //            break;
        //        case "3_3_A_S":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
        //            break;
        //        #endregion // DelvType : 3
        //        #region // DelvType : 6
        //        // 6_0
        //        case "6_0_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            break;
        //        case "6_0_A_S":
        //            filePaths.Add(path + "個人範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_個人.doc");
        //            break;
        //        // 6_1
        //        case "6_1_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            break;
        //        case "6_1_A_S":
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
        //            filePaths.Add(path + "個人範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_個人.doc");
        //            break;
        //        // 6_2
        //        case "6_2_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_公司.doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
        //            break;
        //        case "6_2_A_S":
        //            filePaths.Add(path + "個人標準局_代理報驗授權書_個人.doc");
        //            filePaths.Add(path + "個人範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_個人.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
        //            break;
        //        // 6_3
        //        case "6_3_A_C":
        //            filePaths.Add(path + "公司範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_公司.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_公司.doc");
        //            break;
        //        case "6_3_A_S":
        //            filePaths.Add(path + "個人範本自用切結書(美國).doc");
        //            filePaths.Add(path + "個案委任書(美國)_個人.doc");
        //            filePaths.Add(path + "電信射頻器具自用切結書_個人.doc");
        //            break;
        //        #endregion // DelvType : 6
        //        default:
        //            break;
        //    }
        //    return filePaths;
        //}
        #endregion // 舊版

        /// <summary>
        /// 寄信資料寫入Server的Mail Log中
        /// </summary>
        /// <param name="path"></param>
        /// <param name="MessageType"></param>
        /// <param name="writeStringendtoFile"></param>
        public void LogtoFileWrite(string path, string MessageType, string writeStringendtoFile)
        {

            string filename = path + string.Format("\\{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd-hh-mm}_" + MessageType + ".txt", DateTime.Now);
            FileInfo finfo = new FileInfo(filename);

            if (finfo.Directory.Exists == false) finfo.Directory.Create();

            System.IO.File.AppendAllText(filename, writeStringendtoFile, Encoding.Unicode);
        }
    }
}
