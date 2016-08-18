using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ManufactureInfo_EditRepoAdapters.Interface
{
    public interface ISeller_ManufactureInfo_EditRepoAdapters
    {
        void Create(Seller_ManufactureInfo_Edit model);
        IQueryable<Seller_ManufactureInfo_Edit> GetAll();
        void Update(Seller_ManufactureInfo_Edit model);
    }
}
