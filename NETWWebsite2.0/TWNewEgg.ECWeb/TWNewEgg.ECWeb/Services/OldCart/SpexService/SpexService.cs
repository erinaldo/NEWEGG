using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class SpexService
    {
        /// <summary>
        /// 如果訂單 DelvType=6，則自動發信通知營服主管。
        /// </summary>
        public static void SendMail(string SOCode)
        {
            TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);
            try
            {
                string env = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                string ECWeb_SMTP = System.Configuration.ConfigurationManager.AppSettings["ECWeb_SMTP"];
                DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                var so = db.SalesOrder.Where(x => x.Code == SOCode).FirstOrDefault();

                if (so != null && so.DelivType == (int)DB.TWSQLDB.Models.Item.tradestatus.海外切貨)
                {
                    List<DB.TWSQLDB.Models.SalesOrderItem> soiList = db.SalesOrderItem.Where(x => x.SalesorderCode == so.Code).ToList();

                    string mailBody = "<html>";
                    mailBody += "<H2>海外切貨</H2>";
                    mailBody += "顧客姓名:" + so.Name;
                    mailBody += "<br><pre>SalesOrderCode:  <span style=\"color:#0200A0;\">" + so.Code + "</span></pre>";
                    foreach (var soi in soiList)
                    {
                        mailBody += "<br><pre><span style=\"color:#0200A0;\">" + soi.Name + "</span>  Qty:  <span style=\"color:#0200A0;\">" + soi.Qty.ToString() + "</span></pre>";
                    }
                    mailBody += "<hr>台灣時間:" + so.CreateDate.ToString() + "<hr>美國時間:" + so.CreateDate.ToUniversalTime().AddHours(-7).ToString();
                    mailBody += "</html>";

                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress(WebSiteData.Email1, WebSiteData.SiteName, System.Text.Encoding.UTF8);
                    msg.To.Add("amos.c.chuang@newegg.com");
                    if (env.ToUpper() == "PRD" || env.ToUpper() == "GQC")
                    {
                        msg.To.Add("Gretchen.H.Yeh@newegg.com");
                    }
                    msg.Subject = "海外切貨訂單";
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    msg.Body = mailBody;
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.IsBodyHtml = true;
                    msg.Priority = MailPriority.Normal;
                    SmtpClient MySmtp = new SmtpClient(ECWeb_SMTP, 25);
                    MySmtp.Send(msg);
                }
            }
            catch { }
        }
    }
}