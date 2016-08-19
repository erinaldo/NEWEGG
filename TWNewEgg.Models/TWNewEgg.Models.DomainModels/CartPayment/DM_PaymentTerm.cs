using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.CartPayment
{
    public class DM_PaymentTerm
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IsOnline { get; set; }
        public int? Status { get; set; }
    }
}
