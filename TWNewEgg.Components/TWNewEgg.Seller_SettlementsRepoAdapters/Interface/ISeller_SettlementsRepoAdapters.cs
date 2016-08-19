using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_SettlementsRepoAdapters.Interface
{
    public interface ISeller_SettlementsRepoAdapters
    {
        void Create(Seller_Settlements model);
        IQueryable<Seller_Settlements> GetAll();
        void Update(Seller_Settlements model);
    }
}
