using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionPointRepoAdapters.Interface
{
    public interface IEDI_Seller_FunctionPointRepoAdapters
    {
        void Create(EDI_Seller_FunctionPoint model);
        IQueryable<EDI_Seller_FunctionPoint> GetAll();
        void Update(EDI_Seller_FunctionPoint model);
    }
}
