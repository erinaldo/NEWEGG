using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TWNewEgg.Models.ViewModels.Redeem
{
    public class PromotionGiftImportCheckData
    {
        /// <summary>
        /// 活動ItemId
        /// </summary>
        [DisplayName("活動ItemId")]
        public string PromotionGiftItemId { get; set; }

        /// <summary>
        /// 上下架狀態: 1 上線 2下線 3 Testing
        /// </summary>
        [DisplayName("上下架狀態")]
        public string status { get; set; }

        /// <summary>
        /// 第幾筆錯誤
        /// </summary>
        [DisplayName("第幾筆錯誤")]
        public int errorRow { get; set; }

        /// <summary>
        /// 錯誤訊息:Nullable
        /// </summary>
        [DisplayName("錯誤訊息:Nullable")]
        public string ImportExcelNullError { get; set; }

        /// <summary>
        /// 錯誤訊息:型態轉換
        /// </summary>
        [DisplayName("錯誤訊息:型態轉換")]
        public string ImportExcelTypeError { get; set; }

        /// <summary>
        /// 錯誤訊息:ItemId重複
        /// </summary>
        [DisplayName("錯誤訊息:ItemId重複")]
        public string ImportExcelCheckDuplicateItemIDError { get; set; }

        /// <summary>
        /// 錯誤訊息:ItemID範圍異常
        /// </summary>
        [DisplayName("錯誤訊息:ItemID範圍異常")]
        public string ImportExcelItemIDRangeError { get; set; }

        /// <summary>
        /// 錯誤訊息:status範圍異常
        /// </summary>
        [DisplayName("錯誤訊息:status範圍異常")]
        public string ImportExcelStatusRangeError { get; set; }

        public enum Status2
        {
            上線 = 1,
            下線 = 2,
            testing = 3
        }
    }
}
