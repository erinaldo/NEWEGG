using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.ItemService.Service
{
    public interface IItemService
    {
        /// <summary>
        /// Get Items by ItemIDs, and check rawData which is one or not, if one then return raw item data, else return items' display price data.
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        List<Item> GetItemsByIDs(List<int> IDs, int rawData);

        /// <summary>
        /// Get Products by ProductIDs
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        List<Product> GetProductsByIDs(List<int> IDs);

        /// <summary>
        /// Get Sellers by SellerIDs
        /// </summary>
        /// <returns></returns>
        List<Seller> GetSellersByIDs(List<int> IDs);

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        IQueryable<Country> GetAllCountry();

        /// <summary>
        /// Get all Items
        /// </summary>
        /// <returns></returns>
        IQueryable<Item> GetAllItem();

        /// <summary>
        /// Get a Currency by country ID
        /// </summary>
        /// <param name="countryID"></param>
        /// <returns></returns>
        Currency GetACurrency(int countryID);

        /// <summary>
        /// Get Item ID's img url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <param name="size"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        string GetItemImagePath(int id, int order, int size, string table);


        /// <summary>
        /// Get Item ID's left QTY
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        int GetSellingQty(int itemID, string tableName);
    }
}
