using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels
{
    public class ViewTracksCartItems
    {
        public enum ViewTracksType
        {
            海外購物車 = 10,
	        海外下次買 = 11,
	        海外追蹤 = 12,
	        一般宅配 = 0,
	        一般下次買 = 1,
            一般追蹤 = 2,
            國內購物車加價商品 = 100,
            海外購物車加價商品 = 101,
            任選館購物車加價商品 = 102

        };

        [Key]
        public int ItemID { get; set; }
        public int ItemDelvType { get; set; }
        public string ItemDelvDate { get; set; }
        /// <summary>
        /// 商品可售數量
        /// </summary>
        public int ItemSellingQty { get; set; }
        public int ItemSellerID { get; set; }
        public string ItemSaleType { get; set; }
        public string TrackCreateDateString { get; set; }
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
        public int CategoryID { get; set; }
        /// <summary>
        /// 使用者購買數量
        /// </summary>
        public int? TrackQty { get; set; }
        public decimal? DisplayPrice { get; set; }
        public decimal? DisplayTax { get; set; }
        public decimal? DisplayShipping { get; set; }
        public DateTime? TrackCreateDate { get; set; }
    }
}
