using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IItemDetailService
    {
        ItemDetail GetItemDetail(int itemId, string turnOn = "on");
        List<ItemDetail> GetItemDetails(List<int> itemIds, string turnOn);
    }
}
