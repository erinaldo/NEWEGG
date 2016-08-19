using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_CountryRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_CountryRepoAdapters
{
    public class Seller_CountryRepoAdapters : ISeller_CountryRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_Country> _seller_Country;
        public Seller_CountryRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Country> seller_Country)
        {
            this._seller_Country = seller_Country;
        }

        public void Create(Seller_Country model)
        {
            this._seller_Country.Create(model);
        }

        public IQueryable<Seller_Country> GetAll()
        {
            return this._seller_Country.GetAll();
        }

        public void Update(Seller_Country model)
        {
            this._seller_Country.Update(model);
        }

        
    }
}
