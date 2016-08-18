using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Bank
{
    public class BankAndPayType
    {
        public BankAndPayType()
        {
            this.payTypeModel = new CartPayment.DM_PayType();
            this.listBankDM = new List<Bank_DM>();
        }
        public TWNewEgg.Models.DomainModels.CartPayment.DM_PayType payTypeModel { get; set; }
        public List<TWNewEgg.Models.DomainModels.Bank.Bank_DM> listBankDM { get; set; }
    }

    public class PayType0rateNumAndBank
    {
        public PayType0rateNumAndBank()
        {
            //this.listBankDM = new List<Bank_DM>();
        }
        public int payTypeTableId { get; set; }
        public string Name { get; set; }
        public int PayType0rateNumber { get; set; }
        public string BankList { get; set; }
        public List<Bank_DM> listBankDM { get; set; }
    }
    public class PayTypeWithBankInfo
    {
        public PayTypeWithBankInfo()
        {
            this.payTypeInfo = new CartPayment.DM_PayType();
            this.BankInfo3DVer = new List<BankInfoWith3DVerification>();
        }
        public TWNewEgg.Models.DomainModels.CartPayment.DM_PayType payTypeInfo { get; set; }
        public List<BankInfoWith3DVerification> BankInfo3DVer { get; set; }
    }

    public class BankInfoWith3DVerification:TWNewEgg.Models.DomainModels.Bank.Bank_DM
    {
        public Nullable<int> _3DVerification { get; set; }
    }
}
