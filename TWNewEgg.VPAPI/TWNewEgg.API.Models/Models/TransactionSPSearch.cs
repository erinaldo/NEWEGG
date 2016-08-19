using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TWNewEgg.API.Models
{
    public class TransactionSPSearch
    {   
        /// <summary>
        /// 廠商ID
        /// </summary>
        public string inputSellerID { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string inputOrderNumber{ get; set;}

        /// <summary>
        /// 發票編號
        /// </summary>
        public string inputInvoiceNumber { get; set; }

        /// <summary>
        /// 結算編號
        /// </summary>
        public string inputSettlementID { get; set; }
        
        /// <summary>
        /// 商家商品編號
        /// </summary>
        public string inputSellerProductNum { get; set; }

        /// <summary>
        /// 新蛋產品編號
        /// </summary>
        public int inputNewEggProductNum { get; set; }

        /// <summary>
        /// 交易類別
        /// </summary>
        public string inputTransType { get; set; }
        
        /// <summary>
        /// 是否查詢已經結算者
        /// </summary>
        public string inputIsOnlySettledRecords { get; set; }

        /// <summary>
        /// 資料起始日期
        /// </summary>
        public string inputStartDate { get; set; }
        
        /// <summary>
        /// 資料結束日期
        /// </summary>
        public string inputEndDate { get; set; }

    }
}
