using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.EDI_Seller_FunctionPointRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.EDI_Seller_FunctionPointRepoAdapters
{
    public class EDI_Seller_FunctionPointRepoAdapters : IEDI_Seller_FunctionPointRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<EDI_Seller_FunctionPoint> _eDI_Seller_FunctionPoint;

        public EDI_Seller_FunctionPointRepoAdapters(ITWSELLERPORTALDBRepository<EDI_Seller_FunctionPoint> eDI_Seller_FunctionPoint)
        {
            this._eDI_Seller_FunctionPoint = eDI_Seller_FunctionPoint;
        }

        public void Create(EDI_Seller_FunctionPoint model)
        {
            this._eDI_Seller_FunctionPoint.Create(model);
        }

        public IQueryable<EDI_Seller_FunctionPoint> GetAll()
        {
            return this._eDI_Seller_FunctionPoint.GetAll();
        }

        public void Update(EDI_Seller_FunctionPoint model)
        {
            this._eDI_Seller_FunctionPoint.Update(model);
        }
    }
}
