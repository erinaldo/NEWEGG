using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    /// <summary>
    /// 正式 Item 更新項目(詳細頁面)
    /// </summary>
    public class BatchItem_DetialEdit
    {

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 中文描述
        /// </summary>
        public string ItemDesc { get; set; }

        /// <summary>
        /// 中文描述
        /// </summary>
        public string DescTW { get; set; }

        /// <summary>
        /// 商品特色標題
        /// </summary>
        public string Sdesc { get; set; }

        /// <summary>
        /// 商品簡要描述
        /// </summary>
        public string Spechead { get; set; }

        /// <summary>
        /// 賣價
        /// </summary>
        public string PriceCard { get; set; }

        /// <summary>
        /// 賣價
        /// </summary>
        public string PriceCash { get; set; }

        /// <summary>
        /// 庫存量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 安全庫存量
        /// </summary>
        public int SafeQty { get; set; }

        /// <summary>
        /// 市場建議售價
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUser { get; set; }
    }

    /// <summary>
    /// 正式 Product 更新項目(詳細頁面)
    /// </summary>
    public class BatchProduct_DetialEdit
    {

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string NameTW { get; set; }

        /// <summary>
        /// 商品中文說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 商品中文說明
        /// </summary>
        public string DescriptionTW { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 商品保固期(月)
        /// </summary>
        public int Warranty { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string SellerProductID { get; set; }

    }

    /// <summary>
    /// 正式 ItemStock 更新項目(詳細頁面)
    /// </summary>
    public class BatchItemStock_DetialEdit
    {
        /// <summary>
        /// 可售數量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 安全庫存
        /// </summary>
        public int SafeQty { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUser { get; set; }

    }

}
