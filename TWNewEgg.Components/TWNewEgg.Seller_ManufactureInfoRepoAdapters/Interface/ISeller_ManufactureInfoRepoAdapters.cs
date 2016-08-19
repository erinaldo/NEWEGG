using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ManufactureInfoRepoAdapters.Interface
{
    public interface ISeller_ManufactureInfoRepoAdapters
    {
        void Create(Seller_ManufactureInfo model);
        IQueryable<Seller_ManufactureInfo> GetAll();
        void Update(Seller_ManufactureInfo model);
    }
}
