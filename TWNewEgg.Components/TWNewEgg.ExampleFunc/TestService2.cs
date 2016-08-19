using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ExampleFunc
{
    public class TestService2: ITestService, TWNewEgg.ExampleFunc.Interface.ITest3, IDisposable
    {
        private IRepository<Product> _product;
        private IRepository<Item> _Item;
        public TestService2(IRepository<Product> product, IRepository<Item> item)
        {
            this._product = product;
            this._Item = item;
        }

        public List<Product> report(int number)
        {
            var testItem = this._Item.GetAll().Take(number).ToList();
            return this._product.GetAll().Take(number).ToList();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._Item.Dispose();
                this._product.Dispose();
            }
        }

        public string report3()
        {
            return "this is TestService22222222222";
        }
    }
}
