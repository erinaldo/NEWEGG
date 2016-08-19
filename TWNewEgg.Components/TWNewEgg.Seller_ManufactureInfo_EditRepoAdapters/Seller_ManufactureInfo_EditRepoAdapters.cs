using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ManufactureInfo_EditRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ManufactureInfo_EditRepoAdapters
{
    public class Seller_ManufactureInfo_EditRepoAdapters : ISeller_ManufactureInfo_EditRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ManufactureInfo_Edit> _seller_ManufactureInfo_Edit;

        public Seller_ManufactureInfo_EditRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ManufactureInfo_Edit> seller_ManufactureInfo_Edit)
        {
            this._seller_ManufactureInfo_Edit = seller_ManufactureInfo_Edit;
        }

        public void Create(Seller_ManufactureInfo_Edit model)
        {
            this._seller_ManufactureInfo_Edit.Create(model);
        }

        public IQueryable<Seller_ManufactureInfo_Edit> GetAll()
        {
            return this._seller_ManufactureInfo_Edit.GetAll();
        }

        public void Update(Seller_ManufactureInfo_Edit model)
        {
            this._seller_ManufactureInfo_Edit.Update(model);
        }
    }
}
