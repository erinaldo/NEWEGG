using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class CartItemTempDM
    {
        public int ID { get; set; }
        public int CartTempID { get; set; }
        public int ItemID { get; set; }
        public int Qty { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<decimal> DisplayPrice { get; set; }
        public Nullable<decimal> PriceCash { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> ShippingPrice { get; set; }
        public Nullable<decimal> ServicePrice { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
