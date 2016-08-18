using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_PurviewRepoAdapters.Interface
{
    public interface ISeller_PurviewRepoAdapters
    {
        void Create(Seller_Purview model);
        IQueryable<Seller_Purview> GetAll();
        void Update(Seller_Purview model);
    }
}
