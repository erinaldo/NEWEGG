using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("itemtemp")]
    public class ItemTemp
    {
        public enum status
        {
            審核通過 = 0,
            未審核 = 1,//建立或修改資料後之狀態
            未通過 = 2

        };
        public enum itemstatus
        {
            上架 = 0,
            下架 = 1,
            強制下架 = 2,
            售價異常 = 3,
            刪除 = 99
        }
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
        public enum itempackage
        {
            Retail = 0, //零售
            OEM = 1,

        };
        public ItemTemp()
        {
            DateTime defaultDate = DateTime.Now;
            DateStart = defaultDate;
            DateEnd = DateStart.AddYears(100);
            DateDel = DateEnd.AddDays(1);
            StatusDate = defaultDate;
            CreateDate = defaultDate;
            this.Status = (byte)status.未審核;
            this.ItemStatus = (byte)itemstatus.下架;
            this.SaleType = 1;
            this.PayType = 0;
            this.Layout = 0;
            this.ItemNumber = "";
            this.Model = "";
            this.SpecDetail = "";
            this.PriceCard = 0;
            this.PricehpType1 = 0;
            this.Pricehpinst1 = 0;
            this.PricehpType2 = 0;
            this.Pricehpinst2 = 0;
            this.Inst0Rate = 0;
            this.RedmfdbckRate = 0;
            this.Coupon = "";
            this.PriceCoupon = 0;
            this.PriceLocalship = 0;
            this.PriceGlobalship = 0;
            this.Qty = 0;
            this.SafeQty = 0;
            this.QtyLimit = 0;
            this.LimitRule = "";
            this.QtyReg = 0;
            this.PhotoName = "";
            this.HtmlName = "";
            this.Showorder = 0;
            this.Class = 1;
            this.Status = 1;
            this.StatusNote = "";
            this.StatusDate = defaultDate;
            this.Note = "";
            this.MarketPrice = 0;
            this.PicStart = 0;
            this.PicEnd = 0;

        }
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// ProduttempID：商品暫存檔ID
        /// </summary>
        public Nullable<int> ProduttempID { get; set; }
        /// <summary>
        /// ItemID：賣場正式檔ID
        /// </summary>
        public Nullable<int> ItemID { get; set; }
        /// <summary>
        /// ItemStatus：item正式檔上下架狀態
        /// </summary>
        public Nullable<int> ItemStatus { get; set; }
        public string Name { get; set; }
        public string Sdesc { get; set; }
        public string DescTW { get; set; }
        public string ItemTempDesc { get; set; }
        public string SpecDetail { get; set; }
        public string Spechead { get; set; }
        public int SaleType { get; set; }
        public int PayType { get; set; }
        public int Layout { get; set; }
        public int DelvType { get; set; }
        public string DelvData { get; set; }
        public string ItemNumber { get; set; }
        public Nullable<int> ProductID { get; set; }
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
        public decimal Pricehpinst1 { get; set; }
        public int PricehpType2 { get; set; }
        public decimal Pricehpinst2 { get; set; }
        public int Inst0Rate { get; set; }
        public decimal RedmfdbckRate { get; set; }
        public string Coupon { get; set; }
        public decimal PriceCoupon { get; set; }
        public decimal PriceLocalship { get; set; }
        public decimal PriceGlobalship { get; set; }
        public int PriceTrade { get; set; }
        public int Qty { get; set; }
        public int SafeQty { get; set; }
        public int QtyLimit { get; set; }
        public string LimitRule { get; set; }
        public int QtyReg { get; set; }
        public string PhotoName { get; set; }
        public string HtmlName { get; set; }
        public int Showorder { get; set; }
        public int Class { get; set; }
        public int Status { get; set; }
        public int ManufactureID { get; set; }
        public string StatusNote { get; set; }
        public System.DateTime StatusDate { get; set; }
        public string Note { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> PicStart { get; set; }
        public Nullable<int> PicEnd { get; set; }
        public Nullable<int> WareHouseID { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        /// <summary>
        /// ShipType：運送類型；N：Newegg運送；S：Seller運送；V：Vendor運送
        /// </summary>
        public string ShipType { get; set; }
        public Nullable<decimal> Taxfee { get; set; }
        /// <summary>
        /// ItemPackage：包裹類型；0：Retail(零售)；1：OEM
        /// </summary>
        public string ItemPackage { get; set; }
        /// <summary>
        /// IsNew：商品狀態；Y：全新；N：二手
        /// </summary>
        public string IsNew { get; set; }
        /// <summary>
        /// 毛利率
        /// </summary>
        public Nullable<decimal> GrossMargin { get; set; }
        /// <summary>
        /// 審核人員
        /// </summary>
        public string ApproveMan { get; set; }
        /// <summary>
        /// 審核日期
        /// </summary>
        public Nullable<DateTime> ApproveDate { get; set; }
        /// <summary>
        /// 送審人員
        /// </summary>
        public string SubmitMan { get; set; }
        /// <summary>
        /// 送審日期
        /// </summary>
        public Nullable<DateTime> SubmitDate { get; set; }

        /// <summary>
        /// 依據 BSATW-173 廢四機需求增加
        /// 癈四機賣場商品, Y=是癈四機 ---------------add by bruce 20160428
        /// </summary>
        public string Discard4 { get; set; }
    }
}
