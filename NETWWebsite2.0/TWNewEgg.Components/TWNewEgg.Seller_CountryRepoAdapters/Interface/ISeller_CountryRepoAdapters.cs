using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_CountryRepoAdapters.Interface
{
    public interface ISeller_CountryRepoAdapters
    {
        void Create(Seller_Country model);
        IQueryable<Seller_Country> GetAll();
        void Update(Seller_Country model);
    }
}
