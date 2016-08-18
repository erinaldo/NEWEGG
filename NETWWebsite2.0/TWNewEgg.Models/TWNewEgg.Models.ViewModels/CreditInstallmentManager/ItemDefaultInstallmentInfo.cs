using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CreditInstallmentManager
{
    /// <summary>
    /// 查詢賣場相關的條件
    /// </summary>
    public class ItemDefaultInstallmentInfo
    {
        /// <summary>
        /// 賣場編號
        /// </summary>
        public string MarketNumber;

        public string SellerID;
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string SellerName;

        /// <summary>
        /// 主分類
        /// </summary>
        public string MainCategoryID_Layer0;

        public string MainCategoryID_Layer1;

        public string MainCategoryID_Layer2;

        /// <summary>
        /// 跨分類
        /// </summary>
        public string SubCategoryID_1_Layer1;

        public string SubCategoryID_1_Layer2;

    }

    /// <summary>
    /// Gird畫面上用到的欄位
    /// </summary>
    public class GridItemDefaultInstallmentInfo
    {
        /// <summary>
        /// Check Box
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string SellerName;

        /// <summary>
        /// 賣場編號
        /// </summary>
        public string MarketNumber;

        /// <summary>
        /// 賣場售價
        /// </summary>
        public decimal? PriceCash;

        /// <summary>
        /// 毛利
        /// </summary>
        public decimal? GossProfit;

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal? GrossMargin;

        /// <summary>
        /// 預設最高期數
        /// </summary>
        public int? Dafaultinstallment;

        /// <summary>
        /// 最高分幾期
        /// </summary>
        public int? TopinstallmentSettings;

        /// <summary>
        /// 分期開放時間
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 分期結束時間
        /// </summary>
        public DateTime DateEnd { get; set; }

    }

}