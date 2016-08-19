using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ActionRepoAdapters.Interface
{
    public interface ISeller_ActionRepoAdapters
    {
        IQueryable<Seller_Action> GetAll();
        void Create(Seller_Action model);
        void Update(Seller_Action model);
    }
}
