using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.SendMail
{
    public class SendMailDM
    {  
        // 收件者
        public string reciver { get; set; }
        // 信件主旨
        public string subject { get; set; }
        // 信件內容
        public string bodyMessage { get; set; }
    }
}
