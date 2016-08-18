using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.EDI_Seller_FunctionPointGroupRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionPointGroupRepoAdapters
{
    public class EDI_Seller_FunctionPointGroupRepoAdapters : IEDI_Seller_FunctionPointGroupRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<EDI_Seller_FunctionPointGroup> _eDI_Seller_FunctionPointGroup;

        public EDI_Seller_FunctionPointGroupRepoAdapters(ITWSELLERPORTALDBRepository<EDI_Seller_FunctionPointGroup> eDI_Seller_FunctionPointGroup)
        {
            this._eDI_Seller_FunctionPointGroup = eDI_Seller_FunctionPointGroup;
        }

        public void Create(EDI_Seller_FunctionPointGroup model)
        {
            this._eDI_Seller_FunctionPointGroup.Create(model);

        }

        public IQueryable<EDI_Seller_FunctionPointGroup> GetAll()
        {
            return this._eDI_Seller_FunctionPointGroup.GetAll();
        }

        public void Update(EDI_Seller_FunctionPointGroup model)
        {
            this._eDI_Seller_FunctionPointGroup.Update(model);
        }
          
    }
}
