using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.ItemService.Models;

namespace TWNewEgg.ItemService.Service
{
    public interface IItemPrice : IDisposable
    {
        /// <summary>
        /// Set Whole Site's Item by Delvtype, if Delvtype == 65535, then it's means Whole Items.
        /// </summary>
        /// <param name="delvType"></param>
        /// <returns></returns>
        string SetWholeDBItemDisplayPrice(int delvType);

        /// <summary>
        /// Set Item Display Price by ItemIDs.
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <returns></returns>
        string SetItemDisplayPriceByIDs(List<int> itemIDs);


        /// <summary>
        /// Get the ItemDisplayPrice and update Item's pricecash
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <returns></returns>
        string UpdateItemPriceCashByIDs(List<int> itemIDs);

        /// <summary>
        /// Get ItemDisplayPrices by ItemIDs
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <returns></returns>
        Dictionary<int, ItemDisplayPrice> GetItemDisplayPriceByIDs(List<int> itemIDs);

        /// <summary>
        /// Get shipping cost and taxdetail, first is count by program, last is call from stored procedure , this is for local items
        /// </summary>
        /// <param name="PostData"></param>
        /// <returns></returns>
        ShipTaxService GetShippingCosts(List<BuyingItems> postData, string shoptype);

        /// <summary>
        /// call stored procedure to get item tax detail
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="itemlistid"></param>
        /// <param name="shippingpricelist"></param>
        /// <returns></returns>
        IEnumerable<GetItemTaxDetail> GetItemTaxDetails(string itemid, string itemlistid, string shippingpricelist);

        /// <summary>
        /// detemer weight and get shipping cost from db
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="sellerID"></param>
        /// <returns></returns>
        decimal GetItemShipping(decimal weight, int sellerID, string shoptype);

        /// <summary>
        /// acturally this function was changed all by Steven.S.Chen, so... XD  
        /// </summary>
        /// <param name="itemTaxDetail"></param>
        /// <param name="sellerID"></param>
        /// <param name="itemforSP"></param>
        /// <param name="totalItemShipping"></param>
        void SetItemTaxDetailData(Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail, string sellerID, List<string> itemforSP, decimal totalItemShipping, List<int> eachItemNumber, List<decimal> eachItemShippingFee, string shoptype);


        /// <summary>
        /// count item and itemlist's length, width, and weight and compare with each weight, choose max one 
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="number"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        decimal GetItemWeight(int itemIDs, int number, string table);


    }
}
