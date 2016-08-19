using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 單一商品的各個屬性
    /// </summary>
    public class StoreItemCell
    {
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Item的識別ID, 唯一值.
        /// </summary>
        public int ItemID { get; set; }
        
        /// <summary>
        /// 商品主標題.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 商品的主要圖片位址.
        /// </summary>
        public string ItemImage { get; set; }
        
        /// <summary>
        /// 商品的品牌Logo圖片位址.
        /// </summary>
        public string LogoImage { get; set; }
        
        /// <summary>
        /// 商品最終顯示的價錢(Final Price).
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// 連到ProductDetail頁面的url(這可能要由UI組出來).
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 副標題
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 原價
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 第三層分類
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 第一層分類
        /// </summary>
        public int StoreID { get; set; }

        /// <summary>
        /// 可賣量
        /// </summary>
        public int SellingQty { get; set; }


    }
}
