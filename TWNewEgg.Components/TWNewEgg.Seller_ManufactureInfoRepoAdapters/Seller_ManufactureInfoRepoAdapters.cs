using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ManufactureInfoRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ManufactureInfoRepoAdapters
{
    public class Seller_ManufactureInfoRepoAdapters : ISeller_ManufactureInfoRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ManufactureInfo> _seller_ManufactureInfo;

        public Seller_ManufactureInfoRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ManufactureInfo> seller_ManufactureInfo)
        {
            this._seller_ManufactureInfo = seller_ManufactureInfo;
        }

        public void Create(Seller_ManufactureInfo model)
        {
            this._seller_ManufactureInfo.Create(model);
        }

        public IQueryable<Seller_ManufactureInfo> GetAll()
        {
            return this._seller_ManufactureInfo.GetAll();
        }

        public void Update(Seller_ManufactureInfo model)
        {
            this._seller_ManufactureInfo.Update(model);
        }
    }
}
