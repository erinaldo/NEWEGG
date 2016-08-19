using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.ItemInstallment
{
    public class ItemTopInstallmentReturn
    {
        public List<ItemTopInstallment> ItemTopInstallmentCell { get; set; }

        public int DataCount { get; set; }
    }

    /// <summary>
    /// 賣場最高分期期數
    /// </summary>
    public class ItemTopInstallment
    {
        public int ID { get; set; }

        public int Edition { get; set; }
        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        public decimal GrossMargin { get; set; }

        /// <summary>
        /// 最高分期期數
        /// </summary>
        public int TopInstallment { get; set; }

        /// <summary>
        /// 開始時間(同Item不可重複區間)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 結束時間(同Item不可重複區間)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        /// <remarks>enum ItemTopInstallmentStatus</remarks>
        public int Status { get; set; }

        /// <summary>
        /// 主分類_第2層
        /// </summary>
        public int Category_Layer2 { get; set; }

        /// <summary>
        /// 廠商ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string SellerName { get; set; }
    }


    /// <summary>
    /// 賣場最高分期期數搜尋條件
    /// </summary>
    public class ItemTopInstallmentSearchCondition
    {
        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 主分類_第 2 層
        /// </summary>
        public int Category_Layer2 { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        /// <remarks>enum ItemTopInstallmentStatus</remarks>
        public int Status { get; set; }

        /// <summary>
        /// 廠商ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 選取哪種類型時間區間
        /// </summary>
        public int DateType { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 分頁數
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 每頁資料筆數
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 排序欄位名稱
        /// </summary>
        public string SortMember { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public ListSortDirection SortDirection { get; set; }

        public ItemTopInstallmentSearchCondition()
        {
            ItemID = -1;
            Category_Layer2 = -1;
            Status = -1;
            SellerID = -1;
            DateType = 2;
            StartDate = null;
            EndDate = null;
            Page = 0;
            PageSize = 10;
            SortMember = "ItemID";
            SortDirection = ListSortDirection.Ascending;
        }
    }

    /// <summary>
    /// 賣場最高分期期數狀態
    /// </summary>
    public enum ItemTopInstallmentStatus
    {
        // 關閉
        Disable = 0,

        // 啟用
        Enable = 1
    }

    public enum InstallmentSearchDateType
    {
        分期開始時間 = 0,
        分期結束時間 = 1,
        活動時間 = 2
    }
}
