using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CartPayment
{
    public class VM_CartPayment
    {
        public enum TopPayTerm
        {
            信用卡一次付清 = 0,
            信用卡分期 = 1,
            信用卡紅利折抵 = 2,
            貨到付款 = 3,
            超商付款 = 4,
            實體ATM = 5,
            網路ATM = 6,
            儲值支付 = 7,
            電匯 = 8
        }

        public List<VM_PaymentTerm> PaymentTerms { get; set; }

        public List<VM_PayType> PayTypes { get; set; }

        public List<VM_BeneficiaryParty> BeneficiaryParty { get; set; }

        public List<VM_Bank> Bank { get; set; }

        public List<VM_BankBonus> BankBonus { get; set; }
    }
}
