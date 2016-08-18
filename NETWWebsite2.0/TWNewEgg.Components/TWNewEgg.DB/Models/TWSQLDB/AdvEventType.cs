using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("adveventtype")]
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("流水號")]
        public int ID { get; set; }

        [DisplayName("廣告型態")]
        public int AdvTypeCode { get; set; }

        [DisplayName("廣告名稱")]
        public string AdvTypeName { get; set; }

        [DisplayName("廣告起始時間")]
        public Nullable<System.DateTime> StartDate { get; set; }

        [DisplayName("廣告結束時間")]
        public Nullable<System.DateTime> EndDate { get; set; }

        [DisplayName("建立時間")]
        public System.DateTime CreateDate { get; set; }

        [DisplayName("建立使用者")]
        public string CreateUser { get; set; }

        [DisplayName("更新次數")]
        public Nullable<int> Updated { get; set; }

        [DisplayName("更新時間")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [DisplayName("更新使用者")]
        public string UpdateUser { get; set; }

        /* ---------------- 以下由Lynn新增 ---------------- */
        /// <summary>
        /// 此版面每次顯示幾個廣告內容
        /// </summary>
        [DisplayName("每次顯示幾個廣告內容")]
        public int? MaxAd { get; set; }
        /// <summary>
        /// 幾秒換頁
        /// </summary>
        [DisplayName("輪播換頁時間")]
        public int? CacheMins { get; set; }

        /// <summary>
        /// 廣告區塊設定使用的CSS名稱
        /// </summary>
        [DisplayName("廣告區塊設定使用的CSS名稱")]
        public string CSS { get; set; }

        /// <summary>
        /// 請參照CouponOption選擇
        /// </summary>
        [DisplayName("地區")]
        public int Country { get; set; }

        /* ---------------- Lynn新增結束 ---------------- */

    }
}
