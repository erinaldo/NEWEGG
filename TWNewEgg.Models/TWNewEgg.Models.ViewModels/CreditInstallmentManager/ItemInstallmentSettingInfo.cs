using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CreditInstallmentManager
{
    /// <summary>
    /// 分期設定查詢
    /// </summary>
    public class ItemInstallmentSettingInfo
    {
        /// <summary>
        /// 賣場編號
        /// </summary>
        public string ItemID { get; set; }

        public string SellerID { get; set; }

        /// <summary>
        /// 主分類
        /// </summary>

        public string Category_Layer2 { get; set; }

        /// <summary>
        /// 選取哪種類型時間區間
        /// </summary>
        public int DateType { get; set; }


        /// <summary>
        /// 查詢分期開始時間
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 查詢分期結束時間
        /// </summary>
        public DateTime? EndDate { get; set; }

        public int Status { get; set; }
    }

    public class GridItemInstallmentSettingInfo
    {
        public int ID { get; set; }

        public int Edition { get; set; }

        /// <summary>
        /// Check Box
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 賣場毛利率
        /// </summary>
        public decimal GrossMargin { get; set; }

        /// <summary>
        /// 分期開放時間
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 分期結束時間
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 最高分期數
        /// </summary>
        public int TopInstallment { get; set; }
    }

}
