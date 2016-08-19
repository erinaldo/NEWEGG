using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Category
{
    public class CategoryTopBanner_ItemView
    {
        public int AdLayer3ID { get; set; }
        public int ItemID { get; set; }
        /// <summary>
        /// 顯示排序，1開始
        /// </summary>
        public int Showorder { get; set; }
        /// <summary>
        /// 商品的主要圖片位址.
        /// </summary>
        public string ItemImage { get; set; }
        /// <summary>
        /// 商品最終顯示的價錢(Final Price).
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 商品原始價錢.
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 商品主標題.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 第三層分類
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// 第一層分類
        /// </summary>
        public int StoreID { get; set; }
    }
}
