using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionRepoAdapters.Interface
{
    public interface IEDI_Seller_FunctionRepoAdapters
    {
        void Create(EDI_Seller_Function model);
        IQueryable<EDI_Seller_Function> GetAll();
        void Update(EDI_Seller_Function model);
    }
}
