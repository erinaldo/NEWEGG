using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;

namespace TWNewEgg.ItemService.Service
{
    public class ItemServiceRepository : IItemService
    {
        


        public List<Seller> GetSellersByIDs(List<int> IDs)
        {
            List<Seller> sellerList = new List<Seller>();
            using (var db = new TWSqlDBContext())
            {
                sellerList = db.Seller.Where(x => IDs.Contains(x.ID)).ToList();
            }
            return sellerList;
        }

        public IQueryable<Country> GetAllCountry()
        {
            IQueryable<Country> countryList;
            var db = new TWSqlDBContext();

            countryList = db.Country;

            return countryList;
        }

        public IQueryable<Item> GetAllItem()
        {
            IQueryable<Item> itemList;
            var db = new TWSqlDBContext();

            itemList = db.Item;

            return itemList;
        }
        public List<Item> GetItemsByIDs(List<int> IDs, int rawData)
        {
            List<Item> itemList = new List<Item>();
            using (var db = new TWSqlDBContext())
            {
                var hashID = new HashSet<int>(IDs);
                itemList = db.Item.Where(x => hashID.Contains(x.ID)).ToList();
            }
            if (rawData != 1)
            {
                IItemPrice ItemPriceData = new ItemPriceRepository();
                var getItemPrice = ItemPriceData.GetItemDisplayPriceByIDs(IDs);
                ItemDisplayPrice aItemDisplayPrice = new ItemDisplayPrice();
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (getItemPrice.TryGetValue(itemList[i].ID, out aItemDisplayPrice))
                    {
                        itemList[i].PriceCash = aItemDisplayPrice.DisplayPrice;
                        itemList[i].PriceGlobalship = aItemDisplayPrice.DisplayShipping;
                        itemList[i].Taxfee = aItemDisplayPrice.DisplayTax;
                    }
                }
            }
            //Newtonsoft.Json.JsonConvert.SerializeObject(itemList);
            return itemList;
        }
        public Currency GetACurrency(int countryID)
        {
            Currency aCurrency = new Currency();
            using (var db = new TWSqlDBContext())
            {
                aCurrency = db.Currency.Where(x => x.CountryID == countryID).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefault();
            }
            return aCurrency;
        }



        /// <summary>
        /// Get Item ID's img url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <param name="size"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public string GetItemImagePath(int id, int order, int size, string table)
        {
            string path = "";
            if (order != 0)
            {
                path = string.Format("/Pic/{0}/{1}/{2}_{3}_{4}.{5}", table, (id / 10000).ToString("0000"), (id % 10000).ToString("0000"), order, size, "jpg");
            }
            else
            {
                path = "";
            }
            return path;
        }


        /// <summary>
        /// Get Item ID's left QTY
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetSellingQty(int itemID, string tableName)
        {

            int itemQty = new int(), itemQtyReg = new int(), itemQtyLimit = new int();
            int? itemStockQty = new int?(), itemStockQtyReg = new int?();
            using (var db = new TWSqlDBContext())
            {
                int productID = new int();

                if (tableName == "Item") //if it is item then get infomation from dbo.item
                {
                    var dbItem = db.Item.Where(x => x.ID == itemID).Select(y => new { item_productid = y.ProductID, item_qty = y.Qty, item_qtyreg = y.QtyReg, item_qtylimit = y.QtyLimit }).FirstOrDefault();
                    if (dbItem == null)
                    {
                        return 0;
                    }
                    productID = dbItem.item_productid;
                    itemQty = dbItem.item_qty;
                    itemQtyReg = dbItem.item_qtyreg;
                    itemQtyLimit = dbItem.item_qtylimit;
                    var dbItemStock = db.ItemStock.Where(x => x.ProductID == productID).Select(y => new { itemstock_qty = y.Qty, itemstock_qtyreg = y.QtyReg }).FirstOrDefault();
                    if (dbItemStock != null)
                    {
                        itemStockQty = dbItemStock.itemstock_qty;
                        itemStockQtyReg = dbItemStock.itemstock_qtyreg;
                    }
                }
                if (tableName == "ItemList") //if it is itemlist then get infomation from dbo.itemlist
                {
                    var dbItem = db.ItemList.Where(x => x.ID == itemID).Select(y => new { itemlist_itemlistproductid = y.ItemlistProductID, itemlist_qty = y.Qty, itemlist_qtyreg = y.QtyReg, itemlist_qtylimit = y.QtyLimit }).FirstOrDefault();
                    if (dbItem == null)
                    {
                        return 0;
                    }
                    productID = dbItem.itemlist_itemlistproductid;
                    itemQty = dbItem.itemlist_qty;
                    itemQtyReg = dbItem.itemlist_qtyreg;
                    itemQtyLimit = dbItem.itemlist_qtylimit;
                    var dbItemStock = db.ItemStock.Where(x => x.ProductID == productID).Select(y => new { itemstock_qty = y.Qty, itemstock_qtyreg = y.QtyReg }).FirstOrDefault();
                    if (dbItemStock != null)
                    {
                        itemStockQty = dbItemStock.itemstock_qty;
                        itemStockQtyReg = dbItemStock.itemstock_qtyreg;
                    }
                }
            }



            //int sellingQty = countSellingQty(itemQty, itemQtyReg, itemQtyLimit, itemStockQty, itemStockQtyReg);

            return CountSellingQty(itemQty, itemQtyReg, itemQtyLimit, itemStockQty, itemStockQtyReg);//call count selling qty

        }

        /// <summary>
        /// this function's content was copy from dbo's function
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// if sql's function change then this should be change
        /// </summary>
        /// <param name="itemQty"></param>
        /// <param name="itemQtyReg"></param>
        /// <param name="itemQtyLimit"></param>
        /// <param name="itemStockQty"></param>
        /// <param name="itemStockQtyReg"></param>
        /// <returns></returns>
        private int CountSellingQty(int itemQty, int itemQtyReg, int itemQtyLimit, int? itemStockQty, int? itemStockQtyReg)
        {
            int sellingQty = 0;
            int itemQtyAmount = itemQty - itemQtyReg; //total qty minus sold out qty
            int itemStockQtyAmount = new int();
            if (itemStockQty != null)
            {
                itemStockQtyAmount = itemStockQty.Value - itemStockQtyReg.Value; //total stock qty minus sold out qty
                if (itemQty > 0)
                {
                    if (itemQtyAmount > itemStockQtyAmount)
                    {
                        if (itemStockQtyAmount > itemQtyLimit && itemQtyLimit > 0)
                        {
                            sellingQty = itemQtyLimit;
                        }
                        else
                        {
                            sellingQty = itemStockQtyAmount;
                        }
                    }
                    else
                    {
                        if (itemQtyAmount > itemQtyLimit && itemQtyLimit > 0)
                        {
                            sellingQty = itemQtyLimit;
                        }
                        else
                        {
                            sellingQty = itemQtyAmount;
                        }
                    }
                }
                else
                {
                    if (itemStockQtyAmount > itemQtyLimit && itemQtyLimit > 0)
                    {
                        sellingQty = itemQtyLimit;
                    }
                    else
                    {
                        sellingQty = itemStockQtyAmount;
                    }
                }
            }



            return sellingQty;
        }


        public List<Product> GetProductsByIDs(List<int> IDs)
        {
            List<Product> productList = new List<Product>();
            using (var db = new TWSqlDBContext())
            {
                productList = db.Product.Where(x => IDs.Contains(x.ID)).ToList();
            }
            //Newtonsoft.Json.JsonConvert.SerializeObject(itemList);
            return productList;
        }
    }
}
