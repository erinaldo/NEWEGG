using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionCategoryRepoAdapters.Interface
{
    public interface IEDI_Seller_FunctionCategoryRepoAdapters
    {
        void Create(EDI_Seller_FunctionCategory model);
        IQueryable<EDI_Seller_FunctionCategory> GetAll();
        void Update(EDI_Seller_FunctionCategory model);
    }
}
