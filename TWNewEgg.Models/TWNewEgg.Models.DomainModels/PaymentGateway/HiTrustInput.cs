using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PaymentGateway
{
    public class HiTrustInput
    {
        public enum isredMoney
        {
            啟用 = 1,
            不啟用 = 0
        }
        public enum payPage
        {
            英文付款頁面 = 1,
            中文付款頁面 = 2,
            中英文付款頁面 = 3
        }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int ordernumber { get; set; }
        /// <summary>
        /// 交易金額
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// 訂單說明
        /// </summary>
        public string orderdesc { get; set; }
        /// <summary>
        /// 期數
        /// </summary>
        public int HpType { get; set; }
        /// <summary>
        /// 是否紅利折抵
        /// </summary>
        public int IsRedMoney { get; set; }
        /// <summary>
        /// 銀行代碼
        /// </summary>
        public string BankID { get; set; }
        /// <summary>
        /// 客戶端指定付款頁面功能代碼
        /// </summary>
        public int PayPage { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 訂單日期
        /// </summary>
        public DateTime orderDate { get; set; }
        /// <summary>
        /// 信用卡號
        /// </summary>
        public string cardNumber { get; set; }
        /// <summary>
        /// 末三碼
        /// </summary>
        public string CVC2 { get; set; }
        /// <summary>
        /// 到期日，格式：YYMM
        /// </summary>
        public string expiry { get; set; }

        public string token { get; set; }
        
    }
    public class HiTrustQueryInput
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 商家一次付清的Conf路徑, EX: E:/HiTRUSTasp/HiTrustConf/61138.conf" 
        /// </summary>
        public string MerConfigName { get; set; }
        /// <summary>
        /// Server Conf 路徑, EX: E:/HiTRUSTasp/HiTrustConf/HiServer.conf
        /// </summary>
        public string SerConfigName { get; set; }
        /// <summary>
        /// 交易結果
        /// </summary>
        public string retcode { get; set; }
    }
}
