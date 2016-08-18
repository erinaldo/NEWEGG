using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using TWNewEgg.Framework.Cache;
using TWNewEgg.Framework.Common;
using TWNewEgg.Framework.Common.SMTP.Models;
using TWNewEgg.Framework.Common.SMTP.Service;
using TWNewEgg.CommonService.Interface;
using TWNewEgg.Framework.ServiceApi.Configuration;
using TWNewEgg.CommonService.DomainModels;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.CommonService
{
    public class NotificationService : INotificationService
    {
        public NotificationModel NotificationInput { get; set; }

        private NotificationPresets NotificationPreset { get; set; }
        private NotificationPreset Preset { get; set; }

        public void Set(NotificationModel input)
        {
            this.NotificationInput = input;
            this.NotificationPreset = CacheConfiguration.Instance.GetFromCache<NotificationPresets>("NotificationPresets", null, false);
            if (this.NotificationPreset == null)
            {
                throw new NullReferenceException("NotificationPreset can't be found, please check key: 'NotificationPresets' in AppSetting.config, and the xml structure in NotificationPresets.xml");
            }

            this.Preset = NotificationPreset.Presets.Where(x => x.id == input.PresetId).FirstOrDefault();
            if (this.Preset == null)
            {
                throw new NullReferenceException("Preset ID: " + input.PresetId + " can't be found");
            }

            this.SetInputByPreset();
        }

        public void NotifyByMailAndSMS()
        {
            this.CheckInput();
            this.CheckMailSetting();
            this.CheckSMSSetting();
            this.SendMail();
            this.SendSMS();
        }

        public void NotifyByMail()
        {
            this.CheckInput();
            this.CheckMailSetting();
            this.SendMail();
        }

        public void NotifyBySMS()
        {
            this.CheckInput();
            this.CheckSMSSetting();
            this.SendSMS();
        }

        private void CheckSMSSetting()
        {
            if (this.NotificationInput.PhoneNumber == null || this.NotificationInput.PhoneNumber.Count() == 0)
            {
                throw new NullReferenceException("PhoneNumber can't be null");
            }

            if (string.IsNullOrWhiteSpace(this.NotificationInput.PhoneContent))
            {
                throw new ArgumentNullException("PhoneContent can't be null or whitespace");
            }
        }

        private void CheckMailSetting()
        {
            if (this.NotificationInput.MailAddresses == null || this.NotificationInput.MailAddresses.Count() == 0)
            {
                throw new NullReferenceException("MailAddress can't be null");
            }

            if (string.IsNullOrWhiteSpace(this.NotificationInput.MailContent))
            {
                throw new ArgumentNullException("MailContent can't be null or whitespace");
            }

            if (string.IsNullOrWhiteSpace(this.NotificationInput.Title))
            {
                throw new ArgumentNullException("Title can't be null or whitespace");
            }
        }

        private void CheckInput()
        {
            if (this.NotificationInput == null)
            {
                throw new NullReferenceException("NotificationInput can't be null");
            }
        }

        private void SetInputByPreset()
        {
            if (NotificationInput.MailAddresses == null || NotificationInput.MailAddresses.Count() == 0)
            {
                NotificationMail[] notificationMails = Preset.MailPreset.MailList;
                if (notificationMails != null && notificationMails.Count() > 0)
                {
                    NotificationInput.MailAddresses = notificationMails.Select(x => x.address).ToArray();
                }
                else
                {
                    NotificationInput.MailAddresses = new string[0];
                }
            }

            if (NotificationInput.ccMailAddresses == null || NotificationInput.ccMailAddresses.Count() == 0)
            {
                NotificationMail[] notificationMails = Preset.MailPreset.ccMailList;
                if (notificationMails != null && notificationMails.Count() > 0)
                {
                    NotificationInput.ccMailAddresses = notificationMails.Select(x => x.address).ToArray();
                }
                else
                {
                    NotificationInput.ccMailAddresses = new string[0];
                }
            }

            if (NotificationInput.bccMailAddresses == null || NotificationInput.bccMailAddresses.Count() == 0)
            {
                NotificationMail[] notificationMails = Preset.MailPreset.bccMailList;
                if (notificationMails != null && notificationMails.Count() > 0)
                {
                    NotificationInput.bccMailAddresses = notificationMails.Select(x => x.address).ToArray();
                }
                else
                {
                    NotificationInput.bccMailAddresses = new string[0];
                }
            }

            if (NotificationInput.PhoneNumber == null || NotificationInput.PhoneNumber.Count() == 0)
            {
                NotificationPhone[] notificationPhones = Preset.PhonePreset.PhoneList;
                if (notificationPhones != null && notificationPhones.Count() > 0)
                {
                    NotificationInput.PhoneNumber = notificationPhones.Select(x => x.number).ToArray();
                }
                else
                {
                    NotificationInput.PhoneNumber = new string[0];
                }
            }
        }

        private void SendMail()
        {
            ServiceApiCacheAppSettings serviceCacheAppSetting = CacheConfiguration.Instance.GetFromCache<ServiceApiCacheAppSettings>(ConfigurationManager.SERVICEAPICACHEAPPSETTING, null, false);
            SendMessage settingMailMessage = new SendMessage();
            settingMailMessage.attachmentsPath = new List<string>();
            settingMailMessage.bccsAddess = this.SettingMailAddressToDict(this.NotificationInput.bccMailAddresses);
            settingMailMessage.ccsAddress = this.SettingMailAddressToDict(this.NotificationInput.ccMailAddresses);
            settingMailMessage.fromAddress = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_DEBUGSENDMAILADDRESS).First().Value;
            settingMailMessage.fromName = "ServiceApiErrorReport";
            settingMailMessage.isBodyHtml = false;
            settingMailMessage.mailBody = this.NotificationInput.MailContent;
            settingMailMessage.mailEncoding = Encoding.UTF8;
            settingMailMessage.mailPriority = SendPriority.High;
            settingMailMessage.mailSubject = string.Format("[{0}] {1}", this.NotificationPreset.env, this.NotificationInput.Title);
            settingMailMessage.recipientsAddress = this.SettingMailAddressToDict(this.NotificationInput.MailAddresses);
            settingMailMessage.replysAddress = new Dictionary<string, string>();
            SmtpMessage settingMailSmtp = new SmtpMessage();
            settingMailSmtp.isCredentials = false;
            settingMailSmtp.isSsl = false;
            settingMailSmtp.userName = string.Empty;
            settingMailSmtp.userPass = string.Empty;
            settingMailSmtp.smtpServerIP = serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERIP).First().Value;
            settingMailSmtp.smtpServerPort = ConverterUtility.ToInt32(serviceCacheAppSetting.serviceApiCacheAppSettings.Where(x => x.Key == ConfigurationManager.CACHE_SMTPSERVERPORT).First().Value);
            MailMessage mailMessage = TWNewEgg.Framework.Common.SMTP.Service.SendMail.SettingEmailMessage(settingMailMessage);
            TWNewEgg.Framework.Common.SMTP.Service.SendMail.SendNow(settingMailSmtp, mailMessage);
        }

        private void SendSMS()
        {
            string phoneNumberStr = string.Join(",", NotificationInput.PhoneNumber);
            var sendResult = Processor.Request<List<SMSSubmitReturnModel>, List<SMSSubmitReturnModel>>("Service.HiNetSolution", "EasySendSMSMessage", phoneNumberStr, string.Format("[{0}] {1}", this.NotificationPreset.env, NotificationInput.PhoneContent));
            if (sendResult.error != null)
            {
                throw new Exception(sendResult.error);
            }
        }

        private Dictionary<string, string> SettingMailAddressToDict(IEnumerable<string> mailAddress)
        {
            Dictionary<string, string> mailAddressDict = new Dictionary<string, string>();
            foreach (string address in mailAddress)
            {
                if (!mailAddressDict.ContainsKey(address))
                {
                    mailAddressDict.Add(address, "");
                }

            }

            return mailAddressDict;
        }
    }
}
