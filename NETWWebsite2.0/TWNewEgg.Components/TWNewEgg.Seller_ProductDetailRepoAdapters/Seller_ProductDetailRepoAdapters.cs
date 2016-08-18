using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ProductDetailRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ProductDetailRepoAdapters
{
    public class Seller_ProductDetailRepoAdapters : ISeller_ProductDetailRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ProductDetail> _seller_ProductDetail;

        public Seller_ProductDetailRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ProductDetail> seller_ProductDetail)
        {
            this._seller_ProductDetail = seller_ProductDetail;
        }

        public void Create(Seller_ProductDetail model)
        {
            this._seller_ProductDetail.Create(model);
        }

        public IQueryable<Seller_ProductDetail> GetAll()
        {
            return this._seller_ProductDetail.GetAll();
        }

        public void Update(Seller_ProductDetail model)
        {
            this._seller_ProductDetail.Update(model);
        }
    }
}
