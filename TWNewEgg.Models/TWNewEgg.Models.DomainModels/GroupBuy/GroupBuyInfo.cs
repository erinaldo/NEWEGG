using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Models.DomainModels.GroupBuy
{
    public class GroupBuyInfo
    {
        public GroupBuyInfo() {
            this.IsAllowEdit = true;
        }

        public int ID { get; set; }
        public int ItemID { get; set; }
        public string Title { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal GroupBuyPrice { get; set; }
        public decimal ProductCost { get; set; }
        public decimal ShippingCost { get; set; }
        public int SalesOrderLimit { get; set; }
        public int SalesOrderBase { get; set; }
        public int SalesOrderCurrentBuffer { get; set; }
        public bool IsExclusive { get; set; }
        public bool IsNeweggUSASync { get; set; }
        public string PromoText { get; set; }
        public string ImgUrl { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string RejectCause { get; set; }
        public string Status { get; set; }
        public string InUser { get; set; }
        public string InDate { get; set; }
        public string EditUser { get; set; }
        public string EditDate { get; set; }
        public string Discount { get; set; }
        public string DiscountPercentage { get; set; }
        public string AvailableSO { get; set; }
        public bool IsAllowEdit { get; set; }
        public bool ISAllowDelete { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAllowHide { get; set; }
        public bool IsHide { get; set; }
        public string SellQuantity { get; set; }
        public string Sdesc { get; set; }
    }
}