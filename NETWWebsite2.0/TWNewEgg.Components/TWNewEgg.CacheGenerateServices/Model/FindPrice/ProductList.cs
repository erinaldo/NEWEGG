using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TWNewEgg.CacheGenerateServices.Model.FindPrice
{
    [XmlRoot("productList")]
    public class ProductLists
    {
        [XmlElement("product")]
        public List<Products> products = new List<Products>();
    }

    public class Products
    {
        /// <summary>
        /// 產品ID
        /// </summary>
        //[XmlElement("product_id")]
        public string itemID { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        [XmlElement("product_name")]
        public string itemName { get; set; }

        /// <summary>
        /// 產品說明
        /// </summary>
        [XmlElement("product_description")]
        public string itemDescription { get; set; }

        /// <summary>
        /// 產品頁
        /// </summary>
        [XmlElement("product_url")]
        public string itemUrl { get; set; }

        /// <summary>
        /// 圖片url
        /// </summary>
        [XmlElement("product_image")]
        public string itemImage1 { get; set; }

        /// <summary>
        /// 預設為new
        /// </summary>
        [XmlElement("product_condition")]
        public string itemCondition { get; set; }

        /// <summary>
        /// 預設為 in stock, 若缺貨則為out of stock
        /// </summary>
        [XmlElement("product_availability")]
        public string itemAvailability { get; set; }

        /// <summary>
        /// 建議售價
        /// </summary>
        [XmlElement("product_retail_price")]
        public string itemMarketPrice { get; set; }

        /// <summary>
        /// 實際售價
        /// </summary>
        [XmlElement("product_price")]
        public string itemSalePrice { get; set; }

        /// <summary>
        /// 廠商名稱
        /// </summary>
        [XmlElement("product_brand")]
        public string itemManufacture { get; set; }

        /// <summary>
        /// 分類, 請代入全部導覽列
        /// </summary>
        [XmlElement("product_category")]
        public string itemCategoryPath { get; set; }

        /// <summary>
        /// google分類, 請代入全部導覽列
        /// </summary>
        [XmlElement("product_google_category")]
        public string itemGooglePath { get; set; }
        
        //[XmlElement("ProductImage2")]
        public string itemImage2 { get; set; }


        //[XmlElement("Categoryurl")]
        public string itemCategoryUrl { get; set; }

        /// <summary>
        /// 第一層分類
        /// </summary>
        [XmlElement("category1")]
        public string itemCategoryPath1 { get; set; }

        /// <summary>
        /// 第二層分類
        /// </summary>
        [XmlElement("category2")]
        public string itemCategoryPath2 { get; set; }

        /// <summary>
        /// 第三層分類
        /// </summary>
        [XmlElement("category3")]
        public string itemCategoryPath3 { get; set; }

        /*
        [XmlElement("ProductID")]
        public string itemID { get; set; }
        [XmlElement("ProductName")]
        public string itemName { get; set; }
        [XmlElement("Category")]
        public string itemCategoryPath { get; set; }
        [XmlElement("ProductImage1")]
        public string itemImage1 { get; set; }
        [XmlElement("ProductImage2")]
        public string itemImage2 { get; set; }
        [XmlElement("BuyURL")]
        public string itemUrl { get; set; }
        [XmlElement("SalePrice")]
        public string itemSalePrice { get; set; }
        [XmlElement("Categoryurl")]
        public string itemCategoryUrl { get; set; }
         */
    }
}
