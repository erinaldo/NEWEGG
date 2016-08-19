using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("item")]
    public class Item
    {
        public enum status
        {
            已上架 = 0,
            未上架 = 1,
            強制下架 = 2,
            系統下架 = 3
        };
        public enum tradestatus
        {
            間配 = 1,
            直配 = 2,
            三角 = 3,
            切貨 = 0,
            國外直購 = 4,
            自貿區 = 5,
            海外切貨 = 6,
            B2C直配 = 7,
            MKPL寄倉 = 8,
            B2c寄倉 = 9
        };
        public enum salestatus
        {
            一般 = 1,
            預購 = 2
        };
        public enum Itemliststatus
        {
            配件 = 0,
            贈品 = 20
        };
        public enum ShowOrderStatus
        {
            正常 = 0,
            隱形 = -1,
            AdditionalItem = -2,
            AddtionalItemForCart = -3,
            AddtionalItemForItem = -5
        };

        public Item()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            DateStart = defaultDate;
            DateEnd = defaultDate;
            DateDel = defaultDate;
            StatusDate = defaultDate;
            CreateDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        /// <summary>
        /// 依據 BSATW-173 廢四機需求增加
        /// 癈四機賣場商品, Y=是癈四機 ---------------add by bruce 20160428
        /// </summary>
        public string Discard4 { get; set; }
    }
}
