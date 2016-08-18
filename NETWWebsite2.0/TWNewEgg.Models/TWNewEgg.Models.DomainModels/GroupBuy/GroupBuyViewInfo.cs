using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Models.DomainModels.GroupBuy
{
    public class GroupBuyViewInfo
    {
        public int ID { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsExpired { get; set; }
        public bool IsSoldOut { get; set; }
        public bool IsShowNew { get; set; }
        public bool IsShowHot { get; set; }
        public bool IsShowExclusive { get; set; }
        public bool IsShowNeweggUSASync { get; set; }
        public string Title { get; set; }
        public string PromoText { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string OriginalPrice { get; set; }
        public string GroupBuyPrice { get; set; }
        public string Discount { get; set; }
        public string DiscountPercentage { get; set; }
        public string SalesOrderCount { get; set; }
        public string ItemLinkButtonImageUrl { get; set; }
        public string ItemLinkButtonText { get; set; }
        public string ItemLink { get; set; }
        public string SellQuantity { get; set; }
        public string Sdesc { get; set; }
        public string AdCopy { get; set; }
        public string ImgUrl { get; set; }
        public int ItemID { get; set; }
    }
}