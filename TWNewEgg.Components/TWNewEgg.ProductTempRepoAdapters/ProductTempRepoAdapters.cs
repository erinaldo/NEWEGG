using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.ProductTempRepoAdapters.Interface;

namespace TWNewEgg.ProductTempRepoAdapters
{
    public class ProductTempRepoAdapters : IProductTempRepoAdapters
    {
        private IRepository<ProductTemp> _productTemp;
        public ProductTempRepoAdapters(IRepository<ProductTemp> productTemp)
        {
            this._productTemp = productTemp;
        }
        public IQueryable<ProductTemp> GetAll()
        {
            return this._productTemp.GetAll();
        }
        public void Update(ProductTemp model)
        {
            this._productTemp.Update(model);
        }
    }
}
