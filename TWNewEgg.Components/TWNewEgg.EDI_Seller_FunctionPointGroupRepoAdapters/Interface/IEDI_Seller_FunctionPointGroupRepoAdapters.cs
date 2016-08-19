using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionPointGroupRepoAdapters.Interface
{
    public interface IEDI_Seller_FunctionPointGroupRepoAdapters
    {
        void Create(EDI_Seller_FunctionPointGroup model);
        IQueryable<EDI_Seller_FunctionPointGroup> GetAll();
        void Update(EDI_Seller_FunctionPointGroup model);
    }
}
