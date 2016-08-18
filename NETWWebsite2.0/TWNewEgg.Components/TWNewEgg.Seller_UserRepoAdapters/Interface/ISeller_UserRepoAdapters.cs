using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_UserRepoAdapters.Interface
{
    public interface ISeller_UserRepoAdapters
    {
        void Create(Seller_User model);
        IQueryable<Seller_User> GetAllSellerUser();
        void Update(Seller_User model);
    }
}
