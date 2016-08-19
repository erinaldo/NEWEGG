using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_BasicInfoRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_BasicInfoRepoAdapters
{
    public class Seller_BasicInfoRepoAdapters : ISeller_BasicInfoRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_BasicInfo> _seller_BasicInfo;

        public Seller_BasicInfoRepoAdapters(ITWSELLERPORTALDBRepository<Seller_BasicInfo> seller_BasicInfo)
        {
            this._seller_BasicInfo = seller_BasicInfo;
        }

        public void Create(Seller_BasicInfo model)
        {
            this._seller_BasicInfo.Create(model);
        }

        public IQueryable<Seller_BasicInfo> GetAll()
        {
            return this._seller_BasicInfo.GetAll();
        }

        public void Update(Seller_BasicInfo model)
        {
            this._seller_BasicInfo.Update(model);
        }
    }
}
