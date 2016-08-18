using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Mail;
using TWNewEgg.InternalSendMail.Model.SendMailModel;
using TWNewEgg.GetConfigData.Models;

namespace TWNewEgg.InternalSendMail.Service
{
    public class GeneratorView
    {
        TWNewEgg.DB.TWBackendDBContext dbafter = new DB.TWBackendDBContext();
        TWNewEgg.DB.TWSqlDBContext dbbefore = new DB.TWSqlDBContext();
        WebSiteListWebSiteData WebSiteData = new WebSiteListWebSiteData(0);
        private IMailSender sender = new MailSender();
        private static string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"].ToUpper();
        string SmtpServer = System.Configuration.ConfigurationManager.AppSettings[environment + "_SmtpServer"];
        string ImagesServer = System.Configuration.ConfigurationManager.AppSettings["Images"];
        string registerActivity = System.Configuration.ConfigurationManager.AppSettings["RegisterActivityStartDate"];
        string registerActivityEndDate = System.Configuration.ConfigurationManager.AppSettings["RegisterActivityEndDate"];
        string activitiesDeadline = System.Configuration.ConfigurationManager.AppSettings["ActivitiesDeadline"];

        public string Header { get; set; }
        public string Fooer { get; set; }
        public bool Bankinfo { get; set; }
        public string mysubject { get; set; }
        public string Recipient { get; set; }
        public string RecipientBcc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="T"></param>
        /// <param name="type"></param>
        /// <param name="Bankinfo"></param>
        /// <param name="CartList"></param>
        /// <returns></returns>
        public bool GeneraterViewPage(object T, string type, List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList, List<TWNewEgg.DB.TWSQLDB.Models.SalesOrder> SalesOrderList, string Reasontext, int? reset_reasonval)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Parsing.....GeneraterViewPage：" + type);
            try
            {
                FormulaMailList FormulaMailList = new FormulaMailList();
                string path = AppDomain.CurrentDomain.BaseDirectory + "Views\\MailTemplate\\" + type + ".cshtml";
                string Url = System.Configuration.ConfigurationManager.AppSettings["ImageServerUrl"];
                StreamReader readStream = new StreamReader(path);
                string template = readStream.ReadToEnd();
                returnRefundList returnRefundList = new returnRefundList();
                MailDataGroupList MailDataGroupList = new MailDataGroupList();
                SendPMWithGrossMargin sendPMWithGrossMargin = new SendPMWithGrossMargin();

                MailAddress mailAddress = null;
                CancelList CancelList = new CancelList();
                string Recipient = "";
                string RecipientBcc = "";
                string RecipientTest = FormulaMailList.GetMailList(null, null, "Test", "", 0);

                var result = "";

                if (type == "retgood")
                {
                    mysubject = WebSiteData.SiteName + "通知-申請退貨通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.RetgoodList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T;  
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨處理中);                                     
                    MailDataGroupList = GeneraterViewtoRetgood((List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T, CartList);                   
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨處理中);
                }
                else if (type == "retgood_CustomerRejection")
                {
                    mysubject = WebSiteData.SiteName + "通知-申請退貨通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.RetgoodList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T;
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨處理中);
                    MailDataGroupList = GeneraterViewtoRetgood((List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T, CartList);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨處理中);
                }
                else if (type == "retgood_DeliveryFailure")
                {
                    mysubject = WebSiteData.SiteName + "通知-申請退貨通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.RetgoodList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T;
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨處理中);
                    MailDataGroupList = GeneraterViewtoRetgood((List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T, CartList);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨處理中);
                }
                else if (type == "abnormalReturn")
                {
                    mysubject = WebSiteData.SiteName + "通知-退貨異常通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.RetgoodList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T;
                    MailDataGroupList = GeneraterViewtoRetgood(MailDataGroupList.RetgoodList, CartList);
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨異常);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    //sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨異常);
                }
                else if (type == "cancelReturn")
                {
                    mysubject = WebSiteData.SiteName + "通知-退貨取消通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.RetgoodList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T;
                    MailDataGroupList = GeneraterViewtoRetgood((List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T, CartList);
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨取消);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退貨取消);
                }
                else if (type == "completeReturn")
                {
                    mysubject = WebSiteData.SiteName + "通知-退貨完成通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.RetgoodList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T;
                    MailDataGroupList = GeneraterViewtoRetgood((List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T, CartList);
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.完成退貨);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.完成退貨);
                }
                else if (type == "CancelSO")
                {
                    mysubject = WebSiteData.SiteName + "通知-訂單取消通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.SalesOrderCancelList = (List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel>)T;
                    MailDataGroupList = GeneraterViewtoCancel((List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel>)T, CartList, SalesOrderList, Reasontext,reset_reasonval);
                    logger.Info("Parsing....." + type + ":" + MailDataGroupList.SalesOrderCancelList.FirstOrDefault().ID);
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, SalesOrderList.FirstOrDefault().Email, 0);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    logger.Info(result);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);
                    Recipient = "";
                }
                else if (type == "InnerCancelSO")
                {
                    mysubject = WebSiteData.SiteName + "通知-內部訂單取消通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);   
                    MailDataGroupList.SalesOrderCancelList = (List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel>)T;
                    MailDataGroupList = GeneraterViewtoCancel((List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel>)T, CartList, SalesOrderList, Reasontext,reset_reasonval);
                    logger.Info("Parsing....." + type + ":" + MailDataGroupList.SalesOrderCancelList.FirstOrDefault().ID);
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", 0); 
        
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    logger.Info(result);
                }
                else if (type == "finreturnRefund")
                {
                    mysubject = WebSiteData.SiteName + "通知-完成退款通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.Refund2cList = (List<TWNewEgg.DB.TWBACKENDDB.Models.refund2c>)T;
                    MailDataGroupList = GeneraterViewtorefund2c((List<TWNewEgg.DB.TWBACKENDDB.Models.refund2c>)T, CartList);
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.完成退款);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.完成退款);
                }
                else if (type == "abnormalRefund")
                {
                    mysubject = WebSiteData.SiteName + "通知-退款異常通知信！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.Refund2cList = (List<TWNewEgg.DB.TWBACKENDDB.Models.refund2c>)T;              
                    MailDataGroupList = GeneraterViewtorefund2c((List<TWNewEgg.DB.TWBACKENDDB.Models.refund2c>)T, CartList);
                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, CartList.FirstOrDefault().Email, (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退款異常);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                    Recipient = FormulaMailList.GetMailList(CartList, SalesOrderList, type, "", (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.status.退款異常);
                }
                else if (type == "inventoryComparison")
                {
                    mysubject = WebSiteData.SiteName + "-庫存比對明細-" + DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd") + "！";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.ItemInStockDetailWithWMS = (List<ItemInStockDetailWithWMS>)T;
                    MailDataGroupList = GeneraterViewtoinventory((List<ItemInStockDetailWithWMS>)T);
                    Recipient = FormulaMailList.GetMailList(null, null, type, null, 0);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    mailAddress = new MailAddress("logistics_tw@newegg.com.tw", "台灣新蛋倉庫", System.Text.Encoding.UTF8);
                    //sender.SendMail(result, Recipient, RecipientBcc, mysubject, mailAddress);
                }

                else if (type == "itemWarranty")
                {
                    mysubject = WebSiteData.SiteName + "通知-包含\"Warranty\"字眼-賣場清單!";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    MailDataGroupList.ItemWarranty = (List<TWNewEgg.DB.TWSQLDB.Models.ItemWarranty>)T;
                    MailDataGroupList = GeneraterViewtoItemWarranty((List<TWNewEgg.DB.TWSQLDB.Models.ItemWarranty>)T);
                    Recipient = FormulaMailList.GetMailList(null, null, type, null, 0);

                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    mailAddress = new MailAddress("logistics_tw@newegg.com.tw", null, System.Text.Encoding.UTF8);
                }

                else if (type == "DelstatusList")
                {
                    MailDataGroupList.ProcessList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Process>)T;
                    MailDataGroupList = GeneraterViewtoProcess((List<TWNewEgg.DB.TWBACKENDDB.Models.Process>)T);
                    Recipient = FormulaMailList.GetMailList(null, null, type, null, 0);
                    mysubject = environment + "環境" + MailDataGroupList.DIC["Specx"] + MailDataGroupList.DIC["Delivtype"] + "訂單:" + MailDataGroupList.DIC["CartID"] + "已配達 請協助開立發票,謝謝";
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, Recipient, RecipientBcc, mysubject, null);

                }
                else if (type == "LogisticsTriggercatch")
                {
                    MailDataGroupList.RetgoodList = (List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T;
                    MailDataGroupList = GeneraterViewtoRetgood((List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood>)T, CartList);
                    MailDataGroupList.DIC.Add("Reasontext", Reasontext);
                    Recipient = FormulaMailList.GetMailList(null, null, type, "", 0);
                    mysubject = "Newegg台灣新蛋通知-通知派送系統失敗！- RMA:" + MailDataGroupList.RetgoodList.FirstOrDefault().Code;
                    sender.SendMail("", RecipientTest, "", mysubject, null);
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                }
                else if (type == "TestDIC")
                {
                    Dictionary<string, string> DIC = new Dictionary<string, string>();
                    DIC.Add("Header", ImagesServer + "/Themes/2013/img/header.png");
                    DIC.Add("Fooer", ImagesServer + "/Themes/2013/img/footer.png");
                    MailDataGroupList.DIC = DIC;
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    mysubject = "TestDIC";
                    Recipient = FormulaMailList.GetMailList(null, null, "Test", "", 0);
                }
                else if (type == "Testmodel")
                {
                    Dictionary<string, string> DIC = new Dictionary<string, string>();
                    MailDataGroupList.Header = (ImagesServer + "/Themes/2013/img/header.png");
                    MailDataGroupList.Footer = (ImagesServer + "/Themes/2013/img/footer.png");
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    mysubject = "Testmodel";
                    Recipient = FormulaMailList.GetMailList(null, null, "Test", "", 0);
                }
                else if (type == "SendPMWithGrossMargin")
                {
                    string emailList = "";
                    SendPMWithGrossMargin getData = (SendPMWithGrossMargin)T;
                    List<string> PMEmailList = getData.PMEmail;

                    foreach (var list in PMEmailList)
                    {
                        if (PMEmailList.IndexOf(list) != (PMEmailList.Count()) - 1)
                        {
                            emailList += list + ",";
                        }
                        else
                        {
                            emailList += list;
                        }
                    }
                    MailDataGroupList.SendPMWithGrossMargin.CategoryID = getData.CategoryID;
                    MailDataGroupList.SendPMWithGrossMargin.ItemID = getData.ItemID;
                    MailDataGroupList.SendPMWithGrossMargin.Name = getData.Name;
                    mysubject = "通知-毛利率低於館別設定值";
                    result = RazorEngine.Razor.Parse(template, MailDataGroupList);
                    sender.SendMail(result, emailList, "", mysubject, null);
                }

                try
                {
                    if (MailDataGroupList.CustomerMail.SalesOrderIDStringList != null)
                    {
                        mysubject = mysubject + " " + MailDataGroupList.CustomerMail.SalesOrderIDStringList;
                    }

                    if (mysubject.Length > 100)
                    {
                        mysubject = mysubject.Substring(0, 95);
                    }
                    if (environment == "DEV" || environment == "GQC")
                    {
                        mysubject = mysubject + "(測試訂單)";
                    }
                }
                catch(Exception e)
                {
                    logger.Info(e.Message);
                    logger.Info(e.StackTrace);
                }
                sender.SendMail(result, Recipient, RecipientBcc, mysubject, mailAddress);
                logger.Info("GeneraterViewPage.....Sending END" + type);
            }
            catch(Exception e){
                logger.Info(e.Message);
                logger.Info(e.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 活動寄出信件
        /// </summary>
        /// <param name="T">序號、Email、活動起始日期、活動結束日期、活動兌換截止日期</param>
        /// <param name="viewName">Email頁面名稱</param>
        /// <returns></returns>
        public bool ActivityViewPage(object T, string viewName)
        {
            SmtpServer = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
            bool boolResult = false;
            try
            {
                //建立view的檔案入徑
                string path = AppDomain.CurrentDomain.BaseDirectory + "Views\\MailTemplate\\" + viewName + ".cshtml";
                //使用StreamReader 類別來讀取檔案
                StreamReader readStream = new StreamReader(path);
                // 讀取從目前位置到資料流末端的所有字元
                string pathTemplate = readStream.ReadToEnd();
                TWNewEgg.Models.ViewModels.Account.AccountVM RegisterVM = (TWNewEgg.Models.ViewModels.Account.AccountVM)T;

                MailDataGroupList mailDataGroupList = new MailDataGroupList();
                mailDataGroupList.Header = (ImagesServer + "/Themes/2013/img/header.png");
                mailDataGroupList.Footer = (ImagesServer + "/Themes/2013/img/footer.png");
                string result = "";

                switch (viewName.ToLower())
                {
                    case "activityomusic":
                        boolResult = ActivityOmusic(T, pathTemplate);
                        break;

                    case "activitymemberrecruit":
                        boolResult = ActivityMemberRecruit(T, pathTemplate);
                        break;

                    case "activitymemberrecruitclose":
                        boolResult = ActivityMemberRecruitClose(T, pathTemplate);
                        break;

                    case "activitytrueyoga20150708":
                        result = RazorEngine.Razor.Parse(pathTemplate, mailDataGroupList);
                        sender.SendMail(result, RegisterVM.Email, "", "【新蛋全球生活網】恭喜您獲得True Yoga雙人同行7天體驗課程", null);
                        break;

                    case "activitysignup":
                        result = RazorEngine.Razor.Parse(pathTemplate, mailDataGroupList);
                        sender.SendMail(result, RegisterVM.Email, "", "【新蛋全球生活網】會員邀請：分享最美好的禮物~折價券歸戶通知", null);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                return boolResult;
            }

            return boolResult;
        }

        /// <summary>
        /// Omusic活動寄出信件
        /// </summary>
        /// <param name="T">序號、Email、活動起始日期、活動結束日期、活動兌換截止日期</param>
        /// <param name="viewName">Email頁面入徑</param>

        public bool ActivityOmusic(object T, string pathTemplate)
        {
            try
            {
                var bodyResult = "";

                    ActivityDataInfo activityData = new ActivityDataInfo();
                    ActivityOmusic activityOmusic = new ActivityOmusic();
                    activityData = (ActivityDataInfo)T;

                    activityOmusic.Email = activityData.Email;
                    activityOmusic.ActivityNO = activityData.ActivityNo;
                    activityOmusic.RegisterActivityStartDate = ActivityDate((DateTime)activityData.StartDate);
                activityOmusic.RegisterActivityEndDate = ActivityDate(((DateTime)activityData.EndDate).AddDays(-1));
                activityOmusic.ActivitiesDeadline = ActivityDate(((DateTime)activityData.Deadline).AddDays(-1));

                    mysubject = "newegg x Omusic 活動說明信";
                    bodyResult = RazorEngine.Razor.Parse(pathTemplate, activityOmusic);
                    return sender.SendMail(bodyResult, activityOmusic.Email, "", mysubject);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 4月份行銷部會員招募活動寄出信件(被推薦人)
        /// </summary>
        /// <param name="T">推薦人、被推薦人、被推薦人、被推薦人   *最少兩筆，最多四筆</param>
        /// <param name="viewName">Email頁面入徑</param>
        public bool ActivityMemberRecruit(object T, string pathTemplate)
        {
            try
            {
                bool boolResult = true;
                string bodyResult = "";
                string resultMessage = string.Empty;
                List<string> EmailList = (List<string>)T;
                mysubject = "您的好友揪您來新蛋拿好康";

                int EmailCount = 0;

                foreach (string SendEmailList in EmailList)
                {
                    //被推薦人
                    if (EmailCount != 0 && !string.IsNullOrEmpty(SendEmailList))//
                    {
                        List<string> receiveEmailList = new List<string>();
                        receiveEmailList.Add(EmailList[0] + "," + SendEmailList);
                        bodyResult = RazorEngine.Razor.Parse(pathTemplate, receiveEmailList);

                        if (sender.SendMail(bodyResult, SendEmailList, "", mysubject))
                        {
                            resultMessage += "[被推薦人] : " + SendEmailList + " 信件成功寄發; ";
                        }
                        else
                        {
                            resultMessage += "[被推薦人] : " + SendEmailList + " 信件寄發失敗; ";
                            boolResult = false;
                        }
            }
                    EmailCount++;
                }
                return boolResult;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 4活動結束派發折價券與通知信(推薦人)
        /// </summary>
        /// <param name="T">推薦人、被推薦人、被推薦人、被推薦人   *最少兩筆，最多四筆</param>
        /// <param name="viewName">Email頁面入徑</param>
        public bool ActivityMemberRecruitClose(object T, string pathTemplate)
        {
            TWNewEgg.DB.TWSqlDBContext db_front = new DB.TWSqlDBContext();
            try
            {
                bool boolResult = true;
                string bodyResult = "";
                string resultMessage = string.Empty;
                List<string> EmailList = (List<string>)T;
                mysubject = "新蛋揪團拿好康推薦結果出爐！";

                int EmailCount = 0;
                // 推薦人Email
                string oldAccount = EmailList[0].Split(',')[0];
                foreach (string SendEmailList in EmailList)
                {
                    List<string> finalInfo = SendEmailList.Split(',').ToList();
                    // 被推薦人Email
                    string newAccount = finalInfo[0];
                    // 推薦人
                    if (EmailCount == 0 && !string.IsNullOrEmpty(newAccount))
                    {
                        // 驗證是否需要寄信給推薦人
                        if (finalInfo[1].ToLower() == "true")
                        {
                            //List<string> receiveEmailList = new List<string>();
                            //receiveEmailList.Add(EmailList[0] + "," + "1");
                            bodyResult = RazorEngine.Razor.Parse(pathTemplate, EmailList);

                            if (sender.SendMail(bodyResult, oldAccount, "", mysubject))
                            {
                                resultMessage += "[推薦人推薦結果] : " + oldAccount + " 信件成功寄發; ";
                            }
                            else
                            {
                                resultMessage += "[推薦人推薦結果] : " + oldAccount + " 信件寄發失敗; ";
                                boolResult = false;
                            }
                        }
                    }
                    else
                    {
                        // 當被推薦人已下訂單且尚未收到信件時執行
                        if (finalInfo[1].ToLower() == "false" && !string.IsNullOrEmpty(finalInfo[2]))
                        {
                            // 被推薦人
                            // 建立view的檔案入徑
                            string path = AppDomain.CurrentDomain.BaseDirectory + "Views\\MailTemplate\\ActivityMembersRecruitClose.cshtml";
                            // 使用StreamReader 類別來讀取檔案
                            StreamReader readStream = new StreamReader(path);
                            // 讀取從目前位置到資料流末端的所有字元
                            string pathTemplates = readStream.ReadToEnd();

                            bodyResult = RazorEngine.Razor.Parse(pathTemplates, "");

                            if (sender.SendMail(bodyResult, newAccount, "", mysubject))
                            {
                                resultMessage += "[被推薦人推薦結果] : " + newAccount + " 信件成功寄發; ";
                                TWNewEgg.DB.TWSQLDB.Models.AccountJoinGroup updateAccountJoin = db_front.AccountJoinGroup.Where(x => x.Old_Account == oldAccount
                                    && x.New_Account == newAccount).FirstOrDefault();
                                // 信件寄發成功
                                updateAccountJoin.ReceivedMail = true;
                                // 資料庫更新
                                try
                                {
                                    db_front.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    log4net.ILog logger;
                                    logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
                                    logger.Error("四月份被推薦人寄信與否無法正確儲存 [ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                                    continue;
                                }
                            }
                            else
                            {
                                resultMessage += "[被推薦人推薦結果] : " + newAccount + " 信件寄發失敗; ";
                                boolResult = false;
                            }
                        }
                    }
                    EmailCount++;
                }
                return boolResult;
            }
            catch (Exception e)
            {
            return false;
        }
        }

        private string view(Func<object, string, string> ActivityMemberRecruit)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 將日期轉成string傳回
        /// </summary>
        /// <param name="activityDate">活動日期</param>
        /// <returns>傳回日期string</returns>
        public string ActivityDate(DateTime activityDate)
        {
            string strActivityDate = string.Empty;
            strActivityDate = activityDate.Year.ToString() + " 年 " + activityDate.Month.ToString() + " 月 " + activityDate.Day.ToString() + " 日";
            return strActivityDate;
        }

        public void GeneraterViewtoDailyReport(string type, List<string> filename)
        {
            FormulaMailList FormulaMailList = new FormulaMailList();
            for (int k = 0; k < filename.Count(); k++)
            {
                filename[k] = "ToExcel\\" + filename[k] + ".xls";

            }
            if (type == "DP")
            {
                string Recipient = FormulaMailList.GetMailList(null, null, "DailyList", "", 0);
                //AttachmentCollection AttachmentCollection = new System.Net.Mail.AttachmentCollection();
                // SendMail("", Recipient, RecipientBcc, mysubject, "ToExcel\\" + filename + ".xls");
                mysubject = "每日商品類別報表";
                sender.SendMail("", Recipient, RecipientBcc, mysubject, null, filename);

            }
            if (type == "DT")
            {
                string Recipient = FormulaMailList.GetMailList(null, null, "DailyList", "", 1);
                mysubject = "每日戰報";
                sender.SendMail("", Recipient, RecipientBcc, mysubject, null, filename);
            }
            if (type == "DCP")
            {
                string Recipient = FormulaMailList.GetMailList(null, null, "DailyList", "", 2);
                mysubject = "每日暢銷品排行榜";
                sender.SendMail("", Recipient, RecipientBcc, mysubject, null, filename);
            }
            if (type == "DTP")
            {
                string Recipient = FormulaMailList.GetMailList(null, null, "DailyList", "", 3);
                mysubject = "每日整點速報";
                sender.SendMail("", Recipient, RecipientBcc, mysubject, null, filename);
            }
            if (type == "DM")
            {
                string Recipient = FormulaMailList.GetMailList(null, null, "DailyList", "", 4);

                mysubject = "每日廠商業績報表";
                sender.SendMail("", Recipient, RecipientBcc, mysubject, null, filename);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RetgoodList"></param>
        /// <param name="CartList"></param>
        /// <returns></returns>
        public MailDataGroupList GeneraterViewtoRetgood(List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood> RetgoodList, List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList)
        {
            MailDataGroupList MailDataGroupList = new MailDataGroupList();
            List<string> CartIDList = CartList.Select(x=>x.ID).ToList();
            Dictionary<string, string> DIC= new Dictionary<string, string>();
            //商品名稱
            //List<TWNewEgg.DB.TWBACKENDDB.Models.Process> ProcessList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).ToList();
            //List<string> ProcessIDList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).Select(x => x.ID).ToList();//MailDataGroupList.MailDataGroupList.FirstOrDefault().ProcessID;
            List<TWNewEgg.DB.TWBACKENDDB.Models.Process> ProcessList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).ToList();
            ProcessList = ProcessList.Where(x => x.Title != "服務費" && x.Title != "國際運費").ToList();
            List<int> ProductIDList = ProcessList.Select(x => (int)(x.ProductID ?? 0)).ToList();
            //List<TWNewEgg.DB.TWSQLDB.Models.Item> ItemList = dbbefore.Item.Where(x => itenIDList.Contains(x.ID)).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.Product> ProductList = dbbefore.Product.Where(x => ProductIDList.Contains(x.ID)).ToList();
            MailDataGroupList.CartList = CartList;
            string itemname = "";
            string CartIDStringList = "";
            string SalesOrderIDStringList = "";
            string Itemlist = "";

            if (RetgoodList == null && RetgoodList.Count == 0)
            {
                MailDataGroupList.RetgoodList = dbafter.Retgood.Where(x => CartIDList.Contains(x.CartID)).ToList();
            }
            else {
                MailDataGroupList.RetgoodList = RetgoodList;
            }

            foreach (var so in CartList)
            {
                if (CartIDStringList != "")
                {
                    CartIDStringList = CartIDStringList + "，";
                }
                if (SalesOrderIDStringList != "")
                {
                    SalesOrderIDStringList = SalesOrderIDStringList + "，";
                }

                CartIDStringList = CartIDStringList + so.ID;
                
                switch (so.ShipType)
                {
                    case 0:
                        SalesOrderIDStringList += "00-切貨-";
                        break;
                    case 1:
                        SalesOrderIDStringList += "01-間配-";
                        break;
                    case 2:
                        SalesOrderIDStringList += "02-直配-";
                        break;
                    case 3:
                        SalesOrderIDStringList += "03-三角-";
                        break;
                    case 4:
                        SalesOrderIDStringList += "04-借賣網-";
                        break;
                    case 5:
                        SalesOrderIDStringList += "05-自貿區-";
                        break;
                    case 6:
                        SalesOrderIDStringList += "06-海外切貨-";
                        break;
                    case 7:
                        SalesOrderIDStringList += "07-B2C直配-";
                        break;
                    case 8:
                        SalesOrderIDStringList += "08-MKPL寄倉-";
                        break;
                    case 9:
                        SalesOrderIDStringList += "09-B2C寄倉-";
                        break;
                    default:
                        SalesOrderIDStringList += "其他未定義-";
                        break;
                }
                SalesOrderIDStringList = SalesOrderIDStringList + so.ID + "";
            }

            foreach (var Itemtemp in ProcessList)
            {
                if (itemname != "")
                {
                    itemname = itemname + ",";
                }
                itemname = itemname + Itemtemp.Title;
            }

            //if (itemname == null || itemname == "")
            //{
            //    foreach (var Producttemp in ProductList)
            //    {
            //        if (itemname != "")
            //        {
            //            itemname = itemname + ",";
            //        }
            //        itemname = itemname + Producttemp.NameTW;
            //    }
            //}

             
            for (int i = 1; i < MailDataGroupList.RetgoodList.Count; i++)
            {
                int productid = (MailDataGroupList.RetgoodList[i].ProductID ?? 0);
                Itemlist += ProcessList.Where(x => x.ProductID == productid).FirstOrDefault().Title;
                switch (MailDataGroupList.RetgoodList[i].RetgoodType)
                {
                    case (int)TWNewEgg.DB.TWSQLDB.Models.Item.Itemliststatus.配件: Itemlist += "(配件)";
                        break;
                    case (int)TWNewEgg.DB.TWSQLDB.Models.Item.Itemliststatus.贈品: Itemlist += "(贈品)";
                        break;
                }
                Itemlist += "*" + MailDataGroupList.RetgoodList[i].Qty + "&nbsp;&nbsp;&nbsp;";
            }

            string retgood_toaddr = MailDataGroupList.RetgoodList.FirstOrDefault().FrmZipcode + MailDataGroupList.RetgoodList.FirstOrDefault().FrmLocation + MailDataGroupList.RetgoodList.FirstOrDefault().FrmADDR; // 地址
            string retgood_salesorderCODE = MailDataGroupList.RetgoodList.Select(x => x.CartID).FirstOrDefault();
            string retgood_causenote = "";

            if (MailDataGroupList.RetgoodList.FirstOrDefault().Cause != null) 
            {
                retgood_causenote = ((TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason)MailDataGroupList.RetgoodList.FirstOrDefault().Cause).ToString();
            }

            #region 原本的寫法20160325
            switch (MailDataGroupList.RetgoodList.FirstOrDefault().Cause)
            {
            case (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.規格不合: 
                retgood_causenote = TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.規格不合.ToString();
                break;
            case (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.與想像不符: 
                retgood_causenote = TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.與想像不符.ToString();       
                break;
            case (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.其他原因: 
                retgood_causenote = TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.其他原因.ToString();
                break;
            case (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.客戶拒收:
                retgood_causenote = TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.客戶拒收.ToString();
                break;
            case (int)TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.配達失敗:
                retgood_causenote = TWNewEgg.DB.TWBACKENDDB.Models.Retgood.reason.配達失敗.ToString();
                break;
            }
            #endregion

            MailDataGroupList.CustomerMail.SalesOrderCodes = CartIDStringList;
            MailDataGroupList.CustomerMail.SalesOrderIDStringList = SalesOrderIDStringList;
            MailDataGroupList.CustomerMail.Itemname = System.Web.HttpUtility.HtmlEncode(itemname);
            MailDataGroupList.CustomerMail.Itemlist = System.Web.HttpUtility.HtmlEncode(Itemlist);
            MailDataGroupList.CustomerMail.retgood_toaddr = retgood_toaddr;
            MailDataGroupList.CustomerMail.retgood_salesorderCODE = retgood_salesorderCODE;
            MailDataGroupList.CustomerMail.retgood_causenote = retgood_causenote;

            DIC.Add("SalesOrderCodes", CartIDStringList);
            DIC.Add("SalesOrderIDStringList", SalesOrderIDStringList);          
            DIC.Add("Itemname", System.Web.HttpUtility.HtmlEncode(itemname));
            DIC.Add("Itemlist", System.Web.HttpUtility.HtmlEncode(Itemlist));
            DIC.Add("retgood_toaddr", retgood_toaddr);
            DIC.Add("retgood_salesorderCODE", retgood_salesorderCODE);
            DIC.Add("retgood_causenote", System.Web.HttpUtility.HtmlEncode(retgood_causenote));
            DIC.Add("Header", ImagesServer + "/Themes/2013/img/header.png");
            DIC.Add("Fooer", ImagesServer + "/Themes/2013/img/footer.png");
            MailDataGroupList.DIC = DIC;
            return MailDataGroupList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SalesOrderCancelList"></param>
        /// <param name="CartList"></param>
        /// <returns></returns>
        public MailDataGroupList GeneraterViewtoCancel(List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel> SalesOrderCancelList, List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList, List<TWNewEgg.DB.TWSQLDB.Models.SalesOrder> SalesOrderList, string Reasontext,int? reset_reasonval)
        {
            MailDataGroupList MailDataGroupList = new MailDataGroupList();
            List<string> CartIDList = CartList.Select(x => x.ID).ToList();
            Dictionary<string, string> DIC = new Dictionary<string, string>();
            //商品名稱
            //List<TWNewEgg.DB.TWBACKENDDB.Models.Process> ProcessList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).ToList();
            //List<string> ProcessIDList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).Select(x => x.ID).ToList();//MailDataGroupList.MailDataGroupList.FirstOrDefault().ProcessID;
            //List<int> itenIDList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).Select(x => (int)(x.StoreID ?? 0)).ToList();
            //List<int> ProductIDList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).Select(x => (int)(x.ProductID ?? 0)).ToList();
            //List<TWNewEgg.DB.TWSQLDB.Models.Item> ItemList = dbbefore.Item.Where(x => itenIDList.Contains(x.ID)).ToList();
            //List<TWNewEgg.DB.TWSQLDB.Models.Product> ProductList = dbbefore.Product.Where(x => ProductIDList.Contains(x.ID)).ToList();
            MailDataGroupList.CartList = CartList;
            MailDataGroupList.SalesOrderList = SalesOrderList;
            string SalesOrderIDStringList = "";
            string CartIDStringList = "";

            if (SalesOrderCancelList == null && SalesOrderCancelList.Count == 0)
            {
                MailDataGroupList.SalesOrderCancelList = dbbefore.SalesOrderCancel.Where(x => CartIDList.Contains(x.SalesorderCode)).ToList();
            }
            else
            {
                MailDataGroupList.SalesOrderCancelList = SalesOrderCancelList;
            }

            foreach (var so in SalesOrderList)
            {
                if (CartIDStringList != "")
                {
                    CartIDStringList = CartIDStringList + "，";
                }
                if (SalesOrderIDStringList != "")
                {
                    SalesOrderIDStringList = SalesOrderIDStringList + "，";
                }

                CartIDStringList = CartIDStringList + so.Code;
                
                switch (so.DelivType)
                {
                    case 0:
                        SalesOrderIDStringList += "00-切貨-";
                        break;
                    case 1:
                        SalesOrderIDStringList += "01-間配-";
                        break;
                    case 2:
                        SalesOrderIDStringList += "02-直配-";
                        break;
                    case 3:
                        SalesOrderIDStringList += "03-三角-";
                        break;
                    case 4:
                        SalesOrderIDStringList += "04-借賣網-";
                        break;
                    case 5:
                        SalesOrderIDStringList += "05-自貿區-";
                        break;
                    case 6:
                        SalesOrderIDStringList += "06-海外切貨-";
                        break;
                    case 7:
                        SalesOrderIDStringList += "07-B2C直配-";
                        break;
                    case 8:
                        SalesOrderIDStringList += "08-MKPL寄倉-";
                        break;
                    case 9:
                        SalesOrderIDStringList += "09-B2C寄倉-";
                        break;
                    default:
                        SalesOrderIDStringList += "其他未定義-";
                        break;
                }
                SalesOrderIDStringList = SalesOrderIDStringList + so.Code + "";
            }
           
            string Salesorder_paytype = "";
            string BankCode = SalesOrderList.FirstOrDefault().CardBank;
            string bankname = "";
            if (BankCode != null)
            {
                bankname = (from p in dbbefore.Bank where p.Code == BankCode select p.Name).FirstOrDefault();
            }  
            Salesorder_paytype += bankname + " ";
  
            switch (SalesOrderList.FirstOrDefault().PayType)
            {
                case 1:
                    Salesorder_paytype = "信用卡付款 (一次付清)"; // 信用卡
                    break;
                case 3:
                    Salesorder_paytype = "信用卡分期付款 3期0利率";
                    break;
                case 6:
                    Salesorder_paytype = "信用卡分期付款 6期0利率";
                    break;
                case 10:
                    Salesorder_paytype = "信用卡分期付款 10期0利率";
                    break;
                case 18:
                    Salesorder_paytype = "信用卡分期付款 18期0利率";
                    break;
                case 24:
                    Salesorder_paytype = "信用卡分期付款 24期0利率";
                    break;
                case 112:
                    Salesorder_paytype = "信用卡分期付款 12期";
                    break;
                case 124:
                    Salesorder_paytype = "信用卡分期付款 24期";
                    break;
                case 30:
                    Salesorder_paytype = "WebATM";
                    break;
                case 31:
                    Salesorder_paytype = "貨到付款"; // 新竹貨運
                    break;
                case 32:
                    Salesorder_paytype = "超商付款";
                    break;
                case 501:
                    Salesorder_paytype = "歐付寶";
                    break;
                default:
                    Salesorder_paytype = "";
                    break;
            }

            MailDataGroupList.CustomerMail.SalesOrderCodes = CartIDStringList;
            MailDataGroupList.CustomerMail.SalesOrderIDStringList = SalesOrderIDStringList;
            MailDataGroupList.CustomerMail.CancelDate = DateTime.Now.ToShortDateString();
            MailDataGroupList.CustomerMail.Paytype = Salesorder_paytype;
            MailDataGroupList.CustomerMail.Reasontext = Reasontext;

            DIC.Add("SalesOrderCodes", CartIDStringList);
            DIC.Add("SalesOrderIDStringList", SalesOrderIDStringList);
            DIC.Add("CancelDate", DateTime.Now.ToShortDateString());
            DIC.Add("Paytype", Salesorder_paytype);
            string ReasonDropList = "";
            if (reset_reasonval != 0)
            {
                ReasonDropList = ((TWNewEgg.DB.TWBACKENDDB.Models.refund2c.reason)reset_reasonval).ToString();
                if (ReasonDropList != null)
                {
                    DIC.Add("ReasonDropList", System.Web.HttpUtility.HtmlEncode(ReasonDropList));
                }
            }
            DIC.Add("Reasontext", System.Web.HttpUtility.HtmlEncode(Reasontext));
            DIC.Add("Header", ImagesServer + "/Themes/2013/img/header.png");
            DIC.Add("Fooer", ImagesServer + "/Themes/2013/img/footer.png");
            MailDataGroupList.DIC = DIC;
            return MailDataGroupList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnRefundList"></param>
        /// <returns></returns>
        public MailDataGroupList GeneraterViewtorefund2c(List<TWNewEgg.DB.TWBACKENDDB.Models.refund2c> refund2cList, List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList)
        {
            MailDataGroupList MailDataGroupList = new MailDataGroupList();
            List<string> CartIDList = CartList.Select(x => x.ID).ToList();
            Dictionary<string, string> DIC = new Dictionary<string, string>();
            //商品名稱
            //List<TWNewEgg.DB.TWBACKENDDB.Models.Process> ProcessList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).ToList();
            //List<string> ProcessIDList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).Select(x => x.ID).ToList();//MailDataGroupList.MailDataGroupList.FirstOrDefault().ProcessID;
            List<int> itenIDList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).Select(x => (int)(x.StoreID ?? 0)).ToList();
            //List<int> ProductIDList = dbafter.Process.Where(x => CartIDList.Contains(x.CartID)).Select(x => (int)(x.ProductID ?? 0)).ToList();
            List<TWNewEgg.DB.TWSQLDB.Models.Item> ItemList = dbbefore.Item.Where(x => itenIDList.Contains(x.ID)).ToList();
            //List<TWNewEgg.DB.TWSQLDB.Models.Product> ProductList = dbbefore.Product.Where(x => ProductIDList.Contains(x.ID)).ToList();
            MailDataGroupList.CartList = CartList;
            string CartIDStringList = "";
            string SalesOrderIDStringList = "";
            string itemname = "";

            if (refund2cList == null && refund2cList.Count == 0)
            {
                MailDataGroupList.Refund2cList = dbafter.refund2c.Where(x => CartIDList.Contains(x.CartID)).ToList();
            }
            else
            {
                MailDataGroupList.Refund2cList = refund2cList;
            }

            foreach (var so in CartList)
            {
                if (CartIDStringList != "")
                {
                    CartIDStringList = CartIDStringList + "，";
                }
                if (SalesOrderIDStringList != "")
                {
                    SalesOrderIDStringList = SalesOrderIDStringList + "，";
                }

                CartIDStringList = CartIDStringList + so.ID;
                
                switch (so.ShipType)
                {
                    case 0:
                        SalesOrderIDStringList += "00-切貨-";
                        break;
                    case 1:
                        SalesOrderIDStringList += "01-間配-";
                        break;
                    case 2:
                        SalesOrderIDStringList += "02-直配-";
                        break;
                    case 3:
                        SalesOrderIDStringList += "03-三角-";
                        break;
                    case 4:
                        SalesOrderIDStringList += "04-借賣網-";
                        break;
                    case 5:
                        SalesOrderIDStringList += "05-自貿區-";
                        break;
                    case 6:
                        SalesOrderIDStringList += "06-海外切貨-";
                        break;
                    case 7:
                        SalesOrderIDStringList += "07-B2C直配-";
                        break;
                    case 8:
                        SalesOrderIDStringList += "08-MKPL寄倉-";
                        break;
                    case 9:
                        SalesOrderIDStringList += "09-B2C寄倉-";
                        break;
                    default:
                        SalesOrderIDStringList += "其他未定義-";
                        break;
                }
                SalesOrderIDStringList = SalesOrderIDStringList + so.ID + "";
            }

            foreach (var Itemtemp in ItemList)
            {
                if (itemname != "")
                {
                    itemname = itemname + ",";
                }
                itemname = itemname + Itemtemp.Name;
            }

            List<TWNewEgg.DB.TWSQLDB.Models.Bank> Bank = dbbefore.Bank.ToList();

            string Salesorder_paytype = "";
            string BankCode = CartList.FirstOrDefault().CardBank;
            string bankname = "";
            if (BankCode != null)
            {
                bankname = (from p in dbbefore.Bank where p.Code == BankCode select p.Name).FirstOrDefault();
            }
            Salesorder_paytype += bankname + " ";

            switch ((int)CartList.FirstOrDefault().PayType)
             {
                 case 1:
                     Salesorder_paytype = "信用卡付款 (一次付清)"; // 信用卡
                     break;
                 case 3:
                     Salesorder_paytype = "信用卡分期付款 3期0利率";
                     break;
                 case 6:
                     Salesorder_paytype = "信用卡分期付款 6期0利率";
                     break;
                 case 10:
                     Salesorder_paytype = "信用卡分期付款 10期0利率";
                     break;
                 case 18:
                     Salesorder_paytype = "信用卡分期付款 18期0利率";
                     break;
                 case 24:
                     Salesorder_paytype = "信用卡分期付款 24期0利率";
                     break;
                 case 112:
                     Salesorder_paytype = "信用卡分期付款 12期";
                     break;
                 case 124:
                     Salesorder_paytype = "信用卡分期付款 24期";
                     break;
                 case 30:
                     Salesorder_paytype = "WebATM";
                     break;
                 case 31:
                     Salesorder_paytype = "貨到付款"; // 新竹貨運
                     break;
                 case 32:
                     Salesorder_paytype = "超商付款";
                     break;
                 case 501:
                     Salesorder_paytype = "歐付寶";
                     break;
                 default:
                     Salesorder_paytype = "";
                     break;
             }

            MailDataGroupList.CustomerMail.SalesOrderCodes = CartIDStringList;
            MailDataGroupList.CustomerMail.SalesOrderIDStringList = SalesOrderIDStringList;
            MailDataGroupList.CustomerMail.Itemname = System.Web.HttpUtility.HtmlEncode(itemname);
            MailDataGroupList.CustomerMail.Paytype = Salesorder_paytype;

            DIC.Add("SalesOrderCodes", CartIDStringList);
            DIC.Add("SalesOrderIDStringList", SalesOrderIDStringList);
            DIC.Add("Itemname", System.Web.HttpUtility.HtmlEncode(itemname));
            DIC.Add("PayType", Salesorder_paytype);
            DIC.Add("Header", ImagesServer + "/Themes/2013/img/header.png");
            DIC.Add("Fooer", ImagesServer + "/Themes/2013/img/footer.png");
            MailDataGroupList.DIC = DIC;
            return MailDataGroupList;      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ItemInStockDetailWithWMSList"></param>
        /// <returns></returns>
        public MailDataGroupList GeneraterViewtoinventory(List<ItemInStockDetailWithWMS> ItemInStockDetailWithWMSList)
                {
            MailDataGroupList MailDataGroupList = new MailDataGroupList();
            Dictionary<string, string> DIC = new Dictionary<string, string>();
            MailDataGroupList.ItemInStockDetailWithWMS = ItemInStockDetailWithWMSList;
        
            DIC.Add("Header", ImagesServer + "/Themes/2013/img/header.png");
            DIC.Add("Fooer", ImagesServer + "/Themes/2013/img/footer.png");
            MailDataGroupList.DIC = DIC;
            return MailDataGroupList;
        }
   
        public MailDataGroupList GeneraterViewtoItemWarranty(List<TWNewEgg.DB.TWSQLDB.Models.ItemWarranty> ItemWarranty)
    {
            MailDataGroupList MailDataGroupList = new MailDataGroupList();
            Dictionary<string, string> DIC = new Dictionary<string, string>();
            MailDataGroupList.ItemWarranty = ItemWarranty;

            DIC.Add("Header", ImagesServer + "/Themes/2013/img/header.png");
            DIC.Add("Fooer", ImagesServer + "/Themes/2013/img/footer.png");
            MailDataGroupList.DIC = DIC;
            return MailDataGroupList;
    }
        public MailDataGroupList GeneraterViewtoProcess(List<TWNewEgg.DB.TWBACKENDDB.Models.Process> Process)
        {
            decimal extend = 0;
            decimal couponpices = 0;
            decimal installmate = 0;
            decimal service = 0;
            string specx = "";
            int Delivtype = 0;
            string CartID = "";
            TWNewEgg.DB.TWBackendDBContext Backdb = new DB.TWBackendDBContext();
            foreach (var process in Process)
            {
                Delivtype = Backdb.Cart.Where(x => x.ID == process.CartID).Select(x => (int)x.ShipType).FirstOrDefault();
                extend = (decimal)process.ShippingExpense + (decimal)process.ServiceExpense + (decimal)process.InstallmentFee + (decimal)process.Tax - (decimal)process.Pricecoupon;

                if (extend < 0)
    {
                    service = 0;
                    extend = 0;
                }

                //sosum = 0;
                couponpices += extend;

                installmate += process.InstallmentFee;


    }
            if (extend == 0)
    {
                specx = "********注意為零元不用開發票*************";
            }
            MailDataGroupList MailDataGroupList = new MailDataGroupList();
            Dictionary<string, string> DIC = new Dictionary<string, string>();
            MailDataGroupList.ProcessList = Process;
    
            DIC.Add("Header", ImagesServer + "/Themes/2013/img/header.png");
            DIC.Add("Fooer", ImagesServer + "/Themes/2013/img/footer.png");
            DIC.Add("Specx", specx);
            DIC.Add("Delivtype", ((TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus)Delivtype).ToString());
            DIC.Add("CartID", CartID);
            MailDataGroupList.DIC = DIC;
            return MailDataGroupList;
    }
    }
}
