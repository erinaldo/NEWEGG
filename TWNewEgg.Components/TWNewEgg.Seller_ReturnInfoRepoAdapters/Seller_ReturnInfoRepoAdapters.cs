using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ReturnInfoRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ReturnInfoRepoAdapters
{
    public class Seller_ReturnInfoRepoAdapters : ISeller_ReturnInfoRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ReturnInfo> _seller_ReturnInfo;

        public Seller_ReturnInfoRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ReturnInfo> seller_ReturnInfo)
        {
            this._seller_ReturnInfo = seller_ReturnInfo;
        }

        public void Create(Seller_ReturnInfo model)
        {
            this._seller_ReturnInfo.Create(model);
        }

        public IQueryable<Seller_ReturnInfo> GrtAll()
        {
            return this._seller_ReturnInfo.GetAll();
        }

        public void Update(Seller_ReturnInfo model)
        {
            this._seller_ReturnInfo.Update(model);
        }
          
    }
}
