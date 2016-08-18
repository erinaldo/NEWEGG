using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_Function_LocalizedResRepoAdapters.Interface
{
    public interface IEDI_Seller_Function_LocalizedResRepoAdapters
    {
        void Create(EDI_Seller_Function_LocalizedRes model);
        IQueryable<EDI_Seller_Function_LocalizedRes> GetAll();
        void Update(EDI_Seller_Function_LocalizedRes model);
    }
}
