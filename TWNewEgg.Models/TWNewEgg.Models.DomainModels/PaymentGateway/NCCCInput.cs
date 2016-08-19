using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PaymentGateway
{
    public class NCCCInput
    {
        public string MerchantID { get; set; }
        public string NotifyURL { get; set; }
        public string OrderID { get; set; }
        public string TerminalID { get; set; }
        public string TransAmt { get; set; }
        public string TransMode { get; set; }

        public string BankNo { get; set; }
        public string Installment { get; set; }
    }
}
