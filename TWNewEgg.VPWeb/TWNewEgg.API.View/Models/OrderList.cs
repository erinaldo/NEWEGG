using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class OrderMainPage
    {
        /// <summary>
        /// 是否具有管理員權限
        /// </summary>
        public readonly bool IsAdmin;

        /// <summary>
        /// 登入者身份(Seller or Vendor)
        /// </summary>
        public readonly string AccountTypeCode;

        public OrderMainPage()
        {
            TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();
            IsAdmin = sellerInfoService.IsAdmin;
            AccountTypeCode = sellerInfoService.AccountTypeCode;
        }
    }

    /// <summary>
    /// 訂單主單查詢條件
    /// </summary>
    public class MainOrderSearchCondition
    {
        /// <summary>
        /// 查詢關鍵字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 關鍵字查詢目標
        /// </summary>
        /// <value>TWNewEgg.API.Models.OrderKeyWordSearchType</value>
        public int KeyWordSearchType { get; set; }

        /// <summary>
        /// 指定查詢訂單狀態
        /// </summary>
        /// <value>TWNewEgg.API.Models.MainOrderStatus</value>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 指定查詢訂單日期
        /// </summary>
        /// <value>TWNewEgg.API.Models.OrderCreateDateSearchType</value>
        public int CreateDateSearchType { get; set; }

        /// <summary>
        /// 指定查詢訂單日期(起)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 指定查詢訂單日期(迄)
        /// </summary>
        public DateTime? EndDate { get; set; }

        public MainOrderSearchCondition()
        {
            KeyWord = string.Empty;
            KeyWordSearchType = 0;
            OrderStatus = 0;
            CreateDateSearchType = 0;
            StartDate = null;
            EndDate = null;
        }
    }
}