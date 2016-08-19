using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CartPayment
{
    public class VM_PaymentTerm
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IsOnline { get; set; }
        public int? Status { get; set; }

        public int EnumID { get; set; }
        public string EnumName { get; set; }
    }
}
