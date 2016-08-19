using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class DelivGroup
    {    
        public string DelivType { get; set; }
        public string Status { get; set; }
        public List<string> MailGroup { get; set; }
    }
}
