using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class CartItemBase_View
    {
        public Nullable<int> ID { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public Nullable<int> DelvType { get; set; }
        public string DelvDate { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> SellerID { get; set; }
        public string SellerName { get; set; }
        public Nullable<decimal> PriceCard { get; set; }
        public Nullable<decimal> PriceCash { get; set; }
        public Nullable<decimal> ServicePrice { get; set; }
        public string Coupon { get; set; }
        public Nullable<decimal> PriceCoupon { get; set; }
        public Nullable<decimal> PriceLocalship { get; set; }
        public Nullable<decimal> PriceGlobalship { get; set; }
        public Nullable<int> QtyAvailable { get; set; }
        public Nullable<int> ManufactureID { get; set; }
        public Nullable<int> PicStart { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        public Nullable<decimal> Taxfee { get; set; }
        // 商品類型
        public int ShowOrder { get; set; }

        // 依據 BSATW-173 廢四機需求增加
        // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160516
        public string Discard4 { get; set; }

    }
}
