using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IProductRepoAdapter
    {
        IQueryable<Product> GetAll();
        Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Product> GetListAll(List<int> ProductID);
        List<Product> GetListAllByProductID(List<int> ProductID);
        List<Product> UpdateMany(List<Product> ProductList);
        Product Update(Product Product);
    }
}
