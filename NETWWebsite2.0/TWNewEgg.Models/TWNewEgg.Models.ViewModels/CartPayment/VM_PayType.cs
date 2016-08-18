using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CartPayment
{
    public class VM_PayType
    {
        /// <summary>
        /// 期數
        /// </summary>
        /// <remarks>0 : 使用Web ATM一次付清</remarks>
        /// <remarks>1 : 1期_1次付清</remarks>
        /// <remarks>3 : 3期_分3次付清</remarks>
        /// <remarks>6 : 6期_分6次付清</remarks>
        /// <remarks>12 : 12期_分12次付清</remarks>
        public Nullable<int> Installment { get; set; }
        public string PaymentTermID { get; set; }
        public string PaymenGetWayID { get; set; }
        public string BeneficiaryPartyID { get; set; }

        public int ID { get; set; }

        /// <summary>
        /// 該方案的描述 ex: 花旗3期 或1期 ; 或Web ATM
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首先支援發卡銀行的銀行ID
        /// </summary>
        public Nullable<int> BankID { get; set; }

        /// <summary>
        /// 利息比率
        /// </summary>
        public Nullable<decimal> InsRate { get; set; }

        /// <summary>
        /// 給銀行的利息比率
        /// </summary>
        public decimal InsRateForBank { get; set; }

        /// <summary>
        /// 期數
        /// </summary>
        /// <remarks>0 : 使用Web ATM一次付清</remarks>
        /// <remarks>1 : 1期_1次付清</remarks>
        /// <remarks>3 : 3期_分3次付清</remarks>
        /// <remarks>6 : 6期_分6次付清</remarks>
        /// <remarks>12 : 12期_分12次付清</remarks>
        public Nullable<int> PayType0rateNum { get; set; }

        /// <summary>
        /// bool 型態 : 0 : 無息 ; 1 : 有息
        /// </summary>
        public Nullable<int> PayType0rateType { get; set; }

        /// <summary>
        /// 提供金流服務的銀行代碼
        /// </summary>
        public string PayTypeCode { get; set; }

        public Nullable<System.DateTime> OnlineStartDate { get; set; }
        public Nullable<System.DateTime> OnlineEndDate { get; set; }
        public Nullable<int> Status { get; set; }
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

        public VM_PayType()
        {
            this.Name = String.Empty;
            this.InsRate = 0m;
            this.InsRateForBank = 0m;
            this.PayType0rateNum = 0;
            this.PayType0rateType = 0;
            this.PayTypeCode = String.Empty;
            this.Status = 0;
        }
    }

    /// <summary>
    /// 賣場及最高分期期數
    /// </summary>
    public class ItemAndPayType
    {
        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItmeID { get; set; }

        /// <summary>
        /// 最高分期期數
        /// </summary>
        public int TopInstallment { get; set; }

        /// <summary>
        /// 賣場售價
        /// </summary>
        public decimal Pirce { get; set; }

        /// <summary>
        /// 賣場成本
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 賣場毛利
        /// </summary>
        public decimal GrossMargin { get; set; }

        /// <summary>
        /// PayType
        /// </summary>
        public List<VM_PayType> PayTypeCell { get; set; }

        public ItemAndPayType()
        {
            ItmeID = 0;
            TopInstallment = 0;
            Pirce = 0;
            Cost = 0;
            GrossMargin = 0;
            PayTypeCell = new List<VM_PayType>();
        }
    }

    public class ItemPayType
    {
        
        public int PayType0rateNum { get; set; }
        //顯示在前台的可用銀行
        public string PayTypeBankStrForList { get; set; }
        //期數
        public int Staging { get; set; }
        //利息
        public string InsRate { get; set; }
        //可使用的銀行數量
        public int canUseBankCount { get; set; }
    }
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
}
