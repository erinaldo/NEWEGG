using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Framework.Cache;
using TWNewEgg.Framework.Common;
using TWNewEgg.Framework.Common.SMTP.Models;
using TWNewEgg.Framework.Common.SMTP.Service;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.Utility.Model;

namespace TWNewEgg.Utility
{
    public static class ExceptionHandler
    {
        public static void SendErrorMail(string groupName, string subject, string bodyMessage)
        {
            try
            {
                MailLists lists = CacheConfiguration.Instance.GetFromCache<MailLists>("2.0MailLists", "2.0MailList", false);
                MailGroup group = lists.MailGroups.Where(x => x.Name == groupName).FirstOrDefault();
                List<string> addresses = group.Mails.Select(x => x.address).ToList();

                ServiceApiCacheAppSettings serviceCacheAppSetting = CacheConfiguration.Instance.GetFromCache<ServiceApiCacheAppSettings>(ConfigurationManager.SERVICEAPICACHEAPPSETTING, null, false);
                SendMessage settingMailMessage = new SendMessage();
                settingMailMessage.attachmentsPath = new List<string>();
                settingMailMessage.bccsAddess = new Dictionary<string, string>();
                settingMailMessage.ccsAddress = new Dictionary<string, string>();
                settingMailMessage.fromAddress = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_DEBUGSENDMAILADDRESS).First().Value;
                settingMailMessage.fromName = "ServiceApiErrorReport";
                settingMailMessage.isBodyHtml = false;
                settingMailMessage.mailBody = bodyMessage;
                settingMailMessage.mailEncoding = Encoding.UTF8;
                settingMailMessage.mailPriority = SendPriority.High;
                settingMailMessage.mailSubject = subject;
                settingMailMessage.recipientsAddress = SettingMailAddressToDict(addresses);
                settingMailMessage.replysAddress = new Dictionary<string, string>();
                SmtpMessage settingMailSmtp = new SmtpMessage();
                settingMailSmtp.isCredentials = false;
                settingMailSmtp.isSsl = false;
                settingMailSmtp.userName = string.Empty;
                settingMailSmtp.userPass = string.Empty;
                settingMailSmtp.smtpServerIP = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERIP).First().Value;
                settingMailSmtp.smtpServerPort = ConverterUtility.ToInt32(serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERPORT).First().Value);
                SendMail.SendNow(settingMailSmtp, SendMail.SettingEmailMessage(settingMailMessage));
            }
            catch (Exception e)
            {

            }
        }

        private static Dictionary<string, string> SettingMailAddressToDict(List<string> mailAddress)
        {
            Dictionary<string, string> mailAddressDict = new Dictionary<string, string>();
            for (int i = 0; i < mailAddress.Count; i++)
            {
                if (!mailAddressDict.ContainsKey(mailAddress[i]))
                {
                    mailAddressDict.Add(mailAddress[i], "");
                }

            }
            return mailAddressDict;
        }
    }
}
