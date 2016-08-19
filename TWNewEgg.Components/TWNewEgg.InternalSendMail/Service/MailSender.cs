using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.GetConfigData.Models;

namespace TWNewEgg.InternalSendMail.Service
{
    public class MailSender:IMailSender
    {
        private static string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"].ToUpper();
        private string SmtpServer = System.Configuration.ConfigurationManager.AppSettings[environment + "_SmtpServer"];
        WebSiteListWebSiteData WebSiteData = new WebSiteListWebSiteData(0);

        public bool SendMail(string body = "", string Recipient = "", string RecipientBcc = "", string mysubject = "", MailAddress mailAddress = null, List<string> Attachments = null)
        {
            log4net.ILog logger;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            logger.Info("Parsing.....SendMail Begin：" + mysubject);
            try
            {
                MailMessage msg = new MailMessage();
                // 收件者，以逗號分隔不同收件者
                msg.To.Add(Recipient);
                // 副本
                if (RecipientBcc!=null && RecipientBcc != "")
                {
                    msg.Bcc.Add(RecipientBcc); // 密件副本
                }

                // 3個參數分別是發件人地址(可以隨便寫)，發件人姓名，編碼
                if (mailAddress == null)
                {
                    msg.From = new MailAddress(WebSiteData.Email1, WebSiteData.Contact1, System.Text.Encoding.UTF8);
                }
                else {
                    msg.From = mailAddress;
                }
                msg.Subject = mysubject; // 郵件主旨
                msg.SubjectEncoding = System.Text.Encoding.UTF8; // 郵件主旨編碼
                msg.Body = body; // 郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8; // 郵件內容編碼
                msg.IsBodyHtml = true; // 是否為HTML郵件
                msg.Priority = MailPriority.Normal; // 郵件優先等級
                if (Attachments != null && Attachments.Count() != 0)
                {
                    foreach (var Attachmentslist in Attachments)
                    {
                        msg.Attachments.Add(new Attachment(AppDomain.CurrentDomain.BaseDirectory + Attachmentslist));
                    }
                }
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient MySmtp = new SmtpClient(SmtpServer, 25);
                // 發送Email
                MySmtp.Send(msg);
                logger.Info("Parsing.....SendMail End：" + mysubject);
                return true;
            }
            catch (Exception e)
            {
                logger.Info(e.Message);
                logger.Info(e.StackTrace);
                return false; }
        }

        /// <summary>
        /// 給前台呼叫寄信
        /// </summary>
        /// <param name="body">郵件內容</param>
        /// <param name="Recipient">收件者</param>
        /// <param name="RecipientBcc">密件副本</param>
        /// <param name="mysubject">郵件主旨</param>
        /// <param name="smtpsever">smtp主機</param>
        /// <returns>bool 成功、失敗訊息</returns>
        //public bool SendMail(string body = "", string Recipient = "", string RecipientBcc = "", string mysubject = "", string smtpsever = "")
        //{
        //    try
        //    {
        //        MailMessage msg = new MailMessage();
        //        // 收件者，以逗號分隔不同收件者
        //        msg.To.Add(Recipient);
        //        // 副本
        //        if (RecipientBcc != "")
        //        {
        //            msg.Bcc.Add(RecipientBcc); // 密件副本
        //        }

        //        // 3個參數分別是發件人地址(可以隨便寫)，發件人姓名，編碼
        //        msg.From = new MailAddress("service@newegg.com.tw", "台灣新蛋", System.Text.Encoding.UTF8);
        //        msg.Subject = mysubject; // 郵件主旨
        //        msg.SubjectEncoding = System.Text.Encoding.UTF8; // 郵件主旨編碼
        //        msg.Body = body; // 郵件內容
        //        msg.BodyEncoding = System.Text.Encoding.UTF8; // 郵件內容編碼
        //        msg.IsBodyHtml = true; // 是否為HTML郵件
        //        msg.Priority = MailPriority.Normal; // 郵件優先等級
        //        // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
        //        SmtpClient MySmtp = new SmtpClient(smtpsever, 25);
        //        // 發送Email
        //        MySmtp.Send(msg);
        //        return true;
        //    }
        //    catch { return false; }
        //}
    }
}
