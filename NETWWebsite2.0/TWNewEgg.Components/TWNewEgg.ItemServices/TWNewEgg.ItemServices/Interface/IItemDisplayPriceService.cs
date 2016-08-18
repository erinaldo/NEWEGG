using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IItemDisplayPriceService
    {
        Dictionary<int, ItemPrice> GetItemDisplayPrice(List<int> itemIDs);
        ItemPrice GetSingleItemDisplayPrice(int itemID);
        string SetItemDisplayPriceByIDs(List<int> itemIDList);
    }
}
