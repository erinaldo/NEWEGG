using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CommonService.DomainModels
{
    public class NotificationModel
    {
        public string PresetId { get; set; }
        public string PresetName { get; set; }
        public string MailContent { get; set; }
        public string PhoneContent { get; set; }
        public string Title { get; set; }
        public string[] PhoneNumber { get; set; }
        public string[] MailAddresses { get; set; }
        public string[] ccMailAddresses { get; set; }
        public string[] bccMailAddresses { get; set; }
        NotificationResponse Response { get; set; }
    }
}