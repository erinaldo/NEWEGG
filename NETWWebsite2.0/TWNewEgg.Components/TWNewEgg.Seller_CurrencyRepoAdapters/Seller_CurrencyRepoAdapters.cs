using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_CurrencyRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_CurrencyRepoAdapters
{
    public class Seller_CurrencyRepoAdapters : ISeller_CurrencyRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_Currency> _seller_Currency;
        
        public Seller_CurrencyRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Currency> seller_Currency)
        {
            this._seller_Currency = seller_Currency;
        }

        public void Create(Seller_Currency model)
        {
            this._seller_Currency.Create(model);
        }

        public IQueryable<Seller_Currency> GetAll()
        {
            return this._seller_Currency.GetAll();
        }

        public void Update(Seller_Currency model)
        {
            this._seller_Currency.Update(model);
        }
    }
    
}
