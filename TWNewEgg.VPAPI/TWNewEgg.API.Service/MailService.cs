using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Seller Manage Service
    /// </summary>
    public class MailService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        private static ILog log = LogManager.GetLogger(typeof(MailService));

        /// <summary>
        /// 寄送邀請信
        /// </summary>
        /// <returns></returns>
        
        //public API.Models.ActionResponse<string> SendEmail(int sellerID, string email)//(DB.TWSELLERPORTALDB.Models.Seller_BasicInfo basicInfo)
        public API.Models.ActionResponse<Models.MailResult> SendEmail(Models.MailTransformation mailTransformation)
        {
            API.Models.ActionResponse<Models.MailResult> result = new Models.ActionResponse<Models.MailResult>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = spdb.Seller_User.Where(x => x.UserEmail == mailTransformation.UserEmail).FirstOrDefault();

            //if (user == null)
            //{
            //    DB.TWSELLERPORTALDB.Models.Seller_Notification notification = spdb.Seller_Notification.Where(x => x.EmailAddress1 == mailTransformation.UserEmail
            //                                                                                                   || x.EmailAddress2 == mailTransformation.UserEmail
            //                                                                                                   || x.EmailAddress3 == mailTransformation.UserEmail).FirstOrDefault();
            //    if (notification == null)
            //    {
            //        result.Code = (int)ResponseCode.Error;
            //        result.IsSuccess = false;
            //        result.Msg = "系統內找不到此Email";
            //        result.Body = new Models.MailResult();
            //        return result;
            //    }
            //}
            

            //改成建立user時生成
            /*//+ 生成亂數
            int rand;
            char word;
            string ranNum = String.Empty;
            // 生成啟用連結路徑中的亂數
            System.Random random = new Random();
            for (int i = 0; i < 30; i++)
            {
                rand = random.Next();
                if (rand % 3 == 0)
                {
                    word = (char)('A' + (char)(rand % 26));
                }
                else if (rand % 3 == 1)
                {
                    word = (char)('a' + (char)(rand % 26));
                }
                else
                {
                    word = (char)('0' + (char)(rand % 10));
                }

                ranNum += word.ToString();
            }

            //2013.11.13 寫入亂數至資料庫 by Jack Lin
            user.RanNum = ranNum;
            spdb.Entry(user).State = EntityState.Modified;
            spdb.SaveChanges();
            */


            string MailMessage = mailTransformation.MailContent; //信件訊息
            //string Mysubject = "台灣新蛋 Seller Portal 邀請信"; //信件主旨    //2014.1.27 改掉信件主旨寫死部分 mark by Smoke
            string Mysubject = mailTransformation.MailSubject;                //2014.1.27 改掉信件主旨寫死部分 by Smoke
            
            //string Recipient = "Jack.c.Lin@newegg.com";       //mailTransformation.UserEmail; //收件者  //2014.1.23 改掉寄信收件人寫死部分 mark by ice
            string Recipient = mailTransformation.UserEmail;    //收件者                                  //2014.1.23 改掉寄信收件人寫死部分 by ice
            string RecipientBcc = mailTransformation.RecipientBcc; //密件收件人

            try
            {
                MailMessage msg = new MailMessage();
                // 收件者，以逗號分隔不同收件者
                msg.To.Add(Recipient);
                // msg.CC.Add("c@msn.com"); // 副本
                if (RecipientBcc != null)
                {
                    msg.Bcc.Add(RecipientBcc); // 密件副本
                }
                // 3個參數分別是發件人地址(可以隨便寫)，發件人姓名，編碼
                msg.From = new MailAddress("marketplace@newegg.com.tw", "台灣新蛋", System.Text.Encoding.UTF8);
                msg.Subject = Mysubject; // 郵件主旨
                msg.SubjectEncoding = System.Text.Encoding.UTF8; // 郵件主旨編碼
                msg.Body = MailMessage; // 郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8; // 郵件內容編碼
                msg.IsBodyHtml = true; // 是否為HTML郵件
                msg.Priority = MailPriority.Normal; // 郵件優先等級
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                string smtp = System.Configuration.ConfigurationManager.AppSettings["SentMailService"] == null ? "st01smtp01.buyabs.corp" : System.Configuration.ConfigurationManager.AppSettings["SentMailService"].ToString();
                string smtPort = System.Configuration.ConfigurationManager.AppSettings["SentMailPort"] == null ? "25" : System.Configuration.ConfigurationManager.AppSettings["SentMailPort"].ToString();
                int int_smtPortp = Convert.ToInt16(smtPort);
                SmtpClient MySmtp = new SmtpClient(smtp, int_smtPortp);
                //SmtpClient MySmtp = new SmtpClient("172.22.5.55", 25);
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                MySmtp.Send(msg);

                DateTime dt = DateTime.UtcNow.AddHours(8);
                dt.GetDateTimeFormats('r')[0].ToString();
                Convert.ToDateTime(dt);

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "寄送成功";
                result.Body = new Models.MailResult();

                if (user != null)
                {
                    result.Body.UserID = user.UserID;
                }
                else
                {   
                    // User Email 未存在 VendorPortal
                    result.Body.UserID = 0;
                }

                result.Body.SendTime = dt;
                result.Body.MailSubject = Mysubject;
                result.Body.MailContent = MailMessage;

                return result;
            }
            catch (Exception ex)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = ex.Message;
                result.Body = new Models.MailResult();
                log.Error(mailTransformation.UserEmail);
                log.Error(mailTransformation.MailSubject);
                log.Error(ex.Message);
                log.Error("[StackTrace]: " + ex.StackTrace + " ;[innerException]: " + (ex.InnerException == null ? "No InnerException" : ex.InnerException.Message));
                return result;
            }
        }
    }
}
