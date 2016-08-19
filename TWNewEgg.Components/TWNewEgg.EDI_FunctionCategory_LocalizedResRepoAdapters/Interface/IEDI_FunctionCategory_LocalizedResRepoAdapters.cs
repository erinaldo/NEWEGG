using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_FunctionCategory_LocalizedResRepoAdapters.Interface
{
    public interface IEDI_FunctionCategory_LocalizedResRepoAdapters
    {
        void Create(EDI_Seller_FunctionCategory_LocalizedRes model);
        IQueryable<EDI_Seller_FunctionCategory_LocalizedRes> GetAll();
        void Update(EDI_Seller_FunctionCategory_LocalizedRes model);
    }
}
