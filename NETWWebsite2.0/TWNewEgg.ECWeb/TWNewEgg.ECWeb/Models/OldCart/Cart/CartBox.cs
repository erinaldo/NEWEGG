using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace TWNewEgg.Website.ECWeb.Models
{
    
    public class CartBox
    {
        public CartBox()
        {
            this.item_sellerid = 0;
            this.item_productid = 0;
            this.item_id = 0;
            this.itemlist_id = 0;
            this.overseaflag = 0;
            this.item_sellercountry = 0;
            this.item_name = "";
            this.item_attribname = ""; 
            this.item_localprice = "";
            this.item_twprice = 0;
            this.item_qty = 0;
            this.item_tax = 0;
            this.item_price = 0;
            this.item_delvdate = "";
            this.states_price = 0;
            this.shipping = 0;
            this.servicefees = 0;
            this.states_pricesum = 0;
            this.pricesum = 0;

            this.Tariff = 0;
            this.VAT = 0;
            this.Excise = 0;
            this.TPSC = 0;
        }
        public int item_sellerid { get; set; }
        public int item_productid { get; set; }
        public int? item_id { get; set; }
        public int itemlist_id { get; set; }
        public int overseaflag { get; set; }
        public int? item_sellercountry { get; set; }
        public string item_name { get; set; }
        public string item_attribname { get; set; }
        public string item_localprice { get; set; }
        public decimal item_twprice { get; set; }
        public int item_qty { get; set; }
        public decimal item_tax { get; set; }
        public decimal item_price { get; set; }
        public string item_delvdate { get; set; }
        public string seller_name { get; set; }
        public string country_name { get; set; }
        public decimal? states_price { get; set; }
        public decimal shipping { get; set; }
        public decimal servicefees { get; set; }
        public decimal? states_pricesum { get; set; }
        public decimal pricesum { get; set; }
        
        public decimal Tariff { get; set; } // 預估關稅
        public decimal VAT { get; set; }    // 預估VAT(value added tax) 加值稅
        public decimal Excise { get; set; } // 預估貨物稅
        public decimal TPSC { get; set; }   // Trade promotion service charges 推廣貿易服務費
    }
}