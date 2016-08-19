using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ChangeToVendorRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ChangeToVendorRepoAdapters
{
    public class Seller_ChangeToVendorRepoAdapters : ISeller_ChangeToVendorRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ChangeToVendor> _seller_ChangeToVendor;

        public Seller_ChangeToVendorRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ChangeToVendor> seller_ChangeToVendor)
        {
            this._seller_ChangeToVendor = seller_ChangeToVendor;
        }

        public void Create(Seller_ChangeToVendor model)
        {
            this._seller_ChangeToVendor.Create(model);
        }

        public IQueryable<Seller_ChangeToVendor> GetAll()
        {
            return this._seller_ChangeToVendor.GetAll();
        }

        public void Update(Seller_ChangeToVendor model)
        {
            this._seller_ChangeToVendor.Update(model);
        }
    }
}
