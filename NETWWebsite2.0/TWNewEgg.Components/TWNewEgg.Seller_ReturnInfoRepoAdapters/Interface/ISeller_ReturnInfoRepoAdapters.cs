using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ReturnInfoRepoAdapters.Interface
{
    public interface ISeller_ReturnInfoRepoAdapters
    {
        void Create(Seller_ReturnInfo model);
        IQueryable<Seller_ReturnInfo> GrtAll();
        void Update(Seller_ReturnInfo model);
    }
}
