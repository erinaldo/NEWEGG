using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ItemService.Models
{
    public class UsedCouponItem
    {
        public int CouponId { get; set; }
        public decimal Value { get; set; }
        public string Categoryies { get; set; }
    }
}