using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class ResultCart
    {
        public ResultCart()
        {
            this.itemSONumber = "";
            this.itemID = 0;
            this.itemAttrID = 0;
            this.itemCountry = 0;
            this.itemCountryName = "";
            this.itemSeller = "";
            this.itemSellerID = 0;
            this.itemName = "";
            this.itemAttribname = ""; 
            this.itemLocalprice = "---";
            this.itemTWprice = 0;
            this.itemQty = 0;
            this.itemTax = 0;
            this.itemPrice = 0;
            this.itemType = "";
            this.statesPrice = 0;
            this.shipping = 0;
            this.serviceFees = 0;
            this.statesPricesum = 0;
            this.priceSum = 0;
            this.displayPrice = 0;
            this.discount = 0;
            this.realPrice = 0;
            this.productid = 0;
            this.itemList = new List<ResultCart>();
            this.itemOriTax = "";
            this.Coupons = "";
            this.Pricecoupon = 0;
            this.apportionedAmount = 0;
            this.InstallmentFee = 0m;
        }
        public string itemSONumber { get; set; }
        public int itemID { get; set; }
        public int itemAttrID { get; set; }
        public string itemSeller { get; set; }
        public int itemSellerID { get; set; }
        public int itemCountry { get; set; }
        public string itemCountryName { get; set; }
        public string itemName { get; set; }
        public string itemAttribname { get; set; }
        public string itemLocalprice { get; set; }
        public decimal? itemTWprice { get; set; }
        public int itemQty { get; set; }
        public decimal? itemTax { get; set; }
        public decimal? itemPrice { get; set; }
        public string itemType { get; set; }
        public decimal? statesPrice { get; set; }
        public decimal? shipping { get; set; }
        public decimal? serviceFees { get; set; }
        public decimal? statesPricesum { get; set; }
        public decimal? priceSum { get; set; }
        public decimal displayPrice { get; set; }
        public decimal discount { get; set; }
        public decimal realPrice { get; set; }
        public int productid { get; set; }
        public List<ResultCart> itemList { get; set; }

        public string itemOriTax { get; set; }
        public string Coupons { get; set; }
        public Nullable<decimal> Pricecoupon { get; set; }
        public Nullable<decimal> apportionedAmount { get; set; }
        public decimal InstallmentFee { get; set; }

        public string PromtionGiftString { get; set; }
        public string CouponString { get; set; }
    }

}