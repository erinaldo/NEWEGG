using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ProductDetailRepoAdapters.Interface
{
    public interface ISeller_ProductDetailRepoAdapters
    {
        void Create(Seller_ProductDetail model);
        IQueryable<Seller_ProductDetail> GetAll();
        void Update(Seller_ProductDetail model);
    }
}
