using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ChangeToVendorRepoAdapters.Interface
{
    public interface  ISeller_ChangeToVendorRepoAdapters
    {
        void Create(Seller_ChangeToVendor model);
        IQueryable<Seller_ChangeToVendor> GetAll();
        void Update(Seller_ChangeToVendor model);
    }
}
