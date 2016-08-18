using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.EDI_FunctionCategory_LocalizedResRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_FunctionCategory_LocalizedResRepoAdapters
{
    public class EDI_FunctionCategory_LocalizedResRepoAdapters : IEDI_FunctionCategory_LocalizedResRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<EDI_Seller_FunctionCategory_LocalizedRes> _eDI_Seller_FunctionCategory_LocalizedRes;

        public EDI_FunctionCategory_LocalizedResRepoAdapters(ITWSELLERPORTALDBRepository<EDI_Seller_FunctionCategory_LocalizedRes> eDI_Seller_FunctionCategory_LocalizedRes)
        {
            this._eDI_Seller_FunctionCategory_LocalizedRes = eDI_Seller_FunctionCategory_LocalizedRes;
        }

        public void Create(EDI_Seller_FunctionCategory_LocalizedRes model)
        {
            this._eDI_Seller_FunctionCategory_LocalizedRes.Create(model);
        }

        public IQueryable<EDI_Seller_FunctionCategory_LocalizedRes> GetAll()
        {
            return this._eDI_Seller_FunctionCategory_LocalizedRes.GetAll();
        }

        public void Update(EDI_Seller_FunctionCategory_LocalizedRes model)
        {
            this._eDI_Seller_FunctionCategory_LocalizedRes.Update(model);
        }
    }
}
