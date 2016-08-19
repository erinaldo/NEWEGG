using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Redeem
{
    public class UserCouponsProducts
    {
        public List<CouponsLite> coupons { get; set; }
        public List<ItemLite> items { get; set; }
    }
}
