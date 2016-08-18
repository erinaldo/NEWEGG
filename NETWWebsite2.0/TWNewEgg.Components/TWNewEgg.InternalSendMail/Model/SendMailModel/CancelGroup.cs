using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class CancelGroup
    {    
        public string DelivType { get; set; }
        public List<string> MailGroup { get; set; }
    }
}
