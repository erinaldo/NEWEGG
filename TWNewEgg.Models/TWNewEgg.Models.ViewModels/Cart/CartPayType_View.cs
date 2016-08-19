using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class CartPayType_View
    {
        public CartPayType_View() {
            this.BankIDwithName = new List<CartPayTypeBankIDwithName_View>();
            this.Installments = 1;
        }
        public int ID { get; set; }
        public string GroupName { get; set; }
        // 該方案的描述 ex: 花旗3期 或1期 ; 或Web ATM
        public string Name { get; set; }
        // 銀行ID
        public Nullable<int> BankID { get; set; }
        // 利息比率
        public Nullable<decimal> InsRate { get; set; }
        // 期數 ; 0 : 使用Web ATM一次付清 ; 1 : 1期_1次付清 ; 3 : 3期_分3次付清 ; 6 : 6期_分6次付清 ; 12 : 12期_分12次付清
        public Nullable<int> PayType0rateNum { get; set; }
        // bool 型態 : 0 : 無息 ; 1 : 有息
        public Nullable<int> PayType0rateType { get; set; }
        public int Installments { get; set; }
        public string PayTypeCode { get; set; }
        public Nullable<System.DateTime> OnlineStartDate { get; set; }
        public Nullable<System.DateTime> OnlineEndDate { get; set; }
        public Nullable<int> Status { get; set; }
        public string BankList { get; set; }
        public string vAccount { get; set; }
        public string BankCode { get; set; }
        public DateTime ExpireDate { get; set; }
        public List<CartPayTypeBankIDwithName_View> BankIDwithName { get; set; }
    }
}
