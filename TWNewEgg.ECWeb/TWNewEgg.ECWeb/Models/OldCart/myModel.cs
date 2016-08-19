using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class myModel
    {
        //這個Model 應該只存在 List<> 型態的屬性
        public List<Store> Store { get; set; }
        public List<StoreFromWS> wsStore { get; set; }
        public List<Country> Country { get; set; }
        public List<Category> Category { get; set; }
        public List<CategoryFromWS> wsCategory { get; set; }
        public List<Account> Account { get; set; }
        public List<SalesOrder> SalesOrder { get; set; }
        public List<SalesOrderItem> SalesOrderItem { get; set; }
        public List<Problem> Problem { get; set; }
        public List<Answer> Answer { get; set; }
        public List<Product> Product { get; set; }
        public List<ProductTemp> ProductTemp { get; set; }
        public List<Seller> Seller { get; set; }
        public List<Item> Item { get; set; }
        public List<ProductFromWS> ProductFromWS { get; set; }
        public List<Manufacture> Manufacture { get; set; }
        public List<Bank> Bank { get; set; }
        public List<PayType> PayType { get; set; }
        public List<ItemList> ItemList { get; set; }
        public List<ItemStock> ItemStock { get; set; }
        public List<ItemTemp> ItemTemp { get; set; }
        public List<ItemListTemp> ItemListTemp { get; set; }
        public List<ItemListGroupTemp> ItemListGroupTemp { get; set; }
        public List<ItemListGroup> ItemListGroup { get; set; }
        public List<ReviewFromWS> ReviewFormWs { get; set; }
        public List<Retgood> Retgood { get; set; }
        public List<MyNewEgg> MyNewEgg { get; set; }

        public InsertSalesOrdersBySellerInput InputData { get; set; }
        public InsertSalesOrdersBySellerOutput OutputData { get; set; }
        public List<BuyingItems> BuyingItems { get; set; }
        //public List<CartBox> CartBoxs { get; set; }
        public List<ItemCartBox> ItemCartBoxs { get; set; }
        public List<CathayUnitedBank> WebATM { get; set; }

    }
}