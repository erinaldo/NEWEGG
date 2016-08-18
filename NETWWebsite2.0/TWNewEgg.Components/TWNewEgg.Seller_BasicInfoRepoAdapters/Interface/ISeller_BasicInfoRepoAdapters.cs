using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_BasicInfoRepoAdapters.Interface
{
    public interface ISeller_BasicInfoRepoAdapters
    {
        void Create(Seller_BasicInfo model);
        IQueryable<Seller_BasicInfo> GetAll();
        void Update(Seller_BasicInfo model);
    }
}
