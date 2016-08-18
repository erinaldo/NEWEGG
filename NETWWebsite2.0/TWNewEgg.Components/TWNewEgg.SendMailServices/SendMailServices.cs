using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.Models.DomainModels.SendMail;
using TWNewEgg.SendMailServices.Interface;

namespace TWNewEgg.SendMailServices
{
    public class SendMailServices : ISendMailServices
    {
        public bool SendATMPaymentSuccessfulMail(SendMailDM SendMailDM)
        {
            bool SendMailResult = false;
            try
            {
                ServiceApiCacheAppSettings serviceCacheAppSetting = TWNewEgg.Framework.Cache.CacheConfiguration.Instance.GetFromCache<ServiceApiCacheAppSettings>(ConfigurationManager.SERVICEAPICACHEAPPSETTING, null, false);
                TWNewEgg.Framework.Common.SMTP.Models.SendMessage settingMailMessage = new TWNewEgg.Framework.Common.SMTP.Models.SendMessage();
                settingMailMessage.attachmentsPath = new List<string>();
                settingMailMessage.bccsAddess = new Dictionary<string, string>();
                settingMailMessage.ccsAddress = new Dictionary<string, string>();
                settingMailMessage.fromAddress = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_DEBUGSENDMAILADDRESS).First().Value;
                settingMailMessage.fromName = "新蛋全球生活網";
                settingMailMessage.isBodyHtml = true;
                settingMailMessage.mailBody = SendMailDM.bodyMessage;
                settingMailMessage.mailEncoding = System.Text.Encoding.UTF8;
                settingMailMessage.mailPriority = TWNewEgg.Framework.Common.SMTP.Models.SendPriority.High;
                settingMailMessage.mailSubject = SendMailDM.subject;
                settingMailMessage.recipientsAddress = SettingMailAddressToDict(SendMailDM.reciver);
                settingMailMessage.replysAddress = new Dictionary<string, string>();
                TWNewEgg.Framework.Common.SMTP.Models.SmtpMessage settingMailSmtp = new TWNewEgg.Framework.Common.SMTP.Models.SmtpMessage();
                settingMailSmtp.isCredentials = false;
                settingMailSmtp.isSsl = false;
                settingMailSmtp.userName = string.Empty;
                settingMailSmtp.userPass = string.Empty;
                settingMailSmtp.smtpServerIP = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERIP).First().Value;
                settingMailSmtp.smtpServerPort = TWNewEgg.Framework.Common.ConverterUtility.ToInt32(serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERPORT).First().Value);
                TWNewEgg.Framework.Common.SMTP.Service.SendMail.SendNow(settingMailSmtp, TWNewEgg.Framework.Common.SMTP.Service.SendMail.SettingEmailMessage(settingMailMessage));
                SendMailResult = true; ;
            }
            catch (Exception e) {
                SendMailResult = false;
            }
            return SendMailResult;
        }

        public bool SendMail(SendMailDM SendMailDM)
        {
            bool SendMailResult = false;
            try
            {
                ServiceApiCacheAppSettings serviceCacheAppSetting = TWNewEgg.Framework.Cache.CacheConfiguration.Instance.GetFromCache<ServiceApiCacheAppSettings>(ConfigurationManager.SERVICEAPICACHEAPPSETTING, null, false);
                TWNewEgg.Framework.Common.SMTP.Models.SendMessage settingMailMessage = new TWNewEgg.Framework.Common.SMTP.Models.SendMessage();
                settingMailMessage.attachmentsPath = new List<string>();
                settingMailMessage.bccsAddess = new Dictionary<string, string>();
                settingMailMessage.ccsAddress = new Dictionary<string, string>();
                settingMailMessage.fromAddress = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_DEBUGSENDMAILADDRESS).First().Value;
                settingMailMessage.fromName = "新蛋全球生活網";
                settingMailMessage.isBodyHtml = true;
                settingMailMessage.mailBody = SendMailDM.bodyMessage;
                settingMailMessage.mailEncoding = System.Text.Encoding.UTF8;
                settingMailMessage.mailPriority = TWNewEgg.Framework.Common.SMTP.Models.SendPriority.High;
                settingMailMessage.mailSubject = SendMailDM.subject;
                settingMailMessage.recipientsAddress = SettingMailAddressToDict(SendMailDM.reciver);
                settingMailMessage.replysAddress = new Dictionary<string, string>();
                TWNewEgg.Framework.Common.SMTP.Models.SmtpMessage settingMailSmtp = new TWNewEgg.Framework.Common.SMTP.Models.SmtpMessage();
                settingMailSmtp.isCredentials = false;
                settingMailSmtp.isSsl = false;
                settingMailSmtp.userName = string.Empty;
                settingMailSmtp.userPass = string.Empty;
                settingMailSmtp.smtpServerIP = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERIP).First().Value;
                settingMailSmtp.smtpServerPort = TWNewEgg.Framework.Common.ConverterUtility.ToInt32(serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERPORT).First().Value);
                TWNewEgg.Framework.Common.SMTP.Service.SendMail.SendNow(settingMailSmtp, TWNewEgg.Framework.Common.SMTP.Service.SendMail.SettingEmailMessage(settingMailMessage));
                SendMailResult = true; ;
            }
            catch (Exception e)
            {
                SendMailResult = false;
            }
            return SendMailResult;
        }

        private static Dictionary<string, string> SettingMailAddressToDict(string mailAddress)
        {
            Dictionary<string, string> mailAddressDict = new Dictionary<string, string>();
            List<string> mailAddresstemp = new List<string>();
            if (mailAddress == "") {
                mailAddress = "Yellow.C.Huang@newegg.com, Bill.S.Wu@newegg.com, Rex.T.Chen@newegg.com, Pin.Y.Tsai@newegg.com, Alan.H.Hsu@newegg.com, Gretchen.H.Yeh@newegg.com, Bencent.K.Huang@newegg.com, Anita.Y.Wu@newegg.com, CF.C.Toh@newegg.com, Chi.C.Lee@newegg.com, Jessie.Y.Tseng@newegg.com, Jasmine.C.Hsieh@newegg.com, Bill.C.Hsu@newegg.com, Kelly.M.Chen@newegg.com, Rhett.W.Sung@newegg.com, service@newegg.com.tw"; 
            }

            if (mailAddress != null)
            {
                mailAddresstemp = mailAddress.Split(',').ToList();
            }

            foreach (var temp in mailAddresstemp) {
                if (temp != "")
                {
                    mailAddressDict.Add(temp, "");
                }
            }

            return mailAddressDict;
        }
    }
}
