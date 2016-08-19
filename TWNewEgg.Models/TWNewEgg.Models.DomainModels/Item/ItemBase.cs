using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemBase
    {
        public int ID { get; set; }
        public Nullable<int> ItemtempID { get; set; }
        public string Name { get; set; }
        public string ItemDesc { get; set; }
        public string DescTW { get; set; }
        public string Sdesc { get; set; }
        public string SpecDetail { get; set; }
        public string Spechead { get; set; }
        public int SaleType { get; set; }
        public int PayType { get; set; }
        public int Layout { get; set; }
        public int DelvType { get; set; }
        public string DelvDate { get; set; }
        public string Itemnumber { get; set; }
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string Model { get; set; }
        public int SellerID { get; set; }
        public System.DateTime DateStart { get; set; }
        public System.DateTime DateEnd { get; set; }
        public System.DateTime DateDel { get; set; }
        public decimal Pricesgst { get; set; }
        public decimal PriceCard { get; set; }
        public decimal PriceCash { get; set; }
        public decimal ServicePrice { get; set; }
        public int PricehpType1 { get; set; }
        public decimal PricehpInst1 { get; set; }
        public int PricehpType2 { get; set; }
        public decimal PricehpInst2 { get; set; }
        public int Inst0Rate { get; set; }
        public decimal RedmfdbckRate { get; set; }
        public string Coupon { get; set; }
        public decimal PriceCoupon { get; set; }
        public decimal PriceLocalship { get; set; }
        public decimal PriceGlobalship { get; set; }
        public int Qty { get; set; }
        public int SafeQty { get; set; }
        public int QtyLimit { get; set; }
        public string LimitRule { get; set; }
        public int QtyReg { get; set; }
        public string PhotoName { get; set; }
        public string HtmlName { get; set; }
        public int ShowOrder { get; set; }
        public int Class { get; set; }
        public int Status { get; set; }
        public int ManufactureID { get; set; }
        public string StatusNote { get; set; }
        public System.DateTime StatusDate { get; set; }
        public string Note { get; set; }
        public Nullable<int> PicStart { get; set; }
        public Nullable<int> PicEnd { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<decimal> ShipFee { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        public string ShipType { get; set; }
        public Nullable<decimal> Taxfee { get; set; }
        public string ItemPackage { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public string IsNew { get; set; }
        // 圖片路徑
        public string imgPath { get; set; }
        //this is for SEO URL
        public int StoreID { get; set; }

        // 依據 BSATW-173 廢四機需求增加
        // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160516
        public string Discard4 { get; set; }

    }
}
