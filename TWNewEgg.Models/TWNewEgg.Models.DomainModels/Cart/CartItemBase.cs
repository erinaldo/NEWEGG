using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class CartItemBase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public int DelvType { get; set; }
        public string DelvDate { get; set; }
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public int SellerID { get; set; }
        public string SellerName { get; set; }
        public decimal PriceCard { get; set; }
        public decimal PriceCash { get; set; }
        public decimal ServicePrice { get; set; }
        public string Coupon { get; set; }
        public decimal PriceCoupon { get; set; }
        public decimal PriceLocalship { get; set; }
        public decimal PriceGlobalship { get; set; }
        public int QtyAvailable { get; set; }
        public int ManufactureID { get; set; }
        public Nullable<int> PicStart { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        public Nullable<decimal> Taxfee { get; set; }
    }
}
