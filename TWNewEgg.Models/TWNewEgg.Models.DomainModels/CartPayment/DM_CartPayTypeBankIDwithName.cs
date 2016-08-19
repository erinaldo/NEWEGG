using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.CartPayment
{
    public class DM_CartPayTypeBankIDwithName
    {
        public int BankID { get; set; }
        public string BankName { get; set; }
        public Nullable<int> PaymentVerification { get; set; }

        public string ConsumeMax { get; set; }
        public string ConsumeMin { get; set; }
    }
}
