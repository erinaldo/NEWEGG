using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class returnRefundList
    {
        public List<TWNewEgg.DB.TWBACKENDDB.Models.refund2c> refund2c { get; set; }
        public List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> Cart { get; set; }
        public List<TWNewEgg.DB.TWBACKENDDB.Models.Process> process { get; set; }
        public List<TWNewEgg.DB.TWSQLDB.Models.Bank> Bank { get; set; }
        public string Header { get; set; }
        public string Fooer { get; set; }
        public bool Bankinfo { get; set; }
        public string mysubject { get; set; }
        public string Recipient { get; set; }
        public string RecipientBcc { get; set; }
        public Dictionary<string, string> DIC { get; set; }
    }
}
