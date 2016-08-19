using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CartRepoAdapters
{
    public class PayTypeRepoAdapter : IPayTypeRepoAdapter
    {
        public IRepository<PayType> _payType;
        public IRepository<BeneficiaryParty> _beneficiaryParty;
        public PayTypeRepoAdapter(IRepository<PayType> payType, IRepository<BeneficiaryParty> beneficiaryParty)
        {
            this._payType = payType;
            this._beneficiaryParty = beneficiaryParty;
        }

        public PayType GetPayType(int Id)
        {
            return this._payType.Get(x => x.ID == Id);
        }

        public IQueryable<PayType> GetAvailable()
        {
            return this._payType.GetAll().Where(x => x.Status == 0);
        }

        public PayType GetPayTypeByPayType0rateNumandBankID(int? PayType, int? BankID)
        {
            return this._payType.GetAll().Where(x => x.PayType0rateNum == PayType && x.BankID == BankID).FirstOrDefault();
        }

        public IQueryable<PayType> getAll()
        {
            return this._payType.GetAll();
        }

        public PayType GetPayTypeByBankCode(string bankCode)
        {
            var query = this._payType.GetAll().Where(x => x.Name == "紅利折抵").Join(this._beneficiaryParty.GetAll(),
                m => m.BeneficiaryPartyID,
                a => a.ID,
                (m, a) => new { m.ID ,a.BankCode});
            int iD;
            iD = query.Where(x => x.BankCode == bankCode).Select(x=>x.ID).FirstOrDefault();

            return this.GetPayType(iD); 
        }
    }
}
