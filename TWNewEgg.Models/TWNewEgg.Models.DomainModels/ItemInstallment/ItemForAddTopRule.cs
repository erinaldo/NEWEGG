using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.ItemInstallment
{
    public class ItemForAddTopRuleReturn
    {
        public List<ItemForAddTopRule> ItemForAddTopRuleCell { get; set; }

        public int DataCount { get; set; }
    }

    /// <summary>
    /// 賣場資訊
    /// </summary>
    public class ItemForAddTopRule
    {
        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 賣場毛利率
        /// </summary>
        public decimal ItemGrossMargin { get; set; }

        /// <summary>
        /// 預設最高期數
        /// </summary>
        public int TopInstallment { get; set; }

        /// <summary>
        /// 供應商
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 供應商名稱
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 第 2 層分類 ID
        /// </summary>
        public int CategoryID_Layer2 { get; set; }
    }

    /// <summary>
    /// 賣場資訊搜尋條件
    /// </summary>
    public class ItemForAddTopRuleSearchCondition
    {
        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 供應商
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 第 2 層分類 ID
        /// </summary>
        public int CategoryID_Layer2 { get; set; }

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

        public ItemForAddTopRuleSearchCondition()
        {
            ItemID = -1;
            SellerID = -1;
            CategoryID_Layer2 = -1;
            Page = 0;
            PageSize = 10;
            SortMember = "ItemID";
            SortDirection = ListSortDirection.Ascending;
        }
    }
}
