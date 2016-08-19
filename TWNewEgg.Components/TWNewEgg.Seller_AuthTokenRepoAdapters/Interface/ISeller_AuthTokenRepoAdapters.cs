using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_AuthTokenRepoAdapters.Interface
{
    public interface ISeller_AuthTokenRepoAdapters
    {
        void Create(Seller_AuthToken model);
        IQueryable<Seller_AuthToken> GetAll();
        void Update(Seller_AuthToken model);
    }
}
