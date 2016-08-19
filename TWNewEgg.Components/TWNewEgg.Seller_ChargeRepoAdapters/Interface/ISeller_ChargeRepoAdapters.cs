using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ChargeRepoAdapters.Interface
{
    public interface ISeller_ChargeRepoAdapters
    {
        void Create(Seller_Charge model);
        IQueryable<Seller_Charge> GetAll();
        void Update(Seller_Charge model);
    }
}
