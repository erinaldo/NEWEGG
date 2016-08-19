using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Bank;
using TWNewEgg.Models.DomainModels.BankBonus;

namespace TWNewEgg.Models.DomainModels.CartPayment
{
    public class DM_CartPayment
    {
        public List<DM_PaymentTerm> PaymentTerms { get; set; }

        public List<DM_PayType> PayTypes { get; set; }

        public List<DM_BeneficiaryParty> BeneficiaryParty { get; set; }

        public List<Bank_DM> Bank { get; set; }

        public List<BankBonus_DM> BankBonus { get; set; }
    }
}
