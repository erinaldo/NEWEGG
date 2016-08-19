using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;

namespace TWNewEgg.ItemRepoAdapters
{

    public class ProductRepoAdapter : IProductRepoAdapter
    {
        private IRepository<Product> _productRepo;
        public ProductRepoAdapter(IRepository<Product> productRepo)
        {
            this._productRepo = productRepo;
        }

        public IQueryable<Product> GetAll()
        {
            return this._productRepo.GetAll();
        }

        public List<Product> UpdateMany(List<Product> ProductList)
        {
            this._productRepo.UpdateMany(ProductList);
            return ProductList;
        }
        public Product Update(Product Product) 
        {
            this._productRepo.Update(Product);
            return Product;        
        }

        public List<Product> GetListAllByProductID(List<int> ProductID)
        {
            return this._productRepo.GetAll().Where(x => ProductID.Contains(x.ID)).ToList();
        }

        public Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Product> GetListAll(List<int> ProductID)
        {
            Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Product> ProductList = new Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Product>();

            List<TWNewEgg.Models.DBModels.TWSQLDB.Product> Producttemp = this._productRepo.GetAll().Where(x => ProductID.Contains(x.ID)).ToList();
            
            foreach(var temp in Producttemp)
            {
                ProductList.Add(temp.ID, temp);
            }
            return ProductList;
        }
    }
}
