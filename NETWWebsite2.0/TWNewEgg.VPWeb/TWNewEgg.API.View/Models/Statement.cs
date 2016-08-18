using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class MainStatementViewModel
    {
       /// <summary>
        /// 是否具有管理員權限
        /// </summary>
        public readonly bool IsAdmin;

        public MainStatementViewModel()
        {
            TWNewEgg.API.View.Service.SellerInfoService sellerInfoService = new Service.SellerInfoService();
            IsAdmin = sellerInfoService.IsAdmin;
        }
    }

    /// <summary>
    /// 對帳單主單查詢條件
    /// </summary>
    public class MainStatementSearchCondition
    {
        /// <summary>
        /// 帳單編號
        /// </summary>
        public string SettlementID { get; set; }

        /// <summary>
        /// 結算日期(起)
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 結算日期(迄)
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 是否已開立發票
        /// </summary>
        public bool? IsInvoiced { get; set; }

        public MainStatementSearchCondition()
        {
            SettlementID = string.Empty;
            DateTime now = DateTime.Now;
            DateStart = now;
            DateEnd = now;
            IsInvoiced = null;
        }
    }
}