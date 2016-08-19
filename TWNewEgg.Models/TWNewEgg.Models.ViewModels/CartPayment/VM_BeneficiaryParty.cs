using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CartPayment
{
    public class VM_BeneficiaryParty
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string BankCode { get; set; }
        public int? Status { get; set; }
    }
}
