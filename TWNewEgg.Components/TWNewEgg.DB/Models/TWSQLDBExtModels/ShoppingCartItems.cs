using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public class ShoppingCartItems
    {
        public int TrackStatus { get; set; }
        [Key]
        public int ItemID { get; set; }
        public int ItemSellingQty { get; set; }
        public int ItemSellerID { get; set; }
        public string TrackCreateDate { get; set; }
        public int? ItemShowOrder { get; set; }
        public string ItemImage { get; set; }
        public string ItemProductType { get; set; }
        public int? ItemListSelectType { get; set; }

        public decimal? ItemProductLength { get; set; }
        public decimal? ItemProductWidth { get; set; }
        public decimal? ItemProductHeight { get; set; }
        public decimal? ItemProductWeight { get; set; }
        public decimal? ItemListProductLength { get; set; }
        public decimal? ItemListProductWidth { get; set; }
        public decimal? ItemlistProductHeight { get; set; }
        public decimal? ItemListProductWeight { get; set; }

        public decimal ItemPriceCash { get; set; }
        public string ItemName { get; set; }
        public int? ItemLayout { get; set; }
        public string ItemSaleType { get; set; }
        //public int item_productid { get; set; }
        //public Int32 item_pricecard { get; set; }
        //public DateTime item_datestart { get; set; }
        //public DateTime item_dateend { get; set; }
        //public DateTime item_datedel { get; set; }
        //public string item_coupon { get; set; }
        //public Byte item_inst0rate { get; set; }
        //public Decimal item_pricecoupon { get; set; }
        //public Byte item_paytype { get; set; }
        //public Byte item_delvtype { get; set; }
        //public string item_attribname { get; set; }






        public int? ItemListID { get; set; }
        public int? ItemListSellingQty { get; set; }
        public int? ItemListSellerID { get; set; }
        public int? ItemListItemListID { get; set; }
        public string ItemListType { get; set; }
        public string ItemListName { get; set; }
        public string ItemListAttribName { get; set; }
        public decimal? ItemListPrice { get; set; }
        public int? ItemListOrder { get; set; }



        //public string item_pricedetail { get; set; }
        //public string itemlist_pricedetail { get; set; }

        public ShoppingCartItems()
        {
        }

        public ShoppingCartItems(ShoppingCartItems previousItem)
        {
            this.TrackStatus = previousItem.TrackStatus;
            this.ItemID = previousItem.ItemID;
            this.ItemSellingQty = previousItem.ItemSellingQty;
            this.ItemSellerID = previousItem.ItemSellerID;
            this.TrackCreateDate = previousItem.TrackCreateDate;
            this.ItemShowOrder = previousItem.ItemShowOrder;
            this.ItemImage = previousItem.ItemImage;
            this.ItemProductType = previousItem.ItemProductType;
            this.ItemListSelectType = previousItem.ItemListSelectType;

            this.ItemProductLength = previousItem.ItemProductLength;
            this.ItemProductWidth = previousItem.ItemProductWidth;
            this.ItemProductHeight = previousItem.ItemProductHeight;
            this.ItemProductWeight = previousItem.ItemProductWeight;
            this.ItemListProductLength = previousItem.ItemListProductLength;
            this.ItemListProductWidth = previousItem.ItemListProductWidth;
            this.ItemlistProductHeight = previousItem.ItemlistProductHeight;
            this.ItemListProductWeight = previousItem.ItemListProductWeight;

            this.ItemPriceCash = previousItem.ItemPriceCash;
            this.ItemName = previousItem.ItemName;
            this.ItemLayout = previousItem.ItemLayout;
            this.ItemSaleType = previousItem.ItemSaleType;








            this.ItemListID = previousItem.ItemListID;
            this.ItemListSellingQty = previousItem.ItemListSellingQty;
            this.ItemListSellerID = previousItem.ItemListSellerID;
            this.ItemListItemListID = previousItem.ItemListItemListID;
            this.ItemListType = previousItem.ItemListType;
            this.ItemListName = previousItem.ItemListName;
            this.ItemListAttribName = previousItem.ItemListAttribName;
            this.ItemListPrice = previousItem.ItemListPrice;
            this.ItemListOrder = previousItem.ItemListOrder;
        }
    }
}