using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;

namespace TWNewEgg.ItemService.Service
{
    public class ItemDisplayPriceDBService : IDisposable
    {
        private TWSqlDBContext twsql = new TWSqlDBContext();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                twsql.Dispose();
                //twsql = null;
            }
        }

        public string UpdateItemPriceCashAndProductCost(int itemID, ItemDisplayPrice aItemDisplayPrice)
        {
            string message = "";

            var aItem = twsql.Item.Where(x => x.ID == itemID).FirstOrDefault();

            if (aItem == null)
            {
                message = " [ Can't find item id : " + itemID.ToString() +  " ] ";
                return message;
            }

            var aProduct = twsql.Product.Where(x => x.ID == aItem.ProductID).FirstOrDefault();

            if (aProduct == null)
            {
                message = " [ Can't find item id : " + itemID.ToString() + "'s product id : " + aItem.ProductID.ToString() + " ] ";
                return message;
            }

            if (aItem.DelvType != 1 && aItem.DelvType != 3 && aItem.DelvType != 4)
            {
                aItem.PriceCash = aItemDisplayPrice.DisplayPrice;
                aProduct.Cost = aItemDisplayPrice.ItemCost;
                try
                {
                    twsql.SaveChanges();
                    message = " DB Update item : " + aItem.ID.ToString() + " and product : " + aProduct.ID.ToString() + " successed.";
                }
                catch (Exception e)
                {
                    message = " DB Update item : " + aItem.ID.ToString() + " and product : " + aProduct.ID.ToString() + " failed.";
                }
            }


            return message;
        }

        /// <summary>
        /// Set Item Display Price by ItemID
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="priceType"></param>
        /// <param name="minNumber"></param>
        /// <param name="maxNumber"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="displayPrice"></param>
        /// <param name="itemCost"></param>
        /// <param name="itemProfitPercent"></param>
        /// <param name="createDate"></param>
        /// <param name="createUser"></param>
        /// <param name="updateDate"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public int setItemDisplayPriceByID(int itemID, int priceType, int minNumber, int maxNumber, DateTime start, DateTime end, decimal displayPrice, decimal displayTax, decimal displayShipping, decimal itemCost, decimal itemCostTW, decimal? itemProfitPercent, DateTime createDate, string createUser)
        {
            int flag = new int();
            if (priceType == (int)ItemDisplayPrice.PriceTypeEnum.系統自動)
            {
                flag = insertItemDisplayPriceBySYS(itemID, priceType, displayPrice, displayTax, displayShipping, itemCost, itemCostTW, createDate, createUser);
            }
            else
            {

            }

            return flag;
        }
        /// <summary>
        /// Insert or Update Item Display Price which create by System
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="priceType"></param>
        /// <param name="displayPrice"></param>
        /// <param name="displayTax"></param>
        /// <param name="displayShipping"></param>
        /// <param name="itemCost"></param>
        /// <param name="itemCostTW"></param>
        /// <param name="insertDate"></param>
        /// <param name="insertUser"></param>
        /// <returns></returns>
        private int insertItemDisplayPriceBySYS(int itemID, int priceType, decimal displayPrice, decimal displayTax, decimal displayShipping, decimal itemCost, decimal itemCostTW, DateTime insertDate, string insertUser)
        {
            int flag = new int();
            var sameItemDisplayPrice = twsql.ItemDisplayPrice.Where(x => x.ItemID == itemID && x.PriceType == priceType).OrderBy(x => x.ID).FirstOrDefault();
            if (itemCostTW == 0)
            {
                itemCostTW = 1; 
            }

            if (sameItemDisplayPrice == null)
            {
                ItemDisplayPrice newItemDisplayPrice = new ItemDisplayPrice();
                newItemDisplayPrice.ItemID = itemID;
                newItemDisplayPrice.PriceType = priceType;
                newItemDisplayPrice.MinNumber = 1;
                newItemDisplayPrice.MaxNumber = 1;
                newItemDisplayPrice.StartDate = insertDate;
                newItemDisplayPrice.EndDate = insertDate.AddYears(100);
                newItemDisplayPrice.DisplayPrice = displayPrice;
                newItemDisplayPrice.DisplayTax = displayTax;
                newItemDisplayPrice.DisplayShipping = displayShipping;
                newItemDisplayPrice.ItemCost = itemCost;
                newItemDisplayPrice.ItemCostTW = itemCostTW;
                newItemDisplayPrice.ItemProfitPercent = Math.Round(((displayPrice - itemCostTW) / displayPrice), 2, MidpointRounding.AwayFromZero);
                if (Math.Abs(newItemDisplayPrice.ItemProfitPercent.Value) > 999)
                {
                    newItemDisplayPrice.ItemProfitPercent = 999.00M;
                }
                newItemDisplayPrice.CreateDate = insertDate;
                newItemDisplayPrice.CreateUser = insertUser;
                newItemDisplayPrice.Updated = 0;

                try
                {
                    twsql.ItemDisplayPrice.Add(newItemDisplayPrice);
                    twsql.SaveChanges();
                    flag = 0;
                }
                catch (Exception e)
                {
                    flag = 101;
                }
            }
            else
            {
                sameItemDisplayPrice.DisplayPrice = displayPrice;
                sameItemDisplayPrice.DisplayTax = displayTax;
                sameItemDisplayPrice.DisplayShipping = displayShipping;
                sameItemDisplayPrice.ItemCost = itemCost;
                sameItemDisplayPrice.ItemCostTW = itemCostTW;
                sameItemDisplayPrice.ItemProfitPercent = Math.Round(((displayPrice - itemCostTW) / displayPrice), 2, MidpointRounding.AwayFromZero);
                if (Math.Abs(sameItemDisplayPrice.ItemProfitPercent.Value) > 999)
                {
                    sameItemDisplayPrice.ItemProfitPercent = 999.00M;
                }
                sameItemDisplayPrice.Updated += 1;
                sameItemDisplayPrice.UpdateDate = insertDate;
                sameItemDisplayPrice.UpdateUser = insertUser;
                try
                {
                    twsql.SaveChanges();
                    flag = 1;
                }
                catch (Exception e)
                {
                    flag = 102;
                }
            }


            return flag;
        }


        /// <summary>
        /// Get ItemDisplayPrice by ItemIDs
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="dateTimeNow"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<ItemDisplayPrice> GetItemDisplayPriceByItemIDs(List<int> itemIDs, DateTime? dateTimeNow, DateTime? startDate, DateTime? endDate)
        {
            List<ItemDisplayPrice> allItemDisplayPrices = new List<ItemDisplayPrice>();
            IQueryable<ItemDisplayPrice> tempItemDisplayPrice;
            var hashID = new HashSet<int>(itemIDs);
            if (startDate != null && endDate != null)
            {
                tempItemDisplayPrice = twsql.ItemDisplayPrice.Where(x => hashID.Contains(x.ItemID) && x.StartDate > startDate && x.EndDate < endDate);
            }
            else if (startDate != null && endDate == null)
            {
                tempItemDisplayPrice = twsql.ItemDisplayPrice.Where(x => hashID.Contains(x.ItemID) && x.StartDate > startDate);
            }
            else if (startDate == null && endDate != null)
            {
                tempItemDisplayPrice = twsql.ItemDisplayPrice.Where(x => hashID.Contains(x.ItemID) && x.EndDate < endDate);
            }
            else
            {
                tempItemDisplayPrice = twsql.ItemDisplayPrice.Where(x => hashID.Contains(x.ItemID));
            }
            if (dateTimeNow != null)
            {
                allItemDisplayPrices.AddRange(tempItemDisplayPrice.Where(x => x.StartDate <= dateTimeNow && x.EndDate > dateTimeNow).ToList());
            }
            else
            {
                allItemDisplayPrices.AddRange(tempItemDisplayPrice.ToList());
            }
            
            return allItemDisplayPrices;
        }
    }
}
