using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ItemService.Models
{
    public class UsedCoupon
    {
        public int ItemId { get; set; }
        public int BuyNumber { get; set; }
        public int DisplayPrice { get; set; }
        public List<UsedCouponItem> Coupons { get; set; }
    }
}