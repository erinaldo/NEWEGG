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
    public class BeneficiaryPartyAdapter : IBeneficiaryPartyAdapter
    {
        public IRepository<BeneficiaryParty> _beneficiaryParty;

        public BeneficiaryPartyAdapter( IRepository<BeneficiaryParty> beneficiaryParty)
        {
            this._beneficiaryParty = beneficiaryParty;
        }

        public IQueryable<Models.DBModels.TWSQLDB.BeneficiaryParty> getAll()
        {
            return this._beneficiaryParty.GetAll();
        }

        public IQueryable<Models.DBModels.TWSQLDB.BeneficiaryParty> GetAvailable()
        {
            return this._beneficiaryParty.GetAll().Where(x => x.Status == 1);
        }
    }
}
