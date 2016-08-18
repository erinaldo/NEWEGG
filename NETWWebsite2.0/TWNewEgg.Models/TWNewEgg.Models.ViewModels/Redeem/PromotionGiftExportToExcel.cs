using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Redeem
{
    /// <summary>
    /// 匯出時用到的所有欄位 Model
    /// </summary>
    public class PromotionGiftExportToExcel
    {
        // 資料來源：PromotionGiftBlackList or PromotionGiftWhiteList 的 ItemID

        /// <summary>
        /// 賣場編號
        /// </summary>
        public string ItemID { get; set; }

        // 資料來源：PromotionGiftBlackList or PromotionGiftWhiteList 的 Status
        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }

        public enum Status2
        {
            上線 = 1,
            下線 = 2,
            Testing = 3
        }

    }
}
