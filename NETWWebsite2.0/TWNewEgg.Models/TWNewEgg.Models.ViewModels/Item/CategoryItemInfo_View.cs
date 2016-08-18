using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Item
{
    public class CategoryItemInfo_View
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Spechead { get; set; }
        public decimal PriceCash { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        public string Specdetail { get; set; }
        public string Photoname { get; set; }
        public int? ProductID { get; set; }
        public int? ProductFK { get; set; }
        public int? Rating { get; set; }
        public int? TotalReviews { get; set; }
        public int? SellingQty { get; set; }
        public decimal PriceGlobalship { get; set; }
        public int? Page { get; set; }
        public int? TotalPage { get; set; }
        public string imgPath { get; set; }
        public int CategoryID { get; set; }
        public int StoreID { get; set; }
    }
}
