using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Redeem
{
    public class CouponsLite
    {
        public string couponID { get; set; }
        public string couponName { get; set; }
        public string couponValue { get; set; }
        public string couponCategories { get; set; }
        public string couponProductID { get; set; }
        public string couponLimit { get; set; }
        public string couponEndDate { get; set; }
        public string couponItems { get; set; }
    }
}
