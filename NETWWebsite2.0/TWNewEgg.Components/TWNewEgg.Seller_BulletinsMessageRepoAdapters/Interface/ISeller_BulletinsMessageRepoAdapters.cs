using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_BulletinsMessageRepoAdapters.Interface
{
    public interface ISeller_BulletinsMessageRepoAdapters
    {
        void Create(Seller_BulletinsMessage model);
        IQueryable<Seller_BulletinsMessage> GetAll();
        void Update(Seller_BulletinsMessage model);
    }
}
