using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_FinancialRepoAdapters.Interface
{
    public interface ISeller_FinancialRepoAdapters
    {
        void Create(Seller_Financial model);
        IQueryable<Seller_Financial> GetAll();
        void Update(Seller_Financial model);
    }
}
