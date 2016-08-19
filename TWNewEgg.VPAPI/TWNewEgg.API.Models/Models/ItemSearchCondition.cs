using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    /// <summary>
    /// 商品搜尋模式
    /// </summary>
    public class ItemSearchCondition
    {
        public int SellerID { set; get; }
        public int SearchMode { set; get; }

        /// <summary>
        /// 搜尋條件關鍵字
        /// </summary>
        public string Keyword { get; set; }

        ///// <summary>
        ///// 商家商品編號
        ///// </summary>
        //public string SellerPartNum { set; get; }

        ///// <summary>
        ///// 廠商產品編號
        ///// </summary>
        //public string ManufacturerPartNum { set; get; }

        ///// <summary>
        ///// 新蛋商品編號
        ///// </summary>
        //public int? ItemId { set; get; }

        ///// <summary>
        ///// 商品描述
        ///// </summary>
        //public string Name { set; get; }

        //=============以下為進階搜尋=============//

        // 審核狀態
        public int? CheckStatus { set; get; }

        /// <summary>
        /// 商品狀態: 0-All、1-Active、2-Inactive
        /// </summary>
        public int? Status { set; get; }

        public int? Manufacturer { set; get; }

        /// <summary>
        /// 商品成色: 0-All、1-New、2-Refurbished
        /// </summary>
        public int? ItemCondition { set; get; }

        /// <summary>
        /// 目前無
        /// </summary>
        public int? Shipping { set; get; }

        /// <summary>
        /// 商品類別
        /// </summary>
        public int? Industry { set; get; }
        public int? SubCategory { set; get; }
        public int? ItemCategory { set; get; }

        /// <summary>
        /// 存貨
        /// </summary>
        public int? Inventory { set; get; }

        public int? CreateDateBefore { set; get; }
        public DateTime? CreateDateStart { set; get; }
        public DateTime? CreateDateEnd { set; get; }

        public PageInfo PageInfo { set; get; }

        public string SortType { get; set; }
        public string SortField { get; set; }
    }
}
