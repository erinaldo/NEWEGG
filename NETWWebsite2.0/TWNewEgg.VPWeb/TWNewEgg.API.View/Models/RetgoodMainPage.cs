using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class RetgoodMainPage
    {
        /// <summary>
        /// 是否具有管理員權限
        /// </summary>
        public readonly bool IsAdmin;

        public RetgoodMainPage()
        {
            TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();
            IsAdmin = sellerInfoService.IsAdmin;
        }
    }

    /// <summary>
    /// 退貨主單查詢條件
    /// </summary>
    public class MainRetgoodSearchCondition
    {
        /// <summary>
        /// 查詢關鍵字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 關鍵字查詢目標
        /// </summary>
        public int KeyWordSearchType { get; set; }

        /// <summary>
        /// 指定查詢退貨狀態
        /// </summary>
        public int RetgoodStatus { get; set; }

        /// <summary>
        /// 指定查詢訂單日期
        /// </summary>
        public int CreateDateSearchType { get; set; }

        /// <summary>
        /// 指定查詢訂單日期(起)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 指定查詢訂單日期(迄)
        /// </summary>
        public DateTime? EndDate { get; set; }

        public MainRetgoodSearchCondition()
        {
            KeyWord = string.Empty;
            KeyWordSearchType = 0;
            RetgoodStatus = 0;
            CreateDateSearchType = 0;
            StartDate = null;
            EndDate = null;
        }
    }
}