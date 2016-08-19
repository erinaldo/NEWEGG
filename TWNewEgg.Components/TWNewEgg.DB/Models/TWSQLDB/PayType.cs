using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("paytype")]
    public class PayType
    {
        public PayType()
        {
            this.Name = String.Empty;
            this.InsRate = 0m;
            this.InsRateForBank = 0m;
            this.PayType0rateNum = 0;
            this.PayType0rateType = 0;
            this.PayTypeCode = String.Empty;
            this.Status = 0;
            this.Updated = 0;
        }

        /// <summary>
        /// 付款方式
        /// </summary>
        public enum nPayType
        { 
            信用卡一次付清 = 1,
            三期零利率 = 3,
            六期零利率 = 6,
            九期零利率 = 9,
            十期零利率 = 10,
            十二期零利率 = 12,
            十八期零利率 = 18,
            二十四期零利率 = 24,
            三期分期 = 103,
            六期分期 = 106,
            九期分期 = 109,
            十期分期 = 110,
            十二期分期 = 112,
            十八期分期 = 118,
            二十四期分期 = 124,
            三十期分期 = 130,
            信用卡紅利折抵 = 201,
            網路ATM = 30,
            貨到付款 = 31,
            超商付款 = 32,
            電匯 = 33,
            實體ATM = 34,
            歐付寶儲值支付 = 501
        }

        /// <summary>
        /// 銀行金流的接口
        /// </summary>
        public enum PayTypeGateOption
        {
            /// <summary>
            /// 歐付寶
            /// </summary>
            AllPay = 1,

            /// <summary>
            /// 花旗
            /// </summary>
            Hitrust = 2,

            /// <summary>
            /// 中國信託
            /// </summary>
            Chinatrust = 3,

            /// <summary>
            /// 國泰世華
            /// </summary>
            Cathay = 4
        }

        public enum PayTypeStatus
        { 
            啟動 = 0,
            關閉 = 1
        }

        public enum PayTypeRateType
        {
            無息 = 0,
            有息 = 1
        }

        public enum PaymentVerification
        {
            無 = 0,
            三Ｄ驗證 = 3
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        // 該方案的描述 ex: 花旗3期 或1期 ; 或Web ATM
        public string Name { get; set; }
        /// <summary>
        /// 首先支援發卡銀行的銀行ID
        /// </summary>
        public Nullable<int> BankID { get; set; }
        // 利息比率
        public Nullable<decimal> InsRate { get; set; }
        // 給銀行的利息比率
        public decimal InsRateForBank { get; set; }
        // 期數 ; 0 : 使用Web ATM一次付清 ; 1 : 1期_1次付清 ; 3 : 3期_分3次付清 ; 6 : 6期_分6次付清 ; 12 : 12期_分12次付清
        public Nullable<int> PayType0rateNum { get; set; }
        // bool 型態 : 0 : 無息 ; 1 : 有息
        public Nullable<int> PayType0rateType { get; set; }
        /// <summary>
        /// 提供金流服務的銀行代碼
        /// </summary>
        public string PayTypeCode { get; set; }
        public Nullable<System.DateTime> OnlineStartDate { get; set; }
        public Nullable<System.DateTime> OnlineEndDate { get; set; }
        public Nullable<int> Status { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string BankList { get; set; }

        /// <summary>
        /// 次項支援發卡銀行的銀行ID LIST, 以逗號分隔
        /// </summary>
        public string BankIDList { get; set; }

        /// <summary>
        /// 金流服務優先順序
        /// </summary>
        public Nullable<int> ChooseOrder { get; set; }
        /// <summary>
        /// 金流驗證方式
        /// </summary>
        public Nullable<int> Verification { get; set; }
    }
}