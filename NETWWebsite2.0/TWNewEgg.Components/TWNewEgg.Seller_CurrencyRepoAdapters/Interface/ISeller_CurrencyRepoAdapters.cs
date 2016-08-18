using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_CurrencyRepoAdapters.Interface
{
    public interface ISeller_CurrencyRepoAdapters
    {
        void Create(Seller_Currency model);
        IQueryable<Seller_Currency> GetAll();
        void Update(Seller_Currency model);
    }
}
