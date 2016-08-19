using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SearchServices.Model
{
    public class ItemSearch
    {
        [DisplayName("產品ID")]
        public int ID { get; set; }
        [DisplayName("產品名稱")]
        public string Name { get { return this.ItemName; } }
        [DisplayName("產品名稱")]
        public string ItemName { get; set; }
        [DisplayName("產品簡短介紹")]
        public string Sdesc { get; set; }
        [DisplayName("產品拍賣方式")]
        public Nullable<int> Saletype { get; set; }
        [DisplayName("產品付款方式")]
        public Nullable<int> PayType { get; set; }
        [DisplayName("產品開賣日期")]
        public Nullable<System.DateTime> DateStart { get; set; }
        [DisplayName("產品結標日期")]
        public Nullable<System.DateTime> DateEnd { get; set; }
        [DisplayName("產品建議售價")]
        public Nullable<decimal> Pricesgst { get; set; }
        [DisplayName("產品售價")]
        public Nullable<decimal> Pricecash { get; set; }
        [DisplayName("產品可購數量")]
        public Nullable<int> SellingQty { get; set; }
        [DisplayName("產品已購數量")]
        public Nullable<int> Qtyreg { get; set; }
        [DisplayName("產品照片")]
        public string PhotoName { get; set; }
        [DisplayName("產品順序")]
        public Nullable<int> Showorder { get; set; }
        [DisplayName("產品類別ID")]
        public Nullable<int> CategoryID { get; set; }
        [DisplayName("產品類別層級")]
        public Nullable<int> CategoryLayer { get { return this.Layer; } }
        [DisplayName("產品類別標題")]
        public string CategoryTitle { get { return this.Title; } }
        [DisplayName("產品類別層級")]
        public Nullable<int> Layer { get; set; }
        [DisplayName("產品類別標題")]
        public string Title { get; set; }
        [DisplayName("產品廠牌ID")]
        public Nullable<int> ManufactureID { get; set; }
        [DisplayName("產品廠牌")]
        public string ManufactureName { get; set; }
        [DisplayName("產品型號")]
        public string ProductModel { get; set; }
        [DisplayName("產品型號")]
        public string Model { get; set; }
        [DisplayName("產品更新時間")]
        public Nullable<System.DateTime> ItemUpdateDate { get { return this.UpdateDate; } }
        [DisplayName("產品更新時間")]
        public Nullable<System.DateTime> UpdateDate { get; set; }
        [DisplayName("產品促銷噱頭")]
        public string ItemSpechead { get { return this.Spechead; } }
        [DisplayName("產品促銷噱頭")]
        public string Spechead { get; set; }
        [DisplayName("產品名稱")]
        public string ItemHtmlName { get { return this.HtmlName; } }
        [DisplayName("產品名稱")]
        public string HtmlName { get; set; }
        [DisplayName("產品名稱")]
        public string ProductName { get; set; }
        [DisplayName("SellerProductID")]
        public string SellerProductID { get; set; }
        [DisplayName("商家ID")]
        public Nullable<int> SellerID { get; set; }
        [DisplayName("商家名稱")]
        public string SellerName { get; set; }
        [DisplayName("商家國別")]
        public Nullable<int> CountryID { get; set; }
        [DisplayName("商家國別")]
        public Nullable<decimal> PriceGlobalship { get; set; }
    }
}
