using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ProductSpecRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ProductSpecRepoAdapters
{
    public class Seller_ProductSpecRepoAdapters : ISeller_ProductSpecRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ProductSpec> _seller_ProductSpec;

        public Seller_ProductSpecRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ProductSpec> seller_ProductSpec)
        {
            this._seller_ProductSpec = seller_ProductSpec;
        }

        public void Create(Seller_ProductSpec model)
        {
            this._seller_ProductSpec.Create(model);
        }

        public IQueryable<Seller_ProductSpec> GetAll()
        {
            return this._seller_ProductSpec.GetAll();
        }

        public void Update(Seller_ProductSpec model)
        {
            this._seller_ProductSpec.Update(model);
        }
    }
}
