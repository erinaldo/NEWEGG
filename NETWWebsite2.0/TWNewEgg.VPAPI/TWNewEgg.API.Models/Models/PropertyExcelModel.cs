using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class PropertyExcelModel
    {
        [DisplayName("第一層類別")]
        public string categoryID_Layer0 { get; set; }
        [DisplayName("第二層類別")]
        public string categoryID_Layer1 { get; set; }
        [DisplayName("第三層類別")]
        public string categoryID_Layer2 { get; set; }
        [DisplayName("第一跨分類次類別")]
        public string SubCategoryLayer1 { get; set; }
        [DisplayName("第一跨分類子類別")]
        public string ItemCategoryLayer1 { get; set; }
        [DisplayName("第二跨分類次類別")]
        public string SubcategoryLayer2 { get; set; }
        [DisplayName("第二跨分類子類別")]
        public string ItemCategoryLayer2 { get; set; }
        [DisplayName("新蛋賣場編號")]
        public int ItemID { get; set; }
        [DisplayName("商家商品編號")]
        public string ProductSellerProductID { get; set; }
        [DisplayName("商品名稱(品名)")]
        public string WebsiteShortTitle { get; set; }
        [DisplayName("顏色")]
        public string color { get; set; }
        [DisplayName("尺寸")]
        public string size { get; set; }
        [DisplayName("市場建議售價")]
        public decimal? MarketPrice { get; set; }
        [DisplayName("售價")]
        public decimal? PriceCash { get; set; }
        [DisplayName("成本")]
        public decimal? Cost { get; set; }
        [DisplayName("可售數量")]
        public int? CanSaleQty { get; set; }
        [DisplayName("上下架")]
        public string GoodsStatus { get; set; }
        [DisplayName("出貨方")]
        public string ShipType { get; set; }
        
    }
}
