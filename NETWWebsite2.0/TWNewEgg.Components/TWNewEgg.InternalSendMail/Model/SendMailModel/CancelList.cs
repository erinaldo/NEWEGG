using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class CancelList
    {
        public List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> Cart { get; set; }
        public List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel> SalesOrderCancel { get; set; }
        public string Header { get; set; }
        public string Fooer { get; set; }
        public bool Bankinfo { get; set; }
        public string mysubject { get; set; }
        public string Recipient { get; set; }
        public string RecipientBcc { get; set; }
        public Dictionary<string, string> DIC { get; set; }

    }
}
