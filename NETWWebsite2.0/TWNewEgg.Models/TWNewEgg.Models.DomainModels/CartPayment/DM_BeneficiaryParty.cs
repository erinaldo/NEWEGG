using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.CartPayment
{
    public class DM_BeneficiaryParty
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string BankCode { get; set; }
        public int? Status { get; set; }
    }
}
