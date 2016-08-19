using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class CustomerMail
    {
        public string SalesOrderCodes { get; set; }
        public string SalesOrderIDStringList { get; set; }
        public string Itemname { get; set; }
        public string Itemlist { get; set; }
        public string retgood_toaddr { get; set; }
        public string retgood_salesorderCODE { get; set; }
        public string retgood_causenote { get; set; }
        public string CancelDate { get; set; }
        public string Paytype { get; set; }
        public string Reasontext { get; set; }
    }
}
