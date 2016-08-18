using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.ItemService.Service;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.CartService.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemServices
{
    public class ItemDisplayPriceService : IItemDisplayPriceService
    {
        IItemPrice _itemPrice;
        ICarts _carts;
        IItemStockService _ItemStockService = null;

        public ItemDisplayPriceService(IItemPrice itemPrice, ICarts carts, IItemStockService argItemStockService)
        {
            this._itemPrice = itemPrice;
            this._carts = carts;
            this._ItemStockService = argItemStockService;
        }
        
        public Dictionary<int, ItemPrice> GetItemDisplayPrice(List<int> itemIDs)
        {
            Dictionary<int, TWNewEgg.DB.TWSQLDB.Models.ItemDisplayPrice> oldPrices = this._itemPrice.GetItemDisplayPriceByIDs(itemIDs);
            Dictionary<int, int> dictItemSellingQty = null;

            //取得SellingQty
            dictItemSellingQty = this._ItemStockService.GetSellingQtyByItemList(oldPrices.Select(x=>x.Key).ToList());
            foreach (var item in oldPrices)
            {
                var MaxNumber = dictItemSellingQty[item.Key];
                if (MaxNumber > 10)
                {
                    item.Value.MaxNumber = 10;
                }
                else {
                    item.Value.MaxNumber = MaxNumber;
                }
            }
            var price = ModelConverter.ConvertTo<Dictionary<int, ItemDisplayPrice>>(oldPrices);
            var result = ModelConverter.ConvertTo<Dictionary<int, ItemPrice>>(price);
            
            return result;
        }

        public ItemPrice GetSingleItemDisplayPrice(int itemID)
        {
            var tmp = this.GetItemDisplayPrice(new List<int>(new int[] { itemID }));
            if (tmp.ContainsKey(itemID) == false)
                return null;
            else
                return tmp[itemID];
        }

        public string SetItemDisplayPriceByIDs(List<int> itemIDList)
        {
            string getResult = string.Empty;
            try
            {
                getResult = this._itemPrice.SetItemDisplayPriceByIDs(itemIDList);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return getResult;
        }
    }
}
