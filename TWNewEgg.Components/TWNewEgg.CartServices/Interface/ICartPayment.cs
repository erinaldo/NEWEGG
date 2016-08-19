using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Bank;
using TWNewEgg.Models.DomainModels.BankBonus;
using TWNewEgg.Models.DomainModels.CartPayment;

namespace TWNewEgg.CartServices.Interface
{
    public interface ICartPayment
    {
        DM_CartPayment GetCartPayment();
        IEnumerable<DM_PaymentTerm> GetPaymentTerms();
        IEnumerable<DM_PayType> GetPayTypes();
        IEnumerable<DM_BeneficiaryParty> GetBeneficiaryPartys();
        IEnumerable<Bank_DM> GetBank();
        IEnumerable<BankBonus_DM> GetBankBonus();
        void Initiate(int cartID, int cartType);
    }
}
