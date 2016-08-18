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
    public class ItemCartBox
    {
        public ItemCartBox()
        {
            this.Item_DelvType = 0;
            this.Item_SellerID = 0;
            this.Item_ProductID = 0;
            this.Item_ID = 0;
            this.Item_AttrID = 0;
            //this.ItemList_ID = 0;
            this.OverSeaFlag = 0;
            this.Item_SellerCountry = 0;
            this.Item_Name = "";
            this.Item_AttribName = "";
            this.Item_LocalPrice = "";
            this.Item_TWPrice = 0;
            this.Item_Qty = 0;
            this.Item_Tax = 0;
            this.Item_Price = 0;
            this.Item_DelvDate = "";
            this.Seller_Name = "";
            this.Country_Name = "";
            this.States_Price = 0;
            this.Shipping = 0;
            this.ServiceFees = 0;
            this.ItemSum = 0; // 商品總計
            this.States_Shipping = 0;
            this.States_PriceSum = 0; // 商品區域總計
            this.Pricesum = 0;
            this.ItemList = new List<ItemCartBox>();
            this.Tariff = 0; // 預估關稅
            this.VAT = 0;    // 預估VAT(value added tax) 加值稅
            this.Excise = 0; // 預估貨物稅
            this.TPSC = 0;   // Trade promotion service charges 推廣貿易服務費
            this.OverSeaTax1 = 0;
            this.OverSeaTax2 = 0;
            this.TaxPriceSum = 0;
        }
        public int Item_DelvType { get; set; }
        public int Item_SellerID { get; set; }
        public int Item_ProductID { get; set; }
        public int Item_ID { get; set; }
        public int Item_AttrID { get; set; }
        //public int ItemList_ID { get; set; }
        public string Item_Url { get; set; }
        public int OverSeaFlag { get; set; }
        public int Item_SellerCountry { get; set; }
        public string Item_Name { get; set; }
        public string Item_AttribName { get; set; }
        public string Item_LocalPrice { get; set; }
        public decimal Item_TWPrice { get; set; }
        public int Item_Qty { get; set; }
        public decimal Item_Tax { get; set; }
        public decimal Item_Price { get; set; }
        public string Item_DelvDate { get; set; }
        public string Seller_Name { get; set; }
        public string Country_Name { get; set; }
        public decimal States_Price { get; set; }
        public decimal Shipping { get; set; }
        public decimal ServiceFees { get; set; }
        public decimal ItemSum { get; set; } // 商品總計
        public decimal States_Shipping { get; set; }
        public decimal States_PriceSum { get; set; }
        public decimal Pricesum { get; set; }

        public List<ItemCartBox> ItemList { get; set; }
        
        public decimal Tariff { get; set; } // 預估關稅
        public decimal VAT { get; set; }    // 預估VAT(value added tax) 加值稅
        public decimal Excise { get; set; } // 預估貨物稅
        public decimal TPSC { get; set; }   // Trade promotion service charges 推廣貿易服務費
        public decimal OverSeaTax1 { get; set; } // 預估進口報關費
        public int OverSeaTax1Num { get; set; }  // 預估進口報關費數
        public decimal OverSeaTax2 { get; set; } // 預估商檢費
        public int OverSeaTax2Num { get; set; }  // 預估商檢費數
        public decimal TaxPriceSum { get; set; } // 稅金總額

        public decimal SinglePrice { get; set; } // 單個商品總價
        public decimal SinglePriceMultiplyQty { get; set; } // 單個商品總價 * 數量
        public decimal DiscountPrice { get; set; } // 折扣金額
        public decimal ActualPrice { get; set; } // 實際購買金額
    }
}