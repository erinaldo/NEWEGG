using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IItemInfoService
    {
        ItemInfo GetItemInfo(int itemId);
        Dictionary<int, ItemInfo> GetItemInfoList(List<int> itemId);
        List<ItemBase> GetAvailableAndVisible(int argNumCategoryId);
    }
}
