using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_FinancialRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_FinancialRepoAdapters
{
    public class Seller_FinancialRepoAdapters : ISeller_FinancialRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_Financial> _seller_Financial;

        public Seller_FinancialRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Financial> seller_Financial)
        {
            this._seller_Financial = seller_Financial;
        }

        public void Create(Seller_Financial model)
        {
            this._seller_Financial.Create(model);
        }

        public IQueryable<Seller_Financial> GetAll()
        {
            return this._seller_Financial.GetAll();
        }

        public void Update(Seller_Financial model)
        {
            this._seller_Financial.Update(model);
        }
    }
}
