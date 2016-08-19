using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("item")]
    public class Item
    {
        public enum status
        {
            已上架 = 0,
            未上架 = 1,
            強制下架=2,
            系統下架=3,
            刪除 = 99
        };
        public enum tradestatus
        {
            切貨 = 0,
            間配 = 1,
            直配 = 2,
            三角 = 3,
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
        public enum ItemPackagestatus
        {
            Retail=0,
            OEM=1
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
            DateTime defaultDate = DateTime.Now;
            DateStart = defaultDate;
            DateEnd = DateStart.AddYears(100);
            DateDel = DateEnd.AddDays(1);
            StatusDate = defaultDate;
            CreateDate = defaultDate;
            this.Status = (byte)status.未上架;
            this.SaleType = 1;
            this.PayType = 0;
            this.Layout = 0;
            this.Itemnumber = "";
            this.Model = "";
            this.SpecDetail = "";
            this.PriceCard = 0;
            this.PricehpType1 = 0;
            this.PricehpInst1 = 0;
            this.PricehpType2 = 0;
            this.PricehpInst2 = 0;
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
            this.ShowOrder = 0;
            this.Class = 1;
            this.Status = 1;
            this.StatusNote = "";
            this.StatusDate = defaultDate;
            this.Note = "";
            this.MarketPrice = 0;
            this.PicStart = 0;
            this.PicEnd = 0;
        }
        /// <summary>
        /// 上架商品編號
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Nullable<int> ItemtempID { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 英文名稱
        /// </summary>
        public string ItemDesc { get; set; }
        /// <summary>
        /// 中文描述
        /// </summary>
        public string DescTW { get; set; }
        /// <summary>
        /// 特色標題
        /// </summary>
        public string Sdesc { get; set; }
        /// <summary>
        /// DetailInfo=product.spec
        /// </summary>
        public string SpecDetail { get; set; }
        /// <summary>
        /// 商品簡要描述(灰色描述)
        /// </summary>
        public string Spechead { get; set; }
        /// <summary>
        /// 1:一般
        /// </summary>
        public int SaleType { get; set; }
        /// <summary>
        /// 0 (付款方式)
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// 0
        /// </summary>
        public int Layout { get; set; }
        /// <summary>
        /// DetailInfo=product.spec
        /// </summary>
        public int DelvType { get; set; }
        /// <summary>
        /// 到貨資訊(7-9)天
        /// </summary>
        public string DelvDate { get; set; }
        /// <summary>
        /// 來源賣場編號
        /// </summary>
        public string Itemnumber { get; set; }
        /// <summary>
        /// 產品編號
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// 商品分類ID
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Produce.model
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 商家ID
        /// </summary>
        public int SellerID { get; set; }
        /// <summary>
        /// 賣場開始日期
        /// </summary>
        public System.DateTime DateStart { get; set; }
        /// <summary>
        /// 賣場結束日期
        /// </summary>
        public System.DateTime DateEnd { get; set; }
        /// <summary>
        /// 賣場刪除日期default = DateEnd + 1天
        /// </summary>
        public System.DateTime DateDel { get; set; }
        public decimal Pricesgst { get; set; }
        /// <summary>
        /// 刷卡價 = 現金價
        /// </summary>
        public decimal PriceCard { get; set; }
        /// <summary>
        /// 現金價
        /// </summary>
        public decimal PriceCash { get; set; }
        /// <summary>
        /// 服務費=0
        /// </summary>
        public decimal ServicePrice { get; set; }
        /// <summary>
        /// 分期期數一 0
        /// </summary>
        public int PricehpType1 { get; set; }
        /// <summary>
        /// 分期利息一 0
        /// </summary>
        public decimal PricehpInst1 { get; set; }
        /// <summary>
        /// 分期期數二 0
        /// </summary>
        public int PricehpType2 { get; set; }
        /// <summary>
        /// 分期利息二 0
        /// </summary>
        public decimal PricehpInst2 { get; set; }
        /// <summary>
        /// 零利率分期期數 0
        /// </summary>
        public int Inst0Rate { get; set; }
        /// <summary>
        /// 回饋比例 0
        /// </summary>
        public decimal RedmfdbckRate { get; set; }
        /// <summary>
        /// 折價券編號 0
        /// </summary>
        public string Coupon { get; set; }
        /// <summary>
        /// 折價券金額 0
        /// </summary>
        public decimal PriceCoupon { get; set; }
        /// <summary>
        /// 本地運費 0
        /// </summary>
        public decimal PriceLocalship { get; set; }
        /// <summary>
        /// 國際運費
        /// </summary>
        public decimal PriceGlobalship { get; set; }
        /// <summary>
        /// 限量數量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 安全警告數量
        /// </summary>
        public int SafeQty { get; set; }
        /// <summary>
        /// 限購數量
        /// </summary>
        public int QtyLimit { get; set; }
        /// <summary>
        /// 限購規則
        /// </summary>
        public string LimitRule { get; set; }
        /// <summary>
        /// 已登記數量 0
        /// </summary>
        public int QtyReg { get; set; }
        /// <summary>
        /// 圖檔名稱
        /// </summary>
        public string PhotoName { get; set; }
        /// <summary>
        /// 網頁名稱
        /// </summary>
        public string HtmlName { get; set; }
        /// <summary>
        /// 顯示順序 0 
        /// -1:隱形
        /// </summary>
        public int ShowOrder { get; set; }
        public int Class { get; set; }
        /// <summary>
        /// 上下架狀態
        /// 0:上架
        /// 1:下架、未上架
        /// 2:強制下架
        /// 3:售價異常(系統判斷下架)
        /// 99:刪除
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 製造商ID
        /// </summary>
        public int ManufactureID { get; set; }
        /// <summary>
        /// 狀態備註
        /// </summary>
        public string StatusNote { get; set; }
        /// <summary>
        /// 狀態最後更改時間
        /// </summary>
        public System.DateTime StatusDate { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// If 7張圖片 填1
        /// </summary>
        public Nullable<int> PicStart { get; set; }
        /// <summary>
        ///  If 7張圖片 填7
        /// </summary>
        public Nullable<int> PicEnd { get; set; }
        /// <summary>
        /// 建檔日期
        /// </summary>
        public System.DateTime CreateDate { get; set; }
        /// <summary>
        /// 建立者
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 0
        /// </summary>
        public int Updated { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
        //2013.11.1 add columns by Ice begin
        //public Nullable<decimal> CommissionFee { get; set; }
        //public Nullable<decimal> StorageFee { get; set; }
        /// <summary>
        /// 運費
        /// </summary>
        public Nullable<decimal> ShipFee { get; set; }
        //2013.11.1 add columns by Ice end
        //2013.11.14 add columns by Ice begin
        /// <summary>
        /// 市場建議售價
        /// </summary>
        public Nullable<decimal> MarketPrice { get; set; }
        /// <summary>
        /// 運送類型
        /// </summary>
        public string ShipType { get; set; }
        /// <summary>
        /// 運送類型
        /// N:Newegg
        /// S:Seller
        /// </summary>
        public Nullable<decimal> Taxfee { get; set; }
        //public string UPC { get; set; }                   //2013.11.27 搬移至product mark by Ice
        //public string SellerPartNum { get; set; }         //2013.11.27 搬移至product mark by Ice
        //public string MenufacturePartNum { get; set; }    //2013.11.27 搬移至product mark by Ice
        /// <summary>
        /// 包裹類型
        /// 0:Retail(零售)
        /// 1:OEM
        /// </summary>
        public string ItemPackage { get; set; }
        public Nullable<int> WarehouseID { get; set; }//Penny
        //public Nullable<decimal> Finalprice { get; set; }
        //2013.11.14 add columns by Ice end
        /// <summary>
        /// IsNew 
        /// Y:全新
        /// N:二手
        /// </summary>
        public string IsNew { get; set; }
        //public string IsApportioned { get; set; }
        /// <summary>
        /// Discard4 
        /// Y:是廢四機
        /// N:廢四機
        /// </summary>
        public string Discard4 { get; set; }
    }
}