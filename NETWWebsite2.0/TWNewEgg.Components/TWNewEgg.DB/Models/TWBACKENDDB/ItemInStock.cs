using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Web.WebPages.Html;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("ItemInStock")]
    public class ItemInStock
    {
        public ItemInStock(List<ItemInStockV2> ItemInStockV2List)
        {
            if (ItemInStockV2List != null && ItemInStockV2List.Count != 0)
            {
                this.id = ItemInStockV2List.FirstOrDefault().id;
                this.SellerProductId = ItemInStockV2List.FirstOrDefault().SellerProductId;
                this.ProductId = ItemInStockV2List.FirstOrDefault().ProductId;
                this.WarehouseNumber = ItemInStockV2List.FirstOrDefault().WarehouseNumber;
                this.InstockQuantity = ItemInStockV2List.Sum(x => x.InstockQuantity);
                this.AverageCost = ItemInStockV2List.Average(x => x.AverageCost);
                this.TotalCost = ItemInStockV2List.Average(x => x.TotalCost);
                this.TotalShippingFee = ItemInStockV2List.Average(x => x.TotalShippingFee);
                this.TotalTaxandDuty = ItemInStockV2List.Average(x => x.TotalTaxandDuty);
                this.TotalCustomsCharges = ItemInStockV2List.Average(x => x.TotalCustomsCharges);
                this.TotalServiceCharges = ItemInStockV2List.Average(x => x.TotalServiceCharges);
                this.LastCost = ItemInStockV2List.Average(x => x.LastCost);
                this.RMAGood = ItemInStockV2List.Sum(x => x.RMAGood);
                this.RMABad = ItemInStockV2List.Sum(x => x.RMABad);
                this.RMAOnVendor = ItemInStockV2List.FirstOrDefault().RMAOnVendor;
                this.RMADead = ItemInStockV2List.Sum(x => x.RMADead);
                this.Borrow = ItemInStockV2List.FirstOrDefault().Borrow;
                this.ExchangePrice = ItemInStockV2List.Average(x => x.LastCost);
                this.ConsignmentQuantity = ItemInStockV2List.Sum(x => x.RMABad);
                this.Available = ItemInStockV2List.FirstOrDefault().Borrow;
                this.CurrencyCode = ItemInStockV2List.FirstOrDefault().CurrencyCode;
                this.createdate = ItemInStockV2List.FirstOrDefault().createdate;
                this.createuser = ItemInStockV2List.FirstOrDefault().createuser;
                this.updated = ItemInStockV2List.FirstOrDefault().updated;
                this.updateuser = ItemInStockV2List.FirstOrDefault().updateuser;
                this.updatedate = ItemInStockV2List.FirstOrDefault().updatedate;
                //this.iteminstocktrans = 
                this.WarehouseNumber = ItemInStockV2List.FirstOrDefault().WarehouseNumber;
            }
            else
            {
                this.InstockQuantity = 0;
                this.AverageCost = 0;
                this.TotalCost = 0;
                this.TotalShippingFee = 0;
                this.TotalTaxandDuty = 0;
                this.TotalCustomsCharges = 0;
                this.TotalServiceCharges = 0;
                this.LastCost = 0;
                this.RMAGood = 0;
                this.RMABad = 0;
                this.RMADead = 0;
                this.ExchangePrice = 0;
                this.ConsignmentQuantity = 0;
            }
        }
        public ItemInStock()
        {
        }

        [Key]
        public int id { get; set; }
        public string SellerProductId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> WarehouseNumber { get; set; }
        public int InstockQuantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal TotalCost { get; set; }
        public Nullable<int> companycode { get; set; } //2014/06/27 add by Bill
        public Nullable<decimal> TotalShippingFee { get; set; }     //2014/01/23 add by Bill
        public Nullable<decimal> TotalTaxandDuty { get; set; }      //2014/01/23 add by Bill
        public Nullable<decimal> TotalCustomsCharges { get; set; }  //2014/01/23 add by Bill
        public Nullable<decimal> TotalServiceCharges { get; set; }  //2014/01/23 add by Bill
        public Nullable<decimal> LastCost { get; set; }
        public Nullable<int> RMAGood { get; set; }
        public Nullable<int> RMABad { get; set; }
        public Nullable<int> RMAOnVendor { get; set; }
        public Nullable<int> RMADead { get; set; }
        public Nullable<int> Borrow { get; set; }
        public Nullable<decimal> ExchangePrice { get; set; }
        public Nullable<int> ConsignmentQuantity { get; set; }
        public Nullable<int> Available { get; set; }
        public string CurrencyCode { get; set; }
        public Nullable<DateTime> createdate { get; set; }
        public string createuser { get; set; }
        public Nullable<int> updated { get; set; }
        public string updateuser { get; set; }
        public Nullable<DateTime> updatedate { get; set; }
        public virtual ICollection<ItemInStock_trans> iteminstocktrans { get; set; }

        public string getWarehouseNumber()
        {
            switch (this.WarehouseNumber)
            {
                case 0:
                    return "0";
                case 8801:
                    return "8801";
                case 8802:
                    return "8802";
                case 8821:
                    return "8821";
                case 0108:
                    return "0108";
                default:
                    return "NULL";
            }
        }
    }
    public class ItemInStockDetail
    {
        public ItemInStock iteminstock { get; set; }
        public TWNewEgg.DB.TWSQLDB.Models.Product product { get; set; }
        public TWNewEgg.DB.TWSQLDB.Models.Seller seller { get; set; }
    }

    [DisplayName("庫存查詢")]
    [DisplayColumn("productid")]
    public class ItemInStockSearchModel
    {
        public enum transTypeWord
        {
            進貨單 = 1,
            出貨單 = 2,
            退貨單 = 3,
            調整單 = 5
        };

        [DisplayName("商品編號")]
        public string productid { get; set; }

        [DisplayName("供應商商品編號")]
        public string sellerproductid { get; set; }

        [DisplayName("存貨建立日期")]
        public DateTime? createdatestart { get; set; }
        public DateTime? createdateend { get; set; }

        [DisplayName("存貨更新日期")]
        public DateTime? updatedatestart { get; set; }
        public DateTime? updatedateend { get; set; }

        [DisplayName("資料時間點")]
        public DateTime? datadate { get; set; }
        public String datatime { get; set; }

        [DisplayName("交易模式")]
        public string delvtype { get; set; }
        public List<SelectListItem> delvtypeList;

        [DisplayName("倉別代碼")]
        public string whid { get; set; }
        public List<SelectListItem> whidList;

        [DisplayName("商品所有權")]
        public string sellerid { get; set; }
        public List<SelectListItem> selleridList;

        [DisplayName("子單建立日期")]
        public DateTime? transdatestart { get; set; }
        public DateTime? transdateend { get; set; }

        [DisplayName("子單類型")]
        public string transType { get; set; }
        public List<SelectListItem> transTypeList;

        [DisplayName("庫存0不顯示")]
        public bool dontShowZero { get; set; }

        public ItemInStockSearchModel()
        {
            delvtypeList = new List<SelectListItem>();
            delvtypeList.Add(new SelectListItem() { Value = "0", Text = "切貨" }); // 切貨實際為"0" 於DBUtil判斷
            delvtypeList.Add(new SelectListItem() { Value = "1", Text = "間配" });
            delvtypeList.Add(new SelectListItem() { Value = "2", Text = "直配" });
            delvtypeList.Add(new SelectListItem() { Value = "3", Text = "三角" });
            delvtypeList.Add(new SelectListItem() { Value = "4", Text = "國外直配" });
            delvtypeList.Add(new SelectListItem() { Value = "5", Text = "自貿區" });
            delvtypeList.Add(new SelectListItem() { Value = "6", Text = "海外切貨" });
            delvtypeList.Add(new SelectListItem() { Value = "7", Text = "B2C直配" }); // 切貨實際為"0" 於DBUtil判斷
            delvtypeList.Add(new SelectListItem() { Value = "8", Text = "MKPL寄倉" });
            delvtypeList.Add(new SelectListItem() { Value = "9", Text = "B2C寄倉" }); // 切貨實際為"0" 於DBUtil判斷

            whidList = new List<SelectListItem>();
            whidList.Add(new SelectListItem() { Value = "1", Text = "WH0" }); // 三角實際為"0" 於DBUtil判斷
            whidList.Add(new SelectListItem() { Value = "8801", Text = "8801" });
            whidList.Add(new SelectListItem() { Value = "8802", Text = "8802" });
            whidList.Add(new SelectListItem() { Value = "8821", Text = "8821" });
            whidList.Add(new SelectListItem() { Value = "0108", Text = "0108" });

            selleridList = new List<SelectListItem>();
            selleridList.Add(new SelectListItem() { Value = "1", Text = "Newegg TW" }); // 三角實際為"0" 於DBUtil判斷
            selleridList.Add(new SelectListItem() { Value = "2", Text = "Newegg USA" });
            selleridList.Add(new SelectListItem() { Value = "3", Text = "OMC 強越資訊" });
            selleridList.Add(new SelectListItem() { Value = "5", Text = "JieMai網" });
            selleridList.Add(new SelectListItem() { Value = "6", Text = "Newegg CN" });

            transTypeList = new List<SelectListItem>();
            transTypeList.Add(new SelectListItem() { Value = ((int)transTypeWord.進貨單).ToString(), Text = transTypeWord.進貨單.ToString() });
            transTypeList.Add(new SelectListItem() { Value = ((int)transTypeWord.出貨單).ToString(), Text = transTypeWord.出貨單.ToString() });
            transTypeList.Add(new SelectListItem() { Value = ((int)transTypeWord.退貨單).ToString(), Text = transTypeWord.退貨單.ToString() });
            transTypeList.Add(new SelectListItem() { Value = ((int)transTypeWord.調整單).ToString(), Text = transTypeWord.調整單.ToString() });
        }
    }
}