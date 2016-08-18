using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Models.DomainModels.Bank;
using TWNewEgg.Models.DomainModels.BankBonus;
using TWNewEgg.Models.DomainModels.CartPayment;

namespace TWNewEgg.CartServices.CartPayment
{
    public abstract class NECartPayment : ICartPayment
    {
        protected DM_CartPayment CartPayment = new DM_CartPayment();
        protected List<DM_PaymentTerm> PayTerms;
        protected List<DM_PayType> PayTypes;
        protected List<DM_BeneficiaryParty> BeneficiaryPartys;
        protected List<BankBonus_DM> BankBonus;
        protected List<Bank_DM> Banks;

        public abstract void Initiate(int accountId, int cartType);

        public DM_CartPayment GetCartPayment()
        {
            return CartPayment;
        }

        public IEnumerable<DM_PaymentTerm> GetPaymentTerms()
        {
            return PayTerms;
        }

        public IEnumerable<DM_PayType> GetPayTypes()
        {
            return PayTypes;
        }
        public IEnumerable<Models.DomainModels.Bank.Bank_DM> GetBank()
        {
            return Banks;
        }

        public IEnumerable<Models.DomainModels.BankBonus.BankBonus_DM> GetBankBonus()
        {
            return BankBonus;
        }
        public IEnumerable<DM_BeneficiaryParty> GetBeneficiaryPartys()
        {
            return BeneficiaryPartys;
        }
    }
}
