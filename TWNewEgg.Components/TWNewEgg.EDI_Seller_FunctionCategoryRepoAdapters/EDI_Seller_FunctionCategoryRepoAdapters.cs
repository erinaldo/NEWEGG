using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.EDI_Seller_FunctionCategoryRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionCategoryRepoAdapters
{
    public class EDI_Seller_FunctionCategoryRepoAdapters : IEDI_Seller_FunctionCategoryRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<EDI_Seller_FunctionCategory> _eDI_Seller_FunctionCategory;

        public EDI_Seller_FunctionCategoryRepoAdapters(ITWSELLERPORTALDBRepository<EDI_Seller_FunctionCategory> eDI_Seller_FunctionCategory)
        {
            this._eDI_Seller_FunctionCategory = eDI_Seller_FunctionCategory;
        }

        public void Create(EDI_Seller_FunctionCategory model)
        {
            this._eDI_Seller_FunctionCategory.Create(model);
        }

        public IQueryable<EDI_Seller_FunctionCategory> GetAll()
        {
            return this._eDI_Seller_FunctionCategory.GetAll();
        }

        public void Update(EDI_Seller_FunctionCategory model)
        {
            this._eDI_Seller_FunctionCategory.Update(model);
        }
    }
}
