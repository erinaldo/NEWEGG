using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ProductTempRepoAdapters.Interface
{
    public interface IProductTempRepoAdapters
    {
        IQueryable<ProductTemp> GetAll();
        void Update(ProductTemp model);
    }
}
