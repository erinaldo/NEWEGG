using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Advertising
{
    public class AdvEventType
    {
        public enum AdvTypeOption
        {
            每日整點限時搶購 = 1,
            EDM = 2,

            /// <summary>
            /// 首頁TopBanner區
            /// </summary>
            IndexTopBanner = 3,

            /// <summary>
            /// 首頁宮格區
            /// </summary>
            IndexGridBanner = 4,

            /// <summary>
            /// 首頁宮格下方區
            /// </summary>
            IndexGridBottomBanner = 5,

            /// <summary>
            /// 右方廣告A區
            /// </summary>
            IndexRightA = 6,

            /// <summary>
            /// 右方廣告B區
            /// </summary>
            IndexRightB = 7,

            /// <summary>
            /// 右方廣告C區
            /// </summary>
            IndexRightC = 8,

            /// <summary>
            /// 團購右方廣告
            /// </summary>
            GroupBuyRightBanner = 21
        }

        public enum CountryOption
        {
            Global = 0,
            America = 1,
            China = 2,
            Taiwan = 3,

            /// <summary>
            /// 發燒極客
            /// </summary>
            HotQuick = 60
        }

        /// <summary>
        /// 流水號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 廣告型態
        /// </summary>
        public int AdvTypeCode { get; set; }

        /// <summary>
        /// 廣告名稱
        /// </summary>
        public string AdvTypeName { get; set; }

        /// <summary>
        /// 廣告起始時間
        /// </summary>
        public Nullable<System.DateTime> StartDate { get; set; }

        /// <summary>
        /// 廣告結束時間
        /// </summary>
        public Nullable<System.DateTime> EndDate { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 建立使用者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 更新次數
        /// </summary>
        public Nullable<int> Updated { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }

        /// <summary>
        /// 更新使用者
        /// </summary>
        public string UpdateUser { get; set; }

        /* ---------------- 以下由Lynn新增 ---------------- */
        /// <summary>
        /// 此版面每次顯示幾個廣告內容
        /// </summary>
        public int? MaxAd { get; set; }
        /// <summary>
        /// 幾秒換頁
        /// </summary>
        public int? CacheMins { get; set; }

        /// <summary>
        /// 廣告區塊設定使用的CSS名稱
        /// </summary>
        public string CSS { get; set; }

        /// <summary>
        /// 請參照CouponOption選擇
        /// </summary>
        public int Country { get; set; }

        /* ---------------- Lynn新增結束 ---------------- */

    }
}
