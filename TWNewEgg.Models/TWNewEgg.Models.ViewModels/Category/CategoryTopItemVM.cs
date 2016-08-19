using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Category
{
    public class CategoryTopItemVM
    {
        public int CategoryID { get; set; }
        public int StoreID { get; set; }
        public int ItemID { get; set; }
        /// <summary>
        /// 1：銷售TOP10
        /// 2：推薦商品
        /// </summary>
        public int ItemType { get; set; }
        /// <summary>
        /// 商品主標題.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 顯示排序，1開始
        /// </summary>
        public int Showorder { get; set; }
        /// <summary>
        /// 商品的主要圖片位址.
        /// </summary>
        public string ItemImage { get; set; }
        /// <summary>
        /// 商品原始市價
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 商品最終顯示的價錢(Final Price).
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 0：不顯示；1：顯示
        /// </summary>
        public int ShowAll { get; set; }
        /// <summary>
        /// 商品數量
        /// </summary>
        public int SellingQty { get; set; } 

    }
}
