using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public class CartItems
    {
        /*
        [Key]
        public int item_id { get; set; }
        public Byte track_status { get; set; }
        public DateTime track_createdate { get; set; }
        public string itemlist_selecttype { get; set; }
        public int item_sellingQty { get; set; }
        public int item_sellerid { get; set; }
        public string item_name { get; set; }
        public int item_pricecash { get; set; }
        public Byte item_layout { get; set; }
        public string item_image { get; set; }
        public int itemlist_id { get; set; }
        public int itemlist_sellingQty { get; set; }
        public int itemlist_sellerid { get; set; }
        public int itemlist_itemlistid { get; set; }
        public string itemlist_name { get; set; }
        public int itemlist_price { get; set; }
        public Byte itemlist_type { get; set; }
        public Byte item_saletype { get; set; }
        public Byte item_producttype { get; set; }
        public Byte item_showorder { get; set; }
        public int itemlist_order { get; set; }
        //public int itemlistgroup_order { get; set; }
        */

        //[Key, Column(Order = 0)]
        [Key]
        public int ItemID { get; set; }
        public int ItemDelvType { get; set; }
        public string ItemDelvDate { get; set; }
        public int ItemSellingQty { get; set; }
        public int ItemSellerID { get; set; }
        public string ItemSaleType { get; set; }
        public string TrackCreateDate { get; set; }
        public int ItemShowOrder { get; set; }
        public string ItemImage { get; set; }
        public string ItemProductType { get; set; }
        public int? ItemListSelectType { get; set; }
        //[Key, Column(Order = 1)]
        public int? ItemListID { get; set; }
        public int? ItemListSellingQty { get; set; }
        public int? ItemListSellerID { get; set; }
        public int? ItemListItemListID { get; set; }
        public string ItemListType { get; set; }
        public string ItemListName { get; set; }
        public decimal ItemPriceCash { get; set; }
        //public string item_attribname { get; set; }
        public string ItemListAttribName { get; set; }
        public int TrackStatus { get; set; }
        public string ItemName { get; set; }
        public int ItemLayout { get; set; }
        public decimal? ItemListPrice { get; set; }
        public int? ItemListOrder { get; set; }


        public decimal? ItemProductLength { get; set; }
        public decimal? ItemProductWidth { get; set; }
        public decimal? ItemProductHeight { get; set; }
        public decimal? ItemProductWeight { get; set; }
        public decimal? ItemListProductLength { get; set; }
        public decimal? ItemListProductWidth { get; set; }
        public decimal? ItemlistProductHeight { get; set; }
        public decimal? ItemListProductWeight { get; set; }
        //public int ItemCategoryID { get; set; }
        //public string item_pricedetail { get; set; }
        //public string itemlist_pricedetail { get; set; }


    }
}