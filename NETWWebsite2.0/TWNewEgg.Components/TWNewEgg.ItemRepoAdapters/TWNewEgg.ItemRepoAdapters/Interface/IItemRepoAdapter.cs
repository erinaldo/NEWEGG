using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IItemRepoAdapter
    {
        IQueryable<Item> GetAll();
        Item GetIfAvailable(int itemId);
        IQueryable<Item> GetAvailableAndVisible(int categoryId);
        IQueryable<Item> GetCrossCategoryAvailableAndVisible(int categoryId);
        IQueryable<Item> GetAvailableAndVisibleItemList(List<int> itemIDList);
        Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item> GetItemList(List<int> ItemListID);
        Item UpdateItem(Item newData);
        List<Item> UpdateItemList(List<Item> newDataList);
        List<int> GetListGroupByItemDelvType(List<int> listItem);
    }
}
