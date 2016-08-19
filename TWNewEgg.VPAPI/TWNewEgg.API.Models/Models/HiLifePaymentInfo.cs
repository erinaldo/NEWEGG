using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class HiLifePaymentInfo
    {
        public string UserAccountEmail { get; set; }

        public string UserName { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
