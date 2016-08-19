using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class QueryCartCondition
    {
        #region ENUM n FUNCTIONS

        public enum OrderStatus
        {
            全部 = 0,
            初始狀態 = 1,
            未遞送 = 2,
            已遞送 = 3,
            作廢 = 4
        }

        public void setStatus(OrderStatus status)
        {
            Status = (int)status;
        }
        #endregion

        /// <summary>
        /// 要搜尋的訂單狀態
        /// </summary>
        /// <value>TWNewEgg.API.Models.MainOrderStatus</value>
        public int OrderSearchMode { get; set; }

        #region PARAMETERS
        /// <summary>
        /// 商(賣)家ID / Seller ID
        /// </summary>
        //public int SellerID { get; set; }
        public string SellerID { get; set; }

        /// <summary>
        /// 訂單編號 / "Order Number"
        /// </summary>
        public string SOCode { get; set; }

        public List<string> ProcessIDs { get; set; }

        /// <summary>
        /// 收據編號/(發票號碼)/"Invoice Number"
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 客戶姓名/"Customer Name"
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// 新蛋商品編號
        /// </summary>
        //public int ProductID { get; set; }
        public string ProductID { get; set; }

        /// <summary>
        /// 客戶電話/"Customer Phone #"
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// 標題描述/"TitleDescription"
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 生產廠商/"Manufacturer"
        /// </summary>
        public string Manufacture { get; set; }


        /// <summary>
        /// 訂單狀態
        /// </summary>
        public int Status { get; set; }
        //public string Status { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 特定天數前成立之訂單
        /// </summary>
        public double? DayBefore { get; set; }

        // 分頁資訊改為nullable Jack.W.W 0626
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; } 
        #endregion

        public API.Models.OrderInfo.EnumAccountTypeCode AccountType { get; set; } 
    }
}
