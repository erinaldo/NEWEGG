using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ProductSpecRepoAdapters.Interface
{
    public interface ISeller_ProductSpecRepoAdapters
    {
        void Create(Seller_ProductSpec model);
        IQueryable<Seller_ProductSpec> GetAll();
        void Update(Seller_ProductSpec model);
    }
}
