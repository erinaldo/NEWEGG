using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.DAL.Repository;
using TWNewEgg.DAL;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Product;
using System.Linq.Expressions;
using System.Configuration;
using TWNewEgg.Framework.Autofac;
using Autofac;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.ExampleFunc
{
    public class TestService : IDisposable
    {
        ILifetimeScope autofacScope;
        IRepository<Product> _product;
        IRepository<Item> _item;

        public TestService()
        {
            autofacScope = AutofacConfig.Container.BeginLifetimeScope();
            _product = autofacScope.Resolve<IRepository<Product>>();
            _item = autofacScope.Resolve<IRepository<Item>>();
        }

        public List<ProductDetailDM> report(int number)
        {
            //IRepository<Product> _product = AutofacConfig.Container.Resolve<IRepository<Product>>();
            var list = _product.GetAll().Take(number);
            var allProducts = list.ToList();
            var result = ModelConverter.ConvertTo<List<Product>, List<ProductDetailDM>>(allProducts);
            return result;
        }

        public List<ProductDetailDM> report2(int number)
        {
            List<ProductDetailDM> result = new List<ProductDetailDM>();
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IRepository<Product> _product2 = scope.Resolve<IRepository<Product>>();
                var list = _product2.GetAll().Take(number);
                var allProducts = list.ToList();
                result = ModelConverter.ConvertTo<List<Product>, List<ProductDetailDM>>(allProducts);
            }
            return result;
        }
        public string test(int a, string b, List<int> aaa, ProductDetailDM pp)
        {
            var result = this._item.GetAll()
                .Where(x => x.ID > 9000)
                .Join(this._product.GetAll().Where(x => x.ID > 8000),
                    ic => ic.ProductID,
                    i => i.ID, (ic, i) => i);
            var ttt = result.Count();
            return "null";
            //return string.Empty;
        }
        public List<string> test2()
        {
            List<string> test = new List<string>();
            test.Add("123456");
            test.Add("654321");
            return test;
        }
        public ProductDetailDM test3(int a, string b, List<int> aaa, ProductDetailDM pp)
        {
            ProductDetailDM test = new ProductDetailDM();
            test.ManufactureID = 8888;
            test.Name = "654948321389981";
            return test;
        }
        public List<ProductDetailDM> test4(int a, string b, List<int> aaa, ProductDetailDM pp)
        {
            List<ProductDetailDM> test = new List<ProductDetailDM>();
            ProductDetailDM test1 = new ProductDetailDM();
            test1.ManufactureID = 8888;
            test1.Name = "654948321389981";
            ProductDetailDM test2 = new ProductDetailDM();
            test2.ManufactureID = 9849846;
            test2.Name = "asegfwgrwegherg";
            test.Add(test1);
            test.Add(test2);
            return test;
        }
        public List<ProductDetailDM> test5(int a, string b, List<int> aaa, List<ProductDetailDM> pp)
        {
            List<string> english = new List<string>();
            english.Add("I");
            english.Add("am");
            english.Add("a");
            english.Add("item");
            english.Add("product");
            english.Add("!");
            List<string> chinese = new List<string>();
            chinese.Add("我");
            chinese.Add("是");
            chinese.Add("賣");
            chinese.Add("場");
            chinese.Add("商");
            chinese.Add("品");
            chinese.Add("！");
            //List<ProductDetailDM> test = new List<ProductDetailDM>();
            //ProductDetailDM test1 = new ProductDetailDM();
            //test1.ManufactureID = 8888;
            //test1.Name = "654948321389981";
            //ProductDetailDM test2 = new ProductDetailDM();
            //test2.ManufactureID = 9849846;
            //test2.Name = "asegfwgrwegherg";
            //test.Add(test1);
            //test.Add(test2);
            //return test;
            Random randdd = new Random();
            List<ProductDetailDM> test = new List<ProductDetailDM>();

            for (int i = 0; i < a; i++)
            {
                ProductDetailDM newPDM = new ProductDetailDM();
                newPDM.ID = i;
                newPDM.Name = english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)];
                newPDM.NameTW = chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)];
                newPDM.Description = i.ToString().PadRight(8, '0') + english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)];
                newPDM.DescriptionTW = i.ToString().PadLeft(8, '0') + chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)];
                test.Add(newPDM);
            }

            return test;
        }
        public void test6()
        {
            var aaa = "123";
            return;
        }
        public List<ComplexProduntDM> test7(int a, string b, List<int> aaa, List<ComplexProduntDM> pp)
        {
            List<string> english = new List<string>();
            english.Add("I");
            english.Add("am");
            english.Add("a");
            english.Add("item");
            english.Add("product");
            english.Add("!");
            List<string> chinese = new List<string>();
            chinese.Add("我");
            chinese.Add("是");
            chinese.Add("賣");
            chinese.Add("場");
            chinese.Add("商");
            chinese.Add("品");
            chinese.Add("！");
            //List<ProductDetailDM> test = new List<ProductDetailDM>();
            //ProductDetailDM test1 = new ProductDetailDM();
            //test1.ManufactureID = 8888;
            //test1.Name = "654948321389981";
            //ProductDetailDM test2 = new ProductDetailDM();
            //test2.ManufactureID = 9849846;
            //test2.Name = "asegfwgrwegherg";
            //test.Add(test1);
            //test.Add(test2);
            //return test;
            Random randdd = new Random();
            List<ComplexProduntDM> test = new List<ComplexProduntDM>();

            for (int i = 0; i < a; i++)
            {
                ComplexProduntDM newPDM = new ComplexProduntDM();
                newPDM.byteTest = Convert.ToByte(randdd.Next(00,99));
                newPDM.shortTest = Convert.ToInt16(randdd.Next(00,99));
                newPDM.intTest = randdd.Next(00, 99);
                newPDM.longTest = Convert.ToInt64(randdd.Next(00, 99));
                newPDM.floatTest = Convert.ToSingle(randdd.Next(00, 99));
                newPDM.doubleTest = Convert.ToDouble(randdd.Next(00, 99));
                newPDM.decimalTest = Convert.ToDecimal(randdd.Next(00, 99));
                newPDM.stringTest = english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)] + chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)];
                newPDM.boolTest = Convert.ToBoolean(randdd.Next(0, 1));
                newPDM.dateTimeTest = DateTime.Now;

                newPDM.byteNullTest = null;
                newPDM.shortNullTest = null;
                newPDM.intNullTest = null;
                newPDM.longNullTest = null;
                newPDM.floatNullTest = null;
                newPDM.doubleNullTest = null;
                newPDM.decimalNullTest = null;
                newPDM.boolNullTest = null;
                newPDM.dateTimeNullTest = null;

                newPDM.complexProductDMTest = new List<ComplexProduntDM>();
                newPDM.complexProductDMNullTest = null;

                ComplexProduntDM newPDM2 = new ComplexProduntDM();
                newPDM2.byteTest = Convert.ToByte(randdd.Next(00, 99));
                newPDM2.shortTest = Convert.ToInt16(randdd.Next(00, 99));
                newPDM2.intTest = randdd.Next(00, 99);
                newPDM2.longTest = Convert.ToInt64(randdd.Next(00, 99));
                newPDM2.floatTest = Convert.ToSingle(randdd.Next(00, 99));
                newPDM2.doubleTest = Convert.ToDouble(randdd.Next(00, 99));
                newPDM2.decimalTest = Convert.ToDecimal(randdd.Next(00, 99));
                newPDM2.stringTest = english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)] + english[randdd.Next(0, 5)] + chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)] + chinese[randdd.Next(0, 6)];
                newPDM2.boolTest = Convert.ToBoolean(randdd.Next(0, 1));
                newPDM2.dateTimeTest = DateTime.Now;

                newPDM2.byteNullTest = null;
                newPDM2.shortNullTest = null;
                newPDM2.intNullTest = null;
                newPDM2.longNullTest = null;
                newPDM2.floatNullTest = null;
                newPDM2.doubleNullTest = null;
                newPDM2.decimalNullTest = null;
                newPDM2.boolNullTest = null;
                newPDM2.dateTimeNullTest = null;

                newPDM2.complexProductDMTest = new List<ComplexProduntDM>();
                newPDM2.complexProductDMNullTest = null;

                newPDM.complexProductDMTest.Add(newPDM2);
                test.Add(newPDM);
            }

            return test;
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
                _product.Dispose();
                _item.Dispose();
                autofacScope.Dispose();
            }
        }
    }
}
