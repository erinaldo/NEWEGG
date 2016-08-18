using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.EDI_Seller_FunctionRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionRepoAdapters
{
    public class EDI_Seller_FunctionRepoAdapters : IEDI_Seller_FunctionRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<EDI_Seller_Function> _eDI_Seller_Function;

        public EDI_Seller_FunctionRepoAdapters(ITWSELLERPORTALDBRepository<EDI_Seller_Function> eDI_Seller_Function)
        {
            this._eDI_Seller_Function = eDI_Seller_Function;
        }

        public void Create(EDI_Seller_Function model)
        {
            this._eDI_Seller_Function.Create(model);
        }

        public IQueryable<EDI_Seller_Function> GetAll()
        {
            return this._eDI_Seller_Function.GetAll();
        }

        public void Update(EDI_Seller_Function model)
        {
            this._eDI_Seller_Function.Update(model);
        }

    }
}
