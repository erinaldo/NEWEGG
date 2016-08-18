using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class SellerRMQuery
    {
        public Nullable<int> SellerID { get; set; }
        public string SellerName { get; set; }
        public string UserEmail { get; set; }

        //PagingInfo
        public string SortField { get; set; }
        public Nullable<int> StartRowIndex { get; set; }
        public Nullable<int> PageSize { get; set; }
    }

    /// <summary>
    /// 商家關係搜詢條件
    /// </summary>
    public class SellerRelationshipSearchCondition
    {
        /// <summary>
        /// 是否有管理權限
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 商家 ID
        /// </summary>
        public int? SellerID { get; set; }

        /// <summary>
        /// 地區
        /// </summary>
        public CountryCode SellerCountryCode { get; set; }

        /// <summary>
        /// 帳戶類別
        /// </summary>
        public AccountType AccountTypeCode { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public SellerStatus SellerStatus { get; set; }

        /// <summary>
        /// 創建日期(開始日期)
        /// </summary>
        public string CreateDateStart { get; set; }

        /// <summary>
        /// 創建日期(結束日期)
        /// </summary>
        public string CreateDateEnd { get; set; }

        /// <summary>
        /// 最後編輯日期(開始日期)
        /// </summary>
        public string UpdateDateStart { get; set; }

        /// <summary>
        /// 最後編輯日期(結束日期)
        /// </summary>
        public string UpdateDateEnd { get; set; }

        /// <summary>
        /// 初始值
        /// </summary>
        public SellerRelationshipSearchCondition()
        {
            IsAdmin = false;
            SellerID = -1;
            SellerCountryCode = CountryCode.All;
            AccountTypeCode = AccountType.All;
            SellerStatus = API.Models.SellerStatus.All;
        }
    }

    public enum CountryCode
    {
        All,
        Canada,
        China,
        HongKong,
        Taiwan,
        UnitedStates
    }

    public enum AccountType
    {
        All,
        Seller,
        Vendor
    }

    public enum SellerStatus
    {
        All,
        Active,
        Inactive,
        Closed
    }
}
